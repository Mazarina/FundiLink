using FluentAssertions;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using FundiLink.Infrastructure.Persistence;
using FundiLink.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FundiLink.Application.Tests.Features.Reporting;

/// <summary>
/// Integration-style tests for the deterministic, aggregate-first reporting repository using
/// the EF InMemory provider. Verifies dashboard counts and the POPIA operations summary are
/// computed correctly from seeded data. Reporting is read-only and exposes no raw PII.
/// </summary>
public class ReportingRepositoryTests
{
    private static FundiLinkDbContext NewDb() =>
        new(new DbContextOptionsBuilder<FundiLinkDbContext>()
            .UseInMemoryDatabase("Reporting_" + Guid.NewGuid())
            .Options);

    private static Learner BuildLearner(string province) => Learner.Create(
        Guid.NewGuid().ToString(), "Lebo", "Mokoena",
        DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-16)),
        "0712345678", province, "Example High", province, GradeLevel.Grade11, true, "v1");

    [Fact]
    public async Task GetOperationsDashboard_ComputesExpectedAggregateCounts()
    {
        await using var db = NewDb();

        db.Learners.Add(BuildLearner("Gauteng"));
        db.Learners.Add(BuildLearner("Gauteng"));
        db.Learners.Add(BuildLearner("Limpopo"));

        var learnerId = Guid.NewGuid();
        db.LearnerApplications.Add(LearnerApplication.Create(learnerId, Guid.NewGuid(), ApplicationStatus.Submitted));
        db.LearnerApplications.Add(LearnerApplication.Create(learnerId, Guid.NewGuid(), ApplicationStatus.Submitted));
        db.LearnerApplications.Add(LearnerApplication.Create(learnerId, Guid.NewGuid(), ApplicationStatus.Interested));

        db.BursaryApplications.Add(BursaryApplication.Create(learnerId, Guid.NewGuid(), BursaryApplicationStatus.Awarded));

        db.Documents.Add(Document.Create(learnerId, DocumentType.IdDocument, "id.pdf", "application/pdf", 100, "k1"));
        db.Documents.Add(Document.Create(learnerId, DocumentType.IdDocument, "id2.pdf", "application/pdf", 100, "k2"));
        var verified = Document.Create(learnerId, DocumentType.IdDocument, "id3.pdf", "application/pdf", 100, "k3");
        verified.Verify("admin-1");
        db.Documents.Add(verified);

        db.ErasureRequests.Add(ErasureRequest.Raise(learnerId, "user-1", "no longer needed"));

        db.GuardianConsents.Add(GuardianConsent.Grant(learnerId, ConsentType.DataProcessing, ConsentScope.ProfileBasic, "Mom", "0820000000"));
        db.GuardianConsents.Add(GuardianConsent.Grant(learnerId, ConsentType.DataProcessing, ConsentScope.ProfileBasic, "Dad", "0820000001"));
        db.GuardianConsents.Add(GuardianConsent.Revoke(learnerId, ConsentType.DataProcessing, ConsentScope.ProfileBasic, "Mom", "0820000000"));

        await db.SaveChangesAsync();

        var sut = new ReportingRepository(db);
        var result = await sut.GetOperationsDashboardAsync(CancellationToken.None);

        result.TotalLearners.Should().Be(3);
        result.LearnersByProvince.Should().ContainEquivalentOf(new { Category = "Gauteng", Count = 2 });
        result.LearnersByProvince.Should().ContainEquivalentOf(new { Category = "Limpopo", Count = 1 });
        result.ApplicationsByStatus.Single(s => s.Category == "Submitted").Count.Should().Be(2);
        result.ApplicationsByStatus.Single(s => s.Category == "Interested").Count.Should().Be(1);
        result.BursaryApplicationsByStatus.Single(s => s.Category == "Awarded").Count.Should().Be(1);
        result.DocumentsByStatus.Single(s => s.Category == "Pending").Count.Should().Be(2);
        result.DocumentsByStatus.Single(s => s.Category == "Verified").Count.Should().Be(1);
        result.PendingDocumentVerifications.Should().Be(2);
        result.PendingErasureRequests.Should().Be(1);
        result.ConsentGrants.Should().Be(2);
        result.ConsentRevocations.Should().Be(1);
    }

    [Fact]
    public async Task GetPopiaOperationsSummary_ReturnsPendingCounts()
    {
        await using var db = NewDb();
        var learnerId = Guid.NewGuid();

        db.Documents.Add(Document.Create(learnerId, DocumentType.IdDocument, "id.pdf", "application/pdf", 100, "k1"));
        db.ErasureRequests.Add(ErasureRequest.Raise(learnerId, "user-1", null));
        db.ErasureRequests.Add(ErasureRequest.Raise(Guid.NewGuid(), "user-2", null));
        await db.SaveChangesAsync();

        var sut = new ReportingRepository(db);
        var result = await sut.GetPopiaOperationsSummaryAsync(CancellationToken.None);

        result.PendingDocumentVerifications.Should().Be(1);
        result.PendingErasureRequests.Should().Be(2);
    }
}
