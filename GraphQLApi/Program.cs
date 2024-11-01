using PizzaOrders.GraphQLApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddGraphQLConfiguration();

var app = builder.Build();

app.UseWebSockets();
app.MapGraphQL();


app.RunWithGraphQLCommands(args);
