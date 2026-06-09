using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundiLink.Infrastructure.Persistence.Configurations;

public class InstitutionConfiguration : IEntityTypeConfiguration<Institution>
{
    public void Configure(EntityTypeBuilder<Institution> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Name).IsRequired().HasMaxLength(250);
        builder.Property(i => i.InstitutionType).HasConversion<string>().HasMaxLength(50);
        builder.Property(i => i.Province).IsRequired().HasMaxLength(100);
        builder.Property(i => i.Website).HasMaxLength(500);

        builder.HasMany(i => i.Programmes)
            .WithOne(p => p.Institution)
            .HasForeignKey(p => p.InstitutionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
