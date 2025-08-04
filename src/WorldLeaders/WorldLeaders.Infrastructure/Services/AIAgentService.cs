using WorldLeaders.Shared.Constants;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.Infrastructure.Services;

/// <summary>
/// AI Agent service with distinct personalities for educational child-safe interactions
/// Context: Educational game component for 12-year-old geography, economics, and language learning
/// Educational Objective: Provide personalized AI mentors with encouraging, safe personalities
/// Safety Requirements: Multi-layer content validation, age-appropriate responses, positive messaging
/// </summary>
public class AIAgentService : IAIAgentService
{
    private readonly IContentModerationService _contentModerationService;
    private readonly Random _random;

    public AIAgentService(IContentModerationService contentModerationService)
    {
        _contentModerationService = contentModerationService;
        _random = new Random();
    }

    /// <summary>
    /// Generate a personality-driven response from a specific AI agent
    /// Includes educational content and child safety validation
    /// </summary>
    public async Task<AIAgentResponse> GenerateResponseAsync(AgentType agentType, string playerInput, string gameContext, Guid playerId)
    {
        try
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(playerInput))
            {
                return await GetSafeFallbackResponseAsync(agentType, "empty input");
            }

            // Get agent personality
            var personality = AIAgentConstants.AgentPersonalities[agentType];
            
            // Generate educational context
            var educationalContext = GetEducationalContext(agentType, gameContext);
            
            // Create personality-driven response
            var response = await GeneratePersonalityResponseAsync(agentType, playerInput, educationalContext, personality);
            
            // Validate response safety
            var isValid = await _contentModerationService.ValidateContentAsync(response, educationalContext);
            
            if (!isValid.IsApproved)
            {
                // Use safe fallback if validation fails
                return await GetSafeFallbackResponseAsync(agentType, "content validation failed");
            }

