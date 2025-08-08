using Microsoft.Extensions.Diagnostics.HealthChecks;
using WorldLeaders.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace WorldLeaders.API.HealthChecks;

/// <summary>
/// Health check for child safety systems in the educational platform
/// Context: Educational game platform for 12-year-old geography and economics learning
/// Safety: Validates that all child protection systems are operational
/// </summary>
public class ChildSafetyHealthCheck(
    IServiceProvider serviceProvider,
    ILogger<ChildSafetyHealthCheck> logger) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Basic safety system checks without requiring complex dependencies
            var checks = new List<(string Name, bool IsHealthy, string Details)>();

            // Check content moderation service
            try
            {
                var contentModerationService = serviceProvider.GetService<IContentModerationService>();
                checks.Add(("ContentModeration", contentModerationService != null, 
                    contentModerationService != null ? "Content moderation service available" : "Service not available"));
            }
            catch (Exception ex)
            {
                checks.Add(("ContentModeration", false, $"Error: {ex.Message}"));
            }

            // Check child safety validator
            try
            {
                var childSafetyValidator = serviceProvider.GetService<IChildSafetyValidator>();
                checks.Add(("ChildSafetyValidator", childSafetyValidator != null,
                    childSafetyValidator != null ? "Child safety validator available" : "Service not available"));
            }
            catch (Exception ex)
            {
                checks.Add(("ChildSafetyValidator", false, $"Error: {ex.Message}"));
            }

            // Check authentication service
            try
            {
                var authService = serviceProvider.GetService<IAuthenticationService>();
                checks.Add(("Authentication", authService != null,
                    authService != null ? "Authentication service available" : "Service not available"));
            }
            catch (Exception ex)
            {
                checks.Add(("Authentication", false, $"Error: {ex.Message}"));
            }

            var allHealthy = checks.All(c => c.IsHealthy);
            var healthyCount = checks.Count(c => c.IsHealthy);
            var overallScore = (double)healthyCount / checks.Count;

            var data = new Dictionary<string, object>
            {
                ["OverallScore"] = overallScore,
                ["HealthyServices"] = healthyCount,
                ["TotalServices"] = checks.Count,
                ["Checks"] = checks.Select(c => new { c.Name, c.IsHealthy, c.Details }),
                ["ChildSafetyMode"] = true,
                ["TargetAge"] = "12 years",
                ["ComplianceFrameworks"] = "COPPA, GDPR, UK Educational Standards"
            };

            if (allHealthy)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Child safety systems are operational", data));
            }
            else if (overallScore >= 0.5)
            {
                return Task.FromResult(HealthCheckResult.Degraded("Some child safety systems have issues", null, data));
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Critical child safety systems are not operational", null, data));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Child safety health check failed");
            return Task.FromResult(HealthCheckResult.Unhealthy("Child safety systems are not responding", ex, new Dictionary<string, object>
            {
                ["Error"] = ex.Message,
                ["ChildSafetyMode"] = true,
                ["CriticalSystem"] = true
            }));
        }
    }
}
