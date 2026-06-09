using FundiLink.Application.Common.Models;
using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.ProgrammeMatching.Queries.SearchProgrammes;

public record SearchProgrammesQuery(
    string? Keyword,
    InstitutionType? Type,
    string? Province,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<ProgrammeDto>>;

public record ProgrammeDto(
    Guid Id,
    string Name,
    string InstitutionName,
    InstitutionType InstitutionType,
    string Province,
    int MinimumAps,
    int? NfqLevel,
    DateTime? ApplicationOpenDate,
    DateTime? ApplicationCloseDate
);
