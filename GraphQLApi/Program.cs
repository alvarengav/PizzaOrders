using Microsoft.EntityFrameworkCore;
using PizzaOrders.GraphQLApi.Data;
using PizzaOrders.GraphQLApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddGraphQLConfiguration();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);


var app = builder.Build();

app.UseWebSockets();
app.MapGraphQL();


app.RunWithGraphQLCommands(args);
