using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Checklist.Queries.GetApplicationChecklist;

public class GetApplicationChecklistHandler : IRequestHandler<GetApplicationChecklistQuery, IEnumerable<ChecklistItemDto>>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IChecklistRepository _checklistRepository;
    private readonly IDocumentRepository _documentRepository;

    public GetApplicationChecklistHandler(
        ILearnerRepository learnerRepository,
        IApplicationRepository applicationRepository,
        IChecklistRepository checklistRepository,
        IDocumentRepository documentRepository)
    {
        _learnerRepository = learnerRepository;
        _applicationRepository = applicationRepository;
        _checklistRepository = checklistRepository;
        _documentRepository = documentRepository;
    }

    public async Task<IEnumerable<ChecklistItemDto>> Handle(GetApplicationChecklistQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var application = await _applicationRepository.GetByIdAsync(request.ApplicationId, cancellationToken)
            ?? throw new KeyNotFoundException("Application not found.");

        if (application.LearnerId != learner.Id)
            throw new UnauthorizedAccessException("You do not have permission to view this checklist.");

        var items = await _checklistRepository.GetByApplicationIdAsync(request.ApplicationId, cancellationToken);

        var result = new List<ChecklistItemDto>();
        foreach (var item in items)
        {
            Domain.Enums.DocumentStatus? linkedStatus = null;
            if (item.LinkedDocumentId.HasValue)
            {
                var doc = await _documentRepository.GetByIdAsync(item.LinkedDocumentId.Value, cancellationToken);
                linkedStatus = doc?.Status;
            }

            result.Add(new ChecklistItemDto(
                item.Id,
                item.DocumentType,
                item.IsRequired,
                item.LinkedDocumentId,
                linkedStatus));
        }

        return result;
    }
}
