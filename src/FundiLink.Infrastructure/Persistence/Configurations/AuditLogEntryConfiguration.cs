using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundiLink.Infrastructure.Persistence.Configurations;

public class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.ToTable("AuditLogEntries");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.ActorUserId).IsRequired().HasMaxLength(450);
        builder.Property(a => a.ActorRole).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Action).IsRequired().HasMaxLength(100);
        builder.Property(a => a.TargetType).IsRequired().HasMaxLength(100);
        builder.Property(a => a.TargetId).IsRequired().HasMaxLength(450);
        // No query filter — audit log is never soft-deleted.
    }
}
