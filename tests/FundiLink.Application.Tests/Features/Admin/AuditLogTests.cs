using FluentAssertions;
using FundiLink.Domain.Entities;

namespace FundiLink.Application.Tests.Features.Admin;

public class AuditLogTests
{
    [Fact]
    public void Create_SetsOccurredAt()
    {
        var entry = AuditLogEntry.Create("actor", "Admin", "TestAction", "Target", "id-1");
        entry.OccurredAt.Should().NotBe(default);
        entry.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_SetsAllProperties()
    {
        var entry = AuditLogEntry.Create("actor", "Admin", "TestAction", "Target", "id-1");
        entry.Id.Should().NotBeEmpty();
        entry.ActorUserId.Should().Be("actor");
        entry.ActorRole.Should().Be("Admin");
        entry.Action.Should().Be("TestAction");
        entry.TargetType.Should().Be("Target");
        entry.TargetId.Should().Be("id-1");
    }
}
