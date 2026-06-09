using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.Documents.Commands.DeleteDocument;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Moq;

namespace FundiLink.Application.Tests.Features.Documents;

public class DeleteDocumentHandlerTests
{
    private readonly Mock<ILearnerRepository> _learnerRepository = new();
    private readonly Mock<IDocumentRepository> _documentRepository = new();
    private readonly Mock<IDocumentStorageService> _storageService = new();
    private readonly DeleteDocumentHandler _sut;

    public DeleteDocumentHandlerTests()
    {
        _sut = new DeleteDocumentHandler(_learnerRepository.Object, _documentRepository.Object, _storageService.Object);
    }

    private static Learner BuildLearner() => Learner.Create(
        "user-1", "Thabo", "Nkosi", new DateOnly(2005, 1, 1),
        "0712345678", "Gauteng", "School", "Gauteng", GradeLevel.Grade12, true, "v1");

    private static Document BuildDocument(Guid learnerId) =>
        Document.Create(learnerId, DocumentType.IdDocument, "file.pdf", "application/pdf", 1000, "key");

    [Fact]
    public async Task Owner_Deletes_StorageDeleteCalled_SoftDeleted()
    {
        var learner = BuildLearner();
        var document = BuildDocument(learner.Id);
        _documentRepository.Setup(x => x.GetByIdAsync(document.Id, It.IsAny<CancellationToken>())).ReturnsAsync(document);
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);

        await _sut.Handle(new DeleteDocumentCommand(document.Id, "user-1"), CancellationToken.None);

        _storageService.Verify(x => x.DeleteAsync("key", It.IsAny<CancellationToken>()), Times.Once);
        document.IsDeleted.Should().BeTrue();
        _documentRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task NonOwner_Throws()
    {
        var document = BuildDocument(Guid.NewGuid());
        _documentRepository.Setup(x => x.GetByIdAsync(document.Id, It.IsAny<CancellationToken>())).ReturnsAsync(document);
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(BuildLearner());

        var act = () => _sut.Handle(new DeleteDocumentCommand(document.Id, "user-1"), CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task NotFound_Throws()
    {
        _documentRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Document?)null);

        var act = () => _sut.Handle(new DeleteDocumentCommand(Guid.NewGuid(), "user-1"), CancellationToken.None);
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
