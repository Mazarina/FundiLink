using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.DataRights.Queries.GetMyErasureRequests;

/// <summary>Returns the caller's own erasure request history, newest first. Owner-scoped.</summary>
public class GetMyErasureRequestsHandler
    : IRequestHandler<GetMyErasureRequestsQuery, IReadOnlyList<ErasureRequestDto>>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IErasureRequestRepository _erasureRequestRepository;

    public GetMyErasureRequestsHandler(
        ILearnerRepository learnerRepository,
        IErasureRequestRepository erasureRequestRepository)
    {
        _learnerRepository = learnerRepository;
        _erasureRequestRepository = erasureRequestRepository;
    }

    public async Task<IReadOnlyList<ErasureRequestDto>> Handle(
        GetMyErasureRequestsQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var requests = await _erasureRequestRepository.GetByLearnerIdAsync(learner.Id, cancellationToken);
        return requests.Select(ErasureRequestDto.From).ToList();
    }
}
