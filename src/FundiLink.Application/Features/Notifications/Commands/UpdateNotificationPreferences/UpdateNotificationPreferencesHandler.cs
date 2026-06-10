using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Notifications.Commands.UpdateNotificationPreferences;

public class UpdateNotificationPreferencesHandler : IRequestHandler<UpdateNotificationPreferencesCommand>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly INotificationPreferenceRepository _preferenceRepository;

    public UpdateNotificationPreferencesHandler(
        ILearnerRepository learnerRepository,
        INotificationPreferenceRepository preferenceRepository)
    {
        _learnerRepository = learnerRepository;
        _preferenceRepository = preferenceRepository;
    }

    public async Task Handle(UpdateNotificationPreferencesCommand request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var preference = await _preferenceRepository.GetByLearnerIdAsync(learner.Id, cancellationToken);
        if (preference is null)
        {
            preference = NotificationPreference.CreateDefault(learner.Id);
            await _preferenceRepository.AddAsync(preference, cancellationToken);
        }

        preference.UpdatePreferences(request.EmailEnabled, request.WhatsAppEnabled, request.SmsEnabled);
        await _preferenceRepository.SaveChangesAsync(cancellationToken);
    }
}
