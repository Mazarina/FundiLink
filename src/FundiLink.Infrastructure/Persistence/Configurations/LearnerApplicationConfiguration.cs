using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundiLink.Infrastructure.Persistence.Configurations;

public class LearnerApplicationConfiguration : IEntityTypeConfiguration<LearnerApplication>
{
    public void Configure(EntityTypeBuilder<LearnerApplication> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(a => a.Notes).HasMaxLength(2000);

        builder.HasOne(a => a.Programme)
            .WithMany()
            .HasForeignKey(a => a.ProgrammeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.LearnerId);
    }
}
