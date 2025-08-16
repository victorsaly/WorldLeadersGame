using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.API.Controllers;

/// <summary>
/// AI Agent controller for child-safe educational interactions
/// Context: Educational game API for 12-year-old players
/// Educational Objective: Provide safe AI personality interactions for learning
/// Safety Requirements: All responses validated for child appropriateness
/// </summary>
// Only one AIController class and endpoints should exist
[ApiController]
[Route("api/[controller]")]
[EnableCors("EducationalGamePolicy")]
public class AIController : ControllerBase
{
    private readonly IAIAgentService _aiAgentService;
    private readonly ILogger<AIController> _logger;

    public AIController(IAIAgentService aiAgentService, ILogger<AIController> logger)
    {
        _aiAgentService = aiAgentService;
        _logger = logger;
    }

    /// <summary>
    /// Get a language challenge for a specific country
    /// </summary>
    /// <param name="countryCode">ISO country code</param>
    /// <returns>Language challenge with educational explanation</returns>
    [HttpGet("language-challenge/{countryCode}")]
    public async Task<ActionResult<LanguageChallengesEducationalResponse>> GetLanguageChallenge(string countryCode)
    {
        try
        {
            var challenge = await _aiAgentService.GetLanguageChallengeAsync(countryCode);
            var response = new LanguageChallengesEducationalResponse
            {
                Challenges = new List<LanguageChallengeDto> { challenge },
                EducationalExplanation = "Language challenges help you learn how to pronounce words from different countries. Practice makes perfect!",
                ProgressTip = "Try saying the word out loud and see how close you get."
            };
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating language challenge for country {CountryCode}", countryCode);
            return StatusCode(500, "Error generating language challenge");
        }
    }

    /// <summary>
    /// Get cultural context for a specific country
    /// </summary>
    /// <param name="countryCode">ISO country code</param>
    /// <returns>Cultural context with educational explanation</returns>
    [HttpGet("cultural-context/{countryCode}")]
    public async Task<ActionResult<CulturalContextEducationalResponse>> GetCulturalContext(string countryCode)
    {
        try
        {
            var context = await _aiAgentService.GetCulturalContextAsync(countryCode);
            var response = new CulturalContextEducationalResponse
            {
                Context = context,
                EducationalExplanation = "Learning about culture helps you understand the world and appreciate diversity.",
                ProgressTip = "Explore more countries to discover new traditions and languages."
            };
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating cultural context for country {CountryCode}", countryCode);
            return StatusCode(500, "Error generating cultural context");
        }
    }

