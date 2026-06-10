using FundiLink.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace FundiLink.Infrastructure.Services;

// Stub implementation — replace with SendGrid or similar in a later phase.
public class StubEmailService : IEmailService
{
    private readonly ILogger<StubEmailService> _logger;

    public StubEmailService(ILogger<StubEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailVerificationAsync(string email, string verificationLink, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[STUB EMAIL] Verification email to {Email}: {Link}", email, verificationLink);
        return Task.CompletedTask;
    }

    public Task SendNotificationEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[STUB EMAIL] Notification to {Email} - {Subject}: {Body}", toEmail, subject, body);
        return Task.CompletedTask;
    }
}
