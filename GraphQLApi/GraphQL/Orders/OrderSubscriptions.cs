using PizzaOrders.GraphQLApi.Models;

namespace PizzaOrders.GraphQLApi.GraphQL.Orders;

[SubscriptionType]
public static class OrderSubscriptions
{
    [Subscribe]
    [Topic]
    public static async Task<Order> OnOrderCreatedAsync(
        [EventMessage] int orderId,
        IOrderByIdDataLoader orderById,
        CancellationToken cancellationToken
    )
    {
        return await orderById.LoadRequiredAsync(orderId, cancellationToken);
    }

    [Subscribe]
    [Topic]
    public static async Task<Order> OnOrderUpdatedAsync(
        [EventMessage] int orderId,
        IOrderByIdDataLoader orderById,
        CancellationToken cancellationToken
    )
    {
        return await orderById.LoadRequiredAsync(orderId, cancellationToken);
    }

    [Subscribe]
    [Topic]
    public static async Task<OrderDeletePayload> OnOrderDeletedAsync(
        [EventMessage] int orderId,
        IOrderByIdDataLoader orderById,
        CancellationToken cancellationToken
    )
    {
        return await Task.FromResult(
            new OrderDeletePayload { OrderId = orderId, Successfull = true }
        );
    }
}
