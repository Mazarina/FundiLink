using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Common.Models;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Admin.Queries.SearchLearners;

public class SearchLearnersHandler : IRequestHandler<SearchLearnersQuery, PagedResult<LearnerSummary>>
{
    private readonly ILearnerQueryRepository _learnerQueryRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public SearchLearnersHandler(
        ILearnerQueryRepository learnerQueryRepository,
        IAuditLogRepository auditLogRepository)
    {
        _learnerQueryRepository = learnerQueryRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<PagedResult<LearnerSummary>> Handle(SearchLearnersQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var (items, total) = await _learnerQueryRepository.SearchAsync(
            request.Keyword, request.Province, page, pageSize, cancellationToken);

        await _auditLogRepository.AddAsync(
            AuditLogEntry.Create(request.ActorUserId, request.ActorRole, "SearchLearners", "Learner", "*"),
            cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);

        return new PagedResult<LearnerSummary>(items, total, page, pageSize);
    }
}
