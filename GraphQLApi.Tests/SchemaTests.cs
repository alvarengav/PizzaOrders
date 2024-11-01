using CookieCrumble;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using PizzaOrders.GraphQLApi.Data;
using PizzaOrders.GraphQLApi.Services;

namespace GraphQLApi.Tests;

public class SchemaTests
{
    [Fact]
    public async Task SchemaChanged()
    {
        // Arrange & act
        var schema = await new ServiceCollection()
            .AddDbContext<ApplicationDbContext>()
            .AddScoped<IEncryptionService, DataProtectionEncryptionService>()
            .AddGraphQLServer()
            .AddMutationConventions()
            .AddFiltering()
            .AddSorting()
            .AddInMemorySubscriptions()
            .AddTypes()
            .BuildSchemaAsync();

        // Assert
        schema.MatchSnapshot();
    }
}
