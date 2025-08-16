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
public class AiAgentService : IAIAgentService
{
    private readonly IContentModerationService _contentModerationService;
    private readonly Random _random;

    public AiAgentService(IContentModerationService contentModerationService)
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
            var response = await GeneratePersonalityResponseAsync(agentType, playerInput, personality);
            
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
    public async Task<AgentPersonalityInfo> GetAgentPersonalityAsync(AgentType agentType)
    {
        var personality = AIAgentConstants.AgentPersonalities[agentType];
        var fallbackResponses = AIAgentConstants.SafeFallbackResponses[agentType];
        
        // Take first 3 fallback responses as examples
        var exampleResponses = fallbackResponses.Take(3).ToList();

        return await Task.FromResult(new AgentPersonalityInfo(
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
    private async Task<string> GeneratePersonalityResponseAsync(AgentType agentType, string playerInput, AgentPersonality personality)
    {
        // Since this is a simplified implementation without external AI services,
        // we'll create structured responses based on personality traits and educational context
        
        var responseBuilder = new List<string>();
        
        // Add personality greeting with agent's key phrase
        var keyPhrase = personality.KeyPhrases[_random.Next(personality.KeyPhrases.Length)];
        responseBuilder.Add(keyPhrase);

        // Add educational context based on agent type
        var educationalResponse = GenerateEducationalContent(agentType, playerInput);
        responseBuilder.Add(educationalResponse);

        // Add encouraging conclusion
        var encouragement = GenerateEncouragement(agentType);
        responseBuilder.Add(encouragement);

        var fullResponse = string.Join(" ", responseBuilder);
        
        // Ensure response doesn't exceed maximum length with word-boundary-aware truncation
        if (fullResponse.Length > AIAgentConstants.MaxResponseLength)
        {
            var maxLen = AIAgentConstants.MaxResponseLength - 3; // account for "..."
            if (fullResponse.Length > maxLen)
            {
                var lastSpace = fullResponse.LastIndexOf(' ', maxLen);
                if (lastSpace > 0)
                {
                    fullResponse = fullResponse.Substring(0, lastSpace) + "...";
                }
                else
                {
                    fullResponse = fullResponse.Substring(0, maxLen) + "...";
                }
            }
        }

        return await Task.FromResult(fullResponse);
    }

    /// <summary>
    /// Generate educational content based on agent type and player input
    /// </summary>
    private string GenerateEducationalContent(AgentType agentType, string playerInput)
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
    private string GenerateEncouragement(AgentType agentType)
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

    /// <summary>
    /// Generate educational code explanation for 12-year-old learners
    /// Creates child-friendly explanations that connect programming to real-world learning
    /// </summary>
    public Task<CodeExplanationResult> GenerateCodeExplanationAsync(string code, string context, string language)
    {
        try
        {
            // For now, return a structured educational explanation
            // In the future, this could integrate with Azure OpenAI for dynamic explanations
            var analysis = AnalyzeCodeForEducation(code);
            
            var result = new CodeExplanationResult
            {
                Summary = GenerateChildFriendlySummary(analysis),
                Breakdown = GenerateStepByStepBreakdown(code),
                EducationalValue = GenerateEducationalValue(),
                RealWorldApplication = GenerateRealWorldExample(analysis),
                NextSteps = GenerateNextSteps(analysis),
                ComplexityLevel = AssessComplexityForAge(code),
                ProgrammingConcepts = analysis.Concepts,
                ChildFriendlyTips = GenerateChildFriendlyTips()
            };

            return Task.FromResult(result);
        }
        catch (Exception)
        {
            // Return safe fallback explanation
            return Task.FromResult(CreateFallbackExplanation());
        }
    }

    #region Code Analysis Helper Methods

    private CodeAnalysis AnalyzeCodeForEducation(string code)
    {
        var concepts = new List<string>();

        // Basic concept detection
        if (code.Contains("class ")) concepts.Add("classes");
        if (code.Contains("function ") || code.Contains("def ") || code.Contains("public ")) concepts.Add("functions");
        if (code.Contains("if ") || code.Contains("else")) concepts.Add("conditionals");
        if (code.Contains("for ") || code.Contains("while ")) concepts.Add("loops");
        if (code.Contains("async ") || code.Contains("await ")) concepts.Add("async-programming");
        if (code.Contains("public class")) concepts.Add("object-oriented-programming");
        if (code.Contains("Territory") || code.Contains("Player") || code.Contains("Game")) concepts.Add("game-programming");
        
        // Assess complexity
        var lines = code.Split('\n').Length;
        if (lines > 50 || concepts.Count > 4)
        {
        }
        else if (lines > 20 || concepts.Count > 2)
        {
        }

        return new CodeAnalysis
        {
            Concepts = concepts
        };
    }

    private string GenerateChildFriendlySummary(CodeAnalysis analysis)
    {
        // Simple, non-technical explanations for 12-year-old blog readers
        var simpleExplanations = new Dictionary<string, string>
        {
            ["classes"] = "This code is like a recipe that tells the computer how to make something! 📝",
            ["functions"] = "This code is like a magic button that makes the computer do a task! ✨",
            ["conditionals"] = "This code helps the computer make choices, like 'if this, then do that!' 🤔",
            ["loops"] = "This code tells the computer to repeat something over and over! 🔄",
            ["game-programming"] = "This code helps create our educational game! 🎮",
            ["web-development"] = "This code helps build websites like this one! 🌐",
            ["data-structures"] = "This code organizes information, like putting things in boxes! 📦"
        };

        // Get the main concept or use a default simple explanation
        var mainConcept = analysis.Concepts.FirstOrDefault() ?? "programming";
        
        return simpleExplanations.GetValueOrDefault(mainConcept, 
            "This code gives instructions to the computer to make something work! 💻");
    }

    private List<CodeLineExplanationResult> GenerateStepByStepBreakdown(string code)
    {
        var lines = code.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)).Take(5).ToList();
        var breakdown = new List<CodeLineExplanationResult>();

        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i].Trim();
            var explanation = "This line helps our educational game work! ✨";

            if (line.Contains("public class"))
                explanation = "This creates a new part of our game - like making a new type of game piece! 🎲";
            else if (line.Contains("public") && (line.Contains("Task") || line.Contains("void")))
                explanation = "This creates a task our game can do - like calculating country costs or updating player scores! 📊";
            else if (line.Contains("if (") || line.Contains("if("))
                explanation = "This helps our game make decisions - like checking if a player can afford a country! 🤔";
            else if (line.Contains("return "))
                explanation = "This gives back a result - like telling the player their final score! 🏆";

            breakdown.Add(new CodeLineExplanationResult
            {
                Line = line,
                Explanation = explanation,
                LineNumber = i + 1
            });
        }

