using MediatR;

namespace FundiLink.Application.Features.Checklist.Commands.LinkDocumentToChecklist;

public record LinkDocumentToChecklistCommand(
    Guid ApplicationId,
    Guid ChecklistItemId,
    Guid DocumentId,
    string UserId) : IRequest;
