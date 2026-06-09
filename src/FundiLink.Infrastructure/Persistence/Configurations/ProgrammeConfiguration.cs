using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundiLink.Infrastructure.Persistence.Configurations;

public class ProgrammeConfiguration : IEntityTypeConfiguration<Programme>
{
    public void Configure(EntityTypeBuilder<Programme> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(250);
        builder.Property(p => p.FacultyOrSchool).HasMaxLength(250);
        builder.Property(p => p.RequiredSubjectsJson)
            .HasColumnName("RequiredSubjects")
            .IsRequired();

        // Ignore the computed deserialized helper property.
        builder.Ignore(p => p.RequiredSubjects);
    }
}
