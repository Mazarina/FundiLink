using FundiLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FundiLink.Infrastructure.Persistence.Configurations;

public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
{
    public void Configure(EntityTypeBuilder<NotificationLog> builder)
    {
        builder.ToTable("NotificationLogs");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.LearnerId).IsRequired();
        builder.Property(l => l.NotificationType).IsRequired().HasConversion<string>().HasMaxLength(50);
        builder.Property(l => l.Channel).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(l => l.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(l => l.Recipient).IsRequired().HasMaxLength(256);
        builder.Property(l => l.SentAt).IsRequired();
        builder.Property(l => l.ErrorMessage).HasMaxLength(1000);
        // No query filter — notification log is append-only and never soft-deleted.
    }
}
