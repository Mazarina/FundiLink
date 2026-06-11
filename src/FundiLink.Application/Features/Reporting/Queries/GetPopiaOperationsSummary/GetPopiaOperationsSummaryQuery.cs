using MediatR;

namespace FundiLink.Application.Features.Reporting.Queries.GetPopiaOperationsSummary;

/// <summary>
/// Surfaces the open POPIA work queues already implemented (pending document verifications
/// and pending erasure requests) as a single staff overview. Aggregate counts only — the
/// underlying queues remain the place where individual items are actioned and audit-logged.
/// RBAC enforced at the API boundary.
/// </summary>
public record GetPopiaOperationsSummaryQuery : IRequest<PopiaOperationsSummaryDto>;

/// <summary>Aggregate counts for the POPIA operations overview. No personal information.</summary>
public record PopiaOperationsSummaryDto(
    int PendingDocumentVerifications,
    int PendingErasureRequests);
