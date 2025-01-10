using Common;
using Common.Interface;
using DataLayer.Databases.Base.NonDomainEntites;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Databases.Base;
/***********************************************************************/
/*TODO's
 * figure out the tables that require super access and plonk them
 under a different schema
*/
/***********************************************************************/

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

    //join table entities
    internal DbSet<OrderProduct> OrderProducts { get; set; }
    internal DbSet<CategoryProducts> CategoryProducts { get; set; }

    protected const string OrderProductsTableName = "OrderProducts";
    protected const string ProductCategoriesTableName = "ProductCategories";
    protected const string ProductForeignKeyName = "ProductId";
    protected const string OrderForeignKeyName = "OrderId";
    protected const string CategoryForeignKeyName = "CategoryId";
    protected const string AddressForeignKeyName = "AddressId";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.ApplyConfigurationsFromAssembly
        base.OnModelCreating(modelBuilder);

        var entityAddress = modelBuilder.Entity<Address>();
        var entityCategory = modelBuilder.Entity<Category>();
        var entityCustomer = modelBuilder.Entity<Customer>();
        var entityOrder = modelBuilder.Entity<Order>();
        var entityProduct = modelBuilder.Entity<Product>();
        var entityFinancialTransaction = modelBuilder.Entity<FinancialTransaction>();
        var entityHistoryLog = modelBuilder.Entity<HistoryLog>();
        var entityOrderUpdate = modelBuilder.Entity<OrderUpdate>();
        var entityPaymentDetails = modelBuilder.Entity<PaymentDetails>();
        var entityReview = modelBuilder.Entity<Review>();

        entityAddress.Property(b => b.Key)
        .HasDefaultValue(Guid.NewGuid());
        entityCategory.Property(b => b.Key)
        .HasDefaultValue(Guid.NewGuid());
        entityCustomer.Property(b => b.Key)
        .HasDefaultValue(Guid.NewGuid());
        entityOrder.Property(b => b.Key)
        .HasDefaultValue(Guid.NewGuid());
        entityProduct.Property(b => b.Key)
        .HasDefaultValue(Guid.NewGuid());
        entityFinancialTransaction.Property(b => b.Key)
        .HasDefaultValue(Guid.NewGuid());
        entityHistoryLog.Property(b => b.Key)
        .HasDefaultValue(Guid.NewGuid());
        entityReview.Property(b => b.Key)
        .HasDefaultValue(Guid.NewGuid());

        //build non-clustered indexs
        entityAddress.HasIndex(a => a.Key);
        entityCategory.HasIndex(a => a.Key);
        entityCustomer.HasIndex(a => a.Key);
        entityCustomer.HasIndex(a => a.Email);
        entityOrder.HasIndex(a => a.Key);
        entityProduct.HasIndex(a => a.Key);
        entityFinancialTransaction.HasIndex(a => a.Key);
        entityHistoryLog.HasIndex(a => a.Key);
        entityReview.HasIndex(a => a.Key);
       
        entityCategory
            .HasOne(c => c.Parent)
            .WithMany();

        entityCategory
            .HasMany(c => c.Products)
            .WithMany(p => p.Categories)
            .UsingEntity<CategoryProducts>();
          /*  .UsingEntity(
                ProductCategoriesTableName,
                rgt => rgt.HasOne(typeof(Product)).WithMany().HasForeignKey(ProductForeignKeyName).HasPrincipalKey(nameof(Product.Id)),
                lft => lft.HasOne(typeof(Category)).WithMany().HasForeignKey(CategoryForeignKeyName).HasPrincipalKey(nameof(Category.Id)),
                join => join.HasKey(ProductForeignKeyName, CategoryForeignKeyName));*/

        entityCustomer
            .HasMany(c => c.OrderHistory)
            .WithOne(o => o.Customer);
        entityCustomer
            .HasMany(c => c.Basket)
            .WithOne();

        entityOrder
            .HasMany(o => o.Products)
            .WithMany()
            .UsingEntity<OrderProduct>();
        entityOrder
            .HasOne(o => o.Address)
            .WithOne()
            .HasForeignKey<Order>(AddressForeignKeyName);
        entityOrder
            .HasOne(o => o.Customer)
            .WithMany(c => c.OrderHistory)
            .HasForeignKey("CustomerId");
        entityOrder
           .HasMany(o => o.Updates)
           .WithOne(u => u.Order);
        entityOrder
            .HasMany(o => o.Refunds)
            .WithOne(r => r.Order);
        entityOrder
           .HasMany(o => o.PaymentDetails)
           .WithOne(p => p.Order)
           .HasForeignKey("OrderId");
        entityProduct
            .HasMany(p => p.Reviews)
            .WithOne(r => r.Product)
            .HasForeignKey(r => r.ProductId);
    }
}
