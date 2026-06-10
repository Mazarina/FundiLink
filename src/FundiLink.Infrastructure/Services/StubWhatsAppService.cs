using FundiLink.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace FundiLink.Infrastructure.Services;

// Stub implementation — no real WhatsApp Business API credentials exist.
// Replace with a real provider once a formal integration is in place.
public class StubWhatsAppService : IWhatsAppService
{
    private readonly ILogger<StubWhatsAppService> _logger;

    public StubWhatsAppService(ILogger<StubWhatsAppService> logger)
    {
        _logger = logger;
    }

    public Task<bool> SendMessageAsync(string toPhoneNumber, string message, CancellationToken ct)
    {
        _logger.LogInformation("[STUB WhatsApp] To: {Phone} - {Message}", toPhoneNumber, message);
        return Task.FromResult(true);
    }
}
