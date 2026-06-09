using System.Security.Claims;
using FundiLink.Application.Features.Documents.Commands.DeleteDocument;
using FundiLink.Application.Features.Documents.Commands.UploadDocument;
using FundiLink.Application.Features.Documents.Queries.DownloadDocument;
using FundiLink.Application.Features.Documents.Queries.GetMyDocuments;
using FundiLink.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundiLink.Api.Controllers;

[ApiController]
[Route("api/v1/documents")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DocumentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> Upload(
        [FromForm] IFormFile file,
        [FromForm] DocumentType documentType,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { detail = "A file is required." });

        await using var stream = file.OpenReadStream();
        var id = await _mediator.Send(
            new UploadDocumentCommand(UserId, documentType, file.FileName, file.ContentType, file.Length, stream),
            cancellationToken);

        return CreatedAtAction(nameof(GetMine), new { }, new { id });
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DocumentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMyDocumentsQuery(UserId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DownloadDocumentQuery(id, UserId), cancellationToken);
        Response.Headers.ContentDisposition = $"attachment; filename=\"{result.FileName}\"";
        return File(result.Stream, result.ContentType);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteDocumentCommand(id, UserId), cancellationToken);
        return NoContent();
    }
}
