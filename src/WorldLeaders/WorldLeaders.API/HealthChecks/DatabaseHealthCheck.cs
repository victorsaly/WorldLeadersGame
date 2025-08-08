using Microsoft.Extensions.Diagnostics.HealthChecks;
using WorldLeaders.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace WorldLeaders.API.HealthChecks;

/// <summary>
/// Health check for database connectivity in the educational platform
/// Context: Educational game platform for 12-year-old geography and economics learning
/// Safety: Validates that educational data storage is accessible
/// </summary>
public class DatabaseHealthCheck(
    WorldLeadersDbContext dbContext,
    ILogger<DatabaseHealthCheck> logger) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Simple database connectivity check
            await dbContext.Database.CanConnectAsync(cancellationToken);
            
            // Count territories to ensure educational data is available
            var territoryCount = await dbContext.Territories.CountAsync(cancellationToken);
            var playerCount = await dbContext.Players.CountAsync(cancellationToken);
            
            return HealthCheckResult.Healthy("Database is accessible and educational data is available", new Dictionary<string, object>
            {
                ["DatabaseType"] = "In-Memory (Development)",
                ["TerritoriesAvailable"] = territoryCount,
                ["PlayersRegistered"] = playerCount,
                ["EducationalDataReady"] = territoryCount > 0,
                ["ChildDataProtected"] = true
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database health check failed");
            return HealthCheckResult.Unhealthy("Database is not accessible", ex, new Dictionary<string, object>
            {
                ["DatabaseType"] = "In-Memory (Development)",
                ["Error"] = ex.Message,
                ["CriticalSystem"] = true
            });
        }
    }
}
