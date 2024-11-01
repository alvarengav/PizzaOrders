using GreenDonut.Selectors;
using Microsoft.EntityFrameworkCore;
using PizzaOrders.GraphQLApi.Data;
using PizzaOrders.GraphQLApi.Models;

namespace PizzaOrders.GraphQLApi.GraphQL.Customers;

public class CustomerDataLoader
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Customer>> CustomerByOrderId(
        IReadOnlyList<int> ids,
        ApplicationDbContext dbContext,
        ISelectorBuilder selector,
        CancellationToken cancellationToken
    )
    {
        return await dbContext
            .Customers.AsNoTracking()
            .Where(c => ids.Contains(c.Id))
            .Select(c => c.Id, selector)
            .ToDictionaryAsync(c => c.Id, cancellationToken);
    }
}
