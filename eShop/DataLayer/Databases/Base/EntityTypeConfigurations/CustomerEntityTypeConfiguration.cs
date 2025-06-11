using Common.Models.Mutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Databases.Base.EntityTypeConfigurations;
internal sealed class CustomerEntityTypeConfiguration() : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        var fk_name = "CustomerId";//shadow state prop

        builder.Property(b => b.Key)
            .HasDefaultValue(Guid.NewGuid());
        builder
            .HasMany(c => c.OrderHistory)
            .WithOne(o => o.Customer)
            .HasForeignKey(fk_name);
        builder
            .HasMany(c => c.BasketItems)
            .WithOne(bi => bi.Customer)
            .HasForeignKey(fk_name);
        builder
            .HasOne(c => c.Address)
            .WithMany()
            .HasForeignKey(fk_name);

        //build non-clustered index
        builder.HasIndex(a => a.Key);
        builder.HasIndex(a => a.Email);
    }
}

