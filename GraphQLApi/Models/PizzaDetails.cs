namespace PizzaOrders.GraphQLApi.Models;

public class PizzaDetails
{
    public PizzaType Type { get; set; }
    public PizzaSize Size { get; set; }

    public List<string> Toppings { get; set; } = new List<string>();
}
