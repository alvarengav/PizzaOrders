﻿namespace PizzaOrders.GraphQLApi.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public List<Order> Orders { get; set; } = new List<Order>();
}
