using FundiLink.Domain.Enums;

namespace FundiLink.Application.Features.Notifications.Queries.GetMyNotifications;

/// <summary>
/// Typed view of a single append-only notification log entry for learner/admin history.
/// </summary>
public record NotificationLogDto(
    Guid Id,
    NotificationType NotificationType,
    NotificationChannel Channel,
    NotificationStatus Status,
    DateTime SentAt,
    string? ErrorMessage);
