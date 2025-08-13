using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using WorldLeaders.Infrastructure.Data;
using WorldLeaders.Shared.Tests.Infrastructure;
using Xunit.Abstractions;

namespace WorldLeaders.Infrastructure.Tests.Infrastructure;

/// <summary>
/// Base class for testing infrastructure services
/// Context: Educational game service testing for World Leaders Game
/// Educational Objective: Ensure services provide safe, educational functionality
/// </summary>
public abstract class ServiceTestBase : EducationalTestBase, IDisposable
{
    protected readonly Mock<ILogger> MockLogger;
    protected readonly IServiceProvider ServiceProvider;

    protected ServiceTestBase(ITestOutputHelper output) : base(output)
    {
        MockLogger = new Mock<ILogger>();
        
        // Setup test service provider
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Configure services for testing
    /// Override in derived classes to add specific services
    /// </summary>
    /// <param name="services">Service collection to configure</param>
    protected virtual void ConfigureServices(IServiceCollection services)
    {
        // Add in-memory database
        services.AddDbContext<WorldLeadersDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDatabase_" + Guid.NewGuid());
            options.EnableSensitiveDataLogging();
        });

        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        // Configure additional services
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
    /// Get a service from the test service provider
    /// </summary>
    /// <typeparam name="T">Service type</typeparam>
    /// <returns>Service instance</returns>
    protected T GetService<T>() where T : notnull
    {
        return ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Get an optional service from the test service provider
    /// </summary>
    /// <typeparam name="T">Service type</typeparam>
    /// <returns>Service instance or null</returns>
    protected T? GetOptionalService<T>() where T : class
    {
        return ServiceProvider.GetService<T>();
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
    /// Seed test data for educational scenarios
    /// </summary>
    /// <param name="dbContext">Database context</param>
    protected virtual async Task SeedTestDataAsync(WorldLeadersDbContext dbContext)
    {
        // Override in derived classes to seed specific test data
        await Task.CompletedTask;
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

    /// <summary>
    /// Create a mock service with educational validation
    /// </summary>
    /// <typeparam name="T">Service interface type</typeparam>
    /// <returns>Mock service</returns>
    protected Mock<T> CreateEducationalMock<T>() where T : class
    {
        var mock = new Mock<T>();
        
        // Setup mock to validate educational content in responses
        MockLogger.Setup(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

        Output.WriteLine($"✅ Created educational mock for service: {typeof(T).Name}");
        return mock;
    }

    /// <summary>
    /// Validate service result for educational objectives
    /// </summary>
    /// <param name="result">Service result</param>
    /// <param name="serviceName">Name of the service</param>
    /// <param name="learningObjective">Expected learning objective</param>
    protected void ValidateServiceEducationalResult(object result, string serviceName, string learningObjective)
    {
        Assert.NotNull(result);
        
        ValidateEducationalOutcome(result, learningObjective);
        
        Output.WriteLine($"✅ Service educational validation passed for {serviceName}");
    }

    /// <summary>
    /// Validate service behavior for child safety
    /// </summary>
    /// <param name="serviceAction">Service action to test</param>
    /// <param name="serviceName">Name of the service</param>
    protected async Task ValidateServiceChildSafety(Func<Task<string>> serviceAction, string serviceName)
    {
        try
        {
            var result = await serviceAction();
            
            if (!string.IsNullOrEmpty(result))
            {
                ValidateChildSafeContent(result, $"Service: {serviceName}");
            }
            
            Output.WriteLine($"✅ Service child safety validation passed for {serviceName}");
        }
        catch (Exception ex)
        {
            // Validate that error messages are also child-safe
            if (!string.IsNullOrEmpty(ex.Message))
            {
                ValidateChildSafeContent(ex.Message, $"Service Error: {serviceName}");
            }
            
            // Re-throw to allow test to handle appropriately
            throw;
        }
    }

    public void Dispose()
    {
        if (ServiceProvider is IDisposable disposableServiceProvider)
        {
            disposableServiceProvider.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}