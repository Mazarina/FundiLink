using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundiLink.Infrastructure.Persistence.Configurations;

public class AccommodationListingConfiguration : IEntityTypeConfiguration<AccommodationListing>
{
    public void Configure(EntityTypeBuilder<AccommodationListing> builder)
    {
        builder.ToTable("AccommodationListings");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name).IsRequired().HasMaxLength(250);
        builder.Property(a => a.ProviderName).IsRequired().HasMaxLength(250);
        builder.Property(a => a.Description).IsRequired().HasMaxLength(4000);
        builder.Property(a => a.AccommodationType).HasConversion<string>().HasMaxLength(50);
        builder.Property(a => a.Province).IsRequired().HasMaxLength(100);
        builder.Property(a => a.City).IsRequired().HasMaxLength(150);
        builder.Property(a => a.NearInstitution).HasMaxLength(250);
        builder.Property(a => a.IndicativeMonthlyCost).HasColumnType("numeric(18,2)");
        builder.Property(a => a.ContactUrl).HasMaxLength(500);

        builder.HasQueryFilter(a => a.DeletedAt == null);
    }
}
