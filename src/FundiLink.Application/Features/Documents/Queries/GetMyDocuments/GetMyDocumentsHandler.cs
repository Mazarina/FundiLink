using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Documents.Queries.GetMyDocuments;

public class GetMyDocumentsHandler : IRequestHandler<GetMyDocumentsQuery, IEnumerable<DocumentDto>>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IDocumentRepository _documentRepository;

    public GetMyDocumentsHandler(ILearnerRepository learnerRepository, IDocumentRepository documentRepository)
    {
        _learnerRepository = learnerRepository;
        _documentRepository = documentRepository;
    }

    public async Task<IEnumerable<DocumentDto>> Handle(GetMyDocumentsQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var documents = await _documentRepository.GetByLearnerIdAsync(learner.Id, cancellationToken);

        return documents.Select(d => new DocumentDto(
            d.Id,
            d.DocumentType,
            d.FileName,
            d.SizeBytes,
            d.Status,
            d.CreatedAt,
            d.RejectionReason));
    }
}
