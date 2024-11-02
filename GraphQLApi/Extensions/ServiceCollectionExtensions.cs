using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using PizzaOrders.GraphQLApi.Data;
using PizzaOrders.GraphQLApi.Services;
using StackExchange.Redis;

namespace PizzaOrders.GraphQLApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var keysPath = configuration["DataProtection:KeysPath"];

        services
            .AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
            .SetApplicationName("PizzaOrdersApp");
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

    public static IServiceCollection AddGraphQLConfiguration(this IServiceCollection services, IWebHostEnvironment env)
    {
        var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST");
        var redisPort = Environment.GetEnvironmentVariable("REDIS_PORT");
        var redisPassword = Environment.GetEnvironmentVariable("REDIS_PASSWORD");

        var configurationOptions = new ConfigurationOptions
        {
            EndPoints = { $"{redisHost}:{redisPort}" },
            AbortOnConnectFail = false,
            Ssl = false
        };

        if (env.IsProduction())
        {
            configurationOptions.Password = redisPassword;
            configurationOptions.Ssl = true;
        }

        services
            .AddGraphQLServer()
            .AddMutationConventions(applyToAllMutations: true)
            .AddFiltering()
            .AddRedisSubscriptions(_ => ConnectionMultiplexer.Connect(configurationOptions))
            .AddTypes();

        return services;
    }
}
