using Microsoft.Extensions.Diagnostics.HealthChecks;
using Movies.Application.Database;

namespace Movies.Api.Health;


public class DatabaseHealthCheck(IDbConnectionFactory dbConnectionFactory , ILogger<DatabaseHealthCheck>  logger) : IHealthCheck
{
    public const string Name = "Database";
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            _ = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            return HealthCheckResult.Healthy("Database is healthy");
        }
        catch (Exception e)
        {
            const string message = "Database health check failed";
            logger.LogError(e, message);
            return HealthCheckResult.Unhealthy(message);
        }
    }
}
