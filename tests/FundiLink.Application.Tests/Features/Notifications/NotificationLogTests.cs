using FluentAssertions;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;

namespace FundiLink.Application.Tests.Features.Notifications;

public class NotificationLogTests
{
    [Fact]
    public void Create_SetsAllPropertiesAndSentAt()
    {
        var learnerId = Guid.NewGuid();
        var before = DateTime.UtcNow;

        var log = NotificationLog.Create(
            learnerId,
            NotificationType.RegistrationWelcome,
            NotificationChannel.Email,
            "learner@example.com",
            NotificationStatus.Sent);

        log.LearnerId.Should().Be(learnerId);
        log.NotificationType.Should().Be(NotificationType.RegistrationWelcome);
        log.Channel.Should().Be(NotificationChannel.Email);
        log.Recipient.Should().Be("learner@example.com");
        log.Status.Should().Be(NotificationStatus.Sent);
        log.ErrorMessage.Should().BeNull();
        log.SentAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(DateTime.UtcNow);
    }

    [Fact]
    public void Create_WithErrorMessage_SetsFailedDetails()
    {
        var log = NotificationLog.Create(
            Guid.NewGuid(),
            NotificationType.DeadlineReminder,
            NotificationChannel.Sms,
            "+27123456789",
            NotificationStatus.Failed,
            "Provider unavailable");

        log.Status.Should().Be(NotificationStatus.Failed);
        log.ErrorMessage.Should().Be("Provider unavailable");
    }
}
