namespace FundiLink.Application.Common.Interfaces;

public interface IWhatsAppService
{
    Task<bool> SendMessageAsync(string toPhoneNumber, string message, CancellationToken ct);
}
