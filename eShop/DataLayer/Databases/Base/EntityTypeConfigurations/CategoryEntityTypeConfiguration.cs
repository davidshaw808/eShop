using Common.Models.Mutable;
using DataLayer.Databases.Base.NonDomainEntites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.Databases.Base.EntityTypeConfigurations;

internal sealed class CategoryEntityTypeConfiguration() : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(b => b.Key).HasDefaultValue(Guid.CreateVersion7());
        //build non-clustered index
        builder.HasIndex(a => a.Key);

        builder
            .HasOne(c => c.Parent)
            .WithMany()
            .HasForeignKey("ParentId");//shadow state prop

        builder
            .HasMany(c => c.Products)
            .WithMany(p => p.Categories)
            .UsingEntity<CategoryProduct>();
    }
}
