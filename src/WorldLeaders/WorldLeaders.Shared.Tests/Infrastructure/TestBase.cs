using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Xunit.Abstractions;

namespace WorldLeaders.Shared.Tests.Infrastructure;

/// <summary>
/// Base test class with common test setup and utilities
/// Context: Educational game testing infrastructure for World Leaders Game
/// </summary>
public abstract class TestBase : IDisposable
{
    protected readonly ITestOutputHelper Output;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly ILogger Logger;

    protected TestBase(ITestOutputHelper output)
    {
        Output = output;
        
        // Setup test service provider
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
        
        // Setup test logging
        var loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
        Logger = loggerFactory.CreateLogger(GetType().Name);
    }

    /// <summary>
    /// Configure services for testing
    /// Override in derived classes to add specific services
    /// </summary>
    /// <param name="services">Service collection to configure</param>
    protected virtual void ConfigureServices(IServiceCollection services)
    {
        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });
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
    /// Log test information
    /// </summary>
    /// <param name="message">Message to log</param>
    /// <param name="args">Message arguments</param>
    protected void LogInfo(string message, params object[] args)
    {
        var formattedMessage = string.Format(message, args);
        Logger.LogInformation(formattedMessage);
        Output.WriteLine($"[INFO] {formattedMessage}");
    }

    /// <summary>
    /// Log test warnings
    /// </summary>
    /// <param name="message">Message to log</param>
    /// <param name="args">Message arguments</param>
    protected void LogWarning(string message, params object[] args)
    {
        var formattedMessage = string.Format(message, args);
        Logger.LogWarning(formattedMessage);
        Output.WriteLine($"[WARN] {formattedMessage}");
    }

    /// <summary>
    /// Log test errors
    /// </summary>
    /// <param name="message">Message to log</param>
    /// <param name="args">Message arguments</param>
    protected void LogError(string message, params object[] args)
    {
        var formattedMessage = string.Format(message, args);
        Logger.LogError(formattedMessage);
        Output.WriteLine($"[ERROR] {formattedMessage}");
    }

    /// <summary>
    /// Create a test scope for dependency injection
    /// </summary>
    /// <returns>Service scope</returns>
    protected IServiceScope CreateScope()
    {
        return ServiceProvider.CreateScope();
    }

    public virtual void Dispose()
    {
        if (ServiceProvider is IDisposable disposableServiceProvider)
        {
            disposableServiceProvider.Dispose();
        }
    }
}