using FluentAssertions;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using FundiLink.Infrastructure.Persistence;
using FundiLink.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Application.Tests.Features.DataRights;

/// <summary>
/// Integration-style tests for the deterministic erasure service using the EF InMemory
/// provider. Verifies that fulfilment anonymises the learner and removes personal data while
/// preserving append-only audit and consent records (POPIA proof-of-processing retention).
/// </summary>
public class ErasureServicePreservationTests
{
    private static FundiLinkDbContext NewDb() =>
        new(new DbContextOptionsBuilder<FundiLinkDbContext>()
            .UseInMemoryDatabase("Erasure_" + Guid.NewGuid())
            .Options);

    private static Learner BuildLearner() => Learner.Create(
        "user-1", "Lebo", "Mokoena", DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-16)),
        "0712345678", "Gauteng", "Example High", "Gauteng", GradeLevel.Grade11, true, "v1");

    [Fact]
    public async Task AnonymiseLearnerData_AnonymisesProfile_RemovesAcademic_PreservesAuditAndConsent()
    {
        await using var db = NewDb();
        var learner = BuildLearner();
        db.Learners.Add(learner);
        db.AcademicProfiles.Add(Domain.Entities.AcademicProfile.Create(learner.Id, 2025, ResultType.Grade12Final));
        db.GuardianConsents.Add(GuardianConsent.Grant(
            learner.Id, ConsentType.DataProcessing, ConsentScope.ProfileBasic, "Mom", "0820000000"));
        db.AuditLogEntries.Add(AuditLogEntry.Create("user-1", "Learner", "RequestErasure", "ErasureRequest", Guid.NewGuid().ToString()));
        await db.SaveChangesAsync();

        var sut = new DeterministicErasureService(db);
        var outcome = await sut.AnonymiseLearnerDataAsync(learner.Id, CancellationToken.None);

        outcome.ProfileAnonymised.Should().BeTrue();
        outcome.AcademicProfileRemoved.Should().BeTrue();

        var stored = await db.Learners.IgnoreQueryFilters().SingleAsync(l => l.Id == learner.Id);
        stored.FirstName.Should().Be("Redacted");
        stored.IsDeleted.Should().BeTrue();

        // Personal data removed.
        (await db.AcademicProfiles.CountAsync()).Should().Be(0);

        // Append-only legal records preserved.
        (await db.GuardianConsents.CountAsync(c => c.LearnerId == learner.Id)).Should().Be(1);
        (await db.AuditLogEntries.CountAsync()).Should().Be(1);
    }
}