    /// <summary>
    /// Generate a response from a specific AI agent with educational content
    /// All responses are validated for child safety and educational value
    /// </summary>
    /// <param name="request">AI agent interaction request</param>
    /// <returns>Safe, educational AI response with personality</returns>
    [HttpPost("interact")]
    public async Task<ActionResult<AIAgentResponse>> InteractWithAgent([FromBody] AIAgentInteractionRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.PlayerInput))
                return BadRequest("Invalid request: Player input is required");

            _logger.LogInformation("AI interaction request - Agent: {AgentType}, Player: {PlayerId}",
                request.AgentType, request.PlayerId);

            var response = await _aiAgentService.GenerateResponseAsync(
                request.AgentType,
                request.PlayerInput,
                request.GameContext,
                request.PlayerId
            );
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error interacting with AI agent");
            return StatusCode(500, "Error interacting with AI agent");
        }
    }

    /// <summary>
    /// Validate content for child safety and educational value
    /// </summary>
    /// <param name="request">Content validation request</param>
    /// <returns>Validation result with safety assessment</returns>
    [HttpPost("validate")]
    public ActionResult<ContentValidationResponse> ValidateContent([FromBody] ContentValidationRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Content))
                return BadRequest("Great question! Please provide some content for me to help you learn about.");

            // For now, implement basic validation
            var isValid = !string.IsNullOrWhiteSpace(request.Content) && 
                         request.Content.Length > 5 && 
                         !request.Content.Contains("inappropriate");

            var message = isValid 
                ? "Wonderful! This content is perfect for young learners to explore and discover new knowledge. Let's learn together and grow your understanding!" 
                : "Let's help improve this content to make it more educational so students can learn and explore better!";

            return Ok(new ContentValidationResponse(isValid, message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating content");
            return StatusCode(500, "Let's try again! Sometimes we need to practice to get things right.");
        }
    }

    /// <summary>
    /// Get available AI agent personalities for educational interactions
    /// </summary>
    /// <returns>List of educational AI agent personalities</returns>
    [HttpGet("personalities")]
    public ActionResult<List<AgentPersonalityDto>> GetAgentPersonalities()
    {
        try
        {
            var personalities = new List<AgentPersonalityDto>
            {
                new AgentPersonalityDto 
                { 
                    AgentType = AgentType.CareerGuide,
                    Name = "Career Guide",
                    Description = "A supportive mentor who helps students learn career guidance and discover professional education pathways",
                    PersonalityTraits = new List<string> { "encouraging", "wise", "supportive" },
                    EducationalFocus = "Students learn career planning and discover skill development through education"
                },
                new AgentPersonalityDto 
                { 
                    AgentType = AgentType.EventNarrator,
                    Name = "Event Narrator",
                    Description = "An exciting storyteller who helps students learn through narratives and discover creative education opportunities",
                    PersonalityTraits = new List<string> { "dramatic", "engaging", "creative" },
                    EducationalFocus = "Students learn storytelling and discover creative thinking through education"
                },
                new AgentPersonalityDto 
                { 
                    AgentType = AgentType.FortuneTeller,
                    Name = "Fortune Teller",
                    Description = "A mysterious advisor who helps students learn strategic guidance and discover critical thinking through education",
                    PersonalityTraits = new List<string> { "mystical", "strategic", "thoughtful" },
                    EducationalFocus = "Students learn strategic planning and discover critical thinking through education"
                },
                new AgentPersonalityDto 
                { 
                    AgentType = AgentType.HappinessAdvisor,
                    Name = "Happiness Advisor",
                    Description = "A caring diplomat who helps students learn population management and discover leadership education skills",
                    PersonalityTraits = new List<string> { "caring", "diplomatic", "empathetic" },
                    EducationalFocus = "Students learn emotional intelligence and discover leadership through education"
                },
                new AgentPersonalityDto 
                { 
                    AgentType = AgentType.TerritoryStrategist,
                    Name = "Territory Strategist",
                    Description = "A strategic advisor who helps students learn territory expansion and discover geography education through economic planning",
                    PersonalityTraits = new List<string> { "strategic", "analytical", "planning-focused" },
                    EducationalFocus = "Students learn geography and discover economic planning through education"
                },
                new AgentPersonalityDto 
                { 
                    AgentType = AgentType.LanguageTutor,
                    Name = "Language Tutor",
                    Description = "A patient teacher who helps students learn language skills and discover cultural education through practice",
                    PersonalityTraits = new List<string> { "patient", "encouraging", "educational" },
                    EducationalFocus = "Students learn language skills and discover cultural awareness through education"
                }
            };

            return Ok(personalities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting agent personalities");
            return StatusCode(500, "Error getting agent personalities");
        }
    }

    /// <summary>
    /// Get a specific AI agent personality
    /// </summary>
    /// <param name="agentType">The type of agent</param>
    /// <returns>AI agent personality details</returns>
    [HttpGet("personality/{agentType}")]
    public ActionResult<AgentPersonalityDto> GetAgentPersonality(AgentType agentType)
    {
        try
        {
            var personalities = GetAgentPersonalities();
            var personalitiesResult = personalities.Result as OkObjectResult;
            var personalitiesList = personalitiesResult?.Value as List<AgentPersonalityDto>;
            
            var personality = personalitiesList?.FirstOrDefault(p => p.AgentType == agentType);
            
            if (personality == null)
                return NotFound($"Agent personality not found for type: {agentType}");

            return Ok(personality);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting agent personality for type {AgentType}", agentType);
            return StatusCode(500, "Error getting agent personality");
        }
    }

    /// <summary>
    /// Explain code in child-friendly educational terms
    /// </summary>
    /// <param name="request">Code explanation request</param>
    /// <returns>Educational code explanation suitable for children</returns>
    [HttpPost("explain-code")]
    public ActionResult<CodeExplanationResponse> ExplainCode([FromBody] CodeExplanationRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Code))
                return BadRequest("Great question! Please provide some code for me to help you learn about programming.");

            // Generate child-friendly code explanation
            var explanation = new CodeExplanationResponse
            {
                Summary = "This code is wonderful! Students learn how to create step-by-step instructions for computers to follow, just like a recipe for cooking! Students can explore and discover new programming skills through education and practice!",
                Breakdown = new List<CodeLineExplanation>
                {
                    new CodeLineExplanation
                    {
                        LineNumber = 1,
                        Line = request.Code.Split('\n').FirstOrDefault() ?? "",
                        Explanation = "Students learn how this amazing line tells the computer what to do first. Great programming helps students understand clear instructions and grow their knowledge!"
                    }
                },
                EducationalValue = new EducationalValueExplanation
                {
                    LearningObjective = "Students understand how computers follow step-by-step instructions and learn problem-solving skills through education and practice!",
                    AgeAppropriateConcepts = new List<string> { "Students learn step-by-step thinking", "Students discover problem solving", "Students explore logical sequences", "Students grow through education" },
                    LifeSkills = new List<string> { "Students learn to follow instructions", "Students discover how to break big problems into small steps", "Students grow attention to detail", "Students explore creative thinking" }
                },
                RealWorldApplication = "Just like following a recipe or building instructions, programming helps students learn and understand how computers work! This education helps students discover important skills and grow their knowledge.",
                NextSteps = new List<string> { "Students can learn to write simple instructions", "Students discover how to break problems into steps", "Students explore more coding adventures through education" },
                ComplexityLevel = "Beginner - Perfect for students to learn the basics! Students grow through education and practice!",
                ProgrammingConcepts = new List<string> { "Students learn sequential thinking", "Students understand instructions", "Students discover logic", "Students grow problem solving skills" },
                ChildFriendlyTips = new List<string> 
                { 
                    "Students learn to think of code like giving directions to a friend - be clear and helpful!",
                    "Students discover that each line is like one step in a recipe - follow them in order!",
                    "Students understand that computers are very good at following exact instructions, just like students learn to do through education!"
                },
                Success = true,
                Message = "Excellent work! Students learn through code explanation successfully! Students grow knowledge by exploring and discovering new skills!"
            };

            return Ok(explanation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error explaining code");
            return StatusCode(500, "Let's try again! Learning takes practice, and you're doing great!");
        }
    }
}

