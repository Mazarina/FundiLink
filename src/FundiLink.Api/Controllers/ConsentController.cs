using System.Security.Claims;
using FundiLink.Api.Models;
using FundiLink.Application.Features.Consent.Commands.LinkGuardian;
using FundiLink.Application.Features.Consent.Commands.RecordConsent;
using FundiLink.Application.Features.Consent.Commands.RevokeConsent;
using FundiLink.Application.Features.Consent.Queries.GetConsentHistory;
using FundiLink.Application.Features.Consent.Queries.GetConsentState;
using FundiLink.Application.Features.Consent.Queries.GetGuardianView;
using FundiLink.Application.Features.Consent.Queries.GetLinkedLearners;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundiLink.Api.Controllers;

// Guardian consent & co-access (POPIA). Where a learner is under 18, guardian consent is
// required before sensitive processing/sharing. Guardian co-access is consent-gated, read-only,
// and data-minimised — a guardian sees only what consent permits. All grants, revocations, and
// guardian access are append-only audit-logged. No real identity-verification / e-signature
// provider integration in this phase (deterministic stub behind the interface).
[ApiController]
[Route("api/v1/consent")]
[Authorize]
public class ConsentController : ControllerBase
{
    private readonly IMediator _mediator;

    public ConsentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // --- Learner-owned consent management (owner-scoped) ---

    [HttpGet("state")]
    [ProducesResponseType(typeof(ConsentStateDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetState(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetConsentStateQuery(UserId), cancellationToken));

    [HttpGet("history")]
    [ProducesResponseType(typeof(IReadOnlyList<ConsentHistoryEntryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetConsentHistoryQuery(UserId), cancellationToken));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Record(
        [FromBody] RecordConsentRequest request, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            new RecordConsentCommand(UserId, request.ConsentType, request.Scope, request.GuardianName, request.GuardianContact),
            cancellationToken);
        return CreatedAtAction(nameof(GetState), new { id }, new { id });
    }

    [HttpPost("revoke")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Revoke(
        [FromBody] RevokeConsentRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RevokeConsentCommand(UserId, request.ConsentType), cancellationToken);
        return NoContent();
    }

    [HttpPost("guardian-links")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> LinkGuardian(
        [FromBody] LinkGuardianRequest request, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            new LinkGuardianCommand(UserId, request.GuardianUserId, request.GuardianName, request.GuardianContact),
            cancellationToken);
        return CreatedAtAction(nameof(GetState), new { id }, new { id });
    }

    // --- Guardian co-access (consent-gated, read-only, minimised) ---

    [HttpGet("guardian/learners")]
    [ProducesResponseType(typeof(IReadOnlyList<LinkedLearnerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLinkedLearners(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetLinkedLearnersQuery(UserId), cancellationToken));

    [HttpGet("guardian/learners/{learnerId:guid}")]
    [ProducesResponseType(typeof(GuardianViewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetGuardianView(Guid learnerId, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetGuardianViewQuery(UserId, learnerId), cancellationToken));
}
