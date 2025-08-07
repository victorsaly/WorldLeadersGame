using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Options;
using WorldLeaders.Infrastructure.Configuration;
using WorldLeaders.Shared.Services;
using System.Diagnostics;

namespace WorldLeaders.Infrastructure.Services;

/// <summary>
/// Enhanced performance optimization with .NET 8 primary constructors
/// Designed for 1000+ concurrent users with child-friendly performance requirements
/// Context: Educational game component for 12-year-old geography and economics learning
/// </summary>
public class PerformanceOptimizedGameComponent(
    IMemoryCache memoryCache,
    IDistributedCache distributedCache,
    TelemetryClient telemetryClient,
    IOptions<PerformanceConfig> performanceOptions) : IGameComponent
{
    private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    private readonly IDistributedCache _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
    private readonly TelemetryClient _telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
    private readonly PerformanceConfig _config = performanceOptions?.Value ?? PerformanceConfig.UKEducationalDefaults;

    /// <summary>
    /// Performance configuration optimized for UK educational deployment
    /// </summary>
    public required PerformanceConfig Config { get; init; } = PerformanceConfig.UKEducationalDefaults;

    /// <summary>
    /// Target region for optimization
    /// </summary>
    public required string Region { get; init; } = "UK South";

    /// <summary>
    /// Executes a child-friendly game operation with performance monitoring and caching
    /// </summary>
    /// <typeparam name="T">Return type of the operation</typeparam>
    /// <param name="operationName">Name of the operation for monitoring</param>
    /// <param name="cacheKey">Cache key for result caching</param>
    /// <param name="operation">The operation to execute</param>
    /// <param name="cacheExpiration">Cache expiration time (optional)</param>
    /// <returns>Operation result with performance optimization</returns>
    public async Task<T> ExecuteWithPerformanceOptimization<T>(
        string operationName,
        string cacheKey,
        Func<Task<T>> operation,
        TimeSpan? cacheExpiration = null)
    {
        using var activity = new Activity($"PerformanceOptimized.{operationName}").Start();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Try memory cache first (fastest)
            if (_memoryCache.TryGetValue(cacheKey, out T? cachedResult) && cachedResult != null)
            {
                stopwatch.Stop();
                _telemetryClient.TrackDependency("MemoryCache", operationName, cacheKey, DateTime.UtcNow.AddMilliseconds(-stopwatch.ElapsedMilliseconds), stopwatch.Elapsed, true);
                _telemetryClient.TrackMetric($"Performance.{operationName}.MemoryCacheHit", 1);
                
                activity?.SetTag("cache.hit", "memory");
                activity?.SetTag("operation.duration_ms", stopwatch.ElapsedMilliseconds);
                
                return cachedResult;
            }

            // Try distributed cache (Redis) next
            var distributedCachedData = await GetFromDistributedCacheAsync<T>(cacheKey);
            if (distributedCachedData != null)
            {
                // Store in memory cache for faster future access
                var memoryCacheExpiration = TimeSpan.FromMinutes(_config.MemoryCache.ExpirationScanFrequencyMinutes);
                _memoryCache.Set(cacheKey, distributedCachedData, memoryCacheExpiration);

                stopwatch.Stop();
                _telemetryClient.TrackDependency("DistributedCache", operationName, cacheKey, DateTime.UtcNow.AddMilliseconds(-stopwatch.ElapsedMilliseconds), stopwatch.Elapsed, true);
                _telemetryClient.TrackMetric($"Performance.{operationName}.DistributedCacheHit", 1);
                
                activity?.SetTag("cache.hit", "distributed");
                activity?.SetTag("operation.duration_ms", stopwatch.ElapsedMilliseconds);
                
                return distributedCachedData;
            }

            // Execute the actual operation
            var result = await ExecuteWithTimeout(operation, _config.MaxResponseTimeMs);
            
            stopwatch.Stop();

            // Cache the result in both layers
            await CacheResultAsync(cacheKey, result, cacheExpiration);

            // Track performance metrics
            _telemetryClient.TrackDependency("GameOperation", operationName, cacheKey, DateTime.UtcNow.AddMilliseconds(-stopwatch.ElapsedMilliseconds), stopwatch.Elapsed, true);
            _telemetryClient.TrackMetric($"Performance.{operationName}.ExecutionTime", stopwatch.ElapsedMilliseconds);
            _telemetryClient.TrackMetric($"Performance.{operationName}.CacheMiss", 1);

            // Warn if operation exceeds child-friendly timeout
            if (stopwatch.ElapsedMilliseconds > _config.MaxResponseTimeMs)
            {
                _telemetryClient.TrackEvent($"Performance.SlowOperation", new Dictionary<string, string>
                {
                    ["OperationName"] = operationName,
                    ["Duration"] = stopwatch.ElapsedMilliseconds.ToString(),
                    ["Region"] = Region,
                    ["TargetDuration"] = _config.MaxResponseTimeMs.ToString()
                });
            }

            activity?.SetTag("cache.hit", "none");
            activity?.SetTag("operation.duration_ms", stopwatch.ElapsedMilliseconds);
            activity?.SetTag("performance.target_ms", _config.MaxResponseTimeMs);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                ["OperationName"] = operationName,
                ["CacheKey"] = cacheKey,
                ["Region"] = Region,
                ["Duration"] = stopwatch.ElapsedMilliseconds.ToString()
            });

            activity?.SetTag("error", true);
            activity?.SetTag("error.message", ex.Message);
            
            throw;
        }
    }

    /// <summary>
    /// Executes an operation with timeout for child-friendly responsiveness
    /// </summary>
    private static async Task<T> ExecuteWithTimeout<T>(Func<Task<T>> operation, int timeoutMs)
    {
        using var cts = new CancellationTokenSource(timeoutMs);
        try
        {
            return await operation().WaitAsync(cts.Token);
        }
        catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
        {
            throw new TimeoutException($"Operation timed out after {timeoutMs}ms. Child-friendly timeout exceeded.");
        }
    }

    /// <summary>
    /// Retrieves data from distributed cache with automatic deserialization
    /// </summary>
    private async Task<T?> GetFromDistributedCacheAsync<T>(string key)
    {
        try
        {
            var cachedData = await _distributedCache.GetStringAsync($"{_config.DistributedCache.KeyPrefix}{key}");
            if (string.IsNullOrEmpty(cachedData))
                return default;

            return System.Text.Json.JsonSerializer.Deserialize<T>(cachedData);
        }
        catch (Exception ex)
        {
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                ["Operation"] = "DistributedCache.Get",
                ["Key"] = key,
                ["Region"] = Region
            });
            return default;
        }
    }

    /// <summary>
    /// Caches result in both memory and distributed cache
    /// </summary>
    private async Task CacheResultAsync<T>(string key, T result, TimeSpan? expiration = null)
    {
        var cacheExpiration = expiration ?? TimeSpan.FromMinutes(_config.DistributedCache.DefaultExpirationMinutes);
        var memoryCacheExpiration = TimeSpan.FromMinutes(Math.Min(_config.MemoryCache.ExpirationScanFrequencyMinutes, (int)cacheExpiration.TotalMinutes));

        try
        {
            // Cache in memory (fastest access)
            _memoryCache.Set(key, result, memoryCacheExpiration);

            // Cache in distributed cache (shared across instances)
            var serializedData = System.Text.Json.JsonSerializer.Serialize(result);
            var distributedCacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheExpiration,
                SlidingExpiration = TimeSpan.FromMinutes(_config.DistributedCache.SlidingExpirationMinutes)
            };

            await _distributedCache.SetStringAsync($"{_config.DistributedCache.KeyPrefix}{key}", serializedData, distributedCacheOptions);
        }
        catch (Exception ex)
        {
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                ["Operation"] = "Cache.Set",
                ["Key"] = key,
                ["Region"] = Region
            });
        }
    }

    /// <summary>
    /// Clears cache for a specific key from both memory and distributed cache
    /// </summary>
    public async Task InvalidateCacheAsync(string key)
    {
        try
        {
            _memoryCache.Remove(key);
            await _distributedCache.RemoveAsync($"{_config.DistributedCache.KeyPrefix}{key}");
            
            _telemetryClient.TrackEvent("Performance.CacheInvalidation", new Dictionary<string, string>
            {
                ["Key"] = key,
                ["Region"] = Region
            });
        }
        catch (Exception ex)
        {
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                ["Operation"] = "Cache.Invalidate",
                ["Key"] = key,
                ["Region"] = Region
            });
        }
    }

    /// <summary>
    /// Gets performance metrics for monitoring dashboard
    /// </summary>
    public PerformanceMetrics GetPerformanceMetrics()
    {
        return new PerformanceMetrics
        {
            Region = Region,
            ConfiguredMaxResponseTime = _config.MaxResponseTimeMs,
            MemoryCacheLimit = _config.MemoryCache.SizeLimit,
            DistributedCacheExpiration = _config.DistributedCache.DefaultExpirationMinutes,
            AIMaxResponseTime = _config.AIAgents.MaxResponseTimeMs,
            UITargetLoadTime = _config.UI.InitialLoadTimeTargetMs,
            Timestamp = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Performance metrics record for monitoring and alerting
/// </summary>
public sealed record PerformanceMetrics
{
    public string Region { get; init; } = string.Empty;
    public int ConfiguredMaxResponseTime { get; init; }
    public long MemoryCacheLimit { get; init; }
    public int DistributedCacheExpiration { get; init; }
    public int AIMaxResponseTime { get; init; }
    public int UITargetLoadTime { get; init; }
    public DateTime Timestamp { get; init; }
}

/// <summary>
/// Base interface for game components with performance optimization
/// </summary>
public interface IGameComponent
{
    // Base interface for game components
}