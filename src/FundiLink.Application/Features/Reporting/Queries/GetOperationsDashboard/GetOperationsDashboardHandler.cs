using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Reporting.Queries.GetOperationsDashboard;

/// <summary>
/// Returns aggregate operational figures for staff. Read-only and aggregate-first —
/// it surfaces no individual learner, so no per-learner audit entry is written.
/// RBAC enforced at the API boundary.
/// </summary>
public class GetOperationsDashboardHandler
    : IRequestHandler<GetOperationsDashboardQuery, OperationsDashboardDto>
{
    private readonly IReportingRepository _reportingRepository;

    public GetOperationsDashboardHandler(IReportingRepository reportingRepository)
    {
        _reportingRepository = reportingRepository;
    }

    public Task<OperationsDashboardDto> Handle(
        GetOperationsDashboardQuery request, CancellationToken cancellationToken)
        => _reportingRepository.GetOperationsDashboardAsync(cancellationToken);
}
