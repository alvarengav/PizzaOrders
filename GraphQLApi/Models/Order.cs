namespace PizzaOrders.GraphQLApi.Models;


public class Order
{
    public int Id { get; set; }
    public Customer Customer { get; set; }
    public int CustomerId { get; set; }
    public PizzaDetails PizzaDetails { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Preparing;
}
