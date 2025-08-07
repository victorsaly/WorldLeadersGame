using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorldLeaders.Infrastructure.Configuration;
using WorldLeaders.Infrastructure.Data;
using WorldLeaders.Infrastructure.Extensions;
using WorldLeaders.Infrastructure.Services;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.Infrastructure.Extensions;

/// <summary>
/// Dependency injection extensions for Infrastructure layer
/// Configures Entity Framework with multiple database providers for educational game data
/// Enhanced with authentication and child safety services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Entity Framework infrastructure services to the DI container
    /// Configured for educational game with child safety considerations
    /// Supports PostgreSQL, SQLite, and In-Memory databases for different environments
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Determine database provider from configuration
        var databaseProvider = configuration.GetValue<string>("Database:Provider") ?? "InMemory";

        // Add Entity Framework with appropriate provider
        services.AddDbContext<WorldLeadersDbContext>(options =>
        {
            switch (databaseProvider.ToLowerInvariant())
            {
                case "postgresql":
                case "postgres":
                    ConfigurePostgreSQL(options, configuration);
                    break;

                case "sqlite":
                    ConfigureSQLite(options, configuration);
                    break;

                case "inmemory":
                default:
                    ConfigureInMemory(options, configuration);
                    break;
            }

            // Enable sensitive data logging in development only
            if (configuration.GetValue<bool>("Logging:EnableSensitiveDataLogging"))
            {
                options.EnableSensitiveDataLogging();
            }

            // Enable detailed errors in development
            if (configuration.GetValue<bool>("Logging:EnableDetailedErrors"))
            {
                options.EnableDetailedErrors();
            }
        });

        // Add educational authentication services
        services.AddEducationalAuthentication(configuration);

        // Add game engine services
        services.AddScoped<IGameEngine, GameEngine>();
        services.AddScoped<IDiceService, DiceService>();
        services.AddScoped<IPlayerService, PlayerService>();
        
        // Add territory management services for educational geography learning
        services.AddScoped<ITerritoryService, TerritoryService>();
        services.AddHttpClient<IExternalDataService, ExternalDataService>();

        // Add AI agent services for child-safe educational interactions
        services.AddScoped<IContentModerationService, ContentModerationService>();
        
        // Add speech recognition services for pronunciation learning
        services.AddScoped<ISpeechRecognitionService, SpeechRecognitionService>();

        // Configure Azure AI options
        services.Configure<AzureAIOptions>(configuration.GetSection(AzureAIOptions.SectionName));
        services.Configure<ContentModeratorOptions>(configuration.GetSection(ContentModeratorOptions.SectionName));
        services.Configure<SpeechServicesOptions>(configuration.GetSection(SpeechServicesOptions.SectionName));
        
        // Configure performance optimization settings
        services.Configure<PerformanceConfig>(configuration.GetSection(PerformanceConfig.SectionName));
        
        // Add performance optimization services
        services.AddPerformanceOptimization(configuration);

        // Register AI services - Cloud service takes priority, falls back to local service
        var azureEndpoint = configuration.GetValue<string>("AzureOpenAI:Endpoint");
        var azureApiKey = configuration.GetValue<string>("AzureOpenAI:ApiKey");

        if (!string.IsNullOrEmpty(azureEndpoint) && !string.IsNullOrEmpty(azureApiKey))
        {
            // Use cloud AI service when Azure credentials are configured
            services.AddScoped<IAIAgentService, CloudAIAgentService>();
        }
        else
        {
            // Fall back to local mock service for development
            services.AddScoped<IAIAgentService, AIAgentService>();
        }

        return services;
    }

    /// <summary>
    /// Configures PostgreSQL database provider
    /// </summary>
    private static void ConfigurePostgreSQL(DbContextOptionsBuilder options, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection string not found for PostgreSQL");

        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            // Configure PostgreSQL specific options
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null);

            // Set command timeout for large operations
            npgsqlOptions.CommandTimeout(30);
        });
    }

    /// <summary>
    /// Configures SQLite database provider for local development
    /// </summary>
    private static void ConfigureSQLite(DbContextOptionsBuilder options, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SQLiteConnection")
            ?? "Data Source=worldleaders.db";

        options.UseSqlite(connectionString, sqliteOptions =>
        {
            // Set command timeout
            sqliteOptions.CommandTimeout(30);
        });
    }

    /// <summary>
    /// Configures In-Memory database provider for testing and rapid development
    /// </summary>
    private static void ConfigureInMemory(DbContextOptionsBuilder options, IConfiguration configuration)
    {
        var databaseName = configuration.GetValue<string>("Database:InMemoryName") ?? "WorldLeadersInMemory";
        options.UseInMemoryDatabase(databaseName);
    }

    /// <summary>
    /// Ensures the database is created and migrations are applied
    /// Use with caution - only for development and initial setup
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <returns>Async task</returns>
    public static async Task EnsureDatabaseCreatedAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WorldLeadersDbContext>();

        // Ensure database is created with all migrations applied
        await context.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Applies pending migrations to the database
    /// Safer alternative to EnsureCreated for production
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <returns>Async task</returns>
    public static async Task MigrateDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WorldLeadersDbContext>();

        // Apply any pending migrations
        await context.Database.MigrateAsync();
    }

    /// <summary>
    /// Adds performance optimization services for 1000+ concurrent users
    /// Includes multi-layer caching, Application Insights monitoring, and performance metrics
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddPerformanceOptimization(this IServiceCollection services, IConfiguration configuration)
    {
        // Add memory cache first (required for PerformanceOptimizedGameComponent)
        services.AddMemoryCache(options =>
        {
            var performanceConfig = configuration.GetSection(PerformanceConfig.SectionName).Get<PerformanceConfig>() 
                ?? PerformanceConfig.UKEducationalDefaults;
            
            options.SizeLimit = performanceConfig.MemoryCache.SizeLimit;
            options.CompactionPercentage = performanceConfig.MemoryCache.CompactionPercentage;
            options.ExpirationScanFrequency = TimeSpan.FromMinutes(performanceConfig.MemoryCache.ExpirationScanFrequencyMinutes);
        });

        // Add Application Insights for performance monitoring
        var applicationInsightsConnectionString = configuration.GetValue<string>("ApplicationInsights:ConnectionString");
        if (!string.IsNullOrEmpty(applicationInsightsConnectionString))
        {
            services.AddApplicationInsightsTelemetry(options =>
            {
                options.ConnectionString = applicationInsightsConnectionString;
                options.EnableAdaptiveSampling = true;
                options.EnableQuickPulseMetricStream = true;
                options.EnablePerformanceCounterCollectionModule = true;
            });
        }

        // Add Redis distributed cache for multi-instance scaling
        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "WorldLeadersGame";
                
                // Performance optimization for child-friendly responsiveness
                options.ConfigurationOptions = StackExchange.Redis.ConfigurationOptions.Parse(redisConnectionString);
                options.ConfigurationOptions.ConnectTimeout = 5000; // 5 seconds
                options.ConfigurationOptions.SyncTimeout = 2000; // 2 seconds for child attention span
                options.ConfigurationOptions.AbortOnConnectFail = false; // Graceful degradation
            });
        }
        else
        {
            // Fallback to in-memory cache only (no distributed cache)
            // Memory cache already added above
        }

        // Register performance optimization component
        services.AddScoped<PerformanceOptimizedGameComponent>();

        return services;
    }
}