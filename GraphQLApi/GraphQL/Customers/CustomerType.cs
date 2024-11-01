using GreenDonut.Selectors;
using HotChocolate.Execution.Processing;
using PizzaOrders.GraphQLApi.GraphQL.Orders;
using PizzaOrders.GraphQLApi.Models;

namespace PizzaOrders.GraphQLApi.GraphQL.Customers;

[ObjectType<Customer>]
public static partial class CustomerType
{
    [UseFiltering]
    public static async Task<IEnumerable<Order>> GetOrders(
        [Parent] Customer customer,
        IOrdersByCustomerIdDataLoader ordersByCustomerId,
        ISelection selection,
        CancellationToken cancellationToken
    )
    {
        return await ordersByCustomerId.Select(selection).LoadAsync(customer.Id, cancellationToken);
    }
}
