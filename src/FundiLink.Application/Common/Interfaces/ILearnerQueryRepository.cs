using FundiLink.Application.Common.Models;

namespace FundiLink.Application.Common.Interfaces;

public interface ILearnerQueryRepository
{
    Task<(IEnumerable<LearnerSummary> Items, int Total)> SearchAsync(
        string? keyword, string? province, int page, int pageSize, CancellationToken ct);

    Task<LearnerOverview?> GetOverviewAsync(Guid learnerId, CancellationToken ct);
}
