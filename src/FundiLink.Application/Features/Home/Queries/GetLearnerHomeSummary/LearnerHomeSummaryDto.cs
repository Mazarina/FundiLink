using FundiLink.Domain.Enums;

namespace FundiLink.Application.Features.Home.Queries.GetLearnerHomeSummary;

/// <summary>
/// Owner-scoped, read-only at-a-glance summary for the authenticated learner's home dashboard.
/// Composed entirely from the learner's own existing data (profile, applications, bursary
/// applications, document checklist, notification history). Aggregate/derived figures only —
/// no new PII surface beyond what the learner already owns. Guidance only: FundiLink is not an
/// official admissions or funding portal.
/// </summary>
public record LearnerHomeSummaryDto(
    string FirstName,
    int ProfileCompleteness,
    IReadOnlyList<StatusCountDto> ProgrammeApplicationCounts,
    int ProgrammeApplicationTotal,
    IReadOnlyList<StatusCountDto> BursaryApplicationCounts,
    int BursaryApplicationTotal,
    int PendingDocumentCount,
    IReadOnlyList<UpcomingDeadlineDto> UpcomingDeadlines,
    IReadOnlyList<RecentNotificationDto> RecentNotifications);

/// <summary>A count of applications grouped by a single status value.</summary>
public record StatusCountDto(string Status, int Count);

/// <summary>A minimal view of one of the learner's own upcoming deadlines.</summary>
public record UpcomingDeadlineDto(string Kind, string OpportunityName, DateTime DeadlineDate);

/// <summary>A minimal view of one recent notification log entry for the learner.</summary>
public record RecentNotificationDto(
    Guid Id,
    NotificationType NotificationType,
    NotificationChannel Channel,
    NotificationStatus Status,
    DateTime SentAt);
