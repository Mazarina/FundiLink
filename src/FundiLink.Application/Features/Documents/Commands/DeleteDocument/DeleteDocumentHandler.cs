using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Documents.Commands.DeleteDocument;

public class DeleteDocumentHandler : IRequestHandler<DeleteDocumentCommand>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentStorageService _storageService;

    public DeleteDocumentHandler(
        ILearnerRepository learnerRepository,
        IDocumentRepository documentRepository,
        IDocumentStorageService storageService)
    {
        _learnerRepository = learnerRepository;
        _documentRepository = documentRepository;
        _storageService = storageService;
    }

    public async Task Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken)
            ?? throw new KeyNotFoundException("Document not found.");

        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        if (document.LearnerId != learner.Id)
            throw new UnauthorizedAccessException("You do not have permission to delete this document.");

        // POPIA right to erasure: remove from disk and soft-delete the record.
        await _storageService.DeleteAsync(document.StorageKey, cancellationToken);
        document.SoftDelete();
        await _documentRepository.SaveChangesAsync(cancellationToken);
    }
}
