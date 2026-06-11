using FundiLink.Domain.Entities;

namespace FundiLink.Application.Common.Interfaces;

public interface IGuardianLinkRepository
{
    Task AddAsync(GuardianLink link, CancellationToken ct);
    Task<GuardianLink?> GetByGuardianAndLearnerAsync(string guardianUserId, Guid learnerId, CancellationToken ct);
    Task<IReadOnlyList<GuardianLink>> GetByGuardianUserIdAsync(string guardianUserId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
