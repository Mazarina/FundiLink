using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.ProgrammeMatching.Queries.GetMatchingProgrammes;

public record GetMatchingProgrammesQuery(string UserId) : IRequest<IEnumerable<ProgrammeMatchDto>>;

public record ProgrammeMatchDto(
    Guid ProgrammeId,
    string ProgrammeName,
    string InstitutionName,
    InstitutionType InstitutionType,
    int MinimumAps,
    bool IsEligible,
    int ApsGap,
    IReadOnlyList<string> MissingSubjects
);
