using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;

namespace WorldLeaders.Shared.Services;

/// <summary>
/// Service for managing AI agent interactions with distinct personalities
/// Educational focus: Geography, economics, and language learning for 12-year-old players
/// Safety priority: Child-safe content with multi-layer validation
/// </summary>
public interface IAIAgentService
{
    /// <summary>
    /// Generate a personality-driven response from a specific AI agent
    /// Includes educational content and child safety validation
    /// </summary>
    /// <param name="agentType">The type of AI agent to respond as</param>
    /// <param name="playerInput">The player's input message</param>
    /// <param name="gameContext">Current game state context for educational relevance</param>
    /// <param name="playerId">Player ID for personalization and safety tracking</param>
    /// <returns>Safe, educational AI response with personality</returns>
    Task<AIAgentResponse> GenerateResponseAsync(AgentType agentType, string playerInput, string gameContext, Guid playerId);

    /// <summary>
    /// Get agent personality information for UI display
    /// </summary>
    /// <param name="agentType">The agent type to get info for</param>
    /// <returns>Agent personality description and traits</returns>
    Task<AgentPersonalityInfo> GetAgentPersonalityAsync(AgentType agentType);

    /// <summary>
    /// Get a safe fallback response when AI generation fails
    /// </summary>
    /// <param name="agentType">The agent type requesting fallback</param>
    /// <param name="context">Context for selecting appropriate fallback</param>
    /// <returns>Pre-approved safe response</returns>
    Task<AIAgentResponse> GetSafeFallbackResponseAsync(AgentType agentType, string context = "");

    /// <summary>
    /// Validate that an AI response is appropriate for 12-year-old players
    /// </summary>
    /// <param name="response">The AI generated response to validate</param>
    /// <param name="agentType">The agent type that generated the response</param>
    /// <returns>True if response is safe and appropriate</returns>
    Task<bool> ValidateResponseSafetyAsync(string response, AgentType agentType);

    /// <summary>
    /// Generate educational code explanation for 12-year-old learners
    /// Uses child-safe AI to explain programming concepts in age-appropriate language
    /// </summary>
    /// <param name="code">The code to explain</param>
    /// <param name="context">Educational context for the explanation</param>
    /// <param name="language">Programming language (optional)</param>
    /// <returns>Structured educational explanation</returns>
    Task<CodeExplanationResult> GenerateCodeExplanationAsync(string code, string context, string language);
}