using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.ProgrammeMatching.Queries.GetProgrammeById;

public class GetProgrammeByIdHandler : IRequestHandler<GetProgrammeByIdQuery, ProgrammeDetailDto?>
{
    private readonly IProgrammeRepository _programmeRepository;

    public GetProgrammeByIdHandler(IProgrammeRepository programmeRepository)
    {
        _programmeRepository = programmeRepository;
    }

    public async Task<ProgrammeDetailDto?> Handle(GetProgrammeByIdQuery request, CancellationToken cancellationToken)
    {
        var programme = await _programmeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (programme is null) return null;

        return new ProgrammeDetailDto(
            programme.Id,
            programme.Name,
            programme.Institution.Name,
            programme.Institution.InstitutionType,
            programme.Institution.Province,
            programme.Institution.Website,
            programme.FacultyOrSchool,
            programme.MinimumAps,
            programme.NfqLevel,
            programme.ApplicationOpenDate,
            programme.ApplicationCloseDate,
            programme.RequiredSubjects
                .Select(s => new RequiredSubjectDto(s.SubjectName, s.MinimumPercentage))
                .ToList()
        );
    }
}
