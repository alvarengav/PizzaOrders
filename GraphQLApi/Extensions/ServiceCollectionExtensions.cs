using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using PizzaOrders.GraphQLApi.Data;
using PizzaOrders.GraphQLApi.Services;
using StackExchange.Redis;

namespace PizzaOrders.GraphQLApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"./keys"));
        services.AddScoped<IEncryptionService, DataProtectionEncryptionService>();

        return services;
    }

    public static IServiceCollection AddDatabaseConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var serverVersion = ServerVersion.AutoDetect(connectionString);
            options.UseMySql(connectionString, serverVersion);
        });

        return services;
    }

    public static IServiceCollection AddGraphQLConfiguration(this IServiceCollection services)
    {
        services
            .AddGraphQLServer()
            .AddMutationConventions(applyToAllMutations: true)
            .AddFiltering()
            .AddRedisSubscriptions(_ => ConnectionMultiplexer.Connect("redis:6379"))
            .AddTypes();

        return services;
    }
}
