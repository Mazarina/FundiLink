using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Common.Models;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Notifications.Commands.TriggerDeadlineReminders;

/// <summary>
/// Runs the deterministic deadline-reminder service for an admin-triggered pass and writes
/// an append-only audit entry recording the run and its aggregate outcome. RBAC is enforced
/// at the API boundary. Delivery uses stub providers only; reminders honour notification
/// preferences and consent inside the reminder service.
/// </summary>
public class TriggerDeadlineRemindersHandler
    : IRequestHandler<TriggerDeadlineRemindersCommand, ReminderRunResult>
{
    // Bounds keep an ops-triggered window sane and deterministic.
    private const int MinWindowDays = 1;
    private const int MaxWindowDays = 90;
    private const int DefaultWindowDays = 14;

    private readonly IDeadlineReminderService _reminderService;
    private readonly IAuditLogRepository _auditLogRepository;

    public TriggerDeadlineRemindersHandler(
        IDeadlineReminderService reminderService,
        IAuditLogRepository auditLogRepository)
    {
        _reminderService = reminderService;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<ReminderRunResult> Handle(
        TriggerDeadlineRemindersCommand request, CancellationToken cancellationToken)
    {
        var windowDays = request.WindowDays is < MinWindowDays or > MaxWindowDays
            ? DefaultWindowDays
            : request.WindowDays;

        var result = await _reminderService.GenerateRemindersAsync(
            DateTime.UtcNow, windowDays, cancellationToken);

        await _auditLogRepository.AddAsync(
            AuditLogEntry.Create(
                request.ActorUserId, request.ActorRole, "TriggerDeadlineReminders",
                "DeadlineReminderRun",
                $"window={windowDays}d;sent={result.RemindersSent};skipped={result.RemindersSkippedAlreadySent}"),
            cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);

        return result;
    }
}
