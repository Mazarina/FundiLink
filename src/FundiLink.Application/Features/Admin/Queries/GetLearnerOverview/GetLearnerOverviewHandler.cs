using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Common.Models;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Admin.Queries.GetLearnerOverview;

public class GetLearnerOverviewHandler : IRequestHandler<GetLearnerOverviewQuery, LearnerOverview>
{
    private readonly ILearnerQueryRepository _learnerQueryRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public GetLearnerOverviewHandler(
        ILearnerQueryRepository learnerQueryRepository,
        IAuditLogRepository auditLogRepository)
    {
        _learnerQueryRepository = learnerQueryRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<LearnerOverview> Handle(GetLearnerOverviewQuery request, CancellationToken cancellationToken)
    {
        var overview = await _learnerQueryRepository.GetOverviewAsync(request.LearnerId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner not found.");

        await _auditLogRepository.AddAsync(
            AuditLogEntry.Create(request.ActorUserId, request.ActorRole, "ViewLearnerOverview", "Learner", request.LearnerId.ToString()),
            cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);

        return overview;
    }
}
