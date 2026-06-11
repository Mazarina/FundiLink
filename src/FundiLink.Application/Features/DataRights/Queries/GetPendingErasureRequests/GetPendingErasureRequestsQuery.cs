using MediatR;

namespace FundiLink.Application.Features.DataRights.Queries.GetPendingErasureRequests;

/// <summary>Admin query: erasure requests awaiting review (Requested), newest first.</summary>
public record GetPendingErasureRequestsQuery : IRequest<IReadOnlyList<ErasureRequestDto>>;
