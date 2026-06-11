using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Consent.Queries.GetGuardianView;

/// <summary>
/// Returns the consent-gated, minimised read-only view of a minor learner to a linked
/// guardian. Access is denied unless (a) a guardian link exists for this guardian and
/// learner, and (b) a current GuardianCoAccess consent is in place. The returned view
/// is limited to the consented scope (data minimisation per POPIA). Every successful
/// access is audit-logged (append-only).
/// </summary>
public class GetGuardianViewHandler : IRequestHandler<GetGuardianViewQuery, GuardianViewDto>
{
    private readonly IGuardianLinkRepository _linkRepository;
    private readonly IGuardianConsentRepository _consentRepository;
    private readonly IConsentCheckService _consentCheckService;
    private readonly ILearnerRepository _learnerRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IBursaryApplicationRepository _bursaryApplicationRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public GetGuardianViewHandler(
        IGuardianLinkRepository linkRepository,
        IGuardianConsentRepository consentRepository,
        IConsentCheckService consentCheckService,
        ILearnerRepository learnerRepository,
        IApplicationRepository applicationRepository,
        IBursaryApplicationRepository bursaryApplicationRepository,
        IAuditLogRepository auditLogRepository)
    {
        _linkRepository = linkRepository;
        _consentRepository = consentRepository;
        _consentCheckService = consentCheckService;
        _learnerRepository = learnerRepository;
        _applicationRepository = applicationRepository;
        _bursaryApplicationRepository = bursaryApplicationRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<GuardianViewDto> Handle(GetGuardianViewQuery request, CancellationToken cancellationToken)
    {
        // 1. The guardian must be linked to this learner.
        var link = await _linkRepository.GetByGuardianAndLearnerAsync(
            request.GuardianUserId, request.LearnerId, cancellationToken)
            ?? throw new UnauthorizedAccessException("You are not linked to this learner.");

        // 2. A current GuardianCoAccess consent must be in place — never bypassable.
        var hasConsent = await _consentCheckService.HasCurrentConsentAsync(
            request.LearnerId, ConsentType.GuardianCoAccess, cancellationToken);
        if (!hasConsent)
            throw new UnauthorizedAccessException("Guardian co-access consent is not in place for this learner.");

        var consent = await _consentRepository.GetLatestAsync(
            request.LearnerId, ConsentType.GuardianCoAccess, cancellationToken)
            ?? throw new UnauthorizedAccessException("Guardian co-access consent is not in place for this learner.");

        var learner = await _learnerRepository.GetByIdAsync(request.LearnerId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        // 3. Build a minimised view limited to the consented scope (data minimisation).
        IReadOnlyList<GuardianApplicationSummaryDto> applications = [];
        if (consent.Scope == ConsentScope.ProfileAndApplications)
        {
            applications = await BuildApplicationSummariesAsync(learner.Id, cancellationToken);
        }

        // 4. Append-only audit log of the guardian access.
        await _auditLogRepository.AddAsync(
            AuditLogEntry.Create(
                request.GuardianUserId, "Guardian", "GuardianViewLearner", "Learner", learner.Id.ToString()),
            cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);

        return new GuardianViewDto(
            learner.Id,
            learner.FirstName,
            learner.Surname,
            learner.GradeLevel,
            learner.SchoolName,
            learner.Province,
            learner.ProfileCompleteness,
            consent.Scope,
            applications,
            ConsentDisclaimer.Text);
    }

    private async Task<IReadOnlyList<GuardianApplicationSummaryDto>> BuildApplicationSummariesAsync(
        Guid learnerId, CancellationToken ct)
    {
        var summaries = new List<GuardianApplicationSummaryDto>();

        var programmeApps = await _applicationRepository.GetByLearnerIdAsync(learnerId, ct);
        summaries.AddRange(programmeApps.Select(a => new GuardianApplicationSummaryDto(
            a.Programme?.Name ?? "Programme", "Programme", a.Status.ToString())));

        var bursaryApps = await _bursaryApplicationRepository.GetByLearnerIdAsync(learnerId, ct);
        summaries.AddRange(bursaryApps.Select(a => new GuardianApplicationSummaryDto(
            a.Bursary?.Name ?? "Bursary", "Bursary", a.Status.ToString())));

        return summaries;
    }
}
