using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.DataRights.Commands.FulfilErasureRequest;

/// <summary>
/// Fulfils an erasure request by invoking the deterministic erasure service to anonymise/
/// soft-delete the learner's personal data, then marks the request Fulfilled and audit-logs
/// the action (append-only). The erasure service NEVER touches append-only audit or consent
/// records — POPIA proof-of-processing retention. No third-party delivery integration here.
/// </summary>
public class FulfilErasureRequestHandler : IRequestHandler<FulfilErasureRequestCommand>
{
    private readonly IErasureRequestRepository _erasureRequestRepository;
    private readonly IErasureService _erasureService;
    private readonly IAuditLogRepository _auditLogRepository;

    public FulfilErasureRequestHandler(
        IErasureRequestRepository erasureRequestRepository,
        IErasureService erasureService,
        IAuditLogRepository auditLogRepository)
    {
        _erasureRequestRepository = erasureRequestRepository;
        _erasureService = erasureService;
        _auditLogRepository = auditLogRepository;
    }

    public async Task Handle(FulfilErasureRequestCommand request, CancellationToken cancellationToken)
    {
        var erasureRequest = await _erasureRequestRepository.GetByIdAsync(request.RequestId, cancellationToken)
            ?? throw new KeyNotFoundException("Erasure request not found.");

        await _erasureService.AnonymiseLearnerDataAsync(erasureRequest.LearnerId, cancellationToken);

        erasureRequest.MarkFulfilled(request.ActorUserId, request.Note);
        await _erasureRequestRepository.SaveChangesAsync(cancellationToken);

        await _auditLogRepository.AddAsync(
            AuditLogEntry.Create(
                request.ActorUserId, request.ActorRole, "FulfilErasureRequest",
                "ErasureRequest", erasureRequest.Id.ToString()),
            cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);
    }
}
