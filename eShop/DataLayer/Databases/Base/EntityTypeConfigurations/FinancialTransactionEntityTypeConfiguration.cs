using Common.Models.Immutable;
using Common.Models.Mutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Databases.Base.EntityTypeConfigurations;

internal sealed class FinancialTransactionEntityTypeConfiguration() : IEntityTypeConfiguration<FinancialTransaction>
{
    public void Configure(EntityTypeBuilder<FinancialTransaction> builder)
    {
        builder.ToTable(nameof(FinancialTransaction), eShopBaseContext.ReadAndInsertOnlySchemaName );
        builder.Property(b => b.Key).HasDefaultValue(Guid.NewGuid());
        //build non-clustered index
        builder.HasIndex(a => a.Key);
    }
}
