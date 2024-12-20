using Common;
using Common.Interface;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Databases.Base;

public class eShopBaseContext : DbContext
{
    internal DbSet<Address> Addresses { get; set; }
    internal DbSet<Category> Categories { get; set; }
    internal DbSet<Customer> Customers { get; set; }
    internal DbSet<Order> Orders { get; set; }
    internal DbSet<Product> Products { get; set; }
    internal DbSet<FinancialTransaction> FinancialTransactions { get; set; }
    internal DbSet<HistoryLog> HistoryLogs { get; set; }
    internal DbSet<OrderUpdate> OrderUpdates { get; set; }
    internal DbSet<PaymentDetails> PaymentDetails { get; set; }
    internal DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.ApplyConfigurationsFromAssembly
        base.OnModelCreating(modelBuilder);
        //build non-clustered indexs
        modelBuilder.Entity<Customer>()
            .HasIndex(a => a.Key);
        modelBuilder.Entity<Customer>()
            .HasIndex(a => a.Email);
        modelBuilder.Entity<FinancialTransaction>()
            .HasIndex(a => a.Key);
        modelBuilder.Entity<Product>()
            .HasIndex(a => a.Key);
        modelBuilder.Entity<Address>()
            .HasIndex(a => a.Key);

        modelBuilder.Entity<Category>()
            .HasOne(c => c.Parent)
            .WithMany();

        modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne();

        modelBuilder.Entity<Customer>()
            .HasMany(c => c.OrderHistory)
            .WithOne(o => o.Customer);
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Basket)
            .WithOne();
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Products)
            .WithMany()
            .UsingEntity("OrderProducts");
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Address)
            .WithOne()
            .HasForeignKey<Order>("AddressId");
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.OrderHistory);
        modelBuilder.Entity<Order>()
           .HasMany(o => o.Updates)
           .WithOne();
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Refunds)
            .WithOne(r => r.Order);
        modelBuilder.Entity<Order>()
           .HasMany(o => o.PaymentDetails)
           .WithOne()
           .HasForeignKey("PaymentId");
        modelBuilder.Entity<Product>()
            .HasMany(p => p.Reviews)
            .WithOne(r => r.Product)
            .HasForeignKey(r => r.ProductId);
    }
}
