using GreenDonut.Selectors;
using Microsoft.EntityFrameworkCore;
using PizzaOrders.GraphQLApi.Data;
using PizzaOrders.GraphQLApi.Models;

namespace PizzaOrders.GraphQLApi.GraphQL.Orders;

public static class OrderDataLoaders
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Order>> OrderByIdAsync(
        IReadOnlyList<int> ids,
        ApplicationDbContext dbContext,
        ISelectorBuilder selector,
        CancellationToken cancellationToken
    )
    {
        return await dbContext
            .Orders.AsNoTracking()
            .Where(o => ids.Contains(o.Id))
            .Select(o => o.Id, selector)
            .ToDictionaryAsync(o => o.Id, cancellationToken);
    }

    [DataLoader]
    public static async Task<IReadOnlyDictionary<int, Order[]>> OrdersByCustomerId(
        IReadOnlyList<int> ids,
        ApplicationDbContext dbContext,
        ISelectorBuilder selector,
        CancellationToken cancellationToken
    )
    {
        return await dbContext
            .Orders.AsNoTracking()
            .Where(o => ids.Contains(o.CustomerId))
            .Select(o => o.CustomerId, selector)
            .GroupBy(o => o.CustomerId)
            .ToDictionaryAsync(g => g.Key, g => g.ToArray(), cancellationToken);
    }
}
