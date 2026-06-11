using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.DataRights.Queries.GetPendingErasureRequests;

/// <summary>Returns erasure requests awaiting admin review. RBAC enforced at the API boundary.</summary>
public class GetPendingErasureRequestsHandler
    : IRequestHandler<GetPendingErasureRequestsQuery, IReadOnlyList<ErasureRequestDto>>
{
    private readonly IErasureRequestRepository _erasureRequestRepository;

    public GetPendingErasureRequestsHandler(IErasureRequestRepository erasureRequestRepository)
    {
        _erasureRequestRepository = erasureRequestRepository;
    }

    public async Task<IReadOnlyList<ErasureRequestDto>> Handle(
        GetPendingErasureRequestsQuery request, CancellationToken cancellationToken)
    {
        var requests = await _erasureRequestRepository.GetPendingAsync(cancellationToken);
        return requests.Select(ErasureRequestDto.From).ToList();
    }
}
