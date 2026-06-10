using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.Notifications.Commands.UpdateNotificationPreferences;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Moq;

namespace FundiLink.Application.Tests.Features.Notifications;

public class UpdateNotificationPreferencesHandlerTests
{
    private readonly Mock<ILearnerRepository> _learnerRepository = new();
    private readonly Mock<INotificationPreferenceRepository> _preferenceRepository = new();
    private readonly UpdateNotificationPreferencesHandler _sut;
    private readonly Learner _learner;

    public UpdateNotificationPreferencesHandlerTests()
    {
        _learner = Learner.Create(
            "user-1", "Thabo", "Mokoena", new DateOnly(2005, 1, 1), "+27123456789",
            "Gauteng", "Test School", "Gauteng", GradeLevel.Grade12, true, "1.0");
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(_learner);
        _sut = new UpdateNotificationPreferencesHandler(_learnerRepository.Object, _preferenceRepository.Object);
    }

    [Fact]
    public async Task NoExistingPreference_CreatesNewRecord()
    {
        _preferenceRepository.Setup(x => x.GetByLearnerIdAsync(_learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationPreference?)null);

        await _sut.Handle(new UpdateNotificationPreferencesCommand("user-1", true, true, false), CancellationToken.None);

        _preferenceRepository.Verify(x => x.AddAsync(
            It.Is<NotificationPreference>(p => p.LearnerId == _learner.Id && p.WhatsAppEnabled),
            It.IsAny<CancellationToken>()), Times.Once);
        _preferenceRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExistingPreference_Updated_NotAdded()
    {
        var pref = NotificationPreference.CreateDefault(_learner.Id);
        _preferenceRepository.Setup(x => x.GetByLearnerIdAsync(_learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pref);

        await _sut.Handle(new UpdateNotificationPreferencesCommand("user-1", false, true, true), CancellationToken.None);

        pref.EmailEnabled.Should().BeFalse();
        pref.WhatsAppEnabled.Should().BeTrue();
        pref.SmsEnabled.Should().BeTrue();
        _preferenceRepository.Verify(x => x.AddAsync(It.IsAny<NotificationPreference>(), It.IsAny<CancellationToken>()), Times.Never);
        _preferenceRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
