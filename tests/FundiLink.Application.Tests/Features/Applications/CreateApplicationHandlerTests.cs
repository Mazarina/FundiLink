using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.Applications.Commands.CreateApplication;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Moq;

namespace FundiLink.Application.Tests.Features.Applications;

public class CreateApplicationHandlerTests
{
    private readonly Mock<ILearnerRepository> _learnerRepository = new();
    private readonly Mock<IProgrammeRepository> _programmeRepository = new();
    private readonly Mock<IApplicationRepository> _applicationRepository = new();
    private readonly CreateApplicationHandler _sut;

    public CreateApplicationHandlerTests()
    {
        _sut = new CreateApplicationHandler(
            _learnerRepository.Object,
            _programmeRepository.Object,
            _applicationRepository.Object);
    }

    private static Learner BuildLearner() => Learner.Create(
        "user-1", "Thabo", "Nkosi", new DateOnly(2005, 1, 1),
        "0712345678", "Gauteng", "School", "Gauteng", GradeLevel.Grade12, true, "v1");

    private static Programme BuildProgramme() =>
        Programme.Create(Guid.NewGuid(), "BSc Computer Science", 42);

    [Fact]
    public async Task Handle_ValidCommand_CreatesApplicationAndReturnsId()
    {
        var learner = BuildLearner();
        var programme = BuildProgramme();
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        _programmeRepository.Setup(x => x.GetByIdAsync(programme.Id, It.IsAny<CancellationToken>())).ReturnsAsync(programme);

        var result = await _sut.Handle(
            new CreateApplicationCommand(programme.Id, "user-1", ApplicationStatus.Interested), CancellationToken.None);

        result.Should().NotBeEmpty();
        _applicationRepository.Verify(x => x.AddAsync(It.Is<LearnerApplication>(a =>
            a.LearnerId == learner.Id && a.ProgrammeId == programme.Id), It.IsAny<CancellationToken>()), Times.Once);
        _applicationRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_LearnerNotFound_ThrowsKeyNotFound()
    {
        _learnerRepository.Setup(x => x.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Learner?)null);

        var act = () => _sut.Handle(new CreateApplicationCommand(Guid.NewGuid(), "user-1"), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("*Learner*");
    }

    [Fact]
    public async Task Handle_ProgrammeNotFound_ThrowsKeyNotFound()
    {
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(BuildLearner());
        _programmeRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Programme?)null);

        var act = () => _sut.Handle(new CreateApplicationCommand(Guid.NewGuid(), "user-1"), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("*Programme*");
    }

    [Fact]
    public async Task Handle_SubmittedStatus_SetsSubmittedAt()
    {
        var learner = BuildLearner();
        var programme = BuildProgramme();
        LearnerApplication? captured = null;
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        _programmeRepository.Setup(x => x.GetByIdAsync(programme.Id, It.IsAny<CancellationToken>())).ReturnsAsync(programme);
        _applicationRepository.Setup(x => x.AddAsync(It.IsAny<LearnerApplication>(), It.IsAny<CancellationToken>()))
            .Callback<LearnerApplication, CancellationToken>((a, _) => captured = a)
            .Returns(Task.CompletedTask);

        await _sut.Handle(
            new CreateApplicationCommand(programme.Id, "user-1", ApplicationStatus.Submitted), CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.SubmittedAt.Should().NotBeNull();
    }
}
