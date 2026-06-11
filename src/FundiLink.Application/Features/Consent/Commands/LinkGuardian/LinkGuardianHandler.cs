using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Entities;
using MediatR;

namespace FundiLink.Application.Features.Consent.Commands.LinkGuardian;

/// <summary>
/// Creates (idempotently) a guardian link for the caller's own minor learner profile
/// and audit-logs it (append-only). Only minors may be linked to a guardian.
/// </summary>
public class LinkGuardianHandler : IRequestHandler<LinkGuardianCommand, Guid>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IGuardianLinkRepository _linkRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public LinkGuardianHandler(
        ILearnerRepository learnerRepository,
        IGuardianLinkRepository linkRepository,
        IAuditLogRepository auditLogRepository)
    {
        _learnerRepository = learnerRepository;
        _linkRepository = linkRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<Guid> Handle(LinkGuardianCommand request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        if (!learner.IsMinor())
            throw new InvalidOperationException("Guardian co-access only applies to learners under 18.");

        var existing = await _linkRepository.GetByGuardianAndLearnerAsync(
            request.GuardianUserId, learner.Id, cancellationToken);
        if (existing is not null)
            return existing.Id;

        var link = GuardianLink.Create(
            learner.Id, request.GuardianUserId, request.GuardianName, request.GuardianContact);

        await _linkRepository.AddAsync(link, cancellationToken);
        await _linkRepository.SaveChangesAsync(cancellationToken);

        await _auditLogRepository.AddAsync(
            AuditLogEntry.Create(request.UserId, "Learner", "LinkGuardian", "GuardianLink", link.Id.ToString()),
            cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);

        return link.Id;
    }
}
