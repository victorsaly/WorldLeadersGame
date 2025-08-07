using Azure.AI.OpenAI;
using Azure;
using WorldLeaders.Infrastructure.Configuration;
using WorldLeaders.Shared.Constants;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace WorldLeaders.Infrastructure.Services;

/// <summary>
/// Cloud-enabled AI Agent service using Azure OpenAI for educational interactions
/// Context: Educational game component for 12-year-old geography, economics, and language learning
/// Educational Objective: Provide real AI-powered mentors with educational focus and child safety
/// Safety Requirements: Multi-layer content validation, age-appropriate responses, positive messaging
/// </summary>
public class CloudAIAgentService : IAIAgentService
{
    private readonly OpenAIClient? _openAIClient;
    private readonly IContentModerationService _contentModerationService;
    private readonly AzureAIOptions _aiOptions;
    private readonly ILogger<CloudAIAgentService> _logger;
    private readonly Random _random;

    public CloudAIAgentService(
        IOptions<AzureAIOptions> aiOptions,
        IContentModerationService contentModerationService,
        ILogger<CloudAIAgentService> logger)
    {
        _aiOptions = aiOptions.Value;
        _contentModerationService = contentModerationService;
        _logger = logger;
        _random = new Random();

        // Initialize Azure OpenAI client with validation
        if (string.IsNullOrEmpty(_aiOptions.Endpoint) || string.IsNullOrEmpty(_aiOptions.ApiKey))
        {
            _logger.LogWarning("Azure OpenAI credentials not configured, using fallback responses only");
        }
        else if (!IsValidEndpoint(_aiOptions.Endpoint))
        {
            _logger.LogError("Azure OpenAI endpoint is not a valid absolute URI: {Endpoint}", _aiOptions.Endpoint);
        }
        else if (!IsValidApiKey(_aiOptions.ApiKey))
        {
            _logger.LogError("Azure OpenAI API key does not match the expected format.");
        }
        else
        {
            _openAIClient = new OpenAIClient(
                new Uri(_aiOptions.Endpoint),
                new AzureKeyCredential(_aiOptions.ApiKey));

            _logger.LogInformation("Azure OpenAI client initialized for educational AI agents");
        }
    }

    private static bool IsValidEndpoint(string endpoint)
    {
        return Uri.TryCreate(endpoint, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttps || uriResult.Scheme == Uri.UriSchemeHttp);
    }

    private static bool IsValidApiKey(string apiKey)
    {
        // Azure OpenAI keys are typically 32+ character alphanumeric strings, sometimes with dashes
        // Example: "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" or "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
        // Adjust the regex as needed for your environment
        if (string.IsNullOrWhiteSpace(apiKey))
            return false;
        // Accepts 32+ alphanumeric or dash characters
        return System.Text.RegularExpressions.Regex.IsMatch(apiKey, @"^[A-Za-z0-9\-]{32,}$");
    }

    /// <summary>
    /// Generate a personality-driven response using Azure OpenAI with educational focus
    /// Includes comprehensive child safety validation
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

            // Check if Azure OpenAI is available
            if (_openAIClient == null)
            {
                _logger.LogInformation("Azure OpenAI not available, using safe fallback for agent {AgentType}", agentType);
                return await GetSafeFallbackResponseAsync(agentType, "azure openai not configured");
            }

            // Generate educational AI response
            var response = await GenerateCloudResponseAsync(agentType, playerInput, gameContext, playerId);

            if (response != null)
            {
                // Multi-layer safety validation
                var educationalContext = $"AgentType: {agentType}, GameContext: {gameContext}, PlayerAge: 12";
                var validationResult = await _contentModerationService.ValidateContentAsync(response, educationalContext);

                if (validationResult.IsApproved)
                {
                    // Log successful educational interaction
                    _logger.LogInformation(
                        "Successful AI interaction: Agent={AgentType}, Player={PlayerId}, InputLength={InputLength}, ResponseLength={ResponseLength}",
                        agentType, playerId, playerInput.Length, response.Length);

                    return new AIAgentResponse(
                        AgentType: agentType,
                        Response: response,
                        IsAppropriate: true,
                        GeneratedAt: DateTime.UtcNow
                    );
                }
                else
                {
                    // Log safety validation failure
                    _logger.LogWarning(
                        "AI response failed safety validation: Agent={AgentType}, Concerns={Concerns}",
                        agentType, string.Join(", ", validationResult.Concerns));

                    return await GetSafeFallbackResponseAsync(agentType, "safety validation failed");
                }
            }

