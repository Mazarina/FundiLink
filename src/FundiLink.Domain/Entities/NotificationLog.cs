using FundiLink.Domain.Enums;

namespace FundiLink.Domain.Entities;

/// <summary>
/// Append-only record of a notification delivery attempt.
/// Immutable after creation — never updated or deleted.
/// </summary>
public class NotificationLog
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid LearnerId { get; private set; }
    public NotificationType NotificationType { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public string Recipient { get; private set; } = default!;
    public NotificationStatus Status { get; private set; }
    public DateTime SentAt { get; private set; }
    public string? ErrorMessage { get; private set; }

    private NotificationLog() { }

    public static NotificationLog Create(
        Guid learnerId,
        NotificationType type,
        NotificationChannel channel,
        string recipient,
        NotificationStatus status,
        string? errorMessage = null)
    {
        return new NotificationLog
        {
            Id = Guid.NewGuid(),
            LearnerId = learnerId,
            NotificationType = type,
            Channel = channel,
            Recipient = recipient,
            Status = status,
            SentAt = DateTime.UtcNow,
            ErrorMessage = errorMessage
        };
    }
}
