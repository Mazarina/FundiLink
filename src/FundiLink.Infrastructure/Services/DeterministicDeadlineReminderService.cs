using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Common.Models;
using FundiLink.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FundiLink.Infrastructure.Services;

/// <summary>
/// Deterministic, in-process deadline-reminder generator. For each active learner with one
/// or more upcoming programme/bursary application deadlines inside the window, it sends at
/// most one aggregated guidance reminder per UTC day (idempotency) via
/// <see cref="INotificationService"/>, which itself honours the learner's notification
/// preferences and writes append-only logs. For minor learners, a current guardian
/// data-processing consent is required before any reminder is sent. Reminders are guidance
/// only — FundiLink is not an official admissions/funding portal. Delivery uses stub
/// providers only in this phase.
/// </summary>
public class DeterministicDeadlineReminderService : IDeadlineReminderService
{
    private readonly IDeadlineQueryRepository _deadlineQueryRepository;
    private readonly ILearnerRepository _learnerRepository;
    private readonly IConsentCheckService _consentCheckService;
    private readonly INotificationLogRepository _logRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<DeterministicDeadlineReminderService> _logger;

    public DeterministicDeadlineReminderService(
        IDeadlineQueryRepository deadlineQueryRepository,
        ILearnerRepository learnerRepository,
        IConsentCheckService consentCheckService,
        INotificationLogRepository logRepository,
        INotificationService notificationService,
        ILogger<DeterministicDeadlineReminderService> logger)
    {
        _deadlineQueryRepository = deadlineQueryRepository;
        _learnerRepository = learnerRepository;
        _consentCheckService = consentCheckService;
        _logRepository = logRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<ReminderRunResult> GenerateRemindersAsync(
        DateTime nowUtc, int windowDays, CancellationToken ct)
    {
        var fromInclusive = nowUtc.Date;
        var toInclusive = nowUtc.Date.AddDays(windowDays).AddDays(1).AddTicks(-1);

        var deadlines = await _deadlineQueryRepository
            .GetUpcomingDeadlinesAsync(fromInclusive, toInclusive, ct);

        var byLearner = deadlines
            .GroupBy(d => d.LearnerId)
            .OrderBy(g => g.Key)
            .ToList();

        var sent = 0;
        var skipped = 0;

        foreach (var group in byLearner)
        {
            var learnerId = group.Key;

            // Idempotency: at most one deadline reminder per learner per UTC day.
            if (await _logRepository.HasLogForTypeOnDateAsync(
                    learnerId, NotificationType.DeadlineReminder, nowUtc.Date, ct))
            {
                skipped++;
                continue;
            }

            var learner = await _learnerRepository.GetByIdAsync(learnerId, ct);
            if (learner is null)
                continue;

            // Minor learners require a current guardian data-processing consent.
            if (learner.IsMinor() &&
                !await _consentCheckService.HasCurrentConsentAsync(
                    learnerId, ConsentType.DataProcessing, ct))
            {
                _logger.LogInformation(
                    "Skipping reminder for minor learner {LearnerId} without current consent", learnerId);
                skipped++;
                continue;
            }

            var (subject, message) = ComposeReminder(group.OrderBy(d => d.DeadlineDate).ToList());
            await _notificationService.NotifyAsync(
                learnerId, NotificationType.DeadlineReminder, subject, message, ct);
            sent++;
        }

        return new ReminderRunResult(byLearner.Count, sent, skipped);
    }

    private static (string Subject, string Message) ComposeReminder(IReadOnlyList<UpcomingDeadline> deadlines)
    {
        var count = deadlines.Count;
        var subject = count == 1
            ? "Upcoming application deadline reminder"
            : $"You have {count} upcoming application deadlines";

        var lines = deadlines.Select(d =>
            $"- {d.OpportunityName} ({KindLabel(d.Kind)}) closes {d.DeadlineDate:yyyy-MM-dd}");

        var message =
            "This is a FundiLink reminder to help you prepare and organise your applications.\n\n"
            + string.Join("\n", lines)
            + "\n\nFundiLink is not an official admissions or funding portal. "
            + "Please submit through the official institution or funder portal before the deadline.";

        return (subject, message);
    }

    private static string KindLabel(DeadlineKind kind) => kind switch
    {
        DeadlineKind.ProgrammeApplication => "programme",
        DeadlineKind.BursaryApplication => "bursary",
        _ => "opportunity"
    };
}
