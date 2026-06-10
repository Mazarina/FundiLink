using System.Security.Claims;
using FundiLink.Api.Models;
using FundiLink.Application.Features.Accommodation.Commands.TrackAccommodationInterest;
using FundiLink.Application.Features.Accommodation.Commands.UpdateAccommodationInterestStatus;
using FundiLink.Application.Features.Accommodation.Queries.GetAccommodationById;
using FundiLink.Application.Features.Accommodation.Queries.GetAccommodationListings;
using FundiLink.Application.Features.Accommodation.Queries.GetAccommodationMatches;
using FundiLink.Application.Features.Accommodation.Queries.GetMyAccommodationInterests;
using FundiLink.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundiLink.Api.Controllers;

// NOTE: Accommodation data is curated public/example information for guidance only — FundiLink is
// NOT a landlord, accommodation provider, or booking agent. No bookings or payments occur here.
[ApiController]
[Route("api/v1/accommodation")]
[Authorize]
public class AccommodationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccommodationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AccommodationListingDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] string? province,
        [FromQuery] string? nearInstitution,
        [FromQuery] AccommodationType? accommodationType,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAccommodationListingsQuery(province, nearInstitution, accommodationType), cancellationToken);
        return Ok(result);
    }

    [HttpGet("matches")]
    [ProducesResponseType(typeof(IEnumerable<AccommodationMatchDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMatches(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAccommodationMatchesQuery(UserId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("interests")]
    [ProducesResponseType(typeof(IEnumerable<AccommodationInterestSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyInterests(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMyAccommodationInterestsQuery(UserId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AccommodationListingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAccommodationByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("interests")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> TrackInterest(
        [FromBody] TrackAccommodationInterestRequest request, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            new TrackAccommodationInterestCommand(request.AccommodationListingId, UserId, request.Status, request.Notes),
            cancellationToken);
        return CreatedAtAction(nameof(GetMyInterests), new { id }, new { id });
    }

    [HttpPut("interests/{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateInterestStatus(
        Guid id, [FromBody] UpdateAccommodationInterestStatusRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new UpdateAccommodationInterestStatusCommand(id, UserId, request.NewStatus, request.Notes), cancellationToken);
        return NoContent();
    }
}
