namespace FundiLink.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(bool Succeeded, string UserId, string[] Errors)> CreateUserAsync(string email, string password);
    Task<bool> AssignRoleAsync(string userId, string role);
    Task<string> GenerateEmailConfirmationTokenAsync(string userId);
    Task<(bool Succeeded, bool IsLockedOut)> CheckPasswordAsync(string email, string password);
    Task<(string UserId, string Email, IEnumerable<string> Roles)?> GetUserByEmailAsync(string email);
    Task<(string UserId, string Email)?> GetUserByIdAsync(string userId);
}
