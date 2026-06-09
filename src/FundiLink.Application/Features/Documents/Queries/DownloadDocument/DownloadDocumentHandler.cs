using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Documents.Queries.DownloadDocument;

public class DownloadDocumentHandler : IRequestHandler<DownloadDocumentQuery, DownloadDocumentResult>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentStorageService _storageService;

    public DownloadDocumentHandler(
        ILearnerRepository learnerRepository,
        IDocumentRepository documentRepository,
        IDocumentStorageService storageService)
    {
        _learnerRepository = learnerRepository;
        _documentRepository = documentRepository;
        _storageService = storageService;
    }

    public async Task<DownloadDocumentResult> Handle(DownloadDocumentQuery request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken)
            ?? throw new KeyNotFoundException("Document not found.");

        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        if (document.LearnerId != learner.Id)
            throw new UnauthorizedAccessException("You do not have permission to access this document.");

        var (stream, _) = await _storageService.GetAsync(document.StorageKey, cancellationToken);
        return new DownloadDocumentResult(stream, document.ContentType, document.FileName);
    }
}
