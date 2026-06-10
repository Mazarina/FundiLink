using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.Bursaries.Queries.GetBursaryMatches;
using FundiLink.Application.Features.ProgrammeMatching.Services;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Assistant.Queries.AskAssistant;

/// <summary>
/// Loads the caller's own FundiLink data, builds a grounded context, and asks the
/// rule-based assistant for a guidance-only answer. Owner-scoped: only the caller's
/// data is ever read. Logs the interaction (POPIA-minimal: intent + timestamp only).
/// </summary>
public class AskAssistantHandler : IRequestHandler<AskAssistantQuery, AssistantResponseDto>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IProgrammeRepository _programmeRepository;
    private readonly IBursaryRepository _bursaryRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IAiAssistantService _assistantService;
    private readonly IAssistantInteractionLogRepository _interactionLogRepository;
    private readonly ProgrammeMatchingService _matchingService;

    public AskAssistantHandler(
        ILearnerRepository learnerRepository,
        IProgrammeRepository programmeRepository,
        IBursaryRepository bursaryRepository,
        IDocumentRepository documentRepository,
        IAiAssistantService assistantService,
        IAssistantInteractionLogRepository interactionLogRepository,
        ProgrammeMatchingService matchingService)
    {
        _learnerRepository = learnerRepository;
        _programmeRepository = programmeRepository;
        _bursaryRepository = bursaryRepository;
        _documentRepository = documentRepository;
        _assistantService = assistantService;
        _interactionLogRepository = interactionLogRepository;
        _matchingService = matchingService;
    }

    public async Task<AssistantResponseDto> Handle(AskAssistantQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var profile = await _learnerRepository.GetAcademicProfileByLearnerIdAsync(learner.Id, cancellationToken);
        var apsScore = profile?.ApsScore;

        var programmeMatches = await BuildProgrammeMatchesAsync(profile, cancellationToken);
        var bursaryMatches = await BuildBursaryMatchesAsync(learner, apsScore, cancellationToken);
        var uploadedDocumentTypes = await BuildUploadedDocumentTypesAsync(learner.Id, cancellationToken);

        var serviceRequest = new AssistantRequest(
            request.Intent,
            learner.FirstName,
            apsScore,
            learner.Province,
            programmeMatches,
            bursaryMatches,
            uploadedDocumentTypes);

        var result = await _assistantService.AnswerAsync(serviceRequest, cancellationToken);

        await _interactionLogRepository.AddAsync(
            AssistantInteractionLog.Create(learner.Id, request.Intent), cancellationToken);
        await _interactionLogRepository.SaveChangesAsync(cancellationToken);

        return new AssistantResponseDto(
            request.Intent,
            result.Answer,
            result.Sources,
            GuidanceOnly: true,
            AssistantDisclaimer.Text);
    }

    private async Task<IReadOnlyList<ProgrammeMatchContext>> BuildProgrammeMatchesAsync(
        Domain.Entities.AcademicProfile? profile, CancellationToken ct)
    {
        if (profile is null) return [];

        var programmes = await _programmeRepository.GetAllWithInstitutionAsync(ct);
        return _matchingService.GetMatchingProgrammes(profile, programmes)
            .Select(m => new ProgrammeMatchContext(
                m.Programme.Name, m.InstitutionName, m.Programme.MinimumAps, m.IsEligible, m.ApsGap))
            .ToList();
    }

    private async Task<IReadOnlyList<BursaryMatchContext>> BuildBursaryMatchesAsync(
        Learner learner, int? apsScore, CancellationToken ct)
    {
        var bursaries = await _bursaryRepository.GetAllActiveAsync(ct);
        return bursaries
            .Where(b => GetBursaryMatchesHandler.IsMatch(b, apsScore, learner.Province, out _))
            .Select(b => new BursaryMatchContext(b.Name, b.ProviderName, b.MinimumAps))
            .ToList();
    }

    private async Task<IReadOnlyList<string>> BuildUploadedDocumentTypesAsync(Guid learnerId, CancellationToken ct)
    {
        var documents = await _documentRepository.GetByLearnerIdAsync(learnerId, ct);
        return documents
            .Select(d => d.DocumentType.ToString())
            .Distinct()
            .ToList();
    }
}
