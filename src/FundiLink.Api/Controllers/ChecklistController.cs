using System.Security.Claims;
using FundiLink.Api.Models;
using FundiLink.Application.Features.Checklist.Commands.LinkDocumentToChecklist;
using FundiLink.Application.Features.Checklist.Queries.GetApplicationChecklist;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundiLink.Api.Controllers;

[ApiController]
[Route("api/v1/applications/{applicationId:guid}/checklist")]
[Authorize]
public class ChecklistController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChecklistController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ChecklistItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid applicationId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetApplicationChecklistQuery(applicationId, UserId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("link")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Link(Guid applicationId, [FromBody] LinkDocumentRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new LinkDocumentToChecklistCommand(applicationId, request.ChecklistItemId, request.DocumentId, UserId),
            cancellationToken);
        return NoContent();
    }
}
