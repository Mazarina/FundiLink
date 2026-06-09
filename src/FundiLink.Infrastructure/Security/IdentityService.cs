using FundiLink.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FundiLink.Infrastructure.Security;

public class IdentityService : IIdentityService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public IdentityService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<(bool Succeeded, string UserId, string[] Errors)> CreateUserAsync(string email, string password)
    {
        var user = new IdentityUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);
        return (result.Succeeded, user.Id, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<bool> AssignRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return false;
        var result = await _userManager.AddToRoleAsync(user, role);
        return result.Succeeded;
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException("User not found.");
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<(bool Succeeded, bool IsLockedOut)> CheckPasswordAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return (false, false);

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        return (result.Succeeded, result.IsLockedOut);
    }

    public async Task<(string UserId, string Email, IEnumerable<string> Roles)?> GetUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return null;
        var roles = await _userManager.GetRolesAsync(user);
        return (user.Id, user.Email!, roles);
    }
}
