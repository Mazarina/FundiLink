using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Checklist.Commands.LinkDocumentToChecklist;

public class LinkDocumentToChecklistHandler : IRequestHandler<LinkDocumentToChecklistCommand>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IChecklistRepository _checklistRepository;
    private readonly IDocumentRepository _documentRepository;

    public LinkDocumentToChecklistHandler(
        ILearnerRepository learnerRepository,
        IChecklistRepository checklistRepository,
        IDocumentRepository documentRepository)
    {
        _learnerRepository = learnerRepository;
        _checklistRepository = checklistRepository;
        _documentRepository = documentRepository;
    }

    public async Task Handle(LinkDocumentToChecklistCommand request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var item = await _checklistRepository.GetByIdAsync(request.ChecklistItemId, cancellationToken)
            ?? throw new KeyNotFoundException("Checklist item not found.");

        if (item.LearnerApplicationId != request.ApplicationId)
            throw new InvalidOperationException("Checklist item does not belong to this application.");

        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken)
            ?? throw new KeyNotFoundException("Document not found.");

        if (document.LearnerId != learner.Id)
            throw new UnauthorizedAccessException("You do not have permission to link this document.");

        item.LinkDocument(document.Id);
        await _checklistRepository.SaveChangesAsync(cancellationToken);
    }
}
