using System.Security.Claims;
using FundiLink.Api.Models;
using FundiLink.Application.Features.Notifications.Commands.UpdateNotificationPreferences;
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
}
