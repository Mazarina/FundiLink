using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundiLink.Infrastructure.Persistence.Configurations;

public class GuardianConsentConfiguration : IEntityTypeConfiguration<GuardianConsent>
{
    public void Configure(EntityTypeBuilder<GuardianConsent> builder)
    {
        builder.ToTable("GuardianConsents");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.LearnerId).IsRequired();
        builder.Property(c => c.ConsentType).IsRequired().HasConversion<string>().HasMaxLength(50);
        builder.Property(c => c.Scope).IsRequired().HasConversion<string>().HasMaxLength(50);
        builder.Property(c => c.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.GuardianName).IsRequired().HasMaxLength(200);
        builder.Property(c => c.GuardianContact).IsRequired().HasMaxLength(200);
        builder.Property(c => c.RecordedAt).IsRequired();

        builder.HasIndex(c => new { c.LearnerId, c.ConsentType, c.RecordedAt });

        // Append-only: grants and revocations are both inserted, never updated or deleted.
        // No soft-delete query filter — the full consent history is always preserved (POPIA).
    }
}
