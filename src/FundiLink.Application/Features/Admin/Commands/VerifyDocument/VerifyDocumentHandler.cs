using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Admin.Commands.VerifyDocument;

public class VerifyDocumentHandler : IRequestHandler<VerifyDocumentCommand>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly INotificationService _notificationService;

    public VerifyDocumentHandler(
        IDocumentRepository documentRepository,
        IAuditLogRepository auditLogRepository,
        INotificationService notificationService)
    {
        _documentRepository = documentRepository;
        _auditLogRepository = auditLogRepository;
        _notificationService = notificationService;
    }

    public async Task Handle(VerifyDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken)
            ?? throw new KeyNotFoundException("Document not found.");

        document.Verify(request.ActorUserId);
        await _documentRepository.SaveChangesAsync(cancellationToken);

        await _auditLogRepository.AddAsync(
            AuditLogEntry.Create(request.ActorUserId, request.ActorRole, "VerifyDocument", "Document", request.DocumentId.ToString()),
            cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);

        await _notificationService.NotifyAsync(
            document.LearnerId,
            NotificationType.DocumentVerificationResult,
            "Document verified",
            "One of your uploaded documents has been verified.",
            cancellationToken);
    }
}
