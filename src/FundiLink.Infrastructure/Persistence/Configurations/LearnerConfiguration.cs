using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundiLink.Infrastructure.Persistence.Configurations;

public class LearnerConfiguration : IEntityTypeConfiguration<Learner>
{
    public void Configure(EntityTypeBuilder<Learner> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.UserId).IsRequired().HasMaxLength(450);
        builder.HasIndex(l => l.UserId).IsUnique();
        builder.Property(l => l.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(l => l.Surname).IsRequired().HasMaxLength(100);
        builder.Property(l => l.MobileNumber).IsRequired().HasMaxLength(20);
        builder.Property(l => l.Province).IsRequired().HasMaxLength(50);
        builder.Property(l => l.Municipality).HasMaxLength(100);
        builder.Property(l => l.Suburb).HasMaxLength(100);
        builder.Property(l => l.SchoolName).IsRequired().HasMaxLength(200);
        builder.Property(l => l.SchoolProvince).IsRequired().HasMaxLength(50);
        builder.Property(l => l.Nationality).IsRequired().HasMaxLength(50);
        builder.Property(l => l.IdNumber).HasMaxLength(13);
        builder.Property(l => l.PassportNumber).HasMaxLength(50);
        builder.Property(l => l.Gender).HasMaxLength(20);
        builder.Property(l => l.HomeLanguage).HasMaxLength(50);
        builder.Property(l => l.GuardianName).HasMaxLength(200);
        builder.Property(l => l.GuardianPhone).HasMaxLength(20);
        builder.Property(l => l.GuardianEmail).HasMaxLength(200);
        builder.Property(l => l.ConsentVersion).IsRequired().HasMaxLength(20);
        builder.Property(l => l.GradeLevel).HasConversion<string>();

        builder.HasOne(l => l.AcademicProfile)
            .WithOne(a => a.Learner)
            .HasForeignKey<AcademicProfile>(a => a.LearnerId);

        builder.HasQueryFilter(l => l.DeletedAt == null);
    }
}
