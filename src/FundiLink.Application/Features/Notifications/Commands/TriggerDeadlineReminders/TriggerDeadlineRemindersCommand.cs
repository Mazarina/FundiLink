using FundiLink.Application.Common.Models;
using MediatR;

namespace FundiLink.Application.Features.Notifications.Commands.TriggerDeadlineReminders;

/// <summary>
/// Admin/ops-triggered deadline-reminder pass (for operations and testing). Deliberate,
/// RBAC-gated, append-only audit-logged action. Uses stub delivery providers only — no
/// real email/SMS/WhatsApp integration. Reminders are guidance only.
/// </summary>
public record TriggerDeadlineRemindersCommand(
    string ActorUserId,
    string ActorRole,
    int WindowDays) : IRequest<ReminderRunResult>;
