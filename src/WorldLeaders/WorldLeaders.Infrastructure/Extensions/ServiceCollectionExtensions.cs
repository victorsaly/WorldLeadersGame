using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorldLeaders.Infrastructure.Data;
using WorldLeaders.Infrastructure.Services;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.Infrastructure.Extensions;

/// <summary>
/// Dependency injection extensions for Infrastructure layer
/// Configures Entity Framework with PostgreSQL for educational game data
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Entity Framework infrastructure services to the DI container
    /// Configured for educational game with child safety considerations
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Entity Framework with PostgreSQL
        services.AddDbContext<WorldLeadersDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("DefaultConnection string not found");
            
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

        // Add game engine services
        services.AddScoped<IGameEngine, GameEngine>();
        services.AddScoped<IDiceService, DiceService>();
        services.AddScoped<IPlayerService, PlayerService>();

        return services;
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
}