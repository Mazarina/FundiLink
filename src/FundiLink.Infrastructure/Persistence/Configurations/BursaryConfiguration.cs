using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundiLink.Infrastructure.Persistence.Configurations;

public class BursaryConfiguration : IEntityTypeConfiguration<Bursary>
{
    public void Configure(EntityTypeBuilder<Bursary> builder)
    {
        builder.ToTable("Bursaries");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name).IsRequired().HasMaxLength(250);
        builder.Property(b => b.ProviderName).IsRequired().HasMaxLength(250);
        builder.Property(b => b.Description).IsRequired().HasMaxLength(4000);
        builder.Property(b => b.FundingType).HasConversion<string>().HasMaxLength(50);
        builder.Property(b => b.ExternalApplicationUrl).HasMaxLength(500);
        builder.Property(b => b.MaxHouseholdIncome).HasColumnType("numeric(18,2)");

        builder.Property(b => b.FieldsOfStudyJson)
            .HasColumnName("FieldsOfStudy")
            .IsRequired();
        builder.Property(b => b.ProvincesEligibleJson)
            .HasColumnName("ProvincesEligible")
            .IsRequired();

        builder.Ignore(b => b.FieldsOfStudy);
        builder.Ignore(b => b.ProvincesEligible);

        // Soft-delete query filter.
        builder.HasQueryFilter(b => b.DeletedAt == null);
    }
}
