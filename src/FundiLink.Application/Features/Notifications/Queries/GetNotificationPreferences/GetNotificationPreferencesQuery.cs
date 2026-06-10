using MediatR;

namespace FundiLink.Application.Features.Notifications.Queries.GetNotificationPreferences;

public record NotificationPreferenceDto(bool EmailEnabled, bool WhatsAppEnabled, bool SmsEnabled);

public record GetNotificationPreferencesQuery(string UserId) : IRequest<NotificationPreferenceDto>;
