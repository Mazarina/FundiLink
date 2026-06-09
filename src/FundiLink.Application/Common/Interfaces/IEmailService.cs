namespace FundiLink.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailVerificationAsync(string email, string verificationLink, CancellationToken cancellationToken = default);
}
