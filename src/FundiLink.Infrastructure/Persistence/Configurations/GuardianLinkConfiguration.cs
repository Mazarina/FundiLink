using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundiLink.Infrastructure.Persistence.Configurations;

public class GuardianLinkConfiguration : IEntityTypeConfiguration<GuardianLink>
{
    public void Configure(EntityTypeBuilder<GuardianLink> builder)
    {
        builder.ToTable("GuardianLinks");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.LearnerId).IsRequired();
        builder.Property(l => l.GuardianUserId).IsRequired().HasMaxLength(450);
        builder.Property(l => l.GuardianName).IsRequired().HasMaxLength(200);
        builder.Property(l => l.GuardianContact).HasMaxLength(200);

        builder.HasIndex(l => new { l.GuardianUserId, l.LearnerId }).IsUnique();

        builder.HasQueryFilter(l => l.DeletedAt == null);
    }
}
