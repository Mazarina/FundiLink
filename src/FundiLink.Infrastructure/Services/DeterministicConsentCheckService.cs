using FundiLink.Application.Common.Interfaces;
using FundiLink.Domain.Enums;

namespace FundiLink.Infrastructure.Services;

/// <summary>
/// Deterministic consent-check service. Decides consent purely from the latest
/// append-only consent record — no external identity-verification or e-signature
/// provider call in this phase. A real provider may be wired later behind the
/// IConsentCheckService interface, with any key supplied via environment only.
/// </summary>
public class DeterministicConsentCheckService : IConsentCheckService
{
    private readonly IGuardianConsentRepository _consentRepository;

    public DeterministicConsentCheckService(IGuardianConsentRepository consentRepository)
    {
        _consentRepository = consentRepository;
    }

    public async Task<bool> HasCurrentConsentAsync(Guid learnerId, ConsentType consentType, CancellationToken ct)
    {
        var latest = await _consentRepository.GetLatestAsync(learnerId, consentType, ct);
        return latest is not null && latest.Status == ConsentStatus.Granted;
    }
}
