using System.ComponentModel.DataAnnotations;

namespace WorldLeaders.Infrastructure.Configuration;

/// <summary>
/// Performance configuration for educational platform optimization
/// Designed for 1000+ concurrent users with child-friendly performance requirements
/// </summary>
public sealed record PerformanceConfig
{
    public const string SectionName = "Performance";

    /// <summary>
    /// Target region for optimization (e.g., "UK South")
    /// </summary>
    [Required]
    public string Region { get; init; } = "UK South";

    /// <summary>
    /// Maximum response time for child-friendly interactions (milliseconds)
    /// </summary>
    [Range(500, 5000)]
    public int MaxResponseTimeMs { get; init; } = 2000;

    /// <summary>
    /// Memory cache configuration
    /// </summary>
    public MemoryCacheConfig MemoryCache { get; init; } = new();

    /// <summary>
    /// Redis distributed cache configuration
    /// </summary>
    public DistributedCacheConfig DistributedCache { get; init; } = new();

    /// <summary>
    /// AI agent performance configuration
    /// </summary>
    public AIPerformanceConfig AIAgents { get; init; } = new();

    /// <summary>
    /// Game state synchronization configuration
    /// </summary>
    public GameSyncConfig GameSync { get; init; } = new();

    /// <summary>
    /// Child-friendly UI performance settings
    /// </summary>
    public UIPerformanceConfig UI { get; init; } = new();

    /// <summary>
    /// Default performance configuration optimized for UK educational deployment
    /// </summary>
    public static PerformanceConfig UKEducationalDefaults => new()
    {
        Region = "UK South",
        MaxResponseTimeMs = 1500, // Child attention span optimized
        MemoryCache = new MemoryCacheConfig
        {
            SizeLimit = 100_000, // 100K items for 1000+ users
            CompactionPercentage = 0.1,
            ExpirationScanFrequencyMinutes = 5
        },
        DistributedCache = new DistributedCacheConfig
        {
            DefaultExpirationMinutes = 30, // Child session length
            SlidingExpirationMinutes = 15,
            EnableCompression = true
        },
        AIAgents = new AIPerformanceConfig
        {
            MaxResponseTimeMs = 1000, // Fast AI responses for engagement
            CacheTimeoutMinutes = 15,
            MaxConcurrentRequests = 50
        },
        GameSync = new GameSyncConfig
        {
            HeartbeatIntervalMs = 5000,
            MaxRetryAttempts = 3,
            BatchUpdateIntervalMs = 1000
        },
        UI = new UIPerformanceConfig
        {
            InitialLoadTimeTargetMs = 1500,
            InteractionResponseTargetMs = 300,
            EnablePreloading = true,
            OptimizeForTouch = true // Child-friendly touch interfaces
        }
    };
}

/// <summary>
/// Memory cache performance configuration
/// </summary>
public sealed record MemoryCacheConfig
{
    /// <summary>
    /// Maximum number of cached items
    /// </summary>
    public long SizeLimit { get; init; } = 50_000;

    /// <summary>
    /// Percentage of cache to remove when size limit is reached
    /// </summary>
    [Range(0.05, 0.5)]
    public double CompactionPercentage { get; init; } = 0.1;

    /// <summary>
    /// How often to scan for expired entries (minutes)
    /// </summary>
    [Range(1, 60)]
    public int ExpirationScanFrequencyMinutes { get; init; } = 5;
}

/// <summary>
/// Distributed cache (Redis) performance configuration
/// </summary>
public sealed record DistributedCacheConfig
{
    /// <summary>
    /// Default cache expiration time (minutes)
    /// </summary>
    [Range(5, 1440)] // 5 minutes to 24 hours
    public int DefaultExpirationMinutes { get; init; } = 30;

    /// <summary>
    /// Sliding expiration time (minutes)
    /// </summary>
    [Range(1, 60)]
    public int SlidingExpirationMinutes { get; init; } = 15;

    /// <summary>
    /// Enable compression for cached data
    /// </summary>
    public bool EnableCompression { get; init; } = true;

    /// <summary>
    /// Key prefix for UK educational deployment
    /// </summary>
    public string KeyPrefix { get; init; } = "wlg:uk:";
}

/// <summary>
/// AI agent performance configuration
/// </summary>
public sealed record AIPerformanceConfig
{
    /// <summary>
    /// Maximum AI response time (milliseconds)
    /// </summary>
    [Range(500, 5000)]
    public int MaxResponseTimeMs { get; init; } = 1000;

    /// <summary>
    /// Cache timeout for AI responses (minutes)
    /// </summary>
    [Range(5, 120)]
    public int CacheTimeoutMinutes { get; init; } = 15;

    /// <summary>
    /// Maximum concurrent AI requests
    /// </summary>
    [Range(10, 200)]
    public int MaxConcurrentRequests { get; init; } = 50;

    /// <summary>
    /// Enable response caching for similar queries
    /// </summary>
    public bool EnableResponseCaching { get; init; } = true;
}

/// <summary>
/// Game state synchronization performance configuration
/// </summary>
public sealed record GameSyncConfig
{
    /// <summary>
    /// SignalR heartbeat interval (milliseconds)
    /// </summary>
    [Range(1000, 30000)]
    public int HeartbeatIntervalMs { get; init; } = 5000;

    /// <summary>
    /// Maximum retry attempts for failed updates
    /// </summary>
    [Range(1, 10)]
    public int MaxRetryAttempts { get; init; } = 3;

    /// <summary>
    /// Batch update interval for efficiency (milliseconds)
    /// </summary>
    [Range(500, 5000)]
    public int BatchUpdateIntervalMs { get; init; } = 1000;
}

/// <summary>
/// Child-friendly UI performance configuration
/// </summary>
public sealed record UIPerformanceConfig
{
    /// <summary>
    /// Target initial page load time (milliseconds)
    /// </summary>
    [Range(500, 5000)]
    public int InitialLoadTimeTargetMs { get; init; } = 1500;

    /// <summary>
    /// Target interaction response time (milliseconds)
    /// </summary>
    [Range(100, 1000)]
    public int InteractionResponseTargetMs { get; init; } = 300;

    /// <summary>
    /// Enable preloading of critical resources
    /// </summary>
    public bool EnablePreloading { get; init; } = true;

    /// <summary>
    /// Optimize for touch interfaces (tablets, phones)
    /// </summary>
    public bool OptimizeForTouch { get; init; } = true;

    /// <summary>
    /// Minimum button size for child-friendly interactions (pixels)
    /// </summary>
    [Range(32, 80)]
    public int MinButtonSizePx { get; init; } = 48;
}