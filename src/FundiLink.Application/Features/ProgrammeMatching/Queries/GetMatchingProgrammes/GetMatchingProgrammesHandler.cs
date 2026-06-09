using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.ProgrammeMatching.Services;
using MediatR;

namespace FundiLink.Application.Features.ProgrammeMatching.Queries.GetMatchingProgrammes;

public class GetMatchingProgrammesHandler : IRequestHandler<GetMatchingProgrammesQuery, IEnumerable<ProgrammeMatchDto>>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IProgrammeRepository _programmeRepository;
    private readonly ProgrammeMatchingService _matchingService;

    public GetMatchingProgrammesHandler(
        ILearnerRepository learnerRepository,
        IProgrammeRepository programmeRepository,
        ProgrammeMatchingService matchingService)
    {
        _learnerRepository = learnerRepository;
        _programmeRepository = programmeRepository;
        _matchingService = matchingService;
    }

    public async Task<IEnumerable<ProgrammeMatchDto>> Handle(GetMatchingProgrammesQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var profile = await _learnerRepository.GetAcademicProfileByLearnerIdAsync(learner.Id, cancellationToken);
        if (profile is null) return [];

        var programmes = await _programmeRepository.GetAllWithInstitutionAsync(cancellationToken);

        return _matchingService.GetMatchingProgrammes(profile, programmes)
            .Select(m => new ProgrammeMatchDto(
                m.Programme.Id,
                m.Programme.Name,
                m.InstitutionName,
                m.Programme.Institution.InstitutionType,
                m.Programme.MinimumAps,
                m.IsEligible,
                m.ApsGap,
                m.MissingSubjects
            ))
            .ToList();
    }
}
