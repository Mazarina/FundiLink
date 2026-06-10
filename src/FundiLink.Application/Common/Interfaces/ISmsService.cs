namespace FundiLink.Application.Common.Interfaces;

public interface ISmsService
{
    Task<bool> SendSmsAsync(string toPhoneNumber, string message, CancellationToken ct);
}
