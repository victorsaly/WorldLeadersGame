using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WorldLeaders.Infrastructure.Configuration;
using WorldLeaders.Infrastructure.Data;

namespace WorldLeaders.Infrastructure.Services;

/// <summary>
/// Background service for cleaning up expired sessions and maintaining child safety compliance
/// Context: Educational game session management for 12-year-old players
/// Safety Requirements: Automatic session cleanup, child session timeout enforcement
/// </summary>
public class SessionCleanupService(
    IServiceProvider serviceProvider,
    IOptions<ChildSafetyOptions> childSafetyOptions,
    ILogger<SessionCleanupService> logger) : BackgroundService
{
    private readonly ChildSafetyOptions _options = childSafetyOptions.Value;
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(5); // Run every 5 minutes

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Session cleanup service starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredSessionsAsync();
                await Task.Delay(_cleanupInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Service is stopping
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in session cleanup service");
                
                // Wait a bit longer on error to avoid tight loop
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        logger.LogInformation("Session cleanup service stopping");
    }

    private async Task CleanupExpiredSessionsAsync()
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WorldLeadersDbContext>();
            
            var cutoffTime = DateTime.UtcNow;
            
            // Find expired sessions
            var expiredSessions = await dbContext.UserSessions
                .Where(s => s.IsActive && s.ExpiresAt < cutoffTime)
                .ToListAsync();

            if (expiredSessions.Any())
            {
                // Mark sessions as inactive
                foreach (var session in expiredSessions)
                {
                    session.IsActive = false;
                }

                await dbContext.SaveChangesAsync();

                logger.LogInformation("Cleaned up {Count} expired sessions", expiredSessions.Count);

                // Log child sessions separately for safety monitoring
                var childSessions = expiredSessions.Where(s => 
                    (DateTime.UtcNow - s.StartedAt).TotalMinutes <= _options.ChildSessionTimeoutMinutes + 5)
                    .ToList();

                if (childSessions.Any())
                {
                    logger.LogInformation("Cleaned up {Count} potential child sessions", childSessions.Count);
                }
            }

            // Clean up very old inactive sessions (older than 30 days)
            var oldCutoff = DateTime.UtcNow.AddDays(-30);
            var oldSessions = await dbContext.UserSessions
                .Where(s => !s.IsActive && s.StartedAt < oldCutoff)
                .ToListAsync();

            if (oldSessions.Any())
            {
                dbContext.UserSessions.RemoveRange(oldSessions);
                await dbContext.SaveChangesAsync();
                
                logger.LogInformation("Removed {Count} old session records", oldSessions.Count);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during session cleanup");
            throw;
        }
    }
}