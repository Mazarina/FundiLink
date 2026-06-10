using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundiLink.Infrastructure.Persistence.Configurations;

public class CareerInterestConfiguration : IEntityTypeConfiguration<CareerInterest>
{
    public void Configure(EntityTypeBuilder<CareerInterest> builder)
    {
        builder.ToTable("CareerInterests");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(i => i.Notes).HasMaxLength(2000);

        builder.HasOne(i => i.CareerOpportunity)
            .WithMany()
            .HasForeignKey(i => i.CareerOpportunityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => new { i.LearnerId, i.CareerOpportunityId });

        builder.HasQueryFilter(i => i.DeletedAt == null);
    }
}
