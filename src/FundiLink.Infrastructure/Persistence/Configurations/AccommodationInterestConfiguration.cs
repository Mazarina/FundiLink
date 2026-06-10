using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundiLink.Infrastructure.Persistence.Configurations;

public class AccommodationInterestConfiguration : IEntityTypeConfiguration<AccommodationInterest>
{
    public void Configure(EntityTypeBuilder<AccommodationInterest> builder)
    {
        builder.ToTable("AccommodationInterests");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(i => i.Notes).HasMaxLength(2000);

        builder.HasOne(i => i.AccommodationListing)
            .WithMany()
            .HasForeignKey(i => i.AccommodationListingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => new { i.LearnerId, i.AccommodationListingId });

        builder.HasQueryFilter(i => i.DeletedAt == null);
    }
}
