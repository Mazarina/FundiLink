using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.Consent.Commands.RecordConsent;
using FundiLink.Application.Features.Consent.Commands.RevokeConsent;
using FundiLink.Application.Features.Consent.Queries.GetGuardianView;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Moq;

namespace FundiLink.Application.Tests.Features.Consent;

public class ConsentHandlerTests
{
    private readonly Mock<ILearnerRepository> _learnerRepository = new();
    private readonly Mock<IGuardianConsentRepository> _consentRepository = new();
    private readonly Mock<IGuardianLinkRepository> _linkRepository = new();
    private readonly Mock<IConsentCheckService> _consentCheck = new();
    private readonly Mock<IApplicationRepository> _applicationRepository = new();
    private readonly Mock<IBursaryApplicationRepository> _bursaryApplicationRepository = new();
    private readonly Mock<IAuditLogRepository> _auditLog = new();

    private static Learner BuildMinor() => Learner.Create(
        "user-1", "Lebo", "Mokoena", DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-15)),
        "0712345678", "Gauteng", "School", "Gauteng", GradeLevel.Grade11, true, "v1");

    private static Learner BuildAdult() => Learner.Create(
        "user-1", "Adult", "Person", DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25)),
        "0712345678", "Gauteng", "School", "Gauteng", GradeLevel.PostMatric, true, "v1");

    [Fact]
    public async Task RecordConsent_ForMinor_AppendsRecordAndWritesAudit()
    {
        var learner = BuildMinor();
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        var sut = new RecordConsentHandler(_learnerRepository.Object, _consentRepository.Object, _auditLog.Object);

        var id = await sut.Handle(
            new RecordConsentCommand("user-1", ConsentType.GuardianCoAccess, ConsentScope.ProfileBasic, "Mom", "0820000000"),
            CancellationToken.None);

        id.Should().NotBeEmpty();
        _consentRepository.Verify(x => x.AddAsync(
            It.Is<GuardianConsent>(c => c.Status == ConsentStatus.Granted && c.ConsentType == ConsentType.GuardianCoAccess),
            It.IsAny<CancellationToken>()), Times.Once);
        _auditLog.Verify(x => x.AddAsync(
            It.Is<AuditLogEntry>(a => a.Action == "RecordGuardianConsent"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RecordConsent_ForAdult_Rejected()
    {
        var learner = BuildAdult();
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        var sut = new RecordConsentHandler(_learnerRepository.Object, _consentRepository.Object, _auditLog.Object);

        var act = () => sut.Handle(
            new RecordConsentCommand("user-1", ConsentType.GuardianCoAccess, ConsentScope.ProfileBasic, "Mom", "0820000000"),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task RevokeConsent_AppendsRevocationRecord()
    {
        var learner = BuildMinor();
        var existing = GuardianConsent.Grant(learner.Id, ConsentType.GuardianCoAccess, ConsentScope.ProfileBasic, "Mom", "0820000000");
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        _consentRepository.Setup(x => x.GetLatestAsync(learner.Id, ConsentType.GuardianCoAccess, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        var sut = new RevokeConsentHandler(_learnerRepository.Object, _consentRepository.Object, _auditLog.Object);

        await sut.Handle(new RevokeConsentCommand("user-1", ConsentType.GuardianCoAccess), CancellationToken.None);

        _consentRepository.Verify(x => x.AddAsync(
            It.Is<GuardianConsent>(c => c.Status == ConsentStatus.Revoked), It.IsAny<CancellationToken>()), Times.Once);
        _auditLog.Verify(x => x.AddAsync(
            It.Is<AuditLogEntry>(a => a.Action == "RevokeGuardianConsent"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RevokeConsent_WhenNoActiveConsent_Rejected()
    {
        var learner = BuildMinor();
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        _consentRepository.Setup(x => x.GetLatestAsync(learner.Id, ConsentType.GuardianCoAccess, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GuardianConsent?)null);
        var sut = new RevokeConsentHandler(_learnerRepository.Object, _consentRepository.Object, _auditLog.Object);

        var act = () => sut.Handle(new RevokeConsentCommand("user-1", ConsentType.GuardianCoAccess), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    private GetGuardianViewHandler BuildGuardianViewHandler() => new(
        _linkRepository.Object, _consentRepository.Object, _consentCheck.Object,
        _learnerRepository.Object, _applicationRepository.Object,
        _bursaryApplicationRepository.Object, _auditLog.Object);

    [Fact]
    public async Task GuardianView_WithoutLink_Rejected()
    {
        var learner = BuildMinor();
        _linkRepository.Setup(x => x.GetByGuardianAndLearnerAsync("guardian-1", learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GuardianLink?)null);
        var sut = BuildGuardianViewHandler();

        var act = () => sut.Handle(new GetGuardianViewQuery("guardian-1", learner.Id), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task GuardianView_LinkedButNoConsent_Rejected()
    {
        var learner = BuildMinor();
        var link = GuardianLink.Create(learner.Id, "guardian-1", "Mom", "0820000000");
        _linkRepository.Setup(x => x.GetByGuardianAndLearnerAsync("guardian-1", learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(link);
        _consentCheck.Setup(x => x.HasCurrentConsentAsync(learner.Id, ConsentType.GuardianCoAccess, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var sut = BuildGuardianViewHandler();

        var act = () => sut.Handle(new GetGuardianViewQuery("guardian-1", learner.Id), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task GuardianView_WithConsent_ReturnsMinimisedScopedView_AndWritesAudit()
    {
        var learner = BuildMinor();
        var link = GuardianLink.Create(learner.Id, "guardian-1", "Mom", "0820000000");
        // Basic scope — applications must NOT be included (data minimisation).
        var consent = GuardianConsent.Grant(learner.Id, ConsentType.GuardianCoAccess, ConsentScope.ProfileBasic, "Mom", "0820000000");

        _linkRepository.Setup(x => x.GetByGuardianAndLearnerAsync("guardian-1", learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(link);
        _consentCheck.Setup(x => x.HasCurrentConsentAsync(learner.Id, ConsentType.GuardianCoAccess, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _consentRepository.Setup(x => x.GetLatestAsync(learner.Id, ConsentType.GuardianCoAccess, It.IsAny<CancellationToken>()))
            .ReturnsAsync(consent);
        _learnerRepository.Setup(x => x.GetByIdAsync(learner.Id, It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        var sut = BuildGuardianViewHandler();

        var result = await sut.Handle(new GetGuardianViewQuery("guardian-1", learner.Id), CancellationToken.None);

        result.FirstName.Should().Be("Lebo");
        result.Scope.Should().Be(ConsentScope.ProfileBasic);
        result.Applications.Should().BeEmpty();
        _applicationRepository.Verify(x => x.GetByLearnerIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _auditLog.Verify(x => x.AddAsync(
            It.Is<AuditLogEntry>(a => a.Action == "GuardianViewLearner"), It.IsAny<CancellationToken>()), Times.Once);
    }
}
