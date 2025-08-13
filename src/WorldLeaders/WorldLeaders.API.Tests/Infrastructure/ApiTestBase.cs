using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WorldLeaders.Infrastructure.Data;
using WorldLeaders.Shared.Tests.Infrastructure;
using Xunit.Abstractions;

namespace WorldLeaders.API.Tests.Infrastructure;

/// <summary>
/// Base class for API controller testing with WebApplicationFactory
/// Context: Educational game API testing for 12-year-old players
/// Educational Objective: Ensure API endpoints provide safe, educational content
/// </summary>
public abstract class ApiTestBase : EducationalTestBase, IClassFixture<TestWebApplicationFactory>, IDisposable
{
    protected readonly TestWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    protected ApiTestBase(TestWebApplicationFactory factory, ITestOutputHelper output) 
        : base(output)
    {
        Factory = factory;
        Client = CreateTestClient();
    }

    /// <summary>
    /// Create a test HTTP client with proper configuration
    /// </summary>
    /// <returns>Configured HTTP client for testing</returns>
    protected virtual HttpClient CreateTestClient()
    {
        return Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                ConfigureTestServices(services);
            });
            
            builder.UseEnvironment("Testing");
            
        }).CreateClient();
    }

    /// <summary>
    /// Configure services for API testing
    /// Override in derived classes to customize test services
    /// </summary>
    /// <param name="services">Service collection to configure</param>
    protected virtual void ConfigureTestServices(IServiceCollection services)
    {
        // Remove existing database context
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<WorldLeadersDbContext>));
        
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        // Add in-memory database for testing
        services.AddDbContext<WorldLeadersDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDatabase_" + Guid.NewGuid());
            options.EnableSensitiveDataLogging();
        });

        // Configure test logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        // Add any additional test services
        ConfigureAdditionalServices(services);
    }

    /// <summary>
    /// Override to add additional test services
    /// </summary>
    /// <param name="services">Service collection</param>
    protected virtual void ConfigureAdditionalServices(IServiceCollection services)
    {
        // Override in derived classes
    }

    /// <summary>
    /// Get a service from the test application
    /// </summary>
    /// <typeparam name="T">Service type</typeparam>
    /// <returns>Service instance</returns>
    protected T GetService<T>() where T : notnull
    {
        using var scope = Factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Seed test data for educational scenarios
    /// </summary>
    /// <param name="dbContext">Database context</param>
    protected virtual async Task SeedTestDataAsync(WorldLeadersDbContext dbContext)
    {
        // Override in derived classes to seed specific test data
        await Task.CompletedTask;
    }

    /// <summary>
    /// Validate API response for child safety
    /// </summary>
    /// <param name="response">HTTP response to validate</param>
    /// <param name="endpoint">API endpoint name</param>
    protected async Task ValidateApiResponseChildSafety(HttpResponseMessage response, string endpoint)
    {
        Assert.True(response.IsSuccessStatusCode, 
            $"API endpoint {endpoint} should return success status for educational game");

        var content = await response.Content.ReadAsStringAsync();
        
        if (!string.IsNullOrEmpty(content))
        {
            ValidateChildSafeContent(content, $"API Response: {endpoint}");
        }

        Output.WriteLine($"✅ API response validation passed for endpoint: {endpoint}");
    }

    /// <summary>
    /// Create test database context
    /// </summary>
    /// <returns>Test database context</returns>
    protected WorldLeadersDbContext CreateTestDbContext()
    {
        var options = new DbContextOptionsBuilder<WorldLeadersDbContext>()
            .UseInMemoryDatabase("TestDatabase_" + Guid.NewGuid())
            .EnableSensitiveDataLogging()
            .Options;

        return new WorldLeadersDbContext(options);
    }

    /// <summary>
    /// Execute test with database context
    /// </summary>
    /// <param name="test">Test action</param>
    protected async Task ExecuteWithDbContextAsync(Func<WorldLeadersDbContext, Task> test)
    {
        using var context = CreateTestDbContext();
        await context.Database.EnsureCreatedAsync();
        await SeedTestDataAsync(context);
        await test(context);
    }

    /// <summary>
    /// Execute test with database context and return result
    /// </summary>
    /// <typeparam name="T">Result type</typeparam>
    /// <param name="test">Test function</param>
    /// <returns>Test result</returns>
    protected async Task<T> ExecuteWithDbContextAsync<T>(Func<WorldLeadersDbContext, Task<T>> test)
    {
        using var context = CreateTestDbContext();
        await context.Database.EnsureCreatedAsync();
        await SeedTestDataAsync(context);
        return await test(context);
    }

    public void Dispose()
    {
        Client?.Dispose();
        GC.SuppressFinalize(this);
    }
}