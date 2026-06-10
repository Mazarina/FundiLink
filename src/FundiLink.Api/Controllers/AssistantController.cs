using System.Security.Claims;
using FundiLink.Api.Models;
using FundiLink.Application.Features.Assistant.Queries.AskAssistant;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundiLink.Api.Controllers;

[ApiController]
[Route("api/v1/assistant")]
[Authorize]
public class AssistantController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssistantController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // NOTE: The AI guidance assistant is owner-scoped — it only ever reads the caller's own
    // FundiLink data. Output is guidance only, grounded strictly in that data, and never
    // fabricates institution, programme, bursary, or NSFAS facts.
    [HttpPost("ask")]
    [ProducesResponseType(typeof(AssistantResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Ask([FromBody] AskAssistantRequest request, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(request.Intent))
        {
            return BadRequest(new { error = "Unsupported assistant intent." });
        }

        var result = await _mediator.Send(new AskAssistantQuery(UserId, request.Intent), cancellationToken);
        return Ok(result);
    }
}
