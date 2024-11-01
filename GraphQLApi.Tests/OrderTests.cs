using CookieCrumble;
using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PizzaOrders.GraphQLApi.Data;
using PizzaOrders.GraphQLApi.Services;
using StackExchange.Redis;
using Testcontainers.MySql;
using Testcontainers.Redis;

namespace GraphQLApi.Tests;

public class FakeEncryptionService : IEncryptionService
{
    public string Encrypt(string input) => input;
    public string Decrypt(string encryptedInput) => encryptedInput;
}

public class OrderTests : IAsyncLifetime
{
    private readonly MySqlContainer _mySqlContainer = new MySqlBuilder()
        .WithImage("mysql:8.0")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:7.4")
        .Build();

    private IRequestExecutor _requestExecutor = null!;

    public async Task InitializeAsync()
    {
        await Task.WhenAll(_mySqlContainer.StartAsync(), _redisContainer.StartAsync());

        _requestExecutor = await new ServiceCollection()
            .AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    _mySqlContainer.GetConnectionString(),
                    ServerVersion.AutoDetect(_mySqlContainer.GetConnectionString())
                )
            )
            .AddScoped<IEncryptionService, FakeEncryptionService>()
            .AddGraphQLServer()
            .AddMutationConventions()
            .AddFiltering()
            .AddSorting()
             .AddRedisSubscriptions(
                _ => ConnectionMultiplexer.Connect(_redisContainer.GetConnectionString()))
            .AddTypes()
            .BuildRequestExecutorAsync();

        var dbContext = _requestExecutor
            .Services.GetApplicationServices()
            .GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    [Fact]
    public async Task CreateOrder_Test()
    {
        var result = await _requestExecutor.ExecuteAsync(
            """
            mutation CreateOrder {
              createOrder(
                input: {
                  customer: {
                    name: "Carlos Perez"
                    phone: "456-321-8080"
                    address: "123 Main St, Ciudad"
                  }
                  pizza: {
                    type: HAWAIIAN
                    size: MEDIUM
                    toppings: [PINEAPPLE, SAUSAGE, SPINACH]
                  }
                }
              ) {
                order {
                  id
                  orderDate
                  pizzaDetails {
                    type
                    size
                    toppings
                  }
                  status
                }
              }
            }
            """
        );

        result.MatchSnapshot(extension: ".json");
    }

    [Fact]
    public async Task UpdateOrder_Test()
    {
        var result = await _requestExecutor.ExecuteAsync(
            """
            mutation UpdateOrder {
              updateOrder(
                input: {
                  orderId: 1
                  newStatus: COMPLETED
                  pizzaDetails: {
                    type: MARGHERITA
                    size: LARGE
                    toppings: [MUSHROOM, BASIL]
                  }
                }
              ) {
                order {
                  id
                  status
                  pizzaDetails {
                    type
                    size
                    toppings
                  }
                }
              }
            }
            """
        );

        result.MatchSnapshot(extension: ".json");
    }

    [Fact]
    public async Task ListOrders_Test()
    {
        var result = await _requestExecutor.ExecuteAsync(
            """
            query ListOrders {
              orders {
                id
                orderDate
                customer {
                  name
                  phone
                  address
                }
                pizzaDetails {
                  type
                  size
                  toppings
                }
                status
              }
            }
            """
        );

        result.MatchSnapshot(extension: ".json");
    }

    public async Task DisposeAsync()
    {
        await _mySqlContainer.DisposeAsync();
        await _redisContainer.DisposeAsync();
    }
}
