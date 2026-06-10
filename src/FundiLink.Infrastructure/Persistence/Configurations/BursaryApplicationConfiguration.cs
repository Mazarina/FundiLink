using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundiLink.Infrastructure.Persistence.Configurations;

public class BursaryApplicationConfiguration : IEntityTypeConfiguration<BursaryApplication>
{
    public void Configure(EntityTypeBuilder<BursaryApplication> builder)
    {
        builder.ToTable("BursaryApplications");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(a => a.Notes).HasMaxLength(2000);

        builder.HasOne(a => a.Bursary)
            .WithMany()
            .HasForeignKey(a => a.BursaryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.LearnerId);

        // Soft-delete query filter.
        builder.HasQueryFilter(a => a.DeletedAt == null);
    }
}
