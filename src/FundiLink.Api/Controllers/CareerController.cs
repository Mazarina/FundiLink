using System.Security.Claims;
using FundiLink.Api.Models;
using FundiLink.Application.Features.Career.Commands.TrackCareerInterest;
using FundiLink.Application.Features.Career.Commands.UpdateCareerInterestStatus;
using FundiLink.Application.Features.Career.Queries.GetCareerMatches;
using FundiLink.Application.Features.Career.Queries.GetCareerOpportunities;
using FundiLink.Application.Features.Career.Queries.GetCareerOpportunityById;
using FundiLink.Application.Features.Career.Queries.GetMyCareerInterests;
using FundiLink.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundiLink.Api.Controllers;

// NOTE: Career opportunity data is curated public/example information for guidance only — FundiLink
// is NOT an employer or recruitment agency. Applications happen on the provider's official channel.
[ApiController]
[Route("api/v1/career")]
[Authorize]
public class CareerController : ControllerBase
{
    private readonly IMediator _mediator;

    public CareerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CareerOpportunityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] string? fieldOfInterest,
        [FromQuery] string? province,
        [FromQuery] CareerOpportunityType? opportunityType,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetCareerOpportunitiesQuery(fieldOfInterest, province, opportunityType), cancellationToken);
        return Ok(result);
    }

    [HttpGet("matches")]
    [ProducesResponseType(typeof(IEnumerable<CareerMatchDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMatches(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCareerMatchesQuery(UserId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("interests")]
    [ProducesResponseType(typeof(IEnumerable<CareerInterestSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyInterests(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMyCareerInterestsQuery(UserId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CareerOpportunityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCareerOpportunityByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("interests")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> TrackInterest(
        [FromBody] TrackCareerInterestRequest request, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            new TrackCareerInterestCommand(request.CareerOpportunityId, UserId, request.Status, request.Notes),
            cancellationToken);
        return CreatedAtAction(nameof(GetMyInterests), new { id }, new { id });
    }

    [HttpPut("interests/{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateInterestStatus(
        Guid id, [FromBody] UpdateCareerInterestStatusRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new UpdateCareerInterestStatusCommand(id, UserId, request.NewStatus, request.Notes), cancellationToken);
        return NoContent();
    }
}
