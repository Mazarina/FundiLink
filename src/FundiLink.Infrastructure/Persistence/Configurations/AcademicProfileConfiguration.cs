using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundiLink.Infrastructure.Persistence.Configurations;

public class AcademicProfileConfiguration : IEntityTypeConfiguration<AcademicProfile>
{
    public void Configure(EntityTypeBuilder<AcademicProfile> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.ResultType).HasConversion<string>();

        builder.HasMany(a => a.Subjects)
            .WithOne(s => s.AcademicProfile)
            .HasForeignKey(s => s.AcademicProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
