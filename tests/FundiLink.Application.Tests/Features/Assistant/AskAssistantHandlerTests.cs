using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.Assistant.Queries.AskAssistant;
using FundiLink.Application.Features.ProgrammeMatching.Services;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Moq;

namespace FundiLink.Application.Tests.Features.Assistant;

public class AskAssistantHandlerTests
{
    private readonly Mock<ILearnerRepository> _learnerRepository = new();
    private readonly Mock<IProgrammeRepository> _programmeRepository = new();
    private readonly Mock<IBursaryRepository> _bursaryRepository = new();
    private readonly Mock<IDocumentRepository> _documentRepository = new();
    private readonly Mock<IAiAssistantService> _assistantService = new();
    private readonly Mock<IAssistantInteractionLogRepository> _logRepository = new();
    private readonly AskAssistantHandler _sut;

    public AskAssistantHandlerTests()
    {
        _programmeRepository.Setup(x => x.GetAllWithInstitutionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _bursaryRepository.Setup(x => x.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _documentRepository.Setup(x => x.GetByLearnerIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _assistantService.Setup(x => x.AnswerAsync(It.IsAny<AssistantRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AssistantResult("grounded answer", ["Your FundiLink academic profile"]));

        _sut = new AskAssistantHandler(
            _learnerRepository.Object,
            _programmeRepository.Object,
            _bursaryRepository.Object,
            _documentRepository.Object,
            _assistantService.Object,
            _logRepository.Object,
            new ProgrammeMatchingService());
    }

    private static Learner BuildLearner() => Learner.Create(
        "user-1", "Thabo", "Nkosi", new DateOnly(2005, 1, 1),
        "0712345678", "Gauteng", "School", "Gauteng", GradeLevel.Grade12, true, "v1");

    private void SetupLearner(Learner learner, int? aps)
    {
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);

        Domain.Entities.AcademicProfile? profile = null;
        if (aps is not null)
        {
            profile = Domain.Entities.AcademicProfile.Create(learner.Id, 2025, ResultType.Grade12Final);
            profile.SetApsScore(aps.Value);
        }
        _learnerRepository.Setup(x => x.GetAcademicProfileByLearnerIdAsync(learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);
    }

    [Fact]
    public async Task Handle_ReturnsGroundedGuidanceOnlyResponse()
    {
        var learner = BuildLearner();
        SetupLearner(learner, aps: 38);

        var result = await _sut.Handle(
            new AskAssistantQuery("user-1", AssistantIntent.WhatIsMyAps), CancellationToken.None);

        result.GuidanceOnly.Should().BeTrue();
        result.Answer.Should().Be("grounded answer");
        result.Sources.Should().NotBeEmpty();
        result.Disclaimer.Should().NotBeNullOrWhiteSpace();
        result.Intent.Should().Be(AssistantIntent.WhatIsMyAps);
    }

    [Fact]
    public async Task Handle_PassesLearnerOwnAps_ToService()
    {
        var learner = BuildLearner();
        SetupLearner(learner, aps: 42);

        await _sut.Handle(new AskAssistantQuery("user-1", AssistantIntent.WhatIsMyAps), CancellationToken.None);

        _assistantService.Verify(x => x.AnswerAsync(
            It.Is<AssistantRequest>(r => r.ApsScore == 42 && r.FirstName == "Thabo"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NoAcademicProfile_PassesNullAps()
    {
        var learner = BuildLearner();
        SetupLearner(learner, aps: null);

        await _sut.Handle(new AskAssistantQuery("user-1", AssistantIntent.WhatDoIQualifyFor), CancellationToken.None);

        _assistantService.Verify(x => x.AnswerAsync(
            It.Is<AssistantRequest>(r => r.ApsScore == null),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_LearnerNotFound_ThrowsKeyNotFound()
    {
        _learnerRepository.Setup(x => x.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Learner?)null);

        var act = () => _sut.Handle(
            new AskAssistantQuery("user-1", AssistantIntent.WhatIsMyAps), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        _assistantService.Verify(x => x.AnswerAsync(It.IsAny<AssistantRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_LogsInteraction_PopiaMinimal()
    {
        var learner = BuildLearner();
        SetupLearner(learner, aps: 30);

        await _sut.Handle(new AskAssistantQuery("user-1", AssistantIntent.WhichBursariesFitMe), CancellationToken.None);

        _logRepository.Verify(x => x.AddAsync(
            It.Is<AssistantInteractionLog>(l =>
                l.LearnerId == learner.Id && l.Intent == AssistantIntent.WhichBursariesFitMe),
            It.IsAny<CancellationToken>()), Times.Once);
        _logRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
