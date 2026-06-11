using MediatR;

namespace FundiLink.Application.Features.DataRights.Queries.GetMyErasureRequests;

/// <summary>Returns the caller's own erasure requests, newest first. Owner-scoped.</summary>
public record GetMyErasureRequestsQuery(string UserId) : IRequest<IReadOnlyList<ErasureRequestDto>>;
