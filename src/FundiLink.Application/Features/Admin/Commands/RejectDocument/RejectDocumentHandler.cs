using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Admin.Commands.RejectDocument;

public class RejectDocumentHandler : IRequestHandler<RejectDocumentCommand>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public RejectDocumentHandler(IDocumentRepository documentRepository, IAuditLogRepository auditLogRepository)
    {
        _documentRepository = documentRepository;
        _auditLogRepository = auditLogRepository;
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
    }
}
