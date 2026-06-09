using System.Security.Claims;
using FundiLink.Api.Models;
using FundiLink.Application.Features.Applications.Commands.CreateApplication;
using FundiLink.Application.Features.Applications.Commands.DeleteApplication;
using FundiLink.Application.Features.Applications.Commands.UpdateApplicationStatus;
using FundiLink.Application.Features.Applications.Queries.GetApplicationById;
using FundiLink.Application.Features.Applications.Queries.GetMyApplications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundiLink.Api.Controllers;

[ApiController]
[Route("api/v1/applications")]
[Authorize]
public class ApplicationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApplicationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateApplicationRequest request, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            new CreateApplicationCommand(request.ProgrammeId, UserId, request.Status, request.Notes, request.DeadlineDate),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ApplicationSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMyApplicationsQuery(UserId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApplicationDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetApplicationByIdQuery(id, UserId), cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateApplicationStatusRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateApplicationStatusCommand(id, UserId, request.NewStatus, request.Notes), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteApplicationCommand(id, UserId), cancellationToken);
        return NoContent();
    }
}
