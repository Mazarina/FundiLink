using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Consent.Commands.RecordConsent;

/// <summary>
/// Appends a new guardian consent grant for the caller's own learner profile and
/// audit-logs the action (append-only). Only minors require guardian consent.
/// </summary>
public class RecordConsentHandler : IRequestHandler<RecordConsentCommand, Guid>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IGuardianConsentRepository _consentRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public RecordConsentHandler(
        ILearnerRepository learnerRepository,
        IGuardianConsentRepository consentRepository,
        IAuditLogRepository auditLogRepository)
    {
        _learnerRepository = learnerRepository;
        _consentRepository = consentRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<Guid> Handle(RecordConsentCommand request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        if (!learner.IsMinor())
            throw new InvalidOperationException("Guardian consent only applies to learners under 18.");

        var consent = GuardianConsent.Grant(
            learner.Id, request.ConsentType, request.Scope, request.GuardianName, request.GuardianContact);

        await _consentRepository.AddAsync(consent, cancellationToken);
        await _consentRepository.SaveChangesAsync(cancellationToken);

        await _auditLogRepository.AddAsync(
            AuditLogEntry.Create(request.UserId, "Learner", "RecordGuardianConsent", "GuardianConsent", consent.Id.ToString()),
            cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);

        return consent.Id;
    }
}
