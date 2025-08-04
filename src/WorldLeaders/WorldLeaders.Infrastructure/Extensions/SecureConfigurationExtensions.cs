using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorldLeaders.Infrastructure.Configuration;

namespace WorldLeaders.Infrastructure.Extensions;

/// <summary>
/// Secure configuration loader for Azure AI services
/// Loads credentials from environment variables or local config files in development
/// </summary>
public static class SecureConfigurationExtensions
{
    public static IServiceCollection AddSecureAzureAIConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        // In development, try to load from local files or environment variables
        if (environment.IsDevelopment())
        {
            // Try to load from appsettings.local.json if it exists
            var localConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.local.json");
            if (File.Exists(localConfigPath))
            {
                var localConfig = new ConfigurationBuilder()
                    .AddJsonFile(localConfigPath, optional: true)
                    .Build();

                services.Configure<AzureAIOptions>(localConfig.GetSection("AzureOpenAI"));
                services.Configure<ContentModeratorOptions>(localConfig.GetSection("ContentModerator"));
                services.Configure<SpeechServicesOptions>(localConfig.GetSection("SpeechServices"));

                // Log that we're using local configuration
                var localServiceProvider = services.BuildServiceProvider();
                var logger = localServiceProvider.GetService<ILoggerFactory>()?.CreateLogger("SecureConfiguration");
                logger?.LogInformation("🔐 Using local Azure AI configuration for development");

                return services;
            }

            // Try environment variables
            var envEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
            var envApiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");

            if (!string.IsNullOrEmpty(envEndpoint) && !string.IsNullOrEmpty(envApiKey))
            {
                services.Configure<AzureAIOptions>(options =>
                {
                    options.Endpoint = envEndpoint;
                    options.ApiKey = envApiKey;
                    options.DeploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4-educational";
                    options.ApiVersion = "2024-02-15-preview";
                    options.MaxTokens = 500;
                    options.Temperature = 0.7f;
                });

                services.Configure<ContentModeratorOptions>(options =>
                {
                    options.Endpoint = Environment.GetEnvironmentVariable("CONTENT_MODERATOR_ENDPOINT") ?? "";
                    options.ApiKey = Environment.GetEnvironmentVariable("CONTENT_MODERATOR_API_KEY") ?? "";
                });

                services.Configure<SpeechServicesOptions>(options =>
                {
                    options.Region = Environment.GetEnvironmentVariable("SPEECH_SERVICES_REGION") ?? "uksouth";
                    options.ApiKey = Environment.GetEnvironmentVariable("SPEECH_SERVICES_API_KEY") ?? "";
                });

                var envServiceProvider = services.BuildServiceProvider();
                var logger = envServiceProvider.GetService<ILoggerFactory>()?.CreateLogger("SecureConfiguration");
                logger?.LogInformation("🔐 Using environment variables for Azure AI configuration");

                return services;
            }
        }

        // Fallback to regular configuration (which will use placeholder values)
        services.Configure<AzureAIOptions>(configuration.GetSection("AzureOpenAI"));
        services.Configure<ContentModeratorOptions>(configuration.GetSection("ContentModerator"));
        services.Configure<SpeechServicesOptions>(configuration.GetSection("SpeechServices"));

        var serviceProvider = services.BuildServiceProvider();
        var fallbackLogger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger("SecureConfiguration");
        fallbackLogger?.LogWarning("⚠️ Using fallback configuration - Azure AI services will use mock responses");

        return services;
    }
}
