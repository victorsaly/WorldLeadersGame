using Microsoft.Extensions.Options;

namespace WorldLeaders.Infrastructure.Configuration;

/// <summary>
/// Azure OpenAI configuration for educational AI agents
/// Provides settings for safe, child-focused AI interactions
/// </summary>
public class AzureAIOptions
{
    public const string SectionName = "AzureOpenAI";

    /// <summary>
    /// Azure OpenAI service endpoint
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// API key for Azure OpenAI service
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Deployment name for the GPT model (e.g., "gpt-4-educational")
    /// </summary>
    public string DeploymentName { get; set; } = "gpt-4";

    /// <summary>
    /// API version for Azure OpenAI
    /// </summary>
    public string ApiVersion { get; set; } = "2024-02-15-preview";

    /// <summary>
    /// Maximum tokens for AI responses (optimized for 12-year-old attention spans)
    /// </summary>
    public int MaxTokens { get; set; } = 500;

    /// <summary>
    /// Temperature for response creativity (balanced for educational consistency)
    /// </summary>
    public double Temperature { get; set; } = 0.7;

    /// <summary>
    /// Base system prompt for all educational interactions
    /// </summary>
    public string EducationalSystemPrompt { get; set; } =
        "You are an educational AI assistant for 12-year-old students learning geography, economics, and languages. " +
        "Always provide encouraging, age-appropriate, and culturally sensitive responses that teach real-world concepts.";
}

/// <summary>
/// Azure Content Moderator configuration for child safety
/// </summary>
public class ContentModeratorOptions
{
    public const string SectionName = "ContentModerator";

    /// <summary>
    /// Azure Content Moderator endpoint
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// API key for Content Moderator service
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Child safety level (Strict, Moderate, Lenient)
    /// </summary>
    public string ChildSafetyLevel { get; set; } = "Strict";

    /// <summary>
    /// Custom block lists for additional content filtering
    /// </summary>
    public List<string> CustomBlockLists { get; set; } = new();
}

/// <summary>
/// Azure Speech Services configuration for language learning
/// </summary>
public class SpeechServicesOptions
{
    public const string SectionName = "SpeechServices";

    /// <summary>
    /// Azure region for Speech Services
    /// </summary>
    public string Region { get; set; } = "eastus";

    /// <summary>
    /// Speech Services endpoint
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// API key for Speech Services
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Supported languages for pronunciation practice
    /// </summary>
    public List<string> SupportedLanguages { get; set; } = new()
    {
        "en-US", "es-ES", "fr-FR", "de-DE", "zh-CN", "ja-JP"
    };
}
