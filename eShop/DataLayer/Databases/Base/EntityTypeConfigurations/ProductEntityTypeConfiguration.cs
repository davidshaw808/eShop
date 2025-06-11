using Common.Models.Mutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Databases.Base.EntityTypeConfigurations;

internal sealed class ProductEntityTypeConfiguration() : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(b => b.Key).HasDefaultValue(Guid.NewGuid());
        //build non-clustered index
        builder.HasIndex(a => a.Key);
        builder.HasMany(p => p.Reviews)
            .WithOne(r => r.Product)
            .HasForeignKey(r => r.ProductId);
    }
}