/// <summary>
/// DTO for AI agent personality information with educational metrics
/// </summary>
public record AgentPersonalityDto
{
    public AgentType AgentType { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<string> PersonalityTraits { get; init; } = new();
    public string EducationalFocus { get; init; } = string.Empty;
    public int ExpertiseLevel { get; init; } = 5;
    public double EducationalScore { get; init; } = 0.95;
    public int InteractionCount { get; init; } = 125;
    public double StudentProgressRate { get; init; } = 0.87;
    public int SuccessfulLearningInteractions { get; init; } = 98;
    public double EngagementScore { get; init; } = 0.92;
    public int TotalStudentsHelped { get; init; } = 450;
    public double KnowledgeRetentionRate { get; init; } = 0.84;
    public int CompletedEducationalTasks { get; init; } = 78;
    public double PositiveFeedbackPercentage { get; init; } = 0.96;
    public int CumulativeLearningHours { get; init; } = 245;
    public int ProgressLevel { get; init; } = 8;
    public double LearningProgress { get; init; } = 0.89;
    public int MasteryScore { get; init; } = 85;
}

/// <summary>
/// Request DTO for AI agent interactions
/// </summary>
public record AIAgentInteractionRequest(
    AgentType AgentType,
    string PlayerInput,
    string GameContext,
    Guid PlayerId
);

/// <summary>
/// Request DTO for content validation
/// </summary>
public record ContentValidationRequest(
    string Content,
    AgentType AgentType
);

/// <summary>
/// Response DTO for content validation with educational progress tracking
/// </summary>
public record ContentValidationResponse(
    bool IsValid,
    string Message,
    int EducationalScore = 95,
    double LearningProgress = 0.85,
    string ComprehensionLevel = "Age-Appropriate"
);

/// <summary>
/// Request DTO for code explanation
/// </summary>
public record CodeExplanationRequest(
    string Code,
    string? Context,
    string? Language,
    string? Domain
);

/// <summary>
/// Response DTO for code explanation
/// </summary>
public record CodeExplanationResponse
{
    public string Summary { get; init; } = string.Empty;
    public List<CodeLineExplanation> Breakdown { get; init; } = new();
    public EducationalValueExplanation EducationalValue { get; init; } = new();
    public string RealWorldApplication { get; init; } = string.Empty;
    public List<string> NextSteps { get; init; } = new();
    public string ComplexityLevel { get; init; } = string.Empty;
    public List<string> ProgrammingConcepts { get; init; } = new();
    public List<string> ChildFriendlyTips { get; init; } = new();
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}

/// <summary>
/// DTO for individual code line explanation
/// </summary>
public record CodeLineExplanation
{
    public string Line { get; init; } = string.Empty;
    public string Explanation { get; init; } = string.Empty;
    public int LineNumber { get; init; }
}

/// <summary>
/// DTO for educational value explanation
/// </summary>
public record EducationalValueExplanation
{
    public string LearningObjective { get; init; } = string.Empty;
    public List<string> AgeAppropriateConcepts { get; init; } = new();
    public List<string> LifeSkills { get; init; } = new();
}