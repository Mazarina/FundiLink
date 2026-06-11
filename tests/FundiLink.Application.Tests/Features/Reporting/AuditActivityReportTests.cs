using FluentAssertions;
using FundiLink.Domain.Entities;
using FundiLink.Infrastructure.Persistence;
using FundiLink.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Application.Tests.Features.Reporting;

/// <summary>
/// Verifies the filtered audit activity view over the append-only audit log (by action and
/// actor role). Read-only — exposes only the existing audit fields, no new PII.
/// </summary>
public class AuditActivityReportTests
{
    private static FundiLinkDbContext NewDb() =>
        new(new DbContextOptionsBuilder<FundiLinkDbContext>()
            .UseInMemoryDatabase("AuditReport_" + Guid.NewGuid())
            .Options);

    [Fact]
    public async Task GetFiltered_FiltersByActionAndActorRole()
    {
        await using var db = NewDb();
        db.AuditLogEntries.Add(AuditLogEntry.Create("u1", "Admin", "VerifyDocument", "Document", "d1"));
        db.AuditLogEntries.Add(AuditLogEntry.Create("u2", "SupportAgent", "SearchLearners", "Learner", "*"));
        db.AuditLogEntries.Add(AuditLogEntry.Create("u3", "Admin", "SearchLearners", "Learner", "*"));
        await db.SaveChangesAsync();

        var sut = new AuditLogRepository(db);

        var (byAction, totalByAction) = await sut.GetFilteredAsync(
            "SearchLearners", null, null, null, 1, 50, CancellationToken.None);
        totalByAction.Should().Be(2);
        byAction.Should().OnlyContain(e => e.Action == "SearchLearners");

        var (byRole, totalByRole) = await sut.GetFilteredAsync(
            null, "Admin", null, null, 1, 50, CancellationToken.None);
        totalByRole.Should().Be(2);
        byRole.Should().OnlyContain(e => e.ActorRole == "Admin");
    }

    [Fact]
    public async Task GetFiltered_FiltersByDateRange()
    {
        await using var db = NewDb();
        db.AuditLogEntries.Add(AuditLogEntry.Create("u1", "Admin", "Recent", "X", "1"));
        await db.SaveChangesAsync();

        var sut = new AuditLogRepository(db);

        var cutoff = DateTime.UtcNow.AddMinutes(-1);
        var (recent, total) = await sut.GetFilteredAsync(
            null, null, cutoff, null, 1, 50, CancellationToken.None);
        total.Should().Be(1);
        recent.Should().HaveCount(1);

        var future = DateTime.UtcNow.AddMinutes(1);
        var (none, noneTotal) = await sut.GetFilteredAsync(
            null, null, future, null, 1, 50, CancellationToken.None);
        noneTotal.Should().Be(0);
        none.Should().BeEmpty();
    }
}
