using FundiLink.Domain.Enums;

namespace FundiLink.Application.Common.Interfaces;

public interface INotificationService
{
    Task NotifyAsync(Guid learnerId, NotificationType type, string subject, string message, CancellationToken ct);
}
