using Microsoft.AspNetCore.Mvc;
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