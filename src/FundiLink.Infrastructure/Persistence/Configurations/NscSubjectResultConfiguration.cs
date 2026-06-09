using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundiLink.Infrastructure.Persistence.Configurations;

public class NscSubjectResultConfiguration : IEntityTypeConfiguration<NscSubjectResult>
{
    public void Configure(EntityTypeBuilder<NscSubjectResult> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.SubjectName).IsRequired().HasMaxLength(100);
        builder.Property(s => s.SubjectCode).HasMaxLength(20);
    }
}
