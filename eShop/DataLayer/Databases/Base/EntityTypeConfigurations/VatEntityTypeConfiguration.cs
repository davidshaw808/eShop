using Common.Models.Immutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Databases.Base.EntityTypeConfigurations;

internal sealed class VatEntityTypeConfiguration() : IEntityTypeConfiguration<Vat>
{
    public void Configure(EntityTypeBuilder<Vat> builder)
    {
        builder.ToTable(nameof(Vat), eShopBaseContext.ReadAndInsertOnlySchemaName);
        builder.Property(b => b.Key).HasDefaultValue(Guid.CreateVersion7());
        //build non-clustered index
        builder.HasIndex(a => a.Key);
    }
}
