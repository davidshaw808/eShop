using Common.Models.Immutable;
using Common.Models.Mutable;
using DataLayer.Databases.Base.NonDomainEntites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Databases.Base.EntityTypeConfigurations;

internal sealed class AddressEntityTypeConfiguration() : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.Property(b => b.Key).HasDefaultValue(Guid.CreateVersion7());

        //build non-clustered index
        builder.HasIndex(a => a.Key);
    }
}