            return new AIAgentResponse(
                AgentType: agentType,
                Response: response,
                IsAppropriate: true,
                GeneratedAt: DateTime.UtcNow
            );
        }
        catch (Exception)
        {
            // Always provide safe fallback on any error
            return await GetSafeFallbackResponseAsync(agentType, "generation error");
        }
    }

    /// <summary>
    /// Get agent personality information for UI display
    /// </summary>
    public async Task<WorldLeaders.Shared.DTOs.AgentPersonalityInfo> GetAgentPersonalityAsync(AgentType agentType)
    {
        var personality = AIAgentConstants.AgentPersonalities[agentType];
        var fallbackResponses = AIAgentConstants.SafeFallbackResponses[agentType];
        
        // Take first 3 fallback responses as examples
        var exampleResponses = fallbackResponses.Take(3).ToList();

        return await Task.FromResult(new WorldLeaders.Shared.DTOs.AgentPersonalityInfo(
            AgentType: agentType,
            Name: personality.Name,
            Description: personality.Description,
            Personality: personality.Personality,
            EducationalFocus: personality.EducationalFocus,
            IconEmoji: personality.IconEmoji,
            ExampleResponses: exampleResponses
        ));
    }

    /// <summary>
    /// Get a safe fallback response when AI generation fails
    /// </summary>
    public async Task<AIAgentResponse> GetSafeFallbackResponseAsync(AgentType agentType, string context = "")
    {
        var fallbackResponses = AIAgentConstants.SafeFallbackResponses[agentType];
        var selectedResponse = fallbackResponses[_random.Next(fallbackResponses.Count)];

        return await Task.FromResult(new AIAgentResponse(
            AgentType: agentType,
            Response: selectedResponse,
            IsAppropriate: true,
            GeneratedAt: DateTime.UtcNow
        ));
    }

    /// <summary>
    /// Validate that an AI response is appropriate for 12-year-old players
    /// </summary>
    public async Task<bool> ValidateResponseSafetyAsync(string response, AgentType agentType)
    {
        var educationalContext = AIAgentConstants.EducationalContexts[agentType].FirstOrDefault() ?? "";
        var validation = await _contentModerationService.ValidateContentAsync(response, educationalContext);
        return validation.IsApproved;
    }

    #region Private Helper Methods

    /// <summary>
    /// Generate a personality-driven response based on agent type and educational context
    /// </summary>
    private async Task<string> GeneratePersonalityResponseAsync(AgentType agentType, string playerInput, string educationalContext, AgentPersonality personality)
    {
        // Since this is a simplified implementation without external AI services,
        // we'll create structured responses based on personality traits and educational context
        
        var responseBuilder = new List<string>();
        
        // Add personality greeting with agent's key phrase
        var keyPhrase = personality.KeyPhrases[_random.Next(personality.KeyPhrases.Length)];
        responseBuilder.Add(keyPhrase);

        // Add educational context based on agent type
        var educationalResponse = GenerateEducationalContent(agentType, playerInput, educationalContext);
        responseBuilder.Add(educationalResponse);

        // Add encouraging conclusion
        var encouragement = GenerateEncouragement(agentType, personality);
        responseBuilder.Add(encouragement);

        var fullResponse = string.Join(" ", responseBuilder);
        
        // Ensure response doesn't exceed maximum length
        if (fullResponse.Length > AIAgentConstants.MaxResponseLength)
        {
            fullResponse = fullResponse.Substring(0, AIAgentConstants.MaxResponseLength - 3) + "...";
        }

        return await Task.FromResult(fullResponse);
    }

    /// <summary>
    /// Generate educational content based on agent type and player input
    /// </summary>
    private string GenerateEducationalContent(AgentType agentType, string playerInput, string educationalContext)
    {
        var inputLower = playerInput.ToLowerInvariant();

        return agentType switch
        {
            AgentType.CareerGuide => GenerateCareerEducationalContent(inputLower),
            AgentType.EventNarrator => GenerateEventEducationalContent(inputLower),
            AgentType.FortuneTeller => GenerateStrategyEducationalContent(inputLower),
            AgentType.HappinessAdvisor => GenerateHappinessEducationalContent(inputLower),
            AgentType.TerritoryStrategist => GenerateGeographyEducationalContent(inputLower),
            AgentType.LanguageTutor => GenerateLanguageEducationalContent(inputLower),
            _ => "Learning is an amazing adventure! Every question you ask helps you grow and discover new things about our wonderful world."
        };
    }

    private string GenerateCareerEducationalContent(string input)
    {
        if (input.Contains("job") || input.Contains("career") || input.Contains("work"))
        {
            return "Different careers around the world help countries grow their economies! From farmers who grow food to business leaders who create jobs, every profession teaches us about how money flows and communities thrive.";
        }
        if (input.Contains("money") || input.Contains("income"))
        {
            return "Understanding income and economics is super important! When people work, they earn money that helps them buy things they need and support their families, which makes the whole economy stronger.";
        }
        return "Career exploration opens doors to understanding how the world works! Each job teaches us about economics, helping others, and building strong communities together.";
    }

    private string GenerateEventEducationalContent(string input)
    {
        if (input.Contains("country") || input.Contains("world") || input.Contains("place"))
        {
            return "Every country has fascinating stories and unique cultures! From the snowy mountains of Canada to the sunny beaches of Australia, each place on our world map has amazing geography and wonderful people to discover.";
        }
        if (input.Contains("adventure") || input.Contains("explore"))
        {
            return "Geographic adventures await in every corner of our planet! Exploring different countries teaches us about diverse climates, amazing landmarks, and how geography shapes the way people live and work.";
        }
        return "Stories from around the world teach us about amazing places and cultures! Geography comes alive when we learn about different countries and the incredible diversity of our planet.";
    }

    private string GenerateStrategyEducationalContent(string input)
    {
        if (input.Contains("plan") || input.Contains("strategy") || input.Contains("decision"))
        {
            return "Strategic thinking is like solving a fun puzzle! When world leaders make decisions about resources and economics, they consider how their choices affect their people's happiness and prosperity.";
        }
        if (input.Contains("future") || input.Contains("goal"))
        {
            return "Planning for the future requires understanding economics and resources! Smart leaders study how money works, what their country produces, and how to trade with other nations to create prosperity.";
        }
        return "Strategic wisdom comes from learning about economics and resource management! Understanding how countries work together helps us make thoughtful decisions for everyone's benefit.";
    }

    private string GenerateHappinessEducationalContent(string input)
    {
        if (input.Contains("happy") || input.Contains("people") || input.Contains("community"))
        {
            return "Happy communities are built on understanding and cooperation! When people from different cultures work together respectfully, they create wonderful societies where everyone can thrive and learn from each other.";
        }
        if (input.Contains("help") || input.Contains("care"))
        {
            return "Caring for others is what makes communities strong! Different cultures around the world have beautiful ways of showing kindness and working together to solve problems and help everyone succeed.";
        }
        return "Building bridges between cultures creates happiness and understanding! Learning about how different countries take care of their people teaches us valuable lessons about cooperation and kindness.";
    }

    private string GenerateGeographyEducationalContent(string input)
    {
        if (input.Contains("country") || input.Contains("territory") || input.Contains("map"))
        {
            return "Geography is absolutely fascinating! Each country has unique features like mountains, rivers, and cities that affect how people live and work. Understanding these geographic differences helps us appreciate our diverse world.";
        }
        if (input.Contains("economy") || input.Contains("resource"))
        {
            return "Geographic resources shape country economies in amazing ways! Some nations have oil, others have fertile farmland, and some have beautiful coastlines for fishing - all contributing to their economic strength and prosperity.";
        }
        return "World geography and economics work together beautifully! Learning about different countries' locations, resources, and economic strengths helps us understand how our interconnected world functions.";
    }

    private string GenerateLanguageEducationalContent(string input)
    {
        if (input.Contains("language") || input.Contains("speak") || input.Contains("pronunciation"))
        {
            return "Language learning connects us to amazing cultures! When we practice pronunciation of different languages, we're not just learning sounds - we're building bridges to understand how people from other countries communicate and express their ideas.";
        }
        if (input.Contains("culture") || input.Contains("country"))
        {
            return "Every language carries the beautiful culture of its people! Learning languages from different countries helps us understand their traditions, ways of thinking, and how geography and culture influence communication.";
        }
        return "Multilingual communication opens doors to the world! Each language we learn teaches us about different cultures and helps us become global citizens who can connect with people everywhere.";
    }

    /// <summary>
    /// Generate encouraging conclusion based on agent personality
    /// </summary>
    private string GenerateEncouragement(AgentType agentType, AgentPersonality personality)
    {
        var encouragements = agentType switch
        {
            AgentType.CareerGuide => new[] { 
                "Your curiosity about careers will take you far!", 
                "Keep exploring - every job teaches us something valuable!",
                "You're building amazing skills for your future!"
            },
            AgentType.EventNarrator => new[] { 
                "Your adventure continues with each new discovery!",
                "What exciting geography will you explore next?",
                "The world is full of stories waiting for you!"
            },
            AgentType.FortuneTeller => new[] { 
                "Your strategic thinking grows stronger every day!",
                "Wise decisions come from curious minds like yours!",
                "The future is bright when you plan thoughtfully!"
            },
            AgentType.HappinessAdvisor => new[] { 
                "Your caring heart makes the world a better place!",
                "Understanding others is a wonderful superpower!",
                "You're learning to build bridges between cultures!"
            },
            AgentType.TerritoryStrategist => new[] { 
                "Your world knowledge expands with every question!",
                "Geography and economics are exciting to explore together!",
                "You're becoming a true global explorer!"
            },
            AgentType.LanguageTutor => new[] { 
                "Your language skills are growing beautifully!",
                "Every word you learn connects you to new cultures!",
                "Communication is the key to understanding our world!"
            },
            _ => new[] { "Keep learning and exploring!" }
        };

        return encouragements[_random.Next(encouragements.Length)];
    }

    /// <summary>
    /// Get educational context for the current interaction
    /// </summary>
    private string GetEducationalContext(AgentType agentType, string gameContext)
    {
        var contexts = AIAgentConstants.EducationalContexts[agentType];
        
        // Select context based on game state or default to first
        if (!string.IsNullOrWhiteSpace(gameContext))
        {
            var gameContextLower = gameContext.ToLowerInvariant();
            
            // Match game context to appropriate educational context
            if (gameContextLower.Contains("geography") || gameContextLower.Contains("country"))
                return contexts.FirstOrDefault(c => c.Contains("geography")) ?? contexts[0];
            
            if (gameContextLower.Contains("economics") || gameContextLower.Contains("money"))
                return contexts.FirstOrDefault(c => c.Contains("economics")) ?? contexts[0];
            
            if (gameContextLower.Contains("language") || gameContextLower.Contains("culture"))
                return contexts.FirstOrDefault(c => c.Contains("language")) ?? contexts[0];
        }

        return contexts[0]; // Default to first context
    }

    #endregion
}