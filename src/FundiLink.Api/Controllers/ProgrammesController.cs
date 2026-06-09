using System.Security.Claims;
using FundiLink.Application.Common.Models;
using FundiLink.Application.Features.ProgrammeMatching.Queries.GetMatchingProgrammes;
using FundiLink.Application.Features.ProgrammeMatching.Queries.GetProgrammeById;
using FundiLink.Application.Features.ProgrammeMatching.Queries.SearchProgrammes;
using FundiLink.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundiLink.Api.Controllers;

[ApiController]
[Route("api/v1/programmes")]
public class ProgrammesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProgrammesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // NOTE: Programme data is for guidance only and not official admission requirements.
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProgrammeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string? keyword,
        [FromQuery] InstitutionType? type,
        [FromQuery] string? province,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new SearchProgrammesQuery(keyword, type, province, page, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("matches")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<ProgrammeMatchDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMatches(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _mediator.Send(new GetMatchingProgrammesQuery(userId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProgrammeDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProgrammeByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
