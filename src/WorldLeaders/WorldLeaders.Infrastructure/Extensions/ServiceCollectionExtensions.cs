using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
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
    /// Smart database provider selection based on environment and available connection strings
    /// Priority: Azure SQL > PostgreSQL > SQLite-Temp (Azure) > SQLite (Local) > InMemory (Fallback)
    /// </summary>
    private static string DetermineBestDatabaseProvider(IConfiguration configuration)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLowerInvariant();
        var isAzure = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));
        
        // Manual override from configuration (highest priority)
        var configuredProvider = configuration.GetValue<string>("Database:Provider");
        if (!string.IsNullOrEmpty(configuredProvider) && configuredProvider.ToLowerInvariant() != "litedb")
        {
            Console.WriteLine($"[DatabaseSelection] Using configured provider: {configuredProvider}");
            return configuredProvider;
        }

        // Check for Azure SQL connection string
        var azureSqlConnection = configuration.GetConnectionString("AzureSQL") ?? 
                               configuration.GetConnectionString("DefaultConnection_Production");
        if (!string.IsNullOrEmpty(azureSqlConnection) && azureSqlConnection.Contains("database.windows.net"))
        {
            Console.WriteLine("[DatabaseSelection] Found Azure SQL connection string");
            return "AzureSQL";
        }

        // Check for PostgreSQL connection string
        var postgresConnection = configuration.GetConnectionString("DefaultConnection") ?? 
                               configuration.GetConnectionString("PostgreSQL");
        if (!string.IsNullOrEmpty(postgresConnection) && 
            (postgresConnection.Contains("Host=") || postgresConnection.Contains("Server=")) &&
            !postgresConnection.Contains("localhost"))
        {
            Console.WriteLine("[DatabaseSelection] Found PostgreSQL connection string");
            return "PostgreSQL";
        }

        // Azure App Service specific logic
        if (isAzure)
        {
            Console.WriteLine("[DatabaseSelection] Running in Azure App Service - using SQLite in temp directory");
            return "SQLite-Temp";
        }

        // Local development
        if (environment == "development")
        {
            Console.WriteLine("[DatabaseSelection] Development environment - using SQLite");
            return "SQLite";
        }

        // Test environment
        if (environment == "test" || environment == "testing")
        {
            Console.WriteLine("[DatabaseSelection] Test environment - using InMemory");
            return "InMemory";
        }

        // Fallback
        Console.WriteLine("[DatabaseSelection] No suitable database found - falling back to InMemory");
        return "InMemory";
    }

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
        // Smart database provider selection based on environment and available connection strings
        var databaseProvider = DetermineBestDatabaseProvider(configuration);
        
        Console.WriteLine($"[Infrastructure] Selected database provider: '{databaseProvider}' for environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");

        // Add Entity Framework with appropriate provider
        services.AddDbContext<WorldLeadersDbContext>(options =>
        {
            switch (databaseProvider.ToLowerInvariant())
            {
                case "azuresql":
                    Console.WriteLine("[Infrastructure] Configuring Azure SQL Database");
                    ConfigureAzureSQL(options, configuration);
                    break;

                case "postgresql":
                case "postgres":
                    Console.WriteLine("[Infrastructure] Configuring PostgreSQL database");
                    ConfigurePostgreSQL(options, configuration);
                    break;

                case "sqlite":
                    Console.WriteLine("[Infrastructure] Configuring SQLite database");
                    ConfigureSQLite(options, configuration);
                    break;

                case "sqlite-temp":
                    Console.WriteLine("[Infrastructure] Configuring SQLite database in temp directory (Azure-compatible)");
                    ConfigureSQLiteTemp(options, configuration);
                    break;

                case "inmemory":
                default:
                    Console.WriteLine("[Infrastructure] Configuring InMemory database (fallback)");
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

        // Configure LiteDB options
        services.Configure<LiteDbOptions>(configuration.GetSection(LiteDbOptions.SectionName));

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
    /// Configures Azure SQL Database provider (requires Microsoft.EntityFrameworkCore.SqlServer package)
    /// </summary>
    private static void ConfigureAzureSQL(DbContextOptionsBuilder options, IConfiguration configuration)
    {
        // TODO: Add Microsoft.EntityFrameworkCore.SqlServer package reference
        // For now, fall back to PostgreSQL for cost effectiveness
        Console.WriteLine("[AzureSQL] Package not available, falling back to PostgreSQL");
        ConfigurePostgreSQL(options, configuration);
    }

    /// <summary>
    /// Configures SQLite database provider for Azure App Service (using temp directory)
    /// Enhanced with robust directory detection and permission validation
    /// </summary>
    private static void ConfigureSQLiteTemp(DbContextOptionsBuilder options, IConfiguration configuration)
    {
        // Enhanced Azure temp directory detection with multiple fallbacks
        var tempPath = GetAzureTempDirectory();
        var dbPath = Path.Combine(tempPath, "worldleaders.db");
        var connectionString = $"Data Source={dbPath}";

        Console.WriteLine($"[SQLiteTemp] Using database path: {dbPath}");
        Console.WriteLine($"[SQLiteTemp] Directory exists: {Directory.Exists(tempPath)}");
        Console.WriteLine($"[SQLiteTemp] Directory writable: {IsDirectoryWritable(tempPath)}");

        options.UseSqlite(connectionString, sqliteOptions =>
        {
            sqliteOptions.CommandTimeout(30);
        });
    }

    /// <summary>
    /// Gets the best available temporary directory for Azure App Service
    /// Tries multiple Azure-specific paths in order of preference
    /// </summary>
    private static string GetAzureTempDirectory()
    {
        // Azure App Service temp directories (in order of preference)
        var azureTempPaths = new[]
        {
            Environment.GetEnvironmentVariable("TEMP"),           // Primary Azure temp
            Environment.GetEnvironmentVariable("TMP"),            // Alternative Azure temp
            @"D:\local\Temp",                                     // Azure Windows temp
            @"D:\home\data\tmp",                                  // Azure persistent temp
            "/tmp",                                               // Linux fallback
            Path.GetTempPath()                                    // System fallback
        };

        foreach (var path in azureTempPaths)
        {
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path) && IsDirectoryWritable(path))
            {
                Console.WriteLine($"[AzureTemp] Using temp directory: {path}");
                return path;
            }
        }

        // Last resort - create temp directory in current directory
        var fallbackTemp = Path.Combine(Directory.GetCurrentDirectory(), "temp");
        try
        {
            Directory.CreateDirectory(fallbackTemp);
            Console.WriteLine($"[AzureTemp] Created fallback temp directory: {fallbackTemp}");
            return fallbackTemp;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AzureTemp] Failed to create fallback directory: {ex.Message}");
            throw new InvalidOperationException("No writable directory found for SQLite database in Azure App Service", ex);
        }
    }

    /// <summary>
    /// Tests if a directory is writable by attempting to create and delete a test file
    /// </summary>
    private static bool IsDirectoryWritable(string dirPath)
    {
        try
        {
            var testFile = Path.Combine(dirPath, $"test_{Guid.NewGuid()}.tmp");
            File.WriteAllText(testFile, "test");
            File.Delete(testFile);
            return true;
        }
        catch
        {
            return false;
        }
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
        
        // Create demo user for testing purposes
        await CreateDemoUserAsync(scope.ServiceProvider);
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
                options.ConfigurationOptions.SyncTimeout = 1500; // 1.5 seconds for child attention span (consistent with Program.cs)
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

    /// <summary>
    /// Creates demo user for testing purposes if it doesn't already exist
    /// Educational Context: Allows users to test the game without registration
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <returns>Async task</returns>
    private static async Task CreateDemoUserAsync(IServiceProvider serviceProvider)
    {
        try
        {
            var userManager = serviceProvider.GetRequiredService<IUserManagerService>();
            var context = serviceProvider.GetRequiredService<WorldLeadersDbContext>();
            var logger = serviceProvider.GetRequiredService<ILogger<object>>();

            // Check if demo user already exists
            var existingUser = await userManager.GetUserByUsernameOrEmailAsync("student123");
            if (existingUser != null)
            {
                logger.LogInformation("Demo user 'student123' already exists");
                return;
            }

            // Create demo user directly in the database for simplicity
            var demoUser = new WorldLeaders.Shared.Models.ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "student123",
                NormalizedUserName = "STUDENT123",
                Email = "demo@worldleadersgame.co.uk",
                NormalizedEmail = "DEMO@WORLDLEADERSGAME.CO.UK",
                DisplayName = "Demo Student",
                DateOfBirth = DateTime.UtcNow.AddYears(-12), // 12 years old
                ParentalEmail = "parent@worldleadersgame.co.uk",
                HasParentalConsent = true,
                Role = WorldLeaders.Shared.Enums.UserRole.Student,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true, // Demo account is pre-confirmed
                PasswordHash = "", // Will be set below
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            // Set password using ASP.NET Core Identity password hasher
            var passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher<WorldLeaders.Shared.Models.ApplicationUser>>();
            demoUser.PasswordHash = passwordHasher.HashPassword(demoUser, "Education123!");

            // Add to database
            context.Users.Add(demoUser);
            await context.SaveChangesAsync();
            
            logger.LogInformation("Demo user 'student123' created successfully for testing purposes");
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<object>>();
            logger.LogError(ex, "Error creating demo user: {Message}", ex.Message);
        }
    }

    /// <summary>
    /// Creates demo user for testing purposes if it doesn't already exist (public method)
    /// Educational Context: Allows users to test the game without registration
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <returns>Async task</returns>
    public static async Task CreateDemoUserIfNeededAsync(this IServiceProvider serviceProvider)
    {
        await CreateDemoUserAsync(serviceProvider);
    }
}