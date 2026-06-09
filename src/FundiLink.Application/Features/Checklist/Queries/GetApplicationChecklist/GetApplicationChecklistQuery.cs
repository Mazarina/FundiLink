using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Checklist.Queries.GetApplicationChecklist;

public record GetApplicationChecklistQuery(Guid ApplicationId, string UserId) : IRequest<IEnumerable<ChecklistItemDto>>;

public record ChecklistItemDto(
    Guid Id,
    DocumentType DocumentType,
    bool IsRequired,
    Guid? LinkedDocumentId,
    DocumentStatus? LinkedDocumentStatus);
