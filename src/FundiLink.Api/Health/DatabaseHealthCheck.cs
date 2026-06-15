using FundiLink.Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FundiLink.Api.Health;

/// <summary>
/// Reports whether the API can connect to PostgreSQL. Used by the /health/db
/// readiness endpoint so deployment health checks can detect a database that is
/// unreachable (e.g. still starting up, or down) without exposing any data.
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly FundiLinkDbContext _dbContext;

    public DatabaseHealthCheck(FundiLinkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);
            return canConnect
                ? HealthCheckResult.Healthy("Database connection succeeded.")
                : HealthCheckResult.Unhealthy("Database connection failed.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database connection threw an exception.", ex);
        }
    }
}
