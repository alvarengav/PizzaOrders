using Microsoft.EntityFrameworkCore;
using PizzaOrders.GraphQLApi.Data;
using PizzaOrders.GraphQLApi.Models;

namespace PizzaOrders.GraphQLApi.GraphQL.Customers;

[QueryType]
public static class CustomerQueries
{
    [UseFiltering]
    public static IQueryable<Customer> GetCustomers(
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken
    ) => dbContext.Customers.AsNoTracking();
}
