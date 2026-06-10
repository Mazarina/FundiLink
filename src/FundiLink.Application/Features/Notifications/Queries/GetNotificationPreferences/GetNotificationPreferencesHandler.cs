using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Notifications.Queries.GetNotificationPreferences;

public class GetNotificationPreferencesHandler : IRequestHandler<GetNotificationPreferencesQuery, NotificationPreferenceDto>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly INotificationPreferenceRepository _preferenceRepository;

    public GetNotificationPreferencesHandler(
        ILearnerRepository learnerRepository,
        INotificationPreferenceRepository preferenceRepository)
    {
        _learnerRepository = learnerRepository;
        _preferenceRepository = preferenceRepository;
    }

    public async Task<NotificationPreferenceDto> Handle(GetNotificationPreferencesQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var preference = await _preferenceRepository.GetByLearnerIdAsync(learner.Id, cancellationToken);

        // Defaults (Email on, others off) returned without creating a record.
        return preference is null
            ? new NotificationPreferenceDto(EmailEnabled: true, WhatsAppEnabled: false, SmsEnabled: false)
            : new NotificationPreferenceDto(preference.EmailEnabled, preference.WhatsAppEnabled, preference.SmsEnabled);
    }
}
