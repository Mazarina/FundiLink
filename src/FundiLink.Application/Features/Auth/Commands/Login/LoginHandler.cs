using FundiLink.Application.Common.Interfaces;
using MediatR;

namespace FundiLink.Application.Features.Auth.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginHandler(IIdentityService identityService, IJwtTokenService jwtTokenService)
    {
        _identityService = identityService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var (succeeded, isLockedOut) = await _identityService.CheckPasswordAsync(request.Email, request.Password);

        if (isLockedOut)
            throw new UnauthorizedAccessException("Account is locked. Please try again later.");

        if (!succeeded)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var userInfo = await _identityService.GetUserByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        var accessToken = _jwtTokenService.GenerateAccessToken(userInfo.UserId, userInfo.Email, userInfo.Roles);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        return new LoginResult(accessToken, refreshToken, ExpiresIn: 3600);
    }
}
