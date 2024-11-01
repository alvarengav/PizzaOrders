using PizzaOrders.GraphQLApi.Models;

namespace PizzaOrders.GraphQLApi.GraphQL.Orders;

public class PizzaDetailsInput
{
    public PizzaType Type { get; set; }
    public PizzaSize Size { get; set; }
    public List<ToppingEnum> Toppings { get; set; }
}
