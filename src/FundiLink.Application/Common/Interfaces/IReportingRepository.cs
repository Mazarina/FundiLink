using FundiLink.Application.Features.Reporting.Queries.GetOperationsDashboard;
using FundiLink.Application.Features.Reporting.Queries.GetPopiaOperationsSummary;

namespace FundiLink.Application.Common.Interfaces;

/// <summary>
/// Read-only, aggregate-first reporting over existing data. Computes counts and grouped
/// figures in-process for the staff operations dashboard. Deliberately exposes no method
/// that returns raw learner personal information — aggregates and derived figures only.
/// No external analytics/telemetry provider is involved; a real provider may be wired
/// later behind this interface (key via env only).
/// </summary>
public interface IReportingRepository
{
    Task<OperationsDashboardDto> GetOperationsDashboardAsync(CancellationToken ct);

    Task<PopiaOperationsSummaryDto> GetPopiaOperationsSummaryAsync(CancellationToken ct);
}
