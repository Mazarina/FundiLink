using System.Security.Claims;
using FundiLink.Api.Models;
using FundiLink.Application.Features.AcademicProfile.Commands.SaveAcademicProfile;
using FundiLink.Application.Features.AcademicProfile.Queries.GetAcademicProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundiLink.Api.Controllers;

[ApiController]
[Route("api/v1/learners/me")]
[Authorize]
public class AcademicProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public AcademicProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("academic-profile")]
    [ProducesResponseType(typeof(AcademicProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAcademicProfile(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _mediator.Send(new GetAcademicProfileQuery(userId), cancellationToken);
        return result is null ? NoContent() : Ok(result);
    }

    [HttpGet("aps")]
    [ProducesResponseType(typeof(ApsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAps(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _mediator.Send(new GetAcademicProfileQuery(userId), cancellationToken);
        if (result is null) return NoContent();

        return Ok(new ApsDto(result.ApsScore, result.ApsCalculatedAt, result.Subjects));
    }

    [HttpPut("academic-profile")]
    [ProducesResponseType(typeof(SaveAcademicProfileResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> SaveAcademicProfile([FromBody] SaveAcademicProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var subjects = request.Subjects.Select(s =>
            new SubjectInputDto(s.SubjectName, s.Percentage, s.IsHomeLanguage, s.IsLifeOrientation, s.SubjectCode)
        ).ToList();

        var result = await _mediator.Send(
            new SaveAcademicProfileCommand(userId, request.Year, request.ResultType, subjects),
            cancellationToken);

        return Ok(result);
    }
}

public record ApsDto(int ApsScore, DateTime? CalculatedAt, IReadOnlyList<SubjectResultDto> Subjects);
