using System.Security.Claims;
using FundiLink.Application.Features.Learners.Commands.UpdatePersonalInfo;
using FundiLink.Application.Features.Learners.Queries.GetMyProfile;
using FundiLink.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundiLink.Api.Controllers;

[ApiController]
[Route("api/v1/learners")]
[Authorize]
public class LearnersController : ControllerBase
{
    private readonly IMediator _mediator;

    public LearnersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(LearnerProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _mediator.Send(new GetMyProfileQuery(userId), cancellationToken);
        return Ok(result);
    }

    [HttpPut("me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdatePersonalInfoRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _mediator.Send(new UpdatePersonalInfoCommand(
            userId,
            request.FirstName, request.Surname, request.IdNumber, request.PassportNumber,
            request.Gender, request.HomeLanguage, request.Nationality, request.MobileNumber,
            request.Province, request.Municipality, request.Suburb, request.SchoolName,
            request.SchoolProvince, request.GradeLevel, request.GuardianName,
            request.GuardianPhone, request.GuardianEmail),
            cancellationToken);

        return NoContent();
    }
}
