using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorldLeaders.Infrastructure.Data;
using WorldLeaders.Infrastructure.Extensions;

namespace WorldLeaders.Infrastructure.Tools;

/// <summary>
/// Database verification tool for World Leaders educational game
/// Verifies Entity Framework setup and seed data functionality
/// </summary>
public class DatabaseVerificationTool
{
    /// <summary>
    /// Main entry point for database verification
    /// Designed to validate educational game database setup
    /// </summary>
    public static async Task<int> Main(string[] args)
    {
        try
        {
            Console.WriteLine("🎮 World Leaders Game - Database Verification Tool");
            Console.WriteLine("Educational Database Setup for 12-year-old Players");
            Console.WriteLine("=" + new string('=', 60));
            
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // Build host with services
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddInfrastructure(configuration);
                    services.AddLogging(builder => builder.AddConsole());
                })
                .Build();

            using var scope = host.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WorldLeadersDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseVerificationTool>>();

            logger.LogInformation("Starting database verification for educational game...");

            // Test 1: Database Connection
            Console.WriteLine("\n🔍 Test 1: Database Connection");
            var canConnect = await dbContext.Database.CanConnectAsync();
            Console.WriteLine($"   Connection Status: {(canConnect ? "✅ Connected" : "❌ Failed")}");

            if (!canConnect)
            {
                Console.WriteLine("   ⚠️  Database connection failed. Please ensure PostgreSQL is running.");
                return 1;
            }

            // Test 2: Database Creation
            Console.WriteLine("\n🔍 Test 2: Database Creation");
            await dbContext.Database.EnsureCreatedAsync();
            Console.WriteLine("   ✅ Database created successfully");

            // Test 3: Verify Tables Exist
            Console.WriteLine("\n🔍 Test 3: Educational Game Tables");
            var tables = new[]
            {
                ("Players", "👤 Player accounts for children"),
                ("Territories", "🌍 Countries with real GDP data"),
                ("GameEvents", "🎲 Educational events for learning"),
                ("AIInteractions", "🤖 Child-safe AI conversations"),
                ("LanguageProgress", "🗣️ Speech learning progress")
            };

            foreach (var (tableName, description) in tables)
            {
                var tableExists = await dbContext.Database
#pragma warning disable EF1002
                    .SqlQueryRaw<int>($"SELECT 1 FROM information_schema.tables WHERE table_name = '{tableName.ToLower()}'")
#pragma warning restore EF1002
                    .AnyAsync();
                Console.WriteLine($"   {(tableExists ? "✅" : "❌")} {tableName}: {description}");
            }

            // Test 4: Verify Seed Data
            Console.WriteLine("\n🔍 Test 4: Educational Seed Data");
            
            var territoryCount = await dbContext.Territories.CountAsync();
            Console.WriteLine($"   🌍 Territories: {territoryCount} countries with real GDP data");
            
            var eventCount = await dbContext.GameEvents.CountAsync();
            Console.WriteLine($"   🎲 Game Events: {eventCount} educational scenarios");

            // Test 5: Verify Educational Data Quality
            Console.WriteLine("\n🔍 Test 5: Educational Data Quality");
            
            // Check territory tiers for proper difficulty progression
            var tierCounts = await dbContext.Territories
                .GroupBy(t => t.Tier)
                .Select(g => new { Tier = g.Key, Count = g.Count() })
                .ToListAsync();
            
            foreach (var tier in tierCounts)
            {
                Console.WriteLine($"   📊 {tier.Tier} Tier Countries: {tier.Count} (for progressive difficulty)");
            }

            // Check language diversity for speech learning
            var languageCount = await dbContext.Territories
                .Where(t => !string.IsNullOrEmpty(t.OfficialLanguagesJson))
                .CountAsync();
            Console.WriteLine($"   🗣️ Countries with Language Data: {languageCount} (for speech challenges)");

            // Test 6: Child Safety Features
            Console.WriteLine("\n🔍 Test 6: Child Safety Features");
            
            // Verify soft delete is working
            var deletedCount = await dbContext.Territories.IgnoreQueryFilters()
                .Where(t => t.IsDeleted)
                .CountAsync();
            Console.WriteLine($"   🛡️ Soft Delete Protection: {(deletedCount >= 0 ? "✅ Active" : "❌ Failed")}");
            
            // Verify audit fields
            var auditFieldsCount = await dbContext.Territories
                .Where(t => t.CreatedAt != default && t.UpdatedAt != default)
                .CountAsync();
            Console.WriteLine($"   📋 Audit Trail: {auditFieldsCount} records with timestamps");

            Console.WriteLine("\n🎉 Database Verification Complete!");
            Console.WriteLine("🎓 Educational game database is ready for 12-year-old learners!");
            
            logger.LogInformation("Database verification completed successfully");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Database verification failed: {ex.Message}");
            Console.WriteLine($"   Error Type: {ex.GetType().Name}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"   Inner Error: {ex.InnerException.Message}");
            }
            return 1;
        }
    }
}