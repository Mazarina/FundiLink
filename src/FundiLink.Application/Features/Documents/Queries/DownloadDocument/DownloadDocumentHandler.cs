using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Documents.Queries.DownloadDocument;

public class DownloadDocumentHandler : IRequestHandler<DownloadDocumentQuery, DownloadDocumentResult>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentStorageService _storageService;
    private readonly IAuditLogRepository _auditLogRepository;

    public DownloadDocumentHandler(
        ILearnerRepository learnerRepository,
        IDocumentRepository documentRepository,
        IDocumentStorageService storageService,
        IAuditLogRepository auditLogRepository)
    {
        _learnerRepository = learnerRepository;
        _documentRepository = documentRepository;
        _storageService = storageService;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<DownloadDocumentResult> Handle(DownloadDocumentQuery request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken)
            ?? throw new KeyNotFoundException("Document not found.");

        if (request.IsAdminOrSupport)
        {
            await _auditLogRepository.AddAsync(
                AuditLogEntry.Create(request.UserId, request.ActorRole ?? "Unknown", "DownloadDocument", "Document", request.DocumentId.ToString()),
                cancellationToken);
            await _auditLogRepository.SaveChangesAsync(cancellationToken);
        }
        else
        {
            var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
                ?? throw new KeyNotFoundException("Learner profile not found.");

            if (document.LearnerId != learner.Id)
                throw new UnauthorizedAccessException("You do not have permission to access this document.");
        }

        var (stream, _) = await _storageService.GetAsync(document.StorageKey, cancellationToken);
        return new DownloadDocumentResult(stream, document.ContentType, document.FileName);
    }
}
