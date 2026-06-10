using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.Documents.Queries.DownloadDocument;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Moq;

namespace FundiLink.Application.Tests.Features.Documents;

public class DownloadDocumentHandlerTests
{
    private readonly Mock<ILearnerRepository> _learnerRepository = new();
    private readonly Mock<IDocumentRepository> _documentRepository = new();
    private readonly Mock<IDocumentStorageService> _storageService = new();
    private readonly Mock<IAuditLogRepository> _auditLogRepository = new();
    private readonly DownloadDocumentHandler _sut;

    public DownloadDocumentHandlerTests()
    {
        _sut = new DownloadDocumentHandler(
            _learnerRepository.Object, _documentRepository.Object, _storageService.Object, _auditLogRepository.Object);
    }

    private static Learner BuildLearner() => Learner.Create(
        "user-1", "Thabo", "Nkosi", new DateOnly(2005, 1, 1),
        "0712345678", "Gauteng", "School", "Gauteng", GradeLevel.Grade12, true, "v1");

    private static Document BuildDocument(Guid learnerId) =>
        Document.Create(learnerId, DocumentType.IdDocument, "file.pdf", "application/pdf", 1000, "key");

    [Fact]
    public async Task Owner_Downloads_Succeeds()
    {
        var learner = BuildLearner();
        var document = BuildDocument(learner.Id);
        _documentRepository.Setup(x => x.GetByIdAsync(document.Id, It.IsAny<CancellationToken>())).ReturnsAsync(document);
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        _storageService.Setup(x => x.GetAsync("key", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Stream.Null, "application/pdf"));

        var result = await _sut.Handle(new DownloadDocumentQuery(document.Id, "user-1"), CancellationToken.None);

        result.FileName.Should().Be("file.pdf");
        _auditLogRepository.Verify(x => x.AddAsync(It.IsAny<AuditLogEntry>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task NonOwner_Throws()
    {
        var document = BuildDocument(Guid.NewGuid());
        _documentRepository.Setup(x => x.GetByIdAsync(document.Id, It.IsAny<CancellationToken>())).ReturnsAsync(document);
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(BuildLearner());

        var act = () => _sut.Handle(new DownloadDocumentQuery(document.Id, "user-1"), CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task NotFound_Throws()
    {
        _documentRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Document?)null);

        var act = () => _sut.Handle(new DownloadDocumentQuery(Guid.NewGuid(), "user-1"), CancellationToken.None);
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task AdminOrSupport_Downloads_WritesAuditLog_NoOwnershipCheck()
    {
        var document = BuildDocument(Guid.NewGuid());
        _documentRepository.Setup(x => x.GetByIdAsync(document.Id, It.IsAny<CancellationToken>())).ReturnsAsync(document);
        _storageService.Setup(x => x.GetAsync("key", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Stream.Null, "application/pdf"));

        var result = await _sut.Handle(
            new DownloadDocumentQuery(document.Id, "support-user", IsAdminOrSupport: true, ActorRole: "SupportAgent"),
            CancellationToken.None);

        result.FileName.Should().Be("file.pdf");
        _learnerRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _auditLogRepository.Verify(x => x.AddAsync(
            It.Is<AuditLogEntry>(e => e.Action == "DownloadDocument" && e.ActorUserId == "support-user" && e.ActorRole == "SupportAgent"),
            It.IsAny<CancellationToken>()), Times.Once);
        _auditLogRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
