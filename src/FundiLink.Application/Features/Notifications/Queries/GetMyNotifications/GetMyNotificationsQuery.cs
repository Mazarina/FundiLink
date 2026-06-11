using MediatR;

namespace FundiLink.Application.Features.Notifications.Queries.GetMyNotifications;

/// <summary>
/// Owner-scoped notification history for the authenticated learner. Reads the append-only
/// <c>NotificationLog</c>; surfaces no PII beyond the existing log fields. The recipient
/// address is deliberately omitted from the DTO.
/// </summary>
public record GetMyNotificationsQuery(string UserId)
    : IRequest<IReadOnlyList<NotificationLogDto>>;
