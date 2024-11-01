using Microsoft.EntityFrameworkCore;
using PizzaOrders.GraphQLApi.Models;
using PizzaOrders.GraphQLApi.Services;

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

    public override int SaveChanges()
    {
        EncryptCustomerData();
        return base.SaveChanges();
    }

    private void EncryptCustomerData()
    {
        foreach (var entry in ChangeTracker.Entries<Customer>())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                entry.Entity.Name = _encryptionService.Encrypt(entry.Entity.Name);
                entry.Entity.Phone = _encryptionService.Encrypt(entry.Entity.Phone);
                entry.Entity.Address = _encryptionService.Encrypt(entry.Entity.Address);
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        EncryptCustomerData();
        return base.SaveChangesAsync(cancellationToken);
    }
}
