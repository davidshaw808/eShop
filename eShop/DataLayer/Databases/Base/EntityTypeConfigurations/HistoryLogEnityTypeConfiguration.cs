using Common.Models.Immutable;
using Common.Models.Mutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Databases.Base.EntityTypeConfigurations;

internal sealed class HistoryLogEntityTypeConfiguration() : IEntityTypeConfiguration<HistoryLog>
{
    public void Configure(EntityTypeBuilder<HistoryLog> builder)
    {
        builder.ToTable(nameof(HistoryLog), eShopBaseContext.ReadAndInsertOnlySchemaName);
        builder.Property(b => b.Key).HasDefaultValue(Guid.CreateVersion7());
        //build non-clustered index
        builder.HasIndex(a => a.Key);
    }
}
