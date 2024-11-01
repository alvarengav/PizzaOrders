namespace PizzaOrders.GraphQLApi.GraphQL.Orders;

public class OrderDeletePayload
{
    public int OrderId { get; set; }
    public bool Successfull { get; set; }
}