        return breakdown;
    }

    private EducationalValueResult GenerateEducationalValue()
    {
        return new EducationalValueResult
        {
            LearningObjective = "Learn how programming creates educational games that teach real-world concepts",
            AgeAppropriateConcepts = new List<string>
            {
                "Programming is like giving step-by-step instructions to a computer 📝",
                "Code helps create interactive learning games 🎮",
                "Good programming makes educational games fun and engaging ✨",
                "Each line of code has a specific purpose in making the game work 🔧"
            },
            LifeSkills = new List<string>
            {
                "Problem-solving by breaking big tasks into smaller steps 🧩",
                "Logical thinking and planning ahead 🧠",
                "Attention to detail and following instructions carefully 🔍",
                "Learning from mistakes and trying again 💪"
            }
        };
    }

    private string GenerateRealWorldExample(CodeAnalysis analysis)
    {
        var examples = new Dictionary<string, string>
        {
            ["classes"] = "Like having a recipe for making different types of cookies - each recipe tells you exactly how to make that type! 🍪",
            ["functions"] = "Like having different buttons on a remote control - each button does one specific thing when you press it! 📺",
            ["conditionals"] = "Like the rules in a board game - 'if you roll a 6, then you get an extra turn!' 🎲",
            ["loops"] = "Like doing jumping jacks - you repeat the same action over and over until you reach your goal! 🤸",
            ["game-programming"] = "Like creating the rules for a new board game that teaches players about different countries! 🎯"
        };

        var mainConcept = analysis.Concepts.FirstOrDefault() ?? "programming";
        return examples.GetValueOrDefault(mainConcept,
            "Like following a recipe to cook something delicious - each step is important and leads to a great result! 👨‍🍳");
    }

    private List<string> GenerateNextSteps(CodeAnalysis analysis)
    {
        var steps = new List<string>
        {
            "Try creating simple code with our interactive coding activities 💻",
            "Explore how different parts of the World Leaders Game work together 🔗",
            "Practice problem-solving with our educational programming puzzles 🧩",
            "Learn more about the countries featured in our game 🌍"
        };

        if (analysis.Concepts.Contains("functions"))
            steps.Add("Create your own simple functions in our coding playground! 🛠️");
        
        if (analysis.Concepts.Contains("game-programming"))
            steps.Add("Design your own educational mini-game idea! 🎨");

        return steps;
    }

    private string AssessComplexityForAge(string code)
    {
        var lines = code.Split('\n').Length;
        if (lines > 50) return "advanced";
        if (lines > 20) return "intermediate";
        return "beginner";
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

    private CodeExplanationResult CreateFallbackExplanation()
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

    // Helper class for code analysis
    private class CodeAnalysis
    {
        public List<string> Concepts { get; set; } = new();
    }

    #endregion

    #endregion

        // Implement missing IAIAgentService methods
        public async Task<LanguageChallengeDto> GetLanguageChallengeAsync(string countryCode)
        {
            // Return a simple language challenge DTO for educational compliance
            return await Task.FromResult(new LanguageChallengeDto(
                LanguageCode: "en",
                LanguageName: "English",
                Word: "Hello",
                Pronunciation: "heh-loh",
                AudioUrl: "",
                RequiredAccuracy: 80,
                SupportsSpeechRecognition: true,
                CulturalContext: "Learn how to greet people in this country!",
                Type: ChallengeType.Greeting
            ));
        }

        public async Task<CulturalContextDto> GetCulturalContextAsync(string countryCode)
        {
            // Return a simple cultural context DTO for educational compliance
            return await Task.FromResult(new CulturalContextDto(
                TerritoryId: Guid.NewGuid(),
                CountryName: countryCode,
                HistoricalSignificance: "This country has a rich history and unique traditions.",
                CulturalTraditions: new List<string> { "Traditional dance", "Local cuisine" },
                FamousLandmarks: new List<string> { "Famous monument", "Historic site" },
                NotableAchievements: new List<string> { "Scientific discovery", "Cultural festival" },
                GeographyLesson: "Learn about the geography and climate of this country.",
                EconomicLesson: "Discover how the country's resources shape its economy.",
                EducationalQuizQuestions: new List<string> { "What is a famous landmark here?", "Name a local tradition." },
                ChildFriendlyDescription: "Explore the customs and stories to become a world leader!"
            ));
        }
}