            // Fallback if generation failed
            return await GetSafeFallbackResponseAsync(agentType, "generation failed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cloud AI generation failed for agent {AgentType}, Player {PlayerId}", agentType, playerId);
            return await GetSafeFallbackResponseAsync(agentType, "generation error");
        }
    }

    /// <summary>
    /// Generate response using Azure OpenAI with educational prompts
    /// </summary>
    private async Task<string?> GenerateCloudResponseAsync(AgentType agentType, string playerInput, string gameContext, Guid playerId)
    {
        try
        {
            // Check if Azure OpenAI client is available
            if (_openAIClient == null)
            {
                _logger.LogWarning("Azure OpenAI client not available, cannot generate cloud response");
                return null;
            }

            // Get educational system prompt for this agent
            var systemPrompt = GetEducationalSystemPrompt(agentType);
            var userPrompt = BuildEducationalUserPrompt(playerInput, gameContext, agentType);

            // Configure chat completion for educational content
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                DeploymentName = _aiOptions.DeploymentName,
                Messages =
                {
                    new ChatRequestSystemMessage(systemPrompt),
                    new ChatRequestUserMessage(userPrompt)
                },
                MaxTokens = 200, // Increased to allow more comprehensive responses; will trim to 300 chars after
                Temperature = (float)_aiOptions.Temperature,
                User = playerId.ToString() // For usage tracking and safety auditing
            };

            // Add educational context and safety instructions
            chatCompletionsOptions.Messages.Add(new ChatRequestSystemMessage(
                "CRITICAL: Response must be under 300 characters total! Use simple words. " +
                "Be encouraging and positive. Use educational words like 'awesome', 'learn', 'explore'. " +
                "Keep it short and exciting for a 12-year-old student!"));

            var response = await _openAIClient.GetChatCompletionsAsync(chatCompletionsOptions);

            if (response?.Value?.Choices?.Count > 0)
            {
                var aiContent = response.Value.Choices[0].Message.Content;

                _logger.LogDebug(
                    "Azure OpenAI response generated: Agent={AgentType}, TokensUsed={TokensUsed}",
                    agentType, response.Value.Usage.TotalTokens);

                return aiContent;
            }

            return null;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Azure OpenAI request failed: {ErrorCode}", ex.ErrorCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during Azure OpenAI generation");
            return null;
        }
    }

    /// <summary>
    /// Get educational system prompt for specific agent personality
    /// </summary>
    private string GetEducationalSystemPrompt(AgentType agentType)
    {
        var personality = AIAgentConstants.AgentPersonalities[agentType];

        return $@"{_aiOptions.EducationalSystemPrompt}

AGENT PERSONALITY: {personality.Name} - {personality.Description}
PERSONALITY TRAITS: {personality.Personality}
EDUCATIONAL FOCUS: {personality.EducationalFocus}
ICON: {personality.IconEmoji}

SPECIFIC INSTRUCTIONS FOR {personality.Name.ToUpper()}:
{GetAgentSpecificInstructions(agentType)}

RESPONSE REQUIREMENTS:
- CRITICAL: Keep responses under 300 characters (about 50-60 words max)
- Use simple, encouraging language with grade 6 vocabulary
- Include educational keywords like: learn, explore, discover, great, awesome
- Be positive and enthusiastic - celebrate every effort
- Use concepts appropriate for 12-year-olds: school, friends, fun, help, grow

SAFETY REQUIREMENTS:
- All content must be appropriate for 12-year-old children
- Use only positive, encouraging words - avoid negative terms
- Keep sentences short and simple - no complex ideas
- End with excitement or encouragement, not questions";
    }

    /// <summary>
    /// Get agent-specific educational instructions
    /// </summary>
    private string GetAgentSpecificInstructions(AgentType agentType)
    {
        return agentType switch
        {
            AgentType.CareerGuide => @"
As Maya the Career Guide, focus on:
- Exploring different careers and what makes them meaningful
- Connecting jobs to helping others and making the world better
- Using simple economic concepts (earning, saving, spending)
- Encouraging all interests and showing diverse career role models
- Teaching that all honest work has value and dignity",

            AgentType.EventNarrator => @"
As Captain Story the Event Narrator, focus on:
- Making geography and history exciting through storytelling
- Using adventure themes without violence or danger
- Teaching about different countries through positive stories
- Connecting places to their unique cultures and contributions
- Creating excitement about exploring and learning about the world",

            AgentType.FortuneTeller => @"
As Sage the Strategic Fortune Teller, focus on:
- Teaching strategic thinking and planning skills
- Using logic and cause-effect reasoning (not supernatural predictions)
- Helping students think through consequences of decisions
- Encouraging careful consideration of choices
- Building problem-solving skills through strategic scenarios",

            AgentType.HappinessAdvisor => @"
As Joy the Happiness Advisor, focus on:
- Teaching emotional intelligence and social skills
- Explaining how communities work together successfully
- Building empathy and understanding between different groups
- Showing how cooperation and kindness create happiness
- Teaching conflict resolution and communication skills",

            AgentType.TerritoryStrategist => @"
As Atlas the Territory Strategist, focus on:
- Making geography exciting with interesting facts about places
- Teaching about different countries' unique contributions to the world
- Using economic concepts like resources, trade, and cooperation
- Encouraging strategic thinking about peaceful expansion and growth
- Connecting territories to real cultures, languages, and achievements",

            AgentType.LanguageTutor => @"
As Poly the Language Tutor, focus on:
- Celebrating all attempts at language learning with enthusiasm
- Teaching cultural appreciation alongside language basics
- Making pronunciation practice fun and encouraging
- Connecting languages to the countries and cultures that speak them
- Building confidence in communication and cultural understanding",

            _ => "Focus on educational content appropriate for 12-year-old learners."
        };
    }

    /// <summary>
    /// Build educational user prompt with context
    /// </summary>
    private string BuildEducationalUserPrompt(string playerInput, string gameContext, AgentType agentType)
    {
        return $@"GAME CONTEXT: {gameContext}

STUDENT INPUT: ""{playerInput}""

Please respond as {AIAgentConstants.AgentPersonalities[agentType].Name}, keeping in mind this is a 12-year-old student playing an educational game about world leadership, geography, and economics. 

Your response should:
1. Address their input directly with enthusiasm
2. Teach something new related to your educational focus
3. Connect to the broader game context of becoming a world leader
4. End with an engaging question or suggestion for next steps

Remember: You're helping a young person learn about the world through an exciting, safe, and educational experience.";
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
    /// Get a safe fallback response when AI generation fails or validation fails
    /// Always provides appropriate educational content for children
    /// </summary>
    public async Task<AIAgentResponse> GetSafeFallbackResponseAsync(AgentType agentType, string context = "")
    {
        var fallbackResponses = AIAgentConstants.SafeFallbackResponses[agentType];
        var selectedResponse = fallbackResponses[_random.Next(fallbackResponses.Count)];

        // Log fallback usage for monitoring
        _logger.LogInformation(
            "Using safe fallback response: Agent={AgentType}, Context={Context}",
            agentType, context);

        return await Task.FromResult(new AIAgentResponse(
            AgentType: agentType,
            Response: selectedResponse,
            IsAppropriate: true,
            GeneratedAt: DateTime.UtcNow
        ));
    }

    /// <summary>
    /// Validate that an AI response is appropriate for 12-year-old players
    /// Multi-layer safety validation for child protection
    /// </summary>
    public async Task<bool> ValidateResponseSafetyAsync(string response, AgentType agentType)
    {
        try
        {
            // Layer 1: Content moderation service validation
            var moderationResult = await _contentModerationService.ValidateContentAsync(response);
            if (!moderationResult.IsApproved || !moderationResult.IsSafe || !moderationResult.IsAgeAppropriate)
            {
                _logger.LogWarning(
                    "Content moderation failed for {AgentType}: {Reason}",
                    agentType, moderationResult.Reason);
                return false;
            }

            // Layer 2: Basic safety checks
            var lowerResponse = response.ToLowerInvariant();

            // Check for inappropriate content patterns
            var inappropriatePatterns = new[]
            {
                "violence", "scary", "frightening", "dangerous", "weapons", "war",
                "adult", "inappropriate", "sexual", "political controversy"
            };

            if (inappropriatePatterns.Any(pattern => lowerResponse.Contains(pattern)))
            {
                _logger.LogWarning(
                    "Safety pattern detection failed for {AgentType}: {Response}",
                    agentType, response);
                return false;
            }

            // Layer 3: Educational value check
            var hasEducationalValue = ContainsEducationalContent(response);
            if (!hasEducationalValue)
            {
                _logger.LogInformation(
                    "Response lacks educational value for {AgentType}: {Response}",
                    agentType, response);
                // Note: We don't fail here, just log for monitoring
            }

            _logger.LogDebug("Safety validation passed for {AgentType}", agentType);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Safety validation error for {AgentType}: {Response}",
                agentType, response);
            return false; // Fail safe - reject on error
        }
    }

    /// <summary>
    /// Check if response contains educational content appropriate for the game
    /// </summary>
    private bool ContainsEducationalContent(string response)
    {
        var educationalKeywords = new[]
        {
            "learn", "geography", "country", "language", "culture", "economy",
            "skill", "job", "career", "strategy", "planning", "knowledge",
            "education", "practice", "improve", "develop", "understand"
        };

        var lowerResponse = response.ToLowerInvariant();
        return educationalKeywords.Any(keyword => lowerResponse.Contains(keyword));
    }

    /// <summary>
    /// Generate educational code explanation using Azure OpenAI
    /// Creates child-friendly explanations that connect programming to real-world learning
    /// </summary>
    public async Task<CodeExplanationResult> GenerateCodeExplanationAsync(string code, string context, string language)
    {
        try
        {
            // If Azure OpenAI is available, use it for dynamic explanations
            if (_openAIClient != null && !string.IsNullOrEmpty(_aiOptions.DeploymentName))
            {
                var prompt = BuildEducationalCodePrompt(code, context, language);
                
                var chatCompletionsOptions = new ChatCompletionsOptions()
                {
                    DeploymentName = _aiOptions.DeploymentName,
                    Messages =
                    {
                        new ChatRequestSystemMessage(GetSystemPromptForCodeExplanation()),
                        new ChatRequestUserMessage(prompt)
                    },
                    MaxTokens = 800,
                    Temperature = 0.7f,
                    NucleusSamplingFactor = 0.9f
                };

                var response = await _openAIClient.GetChatCompletionsAsync(chatCompletionsOptions);
                var explanation = response.Value.Choices[0].Message.Content;

                // Parse and structure the response
                return ParseAICodeExplanation(explanation);
            }
            else
            {
                _logger.LogInformation("Azure OpenAI not configured, using local analysis");
                // Fall back to local analysis (same as AIAgentService)
                return CreateLocalCodeExplanation(code, language);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating code explanation with Azure OpenAI");
            // Return safe fallback
            return CreateFallbackCodeExplanation();
        }
    }

    private string GetSystemPromptForCodeExplanation()
    {
        return @"You are a friendly helper explaining code to 12-year-old kids reading a blog.

Your response should be:
- ONE short, simple sentence that explains what the code does
- Use words a 12-year-old understands
- Include one emoji that fits
- NO markdown formatting, NO headers, NO sections
- Think of it like explaining to a younger sibling

Examples:
- 'This code is like a magic button that makes the computer do something! ✨'
- 'This code helps the computer make decisions! 🤔'
- 'This code tells the computer to repeat something! 🔄'

Just give ONE simple, friendly sentence with an emoji. Nothing else.";
    }

    private string BuildEducationalCodePrompt(string code, string context, string language)
    {
        return $@"Please explain this {language} code in a way that a 12-year-old can understand. This code is part of our educational World Leaders Game that teaches geography and economics.

Code to explain:
```{language}
{code}
```

Context: {context}

Please provide:
1. A simple summary of what the code does (1-2 sentences)
2. Step-by-step breakdown of key parts (max 5 steps)
3. Why this matters for learning programming
4. A real-world analogy a 12-year-old would understand
5. How this connects to learning geography/economics
6. Next steps for continued learning

Remember: Be encouraging, use simple language, and make it fun!";
    }

    private CodeExplanationResult ParseAICodeExplanation(string explanation)
    {
        // For now, create a structured response from the AI text
        // In the future, could request structured JSON from the AI
        return new CodeExplanationResult
        {
            Summary = ExtractSummaryFromText(explanation),
            Breakdown = ExtractBreakdownFromText(explanation),
            EducationalValue = ExtractEducationalValueFromText(explanation),
            RealWorldApplication = ExtractRealWorldApplicationFromText(explanation),
            NextSteps = ExtractNextStepsFromText(explanation),
            ComplexityLevel = "beginner", // Default for 12-year-olds
            ProgrammingConcepts = ExtractConceptsFromText(explanation),
            ChildFriendlyTips = GenerateChildFriendlyTips()
        };
    }

    private string ExtractSummaryFromText(string text)
    {
        // Simple extraction - take first few sentences
        var sentences = text.Split('.', '!', '?').Take(2);
        return string.Join(". ", sentences).Trim() + (sentences.Count() > 0 ? "!" : "");
    }

    private List<CodeLineExplanationResult> ExtractBreakdownFromText(string text)
    {
        // Create a simple breakdown structure
        return new List<CodeLineExplanationResult>
        {
            new CodeLineExplanationResult
            {
                Line = "// AI-generated explanation",
                Explanation = text.Length > 200 ? text.Substring(0, 200) + "..." : text,
                LineNumber = 1
            }
        };
    }

    private EducationalValueResult ExtractEducationalValueFromText(string text)
    {
        return new EducationalValueResult
        {
            LearningObjective = "Understand programming concepts through our educational game",
            AgeAppropriateConcepts = new List<string>
            {
                "This code helps create fun learning experiences 🎮",
                "Programming is like giving step-by-step instructions 📝"
            },
            LifeSkills = new List<string>
            {
                "Logical thinking and problem-solving 🧠",
                "Understanding how technology works 💻"
            }
        };
    }

    private string ExtractRealWorldApplicationFromText(string text)
    {
        return "Like following a recipe to create something amazing - each step helps you reach your goal! 👨‍🍳";
    }

    private List<string> ExtractNextStepsFromText(string text)
    {
        return new List<string>
        {
            "Try changing small parts of the code to see what happens! 🧪",
            "Ask questions about what each part does 🤔",
            "Keep exploring our educational game to see more examples! 🌍"
        };
    }

    private List<string> ExtractConceptsFromText(string text)
    {
        var concepts = new List<string>();
        var lowerText = text.ToLowerInvariant();
        
        if (lowerText.Contains("class")) concepts.Add("classes");
        if (lowerText.Contains("function")) concepts.Add("functions");
        if (lowerText.Contains("if") || lowerText.Contains("condition")) concepts.Add("conditionals");
        if (lowerText.Contains("loop") || lowerText.Contains("repeat")) concepts.Add("loops");
        
        return concepts.Any() ? concepts : new List<string> { "basic-programming" };
    }

    private CodeExplanationResult CreateLocalCodeExplanation(string code, string language)
    {
        // Simple explanations for non-technical 12-year-old blog readers
        var concepts = new List<string>();
        if (code.Contains("class ")) concepts.Add("classes");
        if (code.Contains("function ") || code.Contains("def ") || code.Contains("public ")) concepts.Add("functions");
        if (code.Contains("if ")) concepts.Add("conditionals");
        if (code.Contains("for ") || code.Contains("while ")) concepts.Add("loops");

        // Get simple explanation based on main concept
        var simpleExplanations = new Dictionary<string, string>
        {
            ["classes"] = "This code is like a recipe that tells the computer how to make something! 📝",
            ["functions"] = "This code is like a magic button that makes the computer do a task! ✨",
            ["conditionals"] = "This code helps the computer make choices, like 'if this, then do that!' 🤔",
            ["loops"] = "This code tells the computer to repeat something over and over! 🔄"
        };

        var mainConcept = concepts.FirstOrDefault() ?? "programming";
        var summary = simpleExplanations.GetValueOrDefault(mainConcept, 
            "This code gives instructions to the computer to make something work! 💻");
        
        return new CodeExplanationResult
        {
            Summary = summary,
            Breakdown = new List<CodeLineExplanationResult>
            {
                new CodeLineExplanationResult
                {
                    Line = code.Split('\n').FirstOrDefault()?.Trim() ?? "// Code",
                    Explanation = "This line helps make the website work! ✨",
                    LineNumber = 1
                }
            },
            EducationalValue = new EducationalValueResult
            {
                LearningObjective = "Learn how programming creates fun things",
                AgeAppropriateConcepts = new List<string>
                {
                    "Programming is like giving instructions 💻",
                    "Code helps create websites and games 🎮"
                },
                LifeSkills = new List<string>
                {
                    "Problem-solving 🧠",
                    "Understanding technology 💻"
                }
            },
            RealWorldApplication = "Like following directions to get somewhere! 🗺️",
            NextSteps = new List<string>
            {
                "Try changing small parts to see what happens! 🧪",
                "Ask questions about what each part does 🤔"
            },
            ComplexityLevel = "beginner",
            ProgrammingConcepts = concepts.Any() ? concepts : new List<string> { "basic-programming" },
            ChildFriendlyTips = GenerateChildFriendlyTips()
        };
    }

    private CodeExplanationResult CreateFallbackCodeExplanation()
    {
        return new CodeExplanationResult
        {
            Summary = "This code helps our educational game teach you about geography and economics while having fun! 🌍",
            Breakdown = new List<CodeLineExplanationResult>
            {
                new CodeLineExplanationResult
                {
                    Line = "// Code explanation temporarily unavailable",
                    Explanation = "Don't worry - learning is a journey with ups and downs! 🌟",
                    LineNumber = 1
                }
            },
            EducationalValue = new EducationalValueResult
            {
                LearningObjective = "Learn that programming helps create educational experiences",
                AgeAppropriateConcepts = new List<string>
                {
                    "Programming is like giving instructions to a computer 💻",
                    "Code helps create fun learning games 🎮"
                },
                LifeSkills = new List<string>
                {
                    "Problem-solving and persistence 💪",
                    "Learning from challenges 🌟"
                }
            },
            RealWorldApplication = "Like following directions to get somewhere new - each step helps you reach your goal! 🗺️",
            NextSteps = new List<string>
            {
                "Ask a parent, teacher, or friend to help explain this code 👨‍👩‍👧‍👦",
                "Try our simpler coding activities to build understanding 📚",
                "Keep playing our educational game to see programming in action! 🎮"
            },
            ComplexityLevel = "beginner",
            ProgrammingConcepts = new List<string> { "basic-programming" },
            ChildFriendlyTips = GenerateChildFriendlyTips()
        };
    }

    private List<string> GenerateChildFriendlyTips()
    {
        return new List<string>
        {
            "💡 Don't worry if coding seems confusing at first - even expert programmers started as beginners!",
            "🚀 Every programmer makes mistakes - that's how we learn and get better!",
            "🎯 Start with small, simple projects and build up to bigger ones!",
            "👨‍👩‍👧‍👦 Ask parents, teachers, or friends to help when you get stuck!",
            "📚 Reading code is just as important as writing code!"
        };
    }
}
