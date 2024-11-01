using PizzaOrders.GraphQLApi.Data;
using PizzaOrders.GraphQLApi.Models;

namespace PizzaOrders.GraphQLApi.GraphQL.Orders;

[QueryType]
public static class OrderQueries
{
    [UseFiltering]
    public static IQueryable<Order> GetOrders(ApplicationDbContext dbContext) => dbContext.Orders;
}
