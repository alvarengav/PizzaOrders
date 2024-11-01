using PizzaOrders.GraphQLApi.GraphQL.Customers;

namespace PizzaOrders.GraphQLApi.GraphQL.Orders;

public class CreateOrderInput
{
    public CustomerInput Customer { get; set; }
    public PizzaDetailsInput Pizza { get; set; }
}
