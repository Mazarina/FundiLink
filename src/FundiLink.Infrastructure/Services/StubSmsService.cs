using FundiLink.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace FundiLink.Infrastructure.Services;

// Stub implementation — no real SMS gateway credentials exist.
// Replace with a real provider once a formal integration is in place.
public class StubSmsService : ISmsService
{
    private readonly ILogger<StubSmsService> _logger;

    public StubSmsService(ILogger<StubSmsService> logger)
    {
        _logger = logger;
    }

    public Task<bool> SendSmsAsync(string toPhoneNumber, string message, CancellationToken ct)
    {
        _logger.LogInformation("[STUB SMS] To: {Phone} - {Message}", toPhoneNumber, message);
        return Task.FromResult(true);
    }
}
