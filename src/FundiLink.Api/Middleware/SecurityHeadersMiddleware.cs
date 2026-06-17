namespace FundiLink.Api.Middleware;

/// <summary>
/// Adds baseline security headers to every API response. The frontend is served
/// separately (its own Nginx/security headers); these headers harden the API
/// responses themselves (JSON, file downloads, Swagger in development).
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        context.Response.Headers["Referrer-Policy"] = "no-referrer";
        context.Response.Headers["Permissions-Policy"] = "geolocation=(), camera=(), microphone=()";

        await _next(context);
    }
}
