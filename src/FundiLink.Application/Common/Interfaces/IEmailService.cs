namespace FundiLink.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailVerificationAsync(string email, string verificationLink, CancellationToken cancellationToken = default);
    Task SendNotificationEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default);
}
