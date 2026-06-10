using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.Bursaries.Commands.CreateBursaryApplication;
using FundiLink.Application.Features.Bursaries.Commands.DeleteBursaryApplication;
using FundiLink.Application.Features.Bursaries.Commands.UpdateBursaryApplicationStatus;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Moq;

namespace FundiLink.Application.Tests.Features.Bursaries;

public class BursaryApplicationHandlerTests
{
    private readonly Mock<ILearnerRepository> _learnerRepository = new();
    private readonly Mock<IBursaryRepository> _bursaryRepository = new();
    private readonly Mock<IBursaryApplicationRepository> _appRepository = new();
    private readonly Mock<INotificationService> _notificationService = new();

    private static Learner BuildLearner() => Learner.Create(
        "user-1", "Thabo", "Nkosi", new DateOnly(2005, 1, 1),
        "0712345678", "Gauteng", "School", "Gauteng", GradeLevel.Grade12, true, "v1");

    private static Bursary BuildBursary() =>
        Bursary.Create("Test Bursary", "Provider", "Desc", BursaryFundingType.TuitionOnly);

    [Fact]
    public async Task Create_ValidCommand_CreatesAndReturnsId()
    {
        var learner = BuildLearner();
        var bursary = BuildBursary();
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        _bursaryRepository.Setup(x => x.GetByIdAsync(bursary.Id, It.IsAny<CancellationToken>())).ReturnsAsync(bursary);
        var sut = new CreateBursaryApplicationHandler(_learnerRepository.Object, _bursaryRepository.Object, _appRepository.Object);

        var result = await sut.Handle(new CreateBursaryApplicationCommand(bursary.Id, "user-1"), CancellationToken.None);

        result.Should().NotBeEmpty();
        _appRepository.Verify(x => x.AddAsync(It.Is<BursaryApplication>(a =>
            a.LearnerId == learner.Id && a.BursaryId == bursary.Id), It.IsAny<CancellationToken>()), Times.Once);
        _appRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatus_OwnerScoped_UpdatesAndNotifies()
    {
        var learner = BuildLearner();
        var app = BursaryApplication.Create(learner.Id, Guid.NewGuid());
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        _appRepository.Setup(x => x.GetByIdAsync(app.Id, It.IsAny<CancellationToken>())).ReturnsAsync(app);
        var sut = new UpdateBursaryApplicationStatusHandler(_learnerRepository.Object, _appRepository.Object, _notificationService.Object);

        await sut.Handle(new UpdateBursaryApplicationStatusCommand(app.Id, "user-1", BursaryApplicationStatus.Submitted, null), CancellationToken.None);

        app.Status.Should().Be(BursaryApplicationStatus.Submitted);
        _notificationService.Verify(x => x.NotifyAsync(
            learner.Id, NotificationType.BursaryStatusChange, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateStatus_UnauthorisedLearner_Rejected()
    {
        var learner = BuildLearner();
        var app = BursaryApplication.Create(Guid.NewGuid(), Guid.NewGuid()); // owned by someone else
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        _appRepository.Setup(x => x.GetByIdAsync(app.Id, It.IsAny<CancellationToken>())).ReturnsAsync(app);
        var sut = new UpdateBursaryApplicationStatusHandler(_learnerRepository.Object, _appRepository.Object, _notificationService.Object);

        var act = () => sut.Handle(new UpdateBursaryApplicationStatusCommand(app.Id, "user-1", BursaryApplicationStatus.Submitted, null), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        _notificationService.Verify(x => x.NotifyAsync(
            It.IsAny<Guid>(), It.IsAny<NotificationType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Delete_OwnerScoped_SoftDeletes()
    {
        var learner = BuildLearner();
        var app = BursaryApplication.Create(learner.Id, Guid.NewGuid());
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        _appRepository.Setup(x => x.GetByIdAsync(app.Id, It.IsAny<CancellationToken>())).ReturnsAsync(app);
        var sut = new DeleteBursaryApplicationHandler(_learnerRepository.Object, _appRepository.Object);

        await sut.Handle(new DeleteBursaryApplicationCommand(app.Id, "user-1"), CancellationToken.None);

        app.IsDeleted.Should().BeTrue();
        _appRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_UnauthorisedLearner_Rejected()
    {
        var learner = BuildLearner();
        var app = BursaryApplication.Create(Guid.NewGuid(), Guid.NewGuid());
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        _appRepository.Setup(x => x.GetByIdAsync(app.Id, It.IsAny<CancellationToken>())).ReturnsAsync(app);
        var sut = new DeleteBursaryApplicationHandler(_learnerRepository.Object, _appRepository.Object);

        var act = () => sut.Handle(new DeleteBursaryApplicationCommand(app.Id, "user-1"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
