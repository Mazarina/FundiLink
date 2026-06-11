using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Notifications.Queries.GetMyNotifications;

public class GetMyNotificationsHandler
    : IRequestHandler<GetMyNotificationsQuery, IReadOnlyList<NotificationLogDto>>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly INotificationLogRepository _logRepository;

    public GetMyNotificationsHandler(
        ILearnerRepository learnerRepository,
        INotificationLogRepository logRepository)
    {
        _learnerRepository = learnerRepository;
        _logRepository = logRepository;
    }

    public async Task<IReadOnlyList<NotificationLogDto>> Handle(
        GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var logs = await _logRepository.GetByLearnerIdAsync(learner.Id, cancellationToken);

        return logs
            .Select(l => new NotificationLogDto(
                l.Id, l.NotificationType, l.Channel, l.Status, l.SentAt, l.ErrorMessage))
            .ToList();
    }
}
