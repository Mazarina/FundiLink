using System.Security.Claims;
using FundiLink.Application.Features.Home.Queries.GetLearnerHomeSummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundiLink.Api.Controllers;

/// <summary>
/// Owner-scoped learner home dashboard. Read-only composition of the authenticated learner's
/// own existing data. Guidance only — FundiLink is not an official admissions or funding portal.
/// </summary>
[ApiController]
[Route("api/v1/home")]
[Authorize]
public class HomeController : ControllerBase
{
    private readonly IMediator _mediator;

    public HomeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // Owner-scoped at-a-glance summary for the learner's landing dashboard.
    [HttpGet("summary")]
    [ProducesResponseType(typeof(LearnerHomeSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetLearnerHomeSummaryQuery(UserId), cancellationToken);
        return Ok(result);
    }
}
