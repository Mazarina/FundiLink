using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundiLink.Infrastructure.Persistence.Configurations;

public class AssistantInteractionLogConfiguration : IEntityTypeConfiguration<AssistantInteractionLog>
{
    public void Configure(EntityTypeBuilder<AssistantInteractionLog> builder)
    {
        builder.ToTable("AssistantInteractionLogs");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.LearnerId).IsRequired();
        builder.Property(l => l.Intent).IsRequired().HasConversion<string>().HasMaxLength(50);
        builder.Property(l => l.CreatedAt).IsRequired();
        // POPIA-minimal: stores only learner, intent, and timestamp — no answer text.
        // No query filter — assistant interaction log is append-only and never soft-deleted.
    }
}
