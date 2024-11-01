using PizzaOrders.GraphQLApi.Models;

namespace PizzaOrders.GraphQLApi.GraphQL.Orders;

public class UpdateOrderInput
{
    public int OrderId { get; set; }
    public OrderStatus NewStatus { get; set; }
    public PizzaDetailsInput PizzaDetails { get; set; }
}
