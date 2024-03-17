using Common;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Databases.Base
{
    public class eShopBaseContext : DbContext
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<RefundRequest> RefundRequests { get; set; }
        public DbSet<PaymentRequest> PaymentRequests { get; set; }
        public DbSet<HistoryLog> HistoryLogs { get; set; }
        public DbSet<OrderUpdate> OrderUpdates { get; set; }
        public DbSet<PaymentDetails> PaymentDetails { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasIndex(a => a.AltId);
            modelBuilder.Entity<Customer>()
                .HasIndex(a => a.Email);
            modelBuilder.Entity<RefundRequest>()
                .HasIndex(a => a.AltId);
            modelBuilder.Entity<Product>()
                .HasIndex(a => a.AltId);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.Parent)
                .WithMany()
                .HasForeignKey(a => a.ParentId);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne();

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Address)
                .WithMany();
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
}
