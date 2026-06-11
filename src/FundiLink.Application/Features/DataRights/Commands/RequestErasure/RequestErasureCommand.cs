using MediatR;

namespace FundiLink.Application.Features.DataRights.Commands.RequestErasure;

/// <summary>
/// Raises an erasure request for the caller's own learner profile (POPIA right to erasure).
/// Owner-scoped: the learner is resolved from the authenticated user id.
/// </summary>
public record RequestErasureCommand(string UserId, string? Reason) : IRequest<Guid>;
