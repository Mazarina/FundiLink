using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Common.Models;
using FundiLink.Application.Features.Notifications.Commands.TriggerDeadlineReminders;
using FundiLink.Domain.Entities;
using Moq;

namespace FundiLink.Application.Tests.Features.Notifications;

public class TriggerDeadlineRemindersHandlerTests
{
    private readonly Mock<IDeadlineReminderService> _reminderService = new();
    private readonly Mock<IAuditLogRepository> _auditLogRepository = new();
    private readonly TriggerDeadlineRemindersHandler _sut;
    private readonly List<AuditLogEntry> _captured = new();

    public TriggerDeadlineRemindersHandlerTests()
    {
        _reminderService.Setup(x => x.GenerateRemindersAsync(
                It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReminderRunResult(3, 2, 1));
        _auditLogRepository.Setup(x => x.AddAsync(It.IsAny<AuditLogEntry>(), It.IsAny<CancellationToken>()))
            .Callback<AuditLogEntry, CancellationToken>((e, _) => _captured.Add(e))
            .Returns(Task.CompletedTask);

        _sut = new TriggerDeadlineRemindersHandler(_reminderService.Object, _auditLogRepository.Object);
    }

    [Fact]
    public async Task Run_WritesAppendOnlyAuditEntry_AndReturnsResult()
    {
        var result = await _sut.Handle(
            new TriggerDeadlineRemindersCommand("admin-1", "Admin", 14), CancellationToken.None);

        result.RemindersSent.Should().Be(2);
        result.RemindersSkippedAlreadySent.Should().Be(1);

        _captured.Should().ContainSingle();
        _captured[0].Action.Should().Be("TriggerDeadlineReminders");
        _captured[0].ActorUserId.Should().Be("admin-1");
        _captured[0].ActorRole.Should().Be("Admin");
        _auditLogRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OutOfRangeWindow_ClampsToDefault()
    {
        await _sut.Handle(
            new TriggerDeadlineRemindersCommand("admin-1", "Admin", 9999), CancellationToken.None);

        _reminderService.Verify(x => x.GenerateRemindersAsync(
            It.IsAny<DateTime>(), 14, It.IsAny<CancellationToken>()), Times.Once);
    }
}
