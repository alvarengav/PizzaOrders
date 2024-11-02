using HotChocolate.Subscriptions;
using Microsoft.EntityFrameworkCore;
using PizzaOrders.GraphQLApi.Data;
using PizzaOrders.GraphQLApi.Models;
using PizzaOrders.GraphQLApi.Services;

namespace PizzaOrders.GraphQLApi.GraphQL.Orders;

[MutationType]
public static class OrderMutations
{
    public static async Task<Order> CreateOrder(
        CreateOrderInput input,
        ApplicationDbContext context,
        IEncryptionService encryptionService,
        ITopicEventSender eventSender,
        CancellationToken cancellationToken
    )
    {
        var customerInput = input.Customer;
        var pizzaInput = input.Pizza;

        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var customer = await context.Customers.FirstOrDefaultAsync(
                c => c.Name == customerInput.Name,
                cancellationToken
            );

            if (customer is null)
            {
                customer = new Customer
                {
                    Name = customerInput.Name,
                    Phone = customerInput.Phone,
                    Address = customerInput.Address
                };
            }
            else
            {
                customer.Phone = customerInput.Phone;
                customer.Address = customerInput.Address;
            }

            var toppingNames = pizzaInput.Toppings.Select(t => t.ToString()).ToList();
            var pizzaDetails = new PizzaDetails
            {
                Type = pizzaInput.Type,
                Size = pizzaInput.Size,
                Toppings = toppingNames
            };

            var order = new Order
            {
                Customer = customer,
                PizzaDetails = pizzaDetails,
                Status = OrderStatus.Preparing,
                OrderDate = DateTime.UtcNow
            };

            context.Orders.Add(order);

            await context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            await eventSender.SendAsync(
                nameof(OrderSubscriptions.OnOrderCreatedAsync),
                order.Id,
                cancellationToken
            );

            return order;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new Exception(
                "Error creating order: " + ex.InnerException?.Message ?? ex.Message
            );
        }
    }

    [Error<OrderNotFoundException>]
    public static async Task<Order> UpdateOrder(
        UpdateOrderInput input,
        ITopicEventSender eventSender,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        var order = await context.Orders.FindAsync(input.OrderId);
        if (order is null)
        {
            throw new OrderNotFoundException();
        }

        order.Status = input.NewStatus;

        order.PizzaDetails.Type = input.PizzaDetails.Type;
        order.PizzaDetails.Size = input.PizzaDetails.Size;
        order.PizzaDetails.Toppings = input
            .PizzaDetails.Toppings.Select(t => t.ToString())
            .ToList();

        context.Orders.Update(order);
        await context.SaveChangesAsync(cancellationToken);

        await eventSender.SendAsync(
            nameof(OrderSubscriptions.OnOrderUpdatedAsync),
            order.Id,
            cancellationToken
        );

        return order;
    }

    public static async Task<Order> UpdateOrderStatus(
        int orderId,
        OrderStatus status,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        var order = await context.Orders.FindAsync(orderId);
        if (order == null)
            throw new Exception("Order not found");

        order.Status = status;
        await context.SaveChangesAsync(cancellationToken);
        return order;
    }

    public static async Task<OrderDeletePayload> DeleteOrder(
        int orderId,
        ApplicationDbContext context,
        ITopicEventSender eventSender,
        CancellationToken cancellationToken
    )
    {
        var order = await context.Orders.FindAsync(orderId);
        if (order == null)
            throw new Exception("Order not found");

        context.Orders.Remove(order);
        await context.SaveChangesAsync(cancellationToken);

        await eventSender.SendAsync(
            nameof(OrderSubscriptions.OnOrderDeletedAsync),
            orderId,
            cancellationToken
        );

        return new OrderDeletePayload { OrderId = orderId, Successfull = true };
    }
}
