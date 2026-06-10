using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Admin.Commands.RejectDocument;

public class RejectDocumentHandler : IRequestHandler<RejectDocumentCommand>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly INotificationService _notificationService;

    public RejectDocumentHandler(
        IDocumentRepository documentRepository,
        IAuditLogRepository auditLogRepository,
        INotificationService notificationService)
    {
        _documentRepository = documentRepository;
        _auditLogRepository = auditLogRepository;
        _notificationService = notificationService;
    }

    public async Task Handle(RejectDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken)
            ?? throw new KeyNotFoundException("Document not found.");

        document.Reject(request.Reason);
        await _documentRepository.SaveChangesAsync(cancellationToken);

        await _auditLogRepository.AddAsync(
            AuditLogEntry.Create(request.ActorUserId, request.ActorRole, "RejectDocument", "Document", request.DocumentId.ToString()),
            cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);

        await _notificationService.NotifyAsync(
            document.LearnerId,
            NotificationType.DocumentVerificationResult,
            "Document needs attention",
            $"One of your uploaded documents was not accepted: {request.Reason}",
            cancellationToken);
    }
}
