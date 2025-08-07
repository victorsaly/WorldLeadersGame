using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.ApplicationInsights;
using WorldLeaders.Infrastructure.Services;
using WorldLeaders.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace WorldLeaders.API.Controllers;

/// <summary>
/// Performance monitoring and metrics API for educational platform scalability
/// Provides real-time performance data for 1000+ concurrent users monitoring
/// Context: Educational game monitoring for 12-year-old child-friendly performance
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PerformanceController(
    PerformanceOptimizedGameComponent performanceComponent,
    TelemetryClient telemetryClient,
    IOptions<PerformanceConfig> performanceOptions,
    ILogger<PerformanceController> logger) : ControllerBase
{
    private readonly PerformanceOptimizedGameComponent _performanceComponent = performanceComponent ?? throw new ArgumentNullException(nameof(performanceComponent));
    private readonly TelemetryClient _telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
    private readonly PerformanceConfig _performanceConfig = performanceOptions?.Value ?? PerformanceConfig.UKEducationalDefaults;
    private readonly ILogger<PerformanceController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Gets current performance metrics for the educational platform
    /// </summary>
    /// <returns>Current performance metrics</returns>
    [HttpGet("metrics")]
    [ProducesResponseType(typeof(PerformanceMetrics), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PerformanceMetrics>> GetPerformanceMetrics()
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            
                var metrics = await _performanceComponent.ExecuteWithPerformanceOptimization(
                "GetPerformanceMetrics",
                "performance:metrics:current",
                () =>
                {
                    var performanceMetrics = _performanceComponent.GetPerformanceMetrics();
                    
                    // Create new record with updated timestamp for real-time metrics
                    return Task.FromResult(performanceMetrics with { Timestamp = DateTime.UtcNow });
                },
                TimeSpan.FromMinutes(1) // Cache for 1 minute
            );            stopwatch.Stop();

            // Track API performance
            _telemetryClient.TrackMetric("API.Performance.GetMetrics.Duration", stopwatch.ElapsedMilliseconds);
            
            _logger.LogInformation("Performance metrics retrieved in {Duration}ms for region {Region}", 
                stopwatch.ElapsedMilliseconds, _performanceConfig.Region);

            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve performance metrics");
            _telemetryClient.TrackException(ex);
            return StatusCode(500, "Failed to retrieve performance metrics");
        }
    }

    /// <summary>
    /// Gets performance health check for educational platform
    /// </summary>
    /// <returns>Health status with performance indicators</returns>
    [HttpGet("health")]
    [ProducesResponseType(typeof(PerformanceHealthStatus), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<PerformanceHealthStatus>> GetHealthStatus()
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            
            var healthStatus = await _performanceComponent.ExecuteWithPerformanceOptimization(
                "GetHealthStatus",
                "performance:health:current",
                () =>
                {
                    var process = Process.GetCurrentProcess();
                    var memoryUsageMB = process.WorkingSet64 / (1024 * 1024);
                    var cpuTime = process.TotalProcessorTime;

                    var isHealthy = memoryUsageMB < 2048 && // Less than 2GB memory usage
                                   stopwatch.ElapsedMilliseconds < _performanceConfig.MaxResponseTimeMs;

                    return Task.FromResult(new PerformanceHealthStatus
                    {
                        Status = isHealthy ? "Healthy" : "Degraded",
                        Region = _performanceConfig.Region,
                        MemoryUsageMB = memoryUsageMB,
                        ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                        TargetResponseTimeMs = _performanceConfig.MaxResponseTimeMs,
                        IsChildFriendlyPerformance = stopwatch.ElapsedMilliseconds <= _performanceConfig.UI.InitialLoadTimeTargetMs,
                        ConcurrentUsersSupported = isHealthy ? 1000 : 500, // Degraded performance estimate
                        Timestamp = DateTime.UtcNow
                    });
                },
                TimeSpan.FromSeconds(30) // Cache for 30 seconds
            );

            stopwatch.Stop();

            var statusCode = healthStatus.Status == "Healthy" ? 200 : 503;
            
            // Track health metrics
            _telemetryClient.TrackMetric("API.Performance.Health.MemoryUsage", healthStatus.MemoryUsageMB);
            _telemetryClient.TrackMetric("API.Performance.Health.ResponseTime", healthStatus.ResponseTimeMs);
            _telemetryClient.TrackEvent("API.Performance.Health.Check", new Dictionary<string, string>
            {
                ["Status"] = healthStatus.Status,
                ["Region"] = healthStatus.Region,
                ["ChildFriendly"] = healthStatus.IsChildFriendlyPerformance.ToString()
            });

            return StatusCode(statusCode, healthStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve health status");
            _telemetryClient.TrackException(ex);
            
            return StatusCode(503, new PerformanceHealthStatus
            {
                Status = "Unhealthy",
                Region = _performanceConfig.Region,
                MemoryUsageMB = 0,
                ResponseTimeMs = 0,
                TargetResponseTimeMs = _performanceConfig.MaxResponseTimeMs,
                IsChildFriendlyPerformance = false,
                ConcurrentUsersSupported = 0,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Clears performance cache for a specific operation
    /// </summary>
    /// <param name="cacheKey">Cache key to invalidate</param>
    /// <returns>Success status</returns>
    [HttpDelete("cache/{cacheKey}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> InvalidateCache(string cacheKey)
    {
        if (string.IsNullOrWhiteSpace(cacheKey))
        {
            return BadRequest("Cache key cannot be empty");
        }

        try
        {
            await _performanceComponent.InvalidateCacheAsync(cacheKey);
            
            _telemetryClient.TrackEvent("API.Performance.Cache.Invalidated", new Dictionary<string, string>
            {
                ["CacheKey"] = cacheKey,
                ["Region"] = _performanceConfig.Region
            });

            _logger.LogInformation("Cache invalidated for key: {CacheKey}", cacheKey);
            return Ok(new { Message = "Cache invalidated successfully", CacheKey = cacheKey });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to invalidate cache for key: {CacheKey}", cacheKey);
            _telemetryClient.TrackException(ex);
            return StatusCode(500, "Failed to invalidate cache");
        }
    }

    /// <summary>
    /// Gets performance configuration for educational platform optimization
    /// </summary>
    /// <returns>Current performance configuration</returns>
    [HttpGet("config")]
    [ProducesResponseType(typeof(PerformanceConfig), StatusCodes.Status200OK)]
    public ActionResult<PerformanceConfig> GetPerformanceConfig()
    {
        _telemetryClient.TrackEvent("API.Performance.Config.Retrieved", new Dictionary<string, string>
        {
            ["Region"] = _performanceConfig.Region
        });

        return Ok(_performanceConfig);
    }
}

/// <summary>
/// Performance health status for educational platform monitoring
/// </summary>
public sealed record PerformanceHealthStatus
{
    public string Status { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public long MemoryUsageMB { get; init; }
    public long ResponseTimeMs { get; init; }
    public int TargetResponseTimeMs { get; init; }
    public bool IsChildFriendlyPerformance { get; init; }
    public int ConcurrentUsersSupported { get; init; }
    public DateTime Timestamp { get; init; }
}