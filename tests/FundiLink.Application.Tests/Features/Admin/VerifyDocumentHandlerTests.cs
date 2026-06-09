using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.Admin.Commands.VerifyDocument;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Moq;

namespace FundiLink.Application.Tests.Features.Admin;

public class VerifyDocumentHandlerTests
{
    private readonly Mock<IDocumentRepository> _documentRepository = new();
    private readonly Mock<IAuditLogRepository> _auditLogRepository = new();
    private readonly VerifyDocumentHandler _sut;

    public VerifyDocumentHandlerTests()
    {
        _sut = new VerifyDocumentHandler(_documentRepository.Object, _auditLogRepository.Object);
    }

    private static Document BuildDocument() =>
        Document.Create(Guid.NewGuid(), DocumentType.IdDocument, "file.pdf", "application/pdf", 1000, "key");

    [Fact]
    public async Task Verify_Succeeds_StatusVerifiedAndVerifierSet()
    {
        var document = BuildDocument();
        _documentRepository.Setup(x => x.GetByIdAsync(document.Id, It.IsAny<CancellationToken>())).ReturnsAsync(document);

        await _sut.Handle(new VerifyDocumentCommand("admin-1", "Admin", document.Id), CancellationToken.None);

        document.Status.Should().Be(DocumentStatus.Verified);
        document.VerifiedByUserId.Should().Be("admin-1");
    }

    [Fact]
    public async Task Verify_WritesAuditLog()
    {
        var document = BuildDocument();
        _documentRepository.Setup(x => x.GetByIdAsync(document.Id, It.IsAny<CancellationToken>())).ReturnsAsync(document);

        await _sut.Handle(new VerifyDocumentCommand("admin-1", "Admin", document.Id), CancellationToken.None);

        _auditLogRepository.Verify(x => x.AddAsync(
            It.Is<AuditLogEntry>(a => a.Action == "VerifyDocument" && a.TargetType == "Document"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task NotFound_Throws()
    {
        _documentRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Document?)null);

        var act = () => _sut.Handle(new VerifyDocumentCommand("admin-1", "Admin", Guid.NewGuid()), CancellationToken.None);
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
