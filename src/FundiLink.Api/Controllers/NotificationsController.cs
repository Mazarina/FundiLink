using System.Security.Claims;
using FundiLink.Api.Models;
using FundiLink.Application.Common.Models;
using FundiLink.Application.Features.Notifications.Commands.TriggerDeadlineReminders;
using FundiLink.Application.Features.Notifications.Commands.UpdateNotificationPreferences;
using FundiLink.Application.Features.Notifications.Queries.GetMyNotifications;
using FundiLink.Application.Features.Notifications.Queries.GetNotificationPreferences;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundiLink.Api.Controllers;

[ApiController]
[Route("api/v1/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private string ActorRole => User.FindFirstValue(ClaimTypes.Role) ?? "Unknown";

    [HttpGet("preferences")]
    [ProducesResponseType(typeof(NotificationPreferenceDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPreferences(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetNotificationPreferencesQuery(UserId), cancellationToken);
        return Ok(result);
    }

    [HttpPut("preferences")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdatePreferences(
        [FromBody] UpdateNotificationPreferencesRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new UpdateNotificationPreferencesCommand(
                UserId, request.EmailEnabled, request.WhatsAppEnabled, request.SmsEnabled),
            cancellationToken);
        return NoContent();
    }

    // Owner-scoped notification history. Reads the append-only log for the authenticated
    // learner so they can see which reminders were generated.
    [HttpGet("history")]
    [ProducesResponseType(typeof(IReadOnlyList<NotificationLogDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMyNotificationsQuery(UserId), cancellationToken);
        return Ok(result);
    }

    // Admin/ops-triggered deadline-reminder pass (for operations and testing). RBAC-gated and
    // append-only audit-logged. Stub delivery providers only — reminders are guidance.
    [HttpPost("admin/run-deadline-reminders")]
    [Authorize(Roles = "SupportAgent,Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ReminderRunResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> RunDeadlineReminders(
        [FromBody] RunDeadlineRemindersRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new TriggerDeadlineRemindersCommand(UserId, ActorRole, request.WindowDays),
            cancellationToken);
        return Ok(result);
    }
}
