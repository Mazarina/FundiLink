using System.Security.Claims;
using FundiLink.Application.Features.Bursaries.Queries.GetBursaries;
using FundiLink.Application.Features.Bursaries.Queries.GetBursaryById;
using FundiLink.Application.Features.Bursaries.Queries.GetBursaryMatches;
using FundiLink.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundiLink.Api.Controllers;

[ApiController]
[Route("api/v1/bursaries")]
[Authorize]
public class BursariesController : ControllerBase
{
    private readonly IMediator _mediator;

    public BursariesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // NOTE: Bursary data is curated public information for guidance only — not official
    // funding admission. Learners must apply on the funder's official portal.
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BursaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] string? fieldOfStudy,
        [FromQuery] string? province,
        [FromQuery] BursaryFundingType? fundingType,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBursariesQuery(fieldOfStudy, province, fundingType), cancellationToken);
        return Ok(result);
    }

    [HttpGet("matches")]
    [ProducesResponseType(typeof(IEnumerable<BursaryMatchDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMatches(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBursaryMatchesQuery(UserId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BursaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBursaryByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
