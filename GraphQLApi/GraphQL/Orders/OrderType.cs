using GreenDonut.Selectors;
using HotChocolate.Execution.Processing;
using PizzaOrders.GraphQLApi.GraphQL.Customers;
using PizzaOrders.GraphQLApi.Models;

namespace PizzaOrders.GraphQLApi.GraphQL.Orders;

[ObjectType<Order>]
public static partial class OrderType
{
    public static async Task<Customer?> GetCustomer(
        [Parent] Order order,
        ICustomerByOrderIdDataLoader customerByOrderId,
        ISelection selection,
        CancellationToken cancellationToken
    )
    {
        return await customerByOrderId.Select(selection).LoadRequiredAsync(order.CustomerId, cancellationToken);
    }
}
