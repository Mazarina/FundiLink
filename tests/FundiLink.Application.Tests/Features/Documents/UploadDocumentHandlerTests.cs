using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.Documents.Commands.UploadDocument;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Moq;

namespace FundiLink.Application.Tests.Features.Documents;

public class UploadDocumentHandlerTests
{
    private readonly Mock<ILearnerRepository> _learnerRepository = new();
    private readonly Mock<IDocumentRepository> _documentRepository = new();
    private readonly Mock<IDocumentStorageService> _storageService = new();
    private readonly UploadDocumentHandler _sut;

    public UploadDocumentHandlerTests()
    {
        _sut = new UploadDocumentHandler(_learnerRepository.Object, _documentRepository.Object, _storageService.Object);
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(BuildLearner());
    }

    private static Learner BuildLearner() => Learner.Create(
        "user-1", "Thabo", "Nkosi", new DateOnly(2005, 1, 1),
        "0712345678", "Gauteng", "School", "Gauteng", GradeLevel.Grade12, true, "v1");

    private UploadDocumentCommand Command(string contentType, long size) =>
        new("user-1", DocumentType.IdDocument, "file", contentType, size, new MemoryStream(new byte[1]));

    [Fact]
    public async Task ValidPdf_Succeeds_DocumentPending()
    {
        Document? captured = null;
        _documentRepository.Setup(x => x.AddAsync(It.IsAny<Document>(), It.IsAny<CancellationToken>()))
            .Callback<Document, CancellationToken>((d, _) => captured = d).Returns(Task.CompletedTask);

        var result = await _sut.Handle(Command("application/pdf", 1000), CancellationToken.None);

        result.Should().NotBeEmpty();
        captured.Should().NotBeNull();
        captured!.Status.Should().Be(DocumentStatus.Pending);
        _storageService.Verify(x => x.StoreAsync(It.IsAny<Stream>(), "application/pdf", It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InvalidContentType_Throws()
    {
        var act = () => _sut.Handle(Command("text/plain", 1000), CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task FileOver10Mb_Throws()
    {
        var act = () => _sut.Handle(Command("application/pdf", 11 * 1024 * 1024), CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ValidJpeg_Accepted()
    {
        var result = await _sut.Handle(Command("image/jpeg", 5000), CancellationToken.None);
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ValidPng_Accepted()
    {
        var result = await _sut.Handle(Command("image/png", 5000), CancellationToken.None);
        result.Should().NotBeEmpty();
    }
}
