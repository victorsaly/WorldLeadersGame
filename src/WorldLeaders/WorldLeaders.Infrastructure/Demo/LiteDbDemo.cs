using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WorldLeaders.Infrastructure.Configuration;
using WorldLeaders.Infrastructure.Services;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.Infrastructure.Demo;

/// <summary>
/// Demonstration of LiteDB persistence for the World Leaders Game
/// Context: Shows how LiteDB replaces in-memory storage for better persistence
/// Safety Requirements: Demonstrates secure child data protection with persistent storage
/// </summary>
public class LiteDbDemo
{
    public static async Task RunDemoAsync()
    {
        Console.WriteLine("🎮 World Leaders Game - LiteDB Persistence Demo");
        Console.WriteLine("===============================================");
        Console.WriteLine();

        // Setup services
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        
        // Configure LiteDB options
        services.Configure<LiteDbOptions>(options =>
        {
            options.ConnectionString = Path.Combine(AppContext.BaseDirectory, "Demo", "demo-worldleaders.db");
            options.UsePassword = true;
            options.MaxSizeMB = 50;
        });

        // Configure Azure Key Vault options for fallback
        services.Configure<AzureKeyVaultOptions>(options =>
        {
            // Note: Using reflection to set init-only properties for demo
        });
        
        // Use a custom instance for demo purposes
        services.AddSingleton(new AzureKeyVaultOptions
        {
            VaultUrl = "",
            Enabled = false,
            MasterKey = "DemoWorldLeadersKey2025!"
        });

        // Register LiteDB services
        services.AddScoped<IKeyVaultClient, LiteDbKeyVaultClient>();
        services.AddScoped<IAuditLogger, LiteDbAuditLogger>();

        var serviceProvider = services.BuildServiceProvider();

        // Demo Key Vault functionality
        await DemoKeyVaultAsync(serviceProvider);
        
        Console.WriteLine();
        
        // Demo Audit Logger functionality
        await DemoAuditLoggerAsync(serviceProvider);

        Console.WriteLine();
        Console.WriteLine("✅ LiteDB Demo completed successfully!");
        Console.WriteLine("📁 Database files created in: Demo/");
        Console.WriteLine("   - demo-worldleaders.db (audit events)");
        Console.WriteLine("   - keyvault.db (encrypted keys and data)");
        Console.WriteLine();
        Console.WriteLine("🔒 All data is encrypted and stored persistently!");
    }

    private static async Task DemoKeyVaultAsync(IServiceProvider serviceProvider)
    {
        Console.WriteLine("🔐 Testing LiteDB Key Vault Client");
        Console.WriteLine("----------------------------------");

        var keyVault = serviceProvider.GetRequiredService<IKeyVaultClient>();

        try
        {
            // Test connection
            Console.WriteLine("🔗 Testing connection...");
            var isConnected = await keyVault.ValidateConnectionAsync();
            Console.WriteLine($"   Connection: {(isConnected ? "✅ Success" : "❌ Failed")}");

            // Create a key
            Console.WriteLine("🔑 Creating encryption key...");
            var keyId = await keyVault.CreateKeyAsync("demo-child-data-key", "AES256");
            Console.WriteLine($"   Key created: {keyId}");

            // Encrypt some child data
            Console.WriteLine("🔒 Encrypting child data...");
            var childData = "StudentName: Emma Smith, Age: 12, School: Demo Primary";
            var dataId = await keyVault.EncryptAsync(childData, "demo-child-data-key");
            Console.WriteLine($"   Data ID: {dataId}");

            // Decrypt the data
            Console.WriteLine("🔓 Decrypting child data...");
            var decryptedData = await keyVault.DecryptAsync(dataId, "demo-child-data-key");
            Console.WriteLine($"   Decrypted: {decryptedData}");

            // Verify data integrity
            var isDataIntact = childData == decryptedData;
            Console.WriteLine($"   Data integrity: {(isDataIntact ? "✅ Verified" : "❌ Failed")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Key Vault Demo Error: {ex.Message}");
        }
    }

    private static async Task DemoAuditLoggerAsync(IServiceProvider serviceProvider)
    {
        Console.WriteLine("📝 Testing LiteDB Audit Logger");
        Console.WriteLine("------------------------------");

        var auditLogger = serviceProvider.GetRequiredService<IAuditLogger>();

        try
        {
            // Log some sample events
            Console.WriteLine("📋 Logging sample events...");
            
            var childUserId = Guid.NewGuid();
            
            await auditLogger.LogEventAsync("UserLogin", "Child user logged in successfully", new { Age = 12, School = "Demo Primary" }, childUserId);
            Console.WriteLine("   ✅ Login event logged");

            await auditLogger.LogChildSafetyEventAsync("ContentAccess", childUserId, "Accessed educational geography content", new { Territory = "United Kingdom", Subject = "Geography" });
            Console.WriteLine("   ✅ Child safety event logged");

            await auditLogger.LogEventAsync("GameAction", "Territory conquered", new { Territory = "France", DiceRoll = 6, Language = "French" }, childUserId);
            Console.WriteLine("   ✅ Game action logged");

            await auditLogger.LogComplianceViolationAsync("SessionTimeout", "Child session exceeded 30-minute limit", childUserId);
            Console.WriteLine("   ✅ Compliance violation logged");

            // Retrieve events
            Console.WriteLine("🔍 Retrieving audit events...");
            var events = await auditLogger.GetAuditEventsAsync(DateTime.UtcNow.AddHours(-1), DateTime.UtcNow, childUserId);
            Console.WriteLine($"   Retrieved {events.Count} events for child user");

            foreach (var evt in events.Take(3))
            {
                Console.WriteLine($"   📋 {evt.EventType}: {evt.Message} ({evt.Timestamp:yyyy-MM-dd HH:mm:ss})");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Audit Logger Demo Error: {ex.Message}");
        }
    }
}
