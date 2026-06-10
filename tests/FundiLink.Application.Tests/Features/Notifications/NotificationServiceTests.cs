using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using FundiLink.Infrastructure.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace FundiLink.Application.Tests.Features.Notifications;

public class NotificationServiceTests
{
    private readonly Mock<ILearnerRepository> _learnerRepository = new();
    private readonly Mock<INotificationPreferenceRepository> _preferenceRepository = new();
    private readonly Mock<INotificationLogRepository> _logRepository = new();
    private readonly Mock<IEmailService> _emailService = new();
    private readonly Mock<IWhatsAppService> _whatsAppService = new();
    private readonly Mock<ISmsService> _smsService = new();
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly List<NotificationLog> _captured = new();
    private readonly NotificationService _sut;
    private readonly Learner _learner;

    public NotificationServiceTests()
    {
        _learner = Learner.Create(
            "user-1", "Thabo", "Mokoena",
            new DateOnly(2005, 1, 1), "+27123456789",
            "Gauteng", "Test School", "Gauteng",
            GradeLevel.Grade12, consentAccepted: true, consentVersion: "1.0");

        _learnerRepository.Setup(x => x.GetByIdAsync(_learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_learner);
        _identityService.Setup(x => x.GetUserByIdAsync("user-1"))
            .ReturnsAsync(("user-1", "thabo@example.com"));
        _emailService.Setup(x => x.SendNotificationEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _whatsAppService.Setup(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _smsService.Setup(x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _logRepository.Setup(x => x.AddAsync(It.IsAny<NotificationLog>(), It.IsAny<CancellationToken>()))
            .Callback<NotificationLog, CancellationToken>((log, _) => _captured.Add(log))
            .Returns(Task.CompletedTask);

        _sut = new NotificationService(
            _learnerRepository.Object,
            _preferenceRepository.Object,
            _logRepository.Object,
            _emailService.Object,
            _whatsAppService.Object,
            _smsService.Object,
            _identityService.Object,
            NullLogger<NotificationService>.Instance);
    }

    private void SetPreference(bool email, bool whatsApp, bool sms)
    {
        var pref = NotificationPreference.CreateDefault(_learner.Id);
        pref.UpdatePreferences(email, whatsApp, sms);
        _preferenceRepository.Setup(x => x.GetByLearnerIdAsync(_learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pref);
    }

    private Task Notify() => _sut.NotifyAsync(
        _learner.Id, NotificationType.RegistrationWelcome, "Subject", "Body", CancellationToken.None);

    [Fact]
    public async Task NoPreferenceRecord_OnlyEmailAttempted()
    {
        // No preference setup => null => defaults (Email on, others off)
        await Notify();

        _captured.Should().ContainSingle();
        _captured[0].Channel.Should().Be(NotificationChannel.Email);
        _whatsAppService.Verify(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _smsService.Verify(x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task EmailAndWhatsApp_BothAttempted_TwoLogs()
    {
        SetPreference(email: true, whatsApp: true, sms: false);

        await Notify();

        _captured.Should().HaveCount(2);
        _captured.Select(l => l.Channel).Should().Contain(new[] { NotificationChannel.Email, NotificationChannel.WhatsApp });
    }

    [Fact]
    public async Task AllChannelsEnabled_ThreeLogs()
    {
        SetPreference(email: true, whatsApp: true, sms: true);

        await Notify();

        _captured.Should().HaveCount(3);
        _captured.Select(l => l.Channel).Should().Contain(new[]
        {
            NotificationChannel.Email, NotificationChannel.WhatsApp, NotificationChannel.Sms
        });
    }

    [Fact]
    public async Task ChannelReturnsFalse_LogsFailed()
    {
        SetPreference(email: false, whatsApp: true, sms: false);
        _whatsAppService.Setup(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await Notify();

        _captured.Should().ContainSingle();
        _captured[0].Channel.Should().Be(NotificationChannel.WhatsApp);
        _captured[0].Status.Should().Be(NotificationStatus.Failed);
    }

    [Fact]
    public async Task DisabledChannel_NotAttempted_NoLog()
    {
        SetPreference(email: true, whatsApp: false, sms: false);

        await Notify();

        _captured.Should().ContainSingle();
        _captured.Should().NotContain(l => l.Channel == NotificationChannel.Sms);
        _smsService.Verify(x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SuccessfulSend_LogsSentWithCorrectChannelAndType()
    {
        SetPreference(email: true, whatsApp: false, sms: false);

        await Notify();

        var log = _captured.Single();
        log.Status.Should().Be(NotificationStatus.Sent);
        log.Channel.Should().Be(NotificationChannel.Email);
        log.NotificationType.Should().Be(NotificationType.RegistrationWelcome);
        log.Recipient.Should().Be("thabo@example.com");
        _logRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
