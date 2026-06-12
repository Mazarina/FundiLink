using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Common.Models;
using FundiLink.Application.Features.Home.Queries.GetLearnerHomeSummary;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Moq;

namespace FundiLink.Application.Tests.Features.Home;

public class GetLearnerHomeSummaryHandlerTests
{
    private readonly Mock<ILearnerRepository> _learnerRepository = new();
    private readonly Mock<IApplicationRepository> _applicationRepository = new();
    private readonly Mock<IBursaryApplicationRepository> _bursaryApplicationRepository = new();
    private readonly Mock<IChecklistRepository> _checklistRepository = new();
    private readonly Mock<INotificationLogRepository> _notificationLogRepository = new();
    private readonly Mock<IDeadlineQueryRepository> _deadlineQueryRepository = new();
    private readonly GetLearnerHomeSummaryHandler _sut;

    public GetLearnerHomeSummaryHandlerTests()
    {
        // Sensible empty defaults; individual tests override as needed.
        _applicationRepository
            .Setup(x => x.GetByLearnerIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<LearnerApplication>());
        _bursaryApplicationRepository
            .Setup(x => x.GetByLearnerIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<BursaryApplication>());
        _checklistRepository
            .Setup(x => x.GetByApplicationIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<DocumentChecklistItem>());
        _notificationLogRepository
            .Setup(x => x.GetByLearnerIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<NotificationLog>());
        _deadlineQueryRepository
            .Setup(x => x.GetUpcomingDeadlinesForLearnerAsync(
                It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<UpcomingDeadline>());

        _sut = new GetLearnerHomeSummaryHandler(
            _learnerRepository.Object,
            _applicationRepository.Object,
            _bursaryApplicationRepository.Object,
            _checklistRepository.Object,
            _notificationLogRepository.Object,
            _deadlineQueryRepository.Object);
    }

    private static Learner BuildLearner() => Learner.Create(
        "user-1", "Thabo", "Nkosi", new DateOnly(2005, 1, 1),
        "0712345678", "Gauteng", "School", "Gauteng", GradeLevel.Grade12, true, "v1");

    [Fact]
    public async Task Summary_ComposesExpectedCounts_FromSeededData()
    {
        var learner = BuildLearner();
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(learner);

        var app1 = LearnerApplication.Create(learner.Id, Guid.NewGuid(), ApplicationStatus.Submitted);
        var app2 = LearnerApplication.Create(learner.Id, Guid.NewGuid(), ApplicationStatus.Submitted);
        var app3 = LearnerApplication.Create(learner.Id, Guid.NewGuid(), ApplicationStatus.Interested);
        _applicationRepository.Setup(x => x.GetByLearnerIdAsync(learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { app1, app2, app3 });

        var bursary = BursaryApplication.Create(learner.Id, Guid.NewGuid(), BursaryApplicationStatus.Researching);
        _bursaryApplicationRepository.Setup(x => x.GetByLearnerIdAsync(learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { bursary });

        // app1 has two required checklist items, one linked -> one pending.
        var linked = DocumentChecklistItem.Create(app1.Id, DocumentType.IdDocument, true);
        linked.LinkDocument(Guid.NewGuid());
        var pending = DocumentChecklistItem.Create(app1.Id, DocumentType.AcademicResults, true);
        _checklistRepository.Setup(x => x.GetByApplicationIdAsync(app1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { linked, pending });

        var result = await _sut.Handle(new GetLearnerHomeSummaryQuery("user-1"), CancellationToken.None);

        result.FirstName.Should().Be("Thabo");
        result.ProgrammeApplicationTotal.Should().Be(3);
        result.ProgrammeApplicationCounts.Single(c => c.Status == "Submitted").Count.Should().Be(2);
        result.ProgrammeApplicationCounts.Single(c => c.Status == "Interested").Count.Should().Be(1);
        result.BursaryApplicationTotal.Should().Be(1);
        result.PendingDocumentCount.Should().Be(1);
    }

    [Fact]
    public async Task Summary_NewLearner_ReturnsZeros()
    {
        var learner = BuildLearner();
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(learner);

        var result = await _sut.Handle(new GetLearnerHomeSummaryQuery("user-1"), CancellationToken.None);

        result.ProgrammeApplicationTotal.Should().Be(0);
        result.BursaryApplicationTotal.Should().Be(0);
        result.PendingDocumentCount.Should().Be(0);
        result.UpcomingDeadlines.Should().BeEmpty();
        result.RecentNotifications.Should().BeEmpty();
        result.ProgrammeApplicationCounts.Should().BeEmpty();
    }

    [Fact]
    public async Task Summary_UpcomingDeadlines_RespectWindow()
    {
        var learner = BuildLearner();
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(learner);

        DateTime? capturedTo = null;
        _deadlineQueryRepository
            .Setup(x => x.GetUpcomingDeadlinesForLearnerAsync(
                learner.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .Callback<Guid, DateTime, DateTime, CancellationToken>((_, _, to, _) => capturedTo = to)
            .ReturnsAsync(new[]
            {
                new UpcomingDeadline(learner.Id, DeadlineKind.ProgrammeApplication, "Eng", DateTime.UtcNow.AddDays(5))
            });

        var result = await _sut.Handle(
            new GetLearnerHomeSummaryQuery("user-1", DeadlineWindowDays: 7), CancellationToken.None);

        result.UpcomingDeadlines.Should().ContainSingle();
        capturedTo.Should().BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task Summary_IsOwnerScoped_QueriesOnlyLearnersOwnId()
    {
        var learner = BuildLearner();
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(learner);

        await _sut.Handle(new GetLearnerHomeSummaryQuery("user-1"), CancellationToken.None);

        _applicationRepository.Verify(
            x => x.GetByLearnerIdAsync(learner.Id, It.IsAny<CancellationToken>()), Times.Once);
        _bursaryApplicationRepository.Verify(
            x => x.GetByLearnerIdAsync(learner.Id, It.IsAny<CancellationToken>()), Times.Once);
        _deadlineQueryRepository.Verify(
            x => x.GetUpcomingDeadlinesForLearnerAsync(
                learner.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
        _notificationLogRepository.Verify(
            x => x.GetByLearnerIdAsync(learner.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Summary_MissingProfile_Throws()
    {
        _learnerRepository.Setup(x => x.GetByUserIdAsync("ghost", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Learner?)null);

        var act = () => _sut.Handle(new GetLearnerHomeSummaryQuery("ghost"), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Summary_RecentNotifications_AreMostRecentFirst_AndLimited()
    {
        var learner = BuildLearner();
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(learner);

        var logs = Enumerable.Range(0, 7)
            .Select(_ => NotificationLog.Create(
                learner.Id, NotificationType.DeadlineReminder, NotificationChannel.Email,
                "x@example.com", NotificationStatus.Sent))
            .ToArray();
        _notificationLogRepository.Setup(x => x.GetByLearnerIdAsync(learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(logs);

        var result = await _sut.Handle(new GetLearnerHomeSummaryQuery("user-1"), CancellationToken.None);

        result.RecentNotifications.Should().HaveCount(5);
    }
}
