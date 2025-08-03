using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace WorldLeaders.Infrastructure.Data;

/// <summary>
/// Design-time factory for creating WorldLeadersDbContext during migrations
/// Required for Entity Framework CLI tools to work properly
/// </summary>
public class WorldLeadersDbContextFactory : IDesignTimeDbContextFactory<WorldLeadersDbContext>
{
    public WorldLeadersDbContext CreateDbContext(string[] args)
    {
        // Build configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Get connection string with fallback
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=worldleadersdb;Username=postgres;Password=postgres;";

        // Create DbContext options
        var optionsBuilder = new DbContextOptionsBuilder<WorldLeadersDbContext>();
        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null);
        });

        return new WorldLeadersDbContext(optionsBuilder.Options);
    }
}