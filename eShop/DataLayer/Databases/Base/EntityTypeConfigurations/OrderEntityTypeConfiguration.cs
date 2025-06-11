using Common.Models.Immutable;
using DataLayer.Databases.Base.NonDomainEntites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Databases.Base.EntityTypeConfigurations;
internal sealed class OrderEntityTypeConfiguration() : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable(nameof(Order), eShopBaseContext.ReadAndInsertOnlySchemaName);

        builder.Property(b => b.Key).HasDefaultValue(Guid.NewGuid());

        //build non-clustered index
        builder.HasIndex(a => a.Key);

        builder
            .HasMany(o => o.Products)
            .WithMany()
            .UsingEntity<OrderProduct>();
        builder
            .HasOne(o => o.Customer)
            .WithMany(c => c.OrderHistory)
            .HasForeignKey("CustomerId");//shadow state prop
        builder
           .HasMany(o => o.Updates)
           .WithOne(u => u.Order);
        builder
            .HasMany(o => o.Payments)
            .WithOne(r => r.Order);
        builder
            .HasOne(o => o.Shipping)
            .WithMany()
            .HasForeignKey("ShippingId");
        builder
            .HasOne(o => o.VatApplied)
            .WithMany()
            .HasForeignKey("VatId");
    }
}

