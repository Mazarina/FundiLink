using FundiLink.Application.Common;
using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Documents.Commands.ReplaceDocument;

public class ReplaceDocumentHandler : IRequestHandler<ReplaceDocumentCommand>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentStorageService _storageService;

    public ReplaceDocumentHandler(
        ILearnerRepository learnerRepository,
        IDocumentRepository documentRepository,
        IDocumentStorageService storageService)
    {
        _learnerRepository = learnerRepository;
        _documentRepository = documentRepository;
        _storageService = storageService;
    }

    public async Task Handle(ReplaceDocumentCommand request, CancellationToken cancellationToken)
    {
        DocumentValidation.Validate(request.FileName, request.ContentType, request.SizeBytes);

        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken)
            ?? throw new KeyNotFoundException("Document not found.");

        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        if (document.LearnerId != learner.Id)
            throw new UnauthorizedAccessException("You do not have permission to replace this document.");

        await _storageService.StoreAsync(request.Content, request.ContentType, document.StorageKey, cancellationToken);

        document.ReplaceFile(request.FileName, request.ContentType, request.SizeBytes);
        await _documentRepository.SaveChangesAsync(cancellationToken);
    }
}
