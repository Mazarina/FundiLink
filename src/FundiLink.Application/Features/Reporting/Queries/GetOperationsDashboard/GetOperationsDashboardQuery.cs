using MediatR;

namespace FundiLink.Application.Features.Reporting.Queries.GetOperationsDashboard;

/// <summary>
/// Requests the staff operations dashboard. Aggregate figures only — no raw learner PII.
/// RBAC enforced at the API boundary (SupportAgent/Admin/SuperAdmin).
/// </summary>
public record GetOperationsDashboardQuery : IRequest<OperationsDashboardDto>;

/// <summary>A single labelled count (e.g. a province name or status with its total).</summary>
public record CountByCategoryDto(string Category, int Count);

/// <summary>
/// Aggregate operational figures derived from existing data. Contains no personal
/// information — counts and grouped totals only.
/// </summary>
public record OperationsDashboardDto(
    int TotalLearners,
    IReadOnlyList<CountByCategoryDto> LearnersByProvince,
    IReadOnlyList<CountByCategoryDto> ApplicationsByStatus,
    IReadOnlyList<CountByCategoryDto> BursaryApplicationsByStatus,
    IReadOnlyList<CountByCategoryDto> DocumentsByStatus,
    int PendingDocumentVerifications,
    int PendingErasureRequests,
    int ConsentGrants,
    int ConsentRevocations);
