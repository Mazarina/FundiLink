using System.Security.Claims;
using FundiLink.Api.Models;
using FundiLink.Application.Features.Bursaries.Commands.CreateBursaryApplication;
using FundiLink.Application.Features.Bursaries.Commands.DeleteBursaryApplication;
using FundiLink.Application.Features.Bursaries.Commands.UpdateBursaryApplicationStatus;
using FundiLink.Application.Features.Bursaries.Queries.GetMyBursaryApplications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundiLink.Api.Controllers;

[ApiController]
[Route("api/v1/bursary-applications")]
[Authorize]
public class BursaryApplicationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BursaryApplicationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateBursaryApplicationRequest request, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            new CreateBursaryApplicationCommand(request.BursaryId, UserId, request.Status, request.Notes, request.DeadlineDate),
            cancellationToken);

        return CreatedAtAction(nameof(GetMine), new { id }, new { id });
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BursaryApplicationSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMyBursaryApplicationsQuery(UserId), cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateBursaryApplicationStatusRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateBursaryApplicationStatusCommand(id, UserId, request.NewStatus, request.Notes), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteBursaryApplicationCommand(id, UserId), cancellationToken);
        return NoContent();
    }
}
