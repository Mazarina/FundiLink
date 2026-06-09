using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Admin.Commands.VerifyDocument;

public class VerifyDocumentHandler : IRequestHandler<VerifyDocumentCommand>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public VerifyDocumentHandler(IDocumentRepository documentRepository, IAuditLogRepository auditLogRepository)
    {
        _documentRepository = documentRepository;
        _auditLogRepository = auditLogRepository;
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
    }
}
