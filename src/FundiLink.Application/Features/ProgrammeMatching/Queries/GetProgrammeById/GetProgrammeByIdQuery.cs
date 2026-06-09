using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.ProgrammeMatching.Queries.GetProgrammeById;

public record GetProgrammeByIdQuery(Guid Id) : IRequest<ProgrammeDetailDto?>;

public record RequiredSubjectDto(string SubjectName, int MinimumPercentage);

public record ProgrammeDetailDto(
    Guid Id,
    string Name,
    string InstitutionName,
    InstitutionType InstitutionType,
    string Province,
    string? Website,
    string? FacultyOrSchool,
    int MinimumAps,
    int? NfqLevel,
    DateTime? ApplicationOpenDate,
    DateTime? ApplicationCloseDate,
    IReadOnlyList<RequiredSubjectDto> RequiredSubjects
);
