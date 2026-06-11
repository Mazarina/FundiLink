using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundiLink.Infrastructure.Persistence.Configurations;

public class ErasureRequestConfiguration : IEntityTypeConfiguration<ErasureRequest>
{
    public void Configure(EntityTypeBuilder<ErasureRequest> builder)
    {
        builder.ToTable("ErasureRequests");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.LearnerId).IsRequired();
        builder.Property(r => r.RequestedByUserId).IsRequired().HasMaxLength(450);
        builder.Property(r => r.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.Reason).HasMaxLength(1000);
        builder.Property(r => r.RequestedAt).IsRequired();
        builder.Property(r => r.ReviewedByUserId).HasMaxLength(450);
        builder.Property(r => r.ReviewNote).HasMaxLength(1000);

        builder.HasIndex(r => new { r.LearnerId, r.Status });
        builder.HasIndex(r => r.Status);

        // Erasure requests are tracked, not append-only — they progress through their
        // lifecycle in place. The full transition history is captured in the audit log.
    }
}
