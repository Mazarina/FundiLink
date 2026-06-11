using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.DataRights.Commands.ReviewErasureRequest;

/// <summary>
/// Approves or rejects a pending erasure request and audit-logs the decision (append-only).
/// Does not delete any data — approval only moves the request to Approved; fulfilment is a
/// separate, deliberate admin action.
/// </summary>
public class ReviewErasureRequestHandler : IRequestHandler<ReviewErasureRequestCommand>
{
    private readonly IErasureRequestRepository _erasureRequestRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public ReviewErasureRequestHandler(
        IErasureRequestRepository erasureRequestRepository,
        IAuditLogRepository auditLogRepository)
    {
        _erasureRequestRepository = erasureRequestRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task Handle(ReviewErasureRequestCommand request, CancellationToken cancellationToken)
    {
        var erasureRequest = await _erasureRequestRepository.GetByIdAsync(request.RequestId, cancellationToken)
            ?? throw new KeyNotFoundException("Erasure request not found.");

        if (request.Approve)
            erasureRequest.Approve(request.ActorUserId, request.Note);
        else
            erasureRequest.Reject(request.ActorUserId, request.Note);

        await _erasureRequestRepository.SaveChangesAsync(cancellationToken);

        await _auditLogRepository.AddAsync(
            AuditLogEntry.Create(
                request.ActorUserId, request.ActorRole,
                request.Approve ? "ApproveErasureRequest" : "RejectErasureRequest",
                "ErasureRequest", erasureRequest.Id.ToString()),
            cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);
    }
}
