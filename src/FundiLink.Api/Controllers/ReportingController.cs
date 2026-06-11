using FundiLink.Application.Features.Reporting.Queries.GetAuditActivityReport;
using FundiLink.Application.Features.Reporting.Queries.GetOperationsDashboard;
using FundiLink.Application.Features.Reporting.Queries.GetPopiaOperationsSummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundiLink.Api.Controllers;

/// <summary>
/// Read-only admin reporting & POPIA operations dashboard (Phase 11).
/// Aggregate-first: dashboard and POPIA summary expose only counts/grouped totals — no raw
/// learner PII. The audit activity report is a filtered view over the existing append-only
/// audit log and is restricted to SuperAdmin. All endpoints are read-only.
/// </summary>
[ApiController]
[Route("api/v1/reporting")]
public class ReportingController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("dashboard")]
    [Authorize(Roles = "SupportAgent,Admin,SuperAdmin")]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOperationsDashboardQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("popia-summary")]
    [Authorize(Roles = "SupportAgent,Admin,SuperAdmin")]
    public async Task<IActionResult> GetPopiaSummary(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPopiaOperationsSummaryQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("audit-activity")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetAuditActivity(
        [FromQuery] string? action,
        [FromQuery] string? actorRole,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetAuditActivityReportQuery(action, actorRole, from, to, page, pageSize),
            cancellationToken);
        return Ok(result);
    }
}
