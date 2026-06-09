using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Common.Models;
using MediatR;

namespace FundiLink.Application.Features.ProgrammeMatching.Queries.SearchProgrammes;

public class SearchProgrammesHandler : IRequestHandler<SearchProgrammesQuery, PagedResult<ProgrammeDto>>
{
    private readonly IProgrammeRepository _programmeRepository;

    public SearchProgrammesHandler(IProgrammeRepository programmeRepository)
    {
        _programmeRepository = programmeRepository;
    }

    public async Task<PagedResult<ProgrammeDto>> Handle(SearchProgrammesQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var (items, total) = await _programmeRepository.SearchAsync(
            request.Keyword, request.Type, request.Province, page, pageSize, cancellationToken);

        var dtos = items.Select(p => new ProgrammeDto(
            p.Id,
            p.Name,
            p.Institution.Name,
            p.Institution.InstitutionType,
            p.Institution.Province,
            p.MinimumAps,
            p.NfqLevel,
            p.ApplicationOpenDate,
            p.ApplicationCloseDate
        )).ToList();

        return new PagedResult<ProgrammeDto>(dtos, total, page, pageSize);
    }
}
