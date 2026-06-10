using System.Security.Claims;
using FundiLink.Api.Models;
using FundiLink.Application.Features.Admin.Commands.CreateBursary;
using FundiLink.Application.Features.Admin.Commands.CreateInstitution;
using FundiLink.Application.Features.Admin.Commands.CreateProgramme;
using FundiLink.Application.Features.Admin.Commands.UpdateBursary;
using FundiLink.Application.Features.Admin.Commands.RejectDocument;
using FundiLink.Application.Features.Admin.Commands.UpdateInstitution;
using FundiLink.Application.Features.Admin.Commands.UpdateProgramme;
using FundiLink.Application.Features.Admin.Commands.VerifyDocument;
using FundiLink.Application.Features.Admin.Queries.GetLearnerOverview;
using FundiLink.Application.Features.Admin.Queries.SearchLearners;
using FundiLink.Application.Features.Documents.Queries.DownloadDocument;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundiLink.Api.Controllers;

[ApiController]
[Route("api/v1/admin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string ActorUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private string ActorRole => User.FindFirstValue(ClaimTypes.Role) ?? "Unknown";

    [HttpGet("learners")]
    [Authorize(Roles = "SupportAgent,Admin,SuperAdmin")]
    public async Task<IActionResult> SearchLearners(
        [FromQuery] string? keyword,
        [FromQuery] string? province,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new SearchLearnersQuery(ActorUserId, ActorRole, keyword, province, page, pageSize),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("learners/{id:guid}")]
    [Authorize(Roles = "SupportAgent,Admin,SuperAdmin")]
    public async Task<IActionResult> GetLearner(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetLearnerOverviewQuery(ActorUserId, ActorRole, id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("documents/{id:guid}/download")]
    [Authorize(Roles = "SupportAgent,Admin,SuperAdmin")]
    public async Task<IActionResult> DownloadDocument(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DownloadDocumentQuery(id, ActorUserId, IsAdminOrSupport: true, ActorRole: ActorRole),
            cancellationToken);
        Response.Headers.ContentDisposition = $"attachment; filename=\"{result.FileName}\"";
        return File(result.Stream, result.ContentType);
    }

    [HttpPost("documents/{id:guid}/verify")]
    [Authorize(Roles = "SupportAgent,Admin,SuperAdmin")]
    public async Task<IActionResult> VerifyDocument(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new VerifyDocumentCommand(ActorUserId, ActorRole, id), cancellationToken);
        return NoContent();
    }

    [HttpPost("documents/{id:guid}/reject")]
    [Authorize(Roles = "SupportAgent,Admin,SuperAdmin")]
    public async Task<IActionResult> RejectDocument(Guid id, [FromBody] RejectDocumentRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RejectDocumentCommand(ActorUserId, ActorRole, id, request.Reason), cancellationToken);
        return NoContent();
    }

    [HttpPost("institutions")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> CreateInstitution([FromBody] CreateInstitutionRequest request, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            new CreateInstitutionCommand(ActorUserId, ActorRole, request.Name, request.InstitutionType, request.Province, request.Website),
            cancellationToken);
        return Ok(new { id });
    }

    [HttpPut("institutions/{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> UpdateInstitution(Guid id, [FromBody] UpdateInstitutionRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new UpdateInstitutionCommand(ActorUserId, ActorRole, id, request.Name, request.InstitutionType, request.Province, request.Website, request.IsActive),
            cancellationToken);
        return NoContent();
    }

    [HttpPost("programmes")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> CreateProgramme([FromBody] CreateProgrammeRequest request, CancellationToken cancellationToken)
    {
        var subjects = request.RequiredSubjects.Select(s => new RequiredSubjectInput(s.SubjectName, s.MinimumPercentage)).ToList();
        var id = await _mediator.Send(
            new CreateProgrammeCommand(ActorUserId, ActorRole, request.InstitutionId, request.Name, request.FacultyOrSchool,
                request.NfqLevel, request.MinimumAps, subjects, request.ApplicationOpenDate, request.ApplicationCloseDate),
            cancellationToken);
        return Ok(new { id });
    }

    [HttpPut("programmes/{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> UpdateProgramme(Guid id, [FromBody] UpdateProgrammeRequest request, CancellationToken cancellationToken)
    {
        var subjects = request.RequiredSubjects.Select(s => new RequiredSubjectInput(s.SubjectName, s.MinimumPercentage)).ToList();
        await _mediator.Send(
            new UpdateProgrammeCommand(ActorUserId, ActorRole, id, request.Name, request.FacultyOrSchool, request.NfqLevel,
                request.MinimumAps, subjects, request.ApplicationOpenDate, request.ApplicationCloseDate, request.IsActive),
            cancellationToken);
        return NoContent();
    }

    // Bursary data is curated public information for guidance only. Admin writes are audit-logged.
    [HttpPost("bursaries")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> CreateBursary([FromBody] CreateBursaryRequest request, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            new CreateBursaryCommand(ActorUserId, ActorRole, request.Name, request.ProviderName, request.Description,
                request.FundingType, request.FieldsOfStudy, request.MinimumAps, request.MaxHouseholdIncome,
                request.ProvincesEligible, request.ApplicationOpenDate, request.ApplicationCloseDate, request.ExternalApplicationUrl),
            cancellationToken);
        return Ok(new { id });
    }

    [HttpPut("bursaries/{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> UpdateBursary(Guid id, [FromBody] UpdateBursaryRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new UpdateBursaryCommand(ActorUserId, ActorRole, id, request.Name, request.ProviderName, request.Description,
                request.FundingType, request.FieldsOfStudy, request.MinimumAps, request.MaxHouseholdIncome,
                request.ProvincesEligible, request.ApplicationOpenDate, request.ApplicationCloseDate,
                request.ExternalApplicationUrl, request.IsActive),
            cancellationToken);
        return NoContent();
    }
}
