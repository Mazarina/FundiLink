using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Common.Models;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using FundiLink.Infrastructure.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FundiLink.Application.Tests.Features.Notifications;

public class DeadlineReminderServiceTests
{
    private readonly Mock<IDeadlineQueryRepository> _deadlineQuery = new();
    private readonly Mock<ILearnerRepository> _learnerRepository = new();
    private readonly Mock<IConsentCheckService> _consentCheck = new();
    private readonly Mock<INotificationLogRepository> _logRepository = new();
    private readonly Mock<INotificationService> _notificationService = new();
    private readonly DeterministicDeadlineReminderService _sut;

    private readonly DateTime _now = new(2026, 6, 11, 9, 0, 0, DateTimeKind.Utc);

    public DeadlineReminderServiceTests()
    {
        _sut = new DeterministicDeadlineReminderService(
            _deadlineQuery.Object,
            _learnerRepository.Object,
            _consentCheck.Object,
            _logRepository.Object,
            _notificationService.Object,
            NullLogger<DeterministicDeadlineReminderService>.Instance);
    }

    private static Learner AdultLearner()
        => Learner.Create(
            "user-1", "Lerato", "Dlamini",
            new DateOnly(1995, 1, 1), "+27123456789",
            "Gauteng", "Test School", "Gauteng",
            GradeLevel.Grade12, consentAccepted: true, consentVersion: "1.0");

    private static Learner MinorLearner()
        => Learner.Create(
            "user-2", "Sipho", "Khumalo",
            new DateOnly(2012, 1, 1), "+27123456780",
            "Gauteng", "Test School", "Gauteng",
            GradeLevel.Grade11, consentAccepted: true, consentVersion: "1.0");

    private void SetupDeadlines(params UpcomingDeadline[] deadlines)
        => _deadlineQuery
            .Setup(x => x.GetUpcomingDeadlinesAsync(
                It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(deadlines.ToList());

    [Fact]
    public async Task DueDeadline_ForAdultLearner_SendsOneReminder()
    {
        var learner = AdultLearner();
        _learnerRepository.Setup(x => x.GetByIdAsync(learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(learner);
        SetupDeadlines(new UpcomingDeadline(
            learner.Id, DeadlineKind.ProgrammeApplication, "BSc Computer Science", _now.Date.AddDays(3)));

        var result = await _sut.GenerateRemindersAsync(_now, windowDays: 14, CancellationToken.None);

        result.RemindersSent.Should().Be(1);
        result.RemindersSkippedAlreadySent.Should().Be(0);
        _notificationService.Verify(x => x.NotifyAsync(
            learner.Id, NotificationType.DeadlineReminder,
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task NoDeadlinesInWindow_SendsNothing()
    {
        SetupDeadlines(); // empty

        var result = await _sut.GenerateRemindersAsync(_now, windowDays: 14, CancellationToken.None);

        result.RemindersSent.Should().Be(0);
        result.LearnersWithUpcomingDeadlines.Should().Be(0);
        _notificationService.Verify(x => x.NotifyAsync(
            It.IsAny<Guid>(), It.IsAny<NotificationType>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AlreadySentToday_SkipsLearner_Idempotent()
    {
        var learner = AdultLearner();
        _learnerRepository.Setup(x => x.GetByIdAsync(learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(learner);
        SetupDeadlines(new UpcomingDeadline(
            learner.Id, DeadlineKind.BursaryApplication, "Funza Lushaka", _now.Date.AddDays(2)));
        _logRepository.Setup(x => x.HasLogForTypeOnDateAsync(
                learner.Id, NotificationType.DeadlineReminder, _now.Date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.GenerateRemindersAsync(_now, windowDays: 14, CancellationToken.None);

        result.RemindersSent.Should().Be(0);
        result.RemindersSkippedAlreadySent.Should().Be(1);
        _notificationService.Verify(x => x.NotifyAsync(
            It.IsAny<Guid>(), It.IsAny<NotificationType>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task MinorWithoutConsent_IsSuppressed()
    {
        var minor = MinorLearner();
        _learnerRepository.Setup(x => x.GetByIdAsync(minor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(minor);
        _consentCheck.Setup(x => x.HasCurrentConsentAsync(
                minor.Id, ConsentType.DataProcessing, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        SetupDeadlines(new UpcomingDeadline(
            minor.Id, DeadlineKind.ProgrammeApplication, "Diploma in Nursing", _now.Date.AddDays(1)));

        var result = await _sut.GenerateRemindersAsync(_now, windowDays: 14, CancellationToken.None);

        result.RemindersSent.Should().Be(0);
        result.RemindersSkippedAlreadySent.Should().Be(1);
        _notificationService.Verify(x => x.NotifyAsync(
            It.IsAny<Guid>(), It.IsAny<NotificationType>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task MinorWithConsent_ReceivesReminder()
    {
        var minor = MinorLearner();
        _learnerRepository.Setup(x => x.GetByIdAsync(minor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(minor);
        _consentCheck.Setup(x => x.HasCurrentConsentAsync(
                minor.Id, ConsentType.DataProcessing, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        SetupDeadlines(new UpcomingDeadline(
            minor.Id, DeadlineKind.ProgrammeApplication, "Diploma in Nursing", _now.Date.AddDays(1)));

        var result = await _sut.GenerateRemindersAsync(_now, windowDays: 14, CancellationToken.None);

        result.RemindersSent.Should().Be(1);
        _notificationService.Verify(x => x.NotifyAsync(
            minor.Id, NotificationType.DeadlineReminder,
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
