using MediatR;

namespace FundiLink.Application.Features.Notifications.Commands.UpdateNotificationPreferences;

public record UpdateNotificationPreferencesCommand(
    string UserId,
    bool EmailEnabled,
    bool WhatsAppEnabled,
    bool SmsEnabled) : IRequest;
