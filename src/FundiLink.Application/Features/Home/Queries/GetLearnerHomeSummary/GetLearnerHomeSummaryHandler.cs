using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Home.Queries.GetLearnerHomeSummary;

/// <summary>
/// Composes the owner-scoped home summary from the learner's own existing data. Read-only:
/// reuses existing repositories and the Phase 12 deadline query (owner-scoped variant). Adds no
/// new audit surface — it follows the same access pattern the learner already uses for each
/// feature. Throws <see cref="KeyNotFoundException"/> when the user has no learner profile.
/// </summary>
public class GetLearnerHomeSummaryHandler
    : IRequestHandler<GetLearnerHomeSummaryQuery, LearnerHomeSummaryDto>
{
    private const int MinWindowDays = 1;
    private const int MaxWindowDays = 90;
    private const int RecentNotificationLimit = 5;

    private readonly ILearnerRepository _learnerRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IBursaryApplicationRepository _bursaryApplicationRepository;
    private readonly IChecklistRepository _checklistRepository;
    private readonly INotificationLogRepository _notificationLogRepository;
    private readonly IDeadlineQueryRepository _deadlineQueryRepository;

    public GetLearnerHomeSummaryHandler(
        ILearnerRepository learnerRepository,
        IApplicationRepository applicationRepository,
        IBursaryApplicationRepository bursaryApplicationRepository,
        IChecklistRepository checklistRepository,
        INotificationLogRepository notificationLogRepository,
        IDeadlineQueryRepository deadlineQueryRepository)
    {
        _learnerRepository = learnerRepository;
        _applicationRepository = applicationRepository;
        _bursaryApplicationRepository = bursaryApplicationRepository;
        _checklistRepository = checklistRepository;
        _notificationLogRepository = notificationLogRepository;
        _deadlineQueryRepository = deadlineQueryRepository;
    }

    public async Task<LearnerHomeSummaryDto> Handle(
        GetLearnerHomeSummaryQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var windowDays = Math.Clamp(request.DeadlineWindowDays, MinWindowDays, MaxWindowDays);
        var now = DateTime.UtcNow;

        var applications =
            (await _applicationRepository.GetByLearnerIdAsync(learner.Id, cancellationToken)).ToList();
        var bursaryApplications =
            (await _bursaryApplicationRepository.GetByLearnerIdAsync(learner.Id, cancellationToken)).ToList();

        var programmeCounts = applications
            .GroupBy(a => a.Status.ToString())
            .Select(g => new StatusCountDto(g.Key, g.Count()))
            .OrderBy(c => c.Status)
            .ToList();

        var bursaryCounts = bursaryApplications
            .GroupBy(a => a.Status.ToString())
            .Select(g => new StatusCountDto(g.Key, g.Count()))
            .OrderBy(c => c.Status)
            .ToList();

        var pendingDocumentCount = await CountPendingDocumentsAsync(applications, cancellationToken);

        var deadlines = await _deadlineQueryRepository.GetUpcomingDeadlinesForLearnerAsync(
            learner.Id, now, now.AddDays(windowDays), cancellationToken);

        var deadlineDtos = deadlines
            .Select(d => new UpcomingDeadlineDto(d.Kind.ToString(), d.OpportunityName, d.DeadlineDate))
            .ToList();

        var notifications = await _notificationLogRepository.GetByLearnerIdAsync(learner.Id, cancellationToken);
        var recentNotifications = notifications
            .OrderByDescending(n => n.SentAt)
            .Take(RecentNotificationLimit)
            .Select(n => new RecentNotificationDto(n.Id, n.NotificationType, n.Channel, n.Status, n.SentAt))
            .ToList();

        return new LearnerHomeSummaryDto(
            learner.FirstName,
            learner.ProfileCompleteness,
            programmeCounts,
            applications.Count,
            bursaryCounts,
            bursaryApplications.Count,
            pendingDocumentCount,
            deadlineDtos,
            recentNotifications);
    }

    private async Task<int> CountPendingDocumentsAsync(
        IEnumerable<LearnerApplication> applications, CancellationToken cancellationToken)
    {
        var pending = 0;
        foreach (var application in applications)
        {
            var items = await _checklistRepository.GetByApplicationIdAsync(application.Id, cancellationToken);
            pending += items.Count(i => i.IsRequired && i.LinkedDocumentId is null);
        }

        return pending;
    }
}
