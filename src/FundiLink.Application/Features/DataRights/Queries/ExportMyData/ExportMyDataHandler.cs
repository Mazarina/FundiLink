using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.DataRights.Queries.ExportMyData;

/// <summary>
/// Generates an owner-scoped, typed export of the caller's FundiLink data (POPIA right of
/// access) and audit-logs the generation (append-only). The export is produced in-process;
/// no third-party storage/email/delivery integration is used in this phase.
/// </summary>
public class ExportMyDataHandler : IRequestHandler<ExportMyDataQuery, DataExportDto>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IBursaryApplicationRepository _bursaryApplicationRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IAccommodationInterestRepository _accommodationInterestRepository;
    private readonly ICareerInterestRepository _careerInterestRepository;
    private readonly IGuardianConsentRepository _consentRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public ExportMyDataHandler(
        ILearnerRepository learnerRepository,
        IApplicationRepository applicationRepository,
        IBursaryApplicationRepository bursaryApplicationRepository,
        IDocumentRepository documentRepository,
        IAccommodationInterestRepository accommodationInterestRepository,
        ICareerInterestRepository careerInterestRepository,
        IGuardianConsentRepository consentRepository,
        IAuditLogRepository auditLogRepository)
    {
        _learnerRepository = learnerRepository;
        _applicationRepository = applicationRepository;
        _bursaryApplicationRepository = bursaryApplicationRepository;
        _documentRepository = documentRepository;
        _accommodationInterestRepository = accommodationInterestRepository;
        _careerInterestRepository = careerInterestRepository;
        _consentRepository = consentRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<DataExportDto> Handle(ExportMyDataQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var academic = await _learnerRepository.GetAcademicProfileByLearnerIdAsync(learner.Id, cancellationToken);
        var applications = await _applicationRepository.GetByLearnerIdAsync(learner.Id, cancellationToken);
        var bursaryApplications = await _bursaryApplicationRepository.GetByLearnerIdAsync(learner.Id, cancellationToken);
        var documents = await _documentRepository.GetByLearnerIdAsync(learner.Id, cancellationToken);
        var accommodationInterests = await _accommodationInterestRepository.GetByLearnerIdAsync(learner.Id, cancellationToken);
        var careerInterests = await _careerInterestRepository.GetByLearnerIdAsync(learner.Id, cancellationToken);
        var consentHistory = await _consentRepository.GetHistoryByLearnerIdAsync(learner.Id, cancellationToken);

        var export = new DataExportDto(
            GeneratedAt: DateTime.UtcNow,
            Profile: MapProfile(learner),
            AcademicProfile: MapAcademic(academic),
            Applications: applications.Select(MapApplication).ToList(),
            BursaryApplications: bursaryApplications.Select(MapBursaryApplication).ToList(),
            Documents: documents.Select(MapDocument).ToList(),
            AccommodationInterests: accommodationInterests
                .Select(i => new ExportInterestDto(i.Id, i.AccommodationListing?.Name ?? "Accommodation listing", i.Status))
                .ToList(),
            CareerInterests: careerInterests
                .Select(i => new ExportInterestDto(i.Id, i.CareerOpportunity?.Title ?? "Career opportunity", i.Status))
                .ToList(),
            ConsentHistory: consentHistory
                .Select(c => new ExportConsentDto(c.ConsentType, c.Scope, c.Status, c.RecordedAt))
                .ToList(),
            Disclaimer: DataRightsDisclaimer.Text);

        await _auditLogRepository.AddAsync(
            AuditLogEntry.Create(request.UserId, "Learner", "ExportData", "Learner", learner.Id.ToString()),
            cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);

        return export;
    }

    private static ExportProfileDto MapProfile(Learner l) => new(
        l.Id, l.FirstName, l.Surname, l.DateOfBirth, l.IdNumber, l.PassportNumber, l.Gender,
        l.HomeLanguage, l.Nationality, l.MobileNumber, l.Province, l.Municipality, l.Suburb,
        l.SchoolName, l.SchoolProvince, l.GradeLevel, l.GuardianName, l.GuardianPhone,
        l.GuardianEmail, l.ProfileCompleteness, l.CreatedAt);

    private static ExportAcademicProfileDto? MapAcademic(Domain.Entities.AcademicProfile? a)
        => a is null
            ? null
            : new ExportAcademicProfileDto(
                a.Year, a.ResultType, a.ApsScore, a.ApsCalculatedAt,
                a.Subjects.Select(s => new ExportSubjectDto(s.SubjectName, s.Percentage, s.ApsPoints)).ToList());

    private static ExportApplicationDto MapApplication(LearnerApplication a) => new(
        a.Id, a.Programme?.Name ?? "Programme", a.Status, a.DeadlineDate, a.SubmittedAt, a.OutcomeAt);

    private static ExportBursaryApplicationDto MapBursaryApplication(BursaryApplication b) => new(
        b.Id, b.Bursary?.Name ?? "Bursary", b.Status, b.DeadlineDate);

    private static ExportDocumentDto MapDocument(Document d) => new(
        d.Id, d.DocumentType, d.FileName, d.Status, d.CreatedAt);
}
