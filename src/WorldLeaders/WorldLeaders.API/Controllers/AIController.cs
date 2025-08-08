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
            // Validate request
            if (request == null || string.IsNullOrWhiteSpace(request.PlayerInput))
            {
                return BadRequest("Invalid request: Player input is required");
            }

            // Log interaction for safety monitoring
            _logger.LogInformation("AI interaction request - Agent: {AgentType}, Player: {PlayerId}", 
                request.AgentType, request.PlayerId);

            // Generate AI response with safety validation
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
            // Log error without sensitive user input to protect child privacy
            _logger.LogError(ex, "Error generating AI response for agent {AgentType} from player {PlayerId}", 
                request?.AgentType, request?.PlayerId);
            
            // Always provide safe fallback on error
            var fallbackResponse = await _aiAgentService.GetSafeFallbackResponseAsync(
                request?.AgentType ?? AgentType.CareerGuide, 
                "API error"
            );
            
            return Ok(fallbackResponse);
        }
    }

    /// <summary>
    /// Get personality information for a specific AI agent
    /// </summary>
    /// <param name="agentType">The agent type to get information for</param>
    /// <returns>Agent personality information for UI display</returns>
    [HttpGet("personality/{agentType}")]
    public async Task<ActionResult<AgentPersonalityInfo>> GetAgentPersonality(AgentType agentType)
    {
        try
        {
            var personalityInfo = await _aiAgentService.GetAgentPersonalityAsync(agentType);
            return Ok(personalityInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving personality for agent {AgentType}", agentType);
            return StatusCode(500, "Error retrieving agent personality information");
        }
    }

    /// <summary>
    /// Get all available AI agent personalities for UI display
    /// </summary>
    /// <returns>List of all agent personality information</returns>
    [HttpGet("personalities")]
    public async Task<ActionResult<List<AgentPersonalityInfo>>> GetAllAgentPersonalities()
    {
        try
        {
            var agentTypes = Enum.GetValues<AgentType>();
            var personalities = new List<AgentPersonalityInfo>();

            foreach (var agentType in agentTypes)
            {
                var personality = await _aiAgentService.GetAgentPersonalityAsync(agentType);
                personalities.Add(personality);
            }

            return Ok(personalities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all agent personalities");
            return StatusCode(500, "Error retrieving agent personalities");
        }
    }

    /// <summary>
    /// Validate if a message is safe for children (utility endpoint for client-side validation)
    /// </summary>
    /// <param name="request">Content validation request</param>
    /// <returns>Safety validation result</returns>
    [HttpPost("validate")]
    public async Task<ActionResult<ContentValidationResponse>> ValidateContent([FromBody] ContentValidationRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("Content is required for validation");
            }

            var isValid = await _aiAgentService.ValidateResponseSafetyAsync(request.Content, request.AgentType);
            
            return Ok(new ContentValidationResponse(
                IsValid: isValid,
                Message: isValid ? "Content is safe for children" : "Content needs review for child safety"
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating content safety");
            return Ok(new ContentValidationResponse(
                IsValid: false,
                Message: "Content validation failed - please try again"
            ));
        }
    }

    /// <summary>
    /// Explain code in child-friendly language for educational purposes
    /// Designed for 12-year-old learners in our educational game
    /// </summary>
    /// <param name="request">Code explanation request</param>
    /// <returns>Educational code explanation optimized for children</returns>
    [HttpPost("explain-code")]
    [ProducesResponseType(typeof(CodeExplanationResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ExplainCode([FromBody] CodeExplanationRequest request)
    {
        try
        {
            // Validate request
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return BadRequest(new { 
                    error = "Code content is required",
                    child_friendly_message = "Oops! We need some code to explain! 😅"
                });
            }

            // Validate domain if provided (CORS alternative)
            if (!string.IsNullOrEmpty(request.Domain))
            {
                var authorizedDomains = new[] { 
                    "localhost", 
                    "127.0.0.1", 
                    "docs.worldleadersgame.co.uk",
                    "worldleadersgame.co.uk"
                };
                
                if (!authorizedDomains.Any(d => request.Domain.Contains(d)))
                {
                    _logger.LogWarning("Unauthorized domain attempted code explanation: {Domain}", request.Domain);
                    return Forbid("Domain not authorized for educational content");
                }
            }

            _logger.LogInformation("Generating educational code explanation for {Language} code", 
                request.Language ?? "unknown");

            // Generate child-friendly explanation using AI service
            var explanation = await _aiAgentService.GenerateCodeExplanationAsync(
                request.Code, 
                request.Context ?? "Educational programming lesson",
                request.Language ?? "general"
            );

            var response = new CodeExplanationResponse
            {
                Summary = explanation.Summary,
                Breakdown = explanation.Breakdown.Select(b => new CodeLineExplanation
                {
                    Line = b.Line,
                    Explanation = b.Explanation,
                    LineNumber = b.LineNumber
                }).ToList(),
                EducationalValue = new EducationalValueExplanation
                {
                    LearningObjective = explanation.EducationalValue.LearningObjective,
                    AgeAppropriateConcepts = explanation.EducationalValue.AgeAppropriateConcepts,
                    LifeSkills = explanation.EducationalValue.LifeSkills
                },
                RealWorldApplication = explanation.RealWorldApplication,
                NextSteps = explanation.NextSteps,
                ComplexityLevel = explanation.ComplexityLevel,
                ProgrammingConcepts = explanation.ProgrammingConcepts,
                ChildFriendlyTips = explanation.ChildFriendlyTips,
                Success = true,
                Message = "Here's your code explanation! 🧑‍🏫"
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating code explanation");
            
            // Return child-friendly fallback response
            var fallbackResponse = new CodeExplanationResponse
            {
                Summary = "This code helps our educational game teach you about geography and economics while having fun! 🌍",
                Breakdown = new List<CodeLineExplanation>
                {
                    new CodeLineExplanation
                    {
                        Line = "// Code explanation temporarily unavailable",
                        Explanation = "Don't worry - learning is a journey with ups and downs! 🌟",
                        LineNumber = 1
                    }
                },
                EducationalValue = new EducationalValueExplanation
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
                ChildFriendlyTips = new List<string>
                {
                    "💡 It's okay when things don't work perfectly - that's how we learn!",
                    "🚀 Every expert was once a beginner!",
                    "📚 Learning takes time and practice!"
                },
                Success = false,
                Message = "Something went wrong, but that's okay! Learning has ups and downs! 🌟"
            };

            return Ok(fallbackResponse);
        }
    }
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
/// Response DTO for content validation
/// </summary>
public record ContentValidationResponse(
    bool IsValid,
    string Message
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