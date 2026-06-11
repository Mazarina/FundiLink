using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Consent.Queries.GetConsentHistory;

/// <summary>
/// Returns the full append-only consent history for the caller's own learner profile,
/// newest first. Owner-scoped.
/// </summary>
public class GetConsentHistoryHandler
    : IRequestHandler<GetConsentHistoryQuery, IReadOnlyList<ConsentHistoryEntryDto>>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IGuardianConsentRepository _consentRepository;

    public GetConsentHistoryHandler(
        ILearnerRepository learnerRepository,
        IGuardianConsentRepository consentRepository)
    {
        _learnerRepository = learnerRepository;
        _consentRepository = consentRepository;
    }

    public async Task<IReadOnlyList<ConsentHistoryEntryDto>> Handle(
        GetConsentHistoryQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var history = await _consentRepository.GetHistoryByLearnerIdAsync(learner.Id, cancellationToken);

        return history
            .Select(c => new ConsentHistoryEntryDto(
                c.Id, c.ConsentType, c.Scope, c.Status, c.GuardianName, c.RecordedAt))
            .ToList();
    }
}
