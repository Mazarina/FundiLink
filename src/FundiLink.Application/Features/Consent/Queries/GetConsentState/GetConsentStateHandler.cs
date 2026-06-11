using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Enums;
using MediatR;

namespace FundiLink.Application.Features.Consent.Queries.GetConsentState;

/// <summary>
/// Returns the effective consent state for the caller's own learner profile.
/// For each consent type, the latest append-only record determines whether it is
/// currently granted. Owner-scoped — only the caller's data is read.
/// </summary>
public class GetConsentStateHandler : IRequestHandler<GetConsentStateQuery, ConsentStateDto>
{
    private readonly ILearnerRepository _learnerRepository;
    private readonly IGuardianConsentRepository _consentRepository;

    public GetConsentStateHandler(
        ILearnerRepository learnerRepository,
        IGuardianConsentRepository consentRepository)
    {
        _learnerRepository = learnerRepository;
        _consentRepository = consentRepository;
    }

    public async Task<ConsentStateDto> Handle(GetConsentStateQuery request, CancellationToken cancellationToken)
    {
        var learner = await _learnerRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException("Learner profile not found.");

        var isMinor = learner.IsMinor();
        var states = new List<ConsentTypeStateDto>();

        foreach (var type in Enum.GetValues<ConsentType>())
        {
            var latest = await _consentRepository.GetLatestAsync(learner.Id, type, cancellationToken);
            var granted = latest is not null && latest.Status == ConsentStatus.Granted;
            states.Add(new ConsentTypeStateDto(
                type,
                granted,
                granted ? latest!.Scope : null,
                granted ? latest!.GuardianName : null,
                latest?.RecordedAt));
        }

        return new ConsentStateDto(
            isMinor,
            GuardianConsentRequired: isMinor,
            states,
            ConsentDisclaimer.Text);
    }
}
