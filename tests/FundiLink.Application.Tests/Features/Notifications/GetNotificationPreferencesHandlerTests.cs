using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.Notifications.Queries.GetNotificationPreferences;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Moq;

namespace FundiLink.Application.Tests.Features.Notifications;

public class GetNotificationPreferencesHandlerTests
{
    private readonly Mock<ILearnerRepository> _learnerRepository = new();
    private readonly Mock<INotificationPreferenceRepository> _preferenceRepository = new();
    private readonly GetNotificationPreferencesHandler _sut;
    private readonly Learner _learner;

    public GetNotificationPreferencesHandlerTests()
    {
        _learner = Learner.Create(
            "user-1", "Thabo", "Mokoena", new DateOnly(2005, 1, 1), "+27123456789",
            "Gauteng", "Test School", "Gauteng", GradeLevel.Grade12, true, "1.0");
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(_learner);
        _sut = new GetNotificationPreferencesHandler(_learnerRepository.Object, _preferenceRepository.Object);
    }

    [Fact]
    public async Task NoRecord_ReturnsDefaults_WithoutCreating()
    {
        _preferenceRepository.Setup(x => x.GetByLearnerIdAsync(_learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationPreference?)null);

        var result = await _sut.Handle(new GetNotificationPreferencesQuery("user-1"), CancellationToken.None);

        result.EmailEnabled.Should().BeTrue();
        result.WhatsAppEnabled.Should().BeFalse();
        result.SmsEnabled.Should().BeFalse();
        _preferenceRepository.Verify(x => x.AddAsync(It.IsAny<NotificationPreference>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExistingRecord_ReturnsStoredValues()
    {
        var pref = NotificationPreference.CreateDefault(_learner.Id);
        pref.UpdatePreferences(false, true, true);
        _preferenceRepository.Setup(x => x.GetByLearnerIdAsync(_learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pref);

        var result = await _sut.Handle(new GetNotificationPreferencesQuery("user-1"), CancellationToken.None);

        result.EmailEnabled.Should().BeFalse();
        result.WhatsAppEnabled.Should().BeTrue();
        result.SmsEnabled.Should().BeTrue();
    }
}
