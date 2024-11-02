using Microsoft.EntityFrameworkCore;
using PizzaOrders.GraphQLApi.Models;
using PizzaOrders.GraphQLApi.Services;
using PizzaOrders.GraphQLApi.Utils;

namespace PizzaOrders.GraphQLApi.Data;

public sealed class ApplicationDbContext : DbContext
{
    private readonly IEncryptionService _encryptionService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IEncryptionService encryptionService
    )
        : base(options)
    {
        _encryptionService = encryptionService;
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var converter = new EncryptedConverter(_encryptionService);

        modelBuilder.Entity<Customer>().Property(c => c.Name);
        modelBuilder.Entity<Customer>().Property(c => c.Phone).HasConversion(converter);
        modelBuilder.Entity<Customer>().Property(c => c.Address).HasConversion(converter);

        modelBuilder.Entity<Order>().HasOne(o => o.Customer).WithMany(c => c.Orders).IsRequired();

        modelBuilder.Entity<Order>().Property(o => o.Status).HasConversion<string>();

        modelBuilder
            .Entity<Order>()
            .OwnsOne(
                o => o.PizzaDetails,
                pd =>
                {
                    pd.Property(p => p.Type).HasConversion<string>();
                    pd.Property(p => p.Size).HasConversion<string>();
                    pd.Property(p => p.Toppings);
                }
            );
    }
}
