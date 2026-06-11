using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.DataRights.Commands.RequestErasure;

/// <summary>
/// Raises an erasure request for the caller's own learner profile and audit-logs the action
/// (append-only). A learner may only have one open request at a time. Deletion itself is a
/// later, deliberate, audited admin fulfilment — this command only records the request.
/// </summary>
public class RequestErasureHandler : IRequestHandler<RequestErasureCommand, Guid>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IErasureRequestRepository _erasureRequestRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public RequestErasureHandler(
        ILearnerRepository learnerRepository,
        IErasureRequestRepository erasureRequestRepository,
        IAuditLogRepository auditLogRepository)
    {
        _learnerRepository = learnerRepository;
        _erasureRequestRepository = erasureRequestRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<Guid> Handle(RequestErasureCommand request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var existing = await _erasureRequestRepository.GetOpenByLearnerIdAsync(learner.Id, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException("An erasure request is already in progress for this profile.");

        var erasureRequest = ErasureRequest.Raise(learner.Id, request.UserId, request.Reason);

        await _erasureRequestRepository.AddAsync(erasureRequest, cancellationToken);
        await _erasureRequestRepository.SaveChangesAsync(cancellationToken);

        await _auditLogRepository.AddAsync(
            AuditLogEntry.Create(request.UserId, "Learner", "RequestErasure", "ErasureRequest", erasureRequest.Id.ToString()),
            cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);

        return erasureRequest.Id;
    }
}
