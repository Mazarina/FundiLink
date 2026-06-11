using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Consent.Commands.RevokeConsent;

/// <summary>
/// Appends a revocation record for the caller's own learner profile (right to withdraw)
/// and audit-logs the action (append-only). Requires an existing granted consent.
/// </summary>
public class RevokeConsentHandler : IRequestHandler<RevokeConsentCommand, Guid>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IGuardianConsentRepository _consentRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public RevokeConsentHandler(
        ILearnerRepository learnerRepository,
        IGuardianConsentRepository consentRepository,
        IAuditLogRepository auditLogRepository)
    {
        _learnerRepository = learnerRepository;
        _consentRepository = consentRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<Guid> Handle(RevokeConsentCommand request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var latest = await _consentRepository.GetLatestAsync(learner.Id, request.ConsentType, cancellationToken);
        if (latest is null || latest.Status == ConsentStatus.Revoked)
            throw new InvalidOperationException("There is no active consent of this type to revoke.");

        var revocation = GuardianConsent.Revoke(
            learner.Id, latest.ConsentType, latest.Scope, latest.GuardianName, latest.GuardianContact);

        await _consentRepository.AddAsync(revocation, cancellationToken);
        await _consentRepository.SaveChangesAsync(cancellationToken);

        await _auditLogRepository.AddAsync(
            AuditLogEntry.Create(request.UserId, "Learner", "RevokeGuardianConsent", "GuardianConsent", revocation.Id.ToString()),
            cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);

        return revocation.Id;
    }
}
