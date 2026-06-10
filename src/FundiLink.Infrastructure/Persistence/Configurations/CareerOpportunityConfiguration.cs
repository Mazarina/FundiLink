using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundiLink.Infrastructure.Persistence.Configurations;

public class CareerOpportunityConfiguration : IEntityTypeConfiguration<CareerOpportunity>
{
    public void Configure(EntityTypeBuilder<CareerOpportunity> builder)
    {
        builder.ToTable("CareerOpportunities");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Title).IsRequired().HasMaxLength(250);
        builder.Property(o => o.ProviderName).IsRequired().HasMaxLength(250);
        builder.Property(o => o.Description).IsRequired().HasMaxLength(4000);
        builder.Property(o => o.OpportunityType).HasConversion<string>().HasMaxLength(50);
        builder.Property(o => o.MinimumGradeLevel).HasConversion<string>().HasMaxLength(50);
        builder.Property(o => o.ExternalApplicationUrl).HasMaxLength(500);

        builder.Property(o => o.FieldsOfInterestJson)
            .HasColumnName("FieldsOfInterest")
            .IsRequired();
        builder.Property(o => o.ProvincesEligibleJson)
            .HasColumnName("ProvincesEligible")
            .IsRequired();

        builder.Ignore(o => o.FieldsOfInterest);
        builder.Ignore(o => o.ProvincesEligible);

        builder.HasQueryFilter(o => o.DeletedAt == null);
    }
}
