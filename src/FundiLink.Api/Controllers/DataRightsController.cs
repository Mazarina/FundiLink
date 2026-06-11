using System.Security.Claims;
using FundiLink.Api.Models;
using FundiLink.Application.Features.DataRights;
using FundiLink.Application.Features.DataRights.Commands.FulfilErasureRequest;
using FundiLink.Application.Features.DataRights.Commands.RequestErasure;
using FundiLink.Application.Features.DataRights.Commands.ReviewErasureRequest;
using FundiLink.Application.Features.DataRights.Queries.ExportMyData;
using FundiLink.Application.Features.DataRights.Queries.GetMyErasureRequests;
using FundiLink.Application.Features.DataRights.Queries.GetPendingErasureRequests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundiLink.Api.Controllers;

// Data subject rights (POPIA): right of access (export) and right to erasure. Learner
// endpoints are owner-scoped (data resolved from the authenticated user id). Erasure is
// reversible-until-confirmed: a learner raises a request; admin review and fulfilment are
// deliberate, RBAC-gated, audited actions. Fulfilment anonymises/soft-deletes personal data
// while preserving append-only audit and consent records (POPIA proof-of-processing retention).
// No third-party storage/email/delivery integration in this phase — the export is generated
// in-process and erasure is a deterministic in-process service behind interfaces.
[ApiController]
[Route("api/v1/data-rights")]
[Authorize]
public class DataRightsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DataRightsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private string ActorRole => User.FindFirstValue(ClaimTypes.Role) ?? "Unknown";

    // --- Learner-owned data rights (owner-scoped) ---

    [HttpGet("export")]
    [ProducesResponseType(typeof(DataExportDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportMyData(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new ExportMyDataQuery(UserId), cancellationToken));

    [HttpGet("erasure-requests")]
    [ProducesResponseType(typeof(IReadOnlyList<ErasureRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyErasureRequests(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetMyErasureRequestsQuery(UserId), cancellationToken));

    [HttpPost("erasure-requests")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> RequestErasure(
        [FromBody] RequestErasureRequest request, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(new RequestErasureCommand(UserId, request.Reason), cancellationToken);
        return CreatedAtAction(nameof(GetMyErasureRequests), new { id }, new { id });
    }

    // --- Admin review queue (RBAC: SupportAgent/Admin/SuperAdmin) ---

    [HttpGet("admin/erasure-requests/pending")]
    [Authorize(Roles = "SupportAgent,Admin,SuperAdmin")]
    [ProducesResponseType(typeof(IReadOnlyList<ErasureRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingErasureRequests(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetPendingErasureRequestsQuery(), cancellationToken));

    [HttpPost("admin/erasure-requests/{id:guid}/approve")]
    [Authorize(Roles = "SupportAgent,Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Approve(
        Guid id, [FromBody] ReviewErasureRequestBody body, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ReviewErasureRequestCommand(id, true, UserId, ActorRole, body.Note), cancellationToken);
        return NoContent();
    }

    [HttpPost("admin/erasure-requests/{id:guid}/reject")]
    [Authorize(Roles = "SupportAgent,Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Reject(
        Guid id, [FromBody] ReviewErasureRequestBody body, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ReviewErasureRequestCommand(id, false, UserId, ActorRole, body.Note), cancellationToken);
        return NoContent();
    }

    [HttpPost("admin/erasure-requests/{id:guid}/fulfil")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Fulfil(
        Guid id, [FromBody] ReviewErasureRequestBody body, CancellationToken cancellationToken)
    {
        await _mediator.Send(new FulfilErasureRequestCommand(id, UserId, ActorRole, body.Note), cancellationToken);
        return NoContent();
    }
}
