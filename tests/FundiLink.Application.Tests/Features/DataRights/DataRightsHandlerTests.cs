using FluentAssertions;
using FundiLink.Application.Common.Interfaces;
using FundiLink.Application.Features.DataRights.Commands.FulfilErasureRequest;
using FundiLink.Application.Features.DataRights.Commands.RequestErasure;
using FundiLink.Application.Features.DataRights.Queries.ExportMyData;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using Moq;

namespace FundiLink.Application.Tests.Features.DataRights;

public class DataRightsHandlerTests
{
    private readonly Mock<ILearnerRepository> _learnerRepository = new();
    private readonly Mock<IApplicationRepository> _applicationRepository = new();
    private readonly Mock<IBursaryApplicationRepository> _bursaryApplicationRepository = new();
    private readonly Mock<IDocumentRepository> _documentRepository = new();
    private readonly Mock<IAccommodationInterestRepository> _accommodationInterestRepository = new();
    private readonly Mock<ICareerInterestRepository> _careerInterestRepository = new();
    private readonly Mock<IGuardianConsentRepository> _consentRepository = new();
    private readonly Mock<IErasureRequestRepository> _erasureRequestRepository = new();
    private readonly Mock<IErasureService> _erasureService = new();
    private readonly Mock<IAuditLogRepository> _auditLog = new();

    private static Learner BuildLearner() => Learner.Create(
        "user-1", "Lebo", "Mokoena", DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20)),
        "0712345678", "Gauteng", "Example High", "Gauteng", GradeLevel.Grade12, true, "v1");

    private ExportMyDataHandler BuildExportHandler() => new(
        _learnerRepository.Object, _applicationRepository.Object, _bursaryApplicationRepository.Object,
        _documentRepository.Object, _accommodationInterestRepository.Object, _careerInterestRepository.Object,
        _consentRepository.Object, _auditLog.Object);

    [Fact]
    public async Task ExportMyData_ReturnsOwnerScopedData_AndWritesAudit()
    {
        var learner = BuildLearner();
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        _applicationRepository.Setup(x => x.GetByLearnerIdAsync(learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<LearnerApplication>());
        _bursaryApplicationRepository.Setup(x => x.GetByLearnerIdAsync(learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<BursaryApplication>());
        _documentRepository.Setup(x => x.GetByLearnerIdAsync(learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Document>());
        _accommodationInterestRepository.Setup(x => x.GetByLearnerIdAsync(learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<AccommodationInterest>());
        _careerInterestRepository.Setup(x => x.GetByLearnerIdAsync(learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<CareerInterest>());
        _consentRepository.Setup(x => x.GetHistoryByLearnerIdAsync(learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<GuardianConsent>());

        var result = await BuildExportHandler().Handle(new ExportMyDataQuery("user-1"), CancellationToken.None);

        result.Profile.FirstName.Should().Be("Lebo");
        result.Profile.LearnerId.Should().Be(learner.Id);
        result.Disclaimer.Should().NotBeNullOrWhiteSpace();
        _auditLog.Verify(x => x.AddAsync(
            It.Is<AuditLogEntry>(a => a.Action == "ExportData"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RequestErasure_CreatesRequest_AndWritesAudit()
    {
        var learner = BuildLearner();
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        _erasureRequestRepository.Setup(x => x.GetOpenByLearnerIdAsync(learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ErasureRequest?)null);
        var sut = new RequestErasureHandler(
            _learnerRepository.Object, _erasureRequestRepository.Object, _auditLog.Object);

        var id = await sut.Handle(new RequestErasureCommand("user-1", "No longer needed"), CancellationToken.None);

        id.Should().NotBeEmpty();
        _erasureRequestRepository.Verify(x => x.AddAsync(
            It.Is<ErasureRequest>(r => r.Status == ErasureRequestStatus.Requested && r.LearnerId == learner.Id),
            It.IsAny<CancellationToken>()), Times.Once);
        _auditLog.Verify(x => x.AddAsync(
            It.Is<AuditLogEntry>(a => a.Action == "RequestErasure"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RequestErasure_WhenOneAlreadyOpen_Rejected()
    {
        var learner = BuildLearner();
        var existing = ErasureRequest.Raise(learner.Id, "user-1", null);
        _learnerRepository.Setup(x => x.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(learner);
        _erasureRequestRepository.Setup(x => x.GetOpenByLearnerIdAsync(learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        var sut = new RequestErasureHandler(
            _learnerRepository.Object, _erasureRequestRepository.Object, _auditLog.Object);

        var act = () => sut.Handle(new RequestErasureCommand("user-1", null), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task FulfilErasure_AnonymisesViaService_MarksFulfilled_AndWritesAudit()
    {
        var learner = BuildLearner();
        var erasureRequest = ErasureRequest.Raise(learner.Id, "user-1", null);
        _erasureRequestRepository.Setup(x => x.GetByIdAsync(erasureRequest.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(erasureRequest);
        _erasureService.Setup(x => x.AnonymiseLearnerDataAsync(learner.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ErasureOutcome(0, 0, 0, 0, 0, false, true));
        var sut = new FulfilErasureRequestHandler(
            _erasureRequestRepository.Object, _erasureService.Object, _auditLog.Object);

        await sut.Handle(new FulfilErasureRequestCommand(erasureRequest.Id, "admin-1", "Admin", "Done"), CancellationToken.None);

        erasureRequest.Status.Should().Be(ErasureRequestStatus.Fulfilled);
        _erasureService.Verify(x => x.AnonymiseLearnerDataAsync(learner.Id, It.IsAny<CancellationToken>()), Times.Once);
        _auditLog.Verify(x => x.AddAsync(
            It.Is<AuditLogEntry>(a => a.Action == "FulfilErasureRequest"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Anonymise_RedactsPersonalData_AndSoftDeletesProfile()
    {
        var learner = BuildLearner();

        learner.Anonymise();

        learner.FirstName.Should().Be("Redacted");
        learner.Surname.Should().Be("Redacted");
        learner.IdNumber.Should().BeNull();
        learner.MobileNumber.Should().Be("REDACTED");
        learner.IsDeleted.Should().BeTrue();
    }
}
