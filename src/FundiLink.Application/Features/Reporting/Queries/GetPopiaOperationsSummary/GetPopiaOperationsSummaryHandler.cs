using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Reporting.Queries.GetPopiaOperationsSummary;

public class GetPopiaOperationsSummaryHandler
    : IRequestHandler<GetPopiaOperationsSummaryQuery, PopiaOperationsSummaryDto>
{
    private readonly IReportingRepository _reportingRepository;

    public GetPopiaOperationsSummaryHandler(IReportingRepository reportingRepository)
    {
        _reportingRepository = reportingRepository;
    }

    public Task<PopiaOperationsSummaryDto> Handle(
        GetPopiaOperationsSummaryQuery request, CancellationToken cancellationToken)
        => _reportingRepository.GetPopiaOperationsSummaryAsync(cancellationToken);
}
