using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Documents.Commands.UploadDocument;

public class UploadDocumentHandler : IRequestHandler<UploadDocumentCommand, Guid>
{
    public const long MaxSizeBytes = 10 * 1024 * 1024;
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "image/jpeg",
        "image/png"
    };

    private readonly ILearnerRepository _learnerRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentStorageService _storageService;

    public UploadDocumentHandler(
        ILearnerRepository learnerRepository,
        IDocumentRepository documentRepository,
        IDocumentStorageService storageService)
    {
        _learnerRepository = learnerRepository;
        _documentRepository = documentRepository;
        _storageService = storageService;
    }

    public async Task<Guid> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        if (!AllowedContentTypes.Contains(request.ContentType))
            throw new InvalidOperationException("Only PDF, JPEG, and PNG files are allowed.");

        if (request.SizeBytes <= 0 || request.SizeBytes > MaxSizeBytes)
            throw new InvalidOperationException("File must be larger than 0 bytes and 10 MB or smaller.");

        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var documentId = Guid.NewGuid();
        var storageKey = $"{learner.Id}/{documentId}";

        await _storageService.StoreAsync(request.Content, request.ContentType, storageKey, cancellationToken);

        var document = Document.Create(
            learner.Id,
            request.DocumentType,
            request.FileName,
            request.ContentType,
            request.SizeBytes,
            storageKey);

        await _documentRepository.AddAsync(document, cancellationToken);
        await _documentRepository.SaveChangesAsync(cancellationToken);

        return document.Id;
    }
}
