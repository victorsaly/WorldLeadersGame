using Microsoft.AspNetCore.Mvc;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.API.Controllers;

/// <summary>
/// Game controller for the World Leaders educational game API
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;
    private readonly IGameEngine _gameEngine;
    private readonly IPlayerService _playerService;
    private readonly IDiceService _diceService;
    private readonly ITerritoryService _territoryService;

    public GameController(
        ILogger<GameController> logger,
        IGameEngine gameEngine,
        IPlayerService playerService,
        IDiceService diceService,
        ITerritoryService territoryService)
    {
        _logger = logger;
        _gameEngine = gameEngine;
        _playerService = playerService;
        _diceService = diceService;
        _territoryService = territoryService;
    }

    /// <summary>
    /// Get health status for educational game systems
    /// </summary>
    /// <returns>Game systems health status</returns>
    [HttpGet("health")]
    public async Task<ActionResult<GameHealthResponse>> GetHealthStatus()
    {
        try
        {
            var checks = new List<GameHealthCheck>();
            var overallHealthy = true;

            // Check game engine
            var gameEngineHealthy = _gameEngine != null;
            checks.Add(new GameHealthCheck
            {
                Name = "GameEngine",
                IsHealthy = gameEngineHealthy,
                Description = gameEngineHealthy 
                    ? "Game engine is operational" 
                    : "Game engine is not available"
            });
            if (!gameEngineHealthy) overallHealthy = false;

            // Check player service
            var playerServiceHealthy = _playerService != null;
            checks.Add(new GameHealthCheck
            {
                Name = "PlayerService",
                IsHealthy = playerServiceHealthy,
                Description = playerServiceHealthy 
                    ? "Player service is operational" 
                    : "Player service is not available"
            });
            if (!playerServiceHealthy) overallHealthy = false;

            // Check territory service
            var territoryServiceHealthy = _territoryService != null;
            checks.Add(new GameHealthCheck
            {
                Name = "TerritoryService",
                IsHealthy = territoryServiceHealthy,
                Description = territoryServiceHealthy 
                    ? "Territory service is operational" 
                    : "Territory service is not available"
            });
            if (!territoryServiceHealthy) overallHealthy = false;

            // Test basic territory retrieval
            if (territoryServiceHealthy)
            {
                try
                {
                    var territories = _territoryService != null 
                        ? await _territoryService.GetAvailableTerritoriesAsync(Guid.Empty)
                        : new List<TerritoryDto>();
                    var territoryDataHealthy = territories.Any();
                    checks.Add(new GameHealthCheck
                    {
                        Name = "TerritoryData",
                        IsHealthy = territoryDataHealthy,
                        Description = territoryDataHealthy 
                            ? $"Territory data available ({territories?.Count ?? 0} territories)" 
                            : "Territory data not available"
                    });
                    if (!territoryDataHealthy) overallHealthy = false;
                }
                catch (Exception ex)
                {
                    checks.Add(new GameHealthCheck
                    {
                        Name = "TerritoryData",
                        IsHealthy = false,
                        Description = $"Territory data check failed: {ex.Message}"
                    });
                    overallHealthy = false;
                }
            }

            var response = new GameHealthResponse
            {
                Status = overallHealthy ? "Healthy" : "Unhealthy",
                Timestamp = DateTime.UtcNow,
                EducationalContext = "Geography, Economics, Language Learning for 12-year-olds",
                GameMode = "Educational Strategy Game",
                Checks = checks,
                TotalChecks = checks.Count,
                HealthyChecks = checks.Count(c => c.IsHealthy),
                OverallScore = (double)checks.Count(c => c.IsHealthy) / checks.Count,
                Message = overallHealthy 
                    ? "Educational game systems are operational and ready for learning"
                    : "Some educational game systems require attention"
            };

            var statusCode = overallHealthy ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable;
            
            _logger.LogInformation("Game health check completed: {Status} ({HealthyChecks}/{TotalChecks} systems healthy)", 
                response.Status, response.HealthyChecks, response.TotalChecks);

            return StatusCode(statusCode, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Game health check failed with exception");
            
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new GameHealthResponse
            {
                Status = "Critical",
                Timestamp = DateTime.UtcNow,
                EducationalContext = "Geography, Economics, Language Learning for 12-year-olds",
                GameMode = "Educational Strategy Game",
                Checks = new List<GameHealthCheck>
                {
                    new GameHealthCheck
                    {
                        Name = "SystemCheck",
                        IsHealthy = false,
                        Description = $"Critical system error: {ex.Message}"
                    }
                },
                TotalChecks = 1,
                HealthyChecks = 0,
                OverallScore = 0.0,
                Message = "Critical educational game system failure"
            });
        }
    }

    /// <summary>
    /// Create a new player for the educational game
    /// </summary>
    /// <param name="request">Player creation request</param>
    /// <returns>Player dashboard information</returns>
    [HttpPost("players")]
    public async Task<ActionResult<PlayerDashboardDto>> CreatePlayer([FromBody] CreatePlayerRequest request)
    {
        try
        {
            var player = await _playerService.CreatePlayerAsync(request);
            var dashboard = await _playerService.GetPlayerDashboardAsync(player.Id);
            var response = new PlayerTerritoriesEducationalResponse
            {
                Territories = dashboard.Territories,
                EducationalExplanation = "Congratulations! You have started your journey as a world leader. Each territory you acquire teaches you about geography, economics, and culture.",
                ProgressTip = "Next: Explore available territories and learn about their unique features."
            };
            _logger.LogInformation("Created new player: {Username} with ID: {PlayerId}", request.Username, player.Id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating player: {Username}", request.Username);
            return StatusCode(500, "An error occurred while creating the player");
        }
    }

    /// <summary>
    /// Get player dashboard information
    /// </summary>
    /// <param name="playerId">The player identifier</param>
    /// <returns>Player dashboard data</returns>
    [HttpGet("players/{playerId}/dashboard")]
    public async Task<ActionResult<PlayerDashboardDto>> GetPlayerDashboard(Guid playerId)
    {
        try
        {
            var dashboard = await _playerService.GetPlayerDashboardAsync(playerId);
            var response = new PlayerTerritoriesEducationalResponse
            {
                Territories = dashboard.Territories,
                EducationalExplanation = "Your dashboard shows your progress as a world leader. Each territory and achievement helps you learn about the world!",
                ProgressTip = "Keep playing to unlock new countries and skills."
            };
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard for player: {PlayerId}", playerId);
            return StatusCode(500, "An error occurred while retrieving player dashboard");
        }
    }

    /// <summary>
    /// Get available territories for purchase
    /// </summary>
    /// <returns>List of available territories</returns>
    [HttpGet("territories")]
    public async Task<ActionResult<IEnumerable<TerritoryDto>>> GetAvailableTerritories()
    {
        try
        {
            // Get available territories (using empty GUID for general territory list)
            // This gets all territories and filters for available ones
            var territories = await _territoryService.GetAvailableTerritoriesAsync(Guid.Empty);
            var response = new AvailableTerritoriesEducationalResponse
            {
                Territories = territories,
                EducationalExplanation = "Each territory represents a real country. Discover their geography, economy, and languages as you play!",
                ProgressTip = "Choose a territory to learn more and expand your leadership."
            };
            _logger.LogInformation("Retrieved {Count} available territories for educational game", territories.Count);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available territories");
            return StatusCode(500, "An error occurred while retrieving territories");
        }
    }

    /// <summary>
    /// Get random game events for educational content
    /// </summary>
    /// <returns>List of game events</returns>
    [HttpGet("events")]
    public ActionResult<IEnumerable<GameEventDto>> GetGameEvents()
    {
        try
        {
            // TODO: Create IGameEventService and inject it into controller
            // TODO: Implement proper event retrieval from database using GameEventSeeder data
            // For now, returning sample events with educational content for 12-year-olds
            var events = new List<GameEventDto>
            {
                new GameEventDto(
                    Id: Guid.NewGuid(),
                    Title: "Community Service Project",
                    Description: "You organized a successful community cleanup. Citizens are impressed!",
                    Type: EventType.Social,
                    IncomeEffect: 0,
                    ReputationEffect: 5,
                    HappinessEffect: 10,
                    IsPositive: true,
                    IconEmoji: "🌟"
                ),
                new GameEventDto(
                    Id: Guid.NewGuid(),
                    Title: "Learning New Skills",
                    Description: "You completed an online course to improve your job performance!",
                    Type: EventType.Career,
                    IncomeEffect: 200,
                    ReputationEffect: 3,
                    HappinessEffect: 5,
                    IsPositive: true,
                    IconEmoji: "📚"
                )
            };
            var response = new GameEventsEducationalResponse
            {
                Events = events,
                EducationalExplanation = "Game events help you learn about teamwork, leadership, and real-world problem solving.",
                ProgressTip = "Participate in events to boost your reputation and happiness!"
            };
            _logger.LogInformation("Retrieved {Count} sample game events for educational gameplay", events.Count);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving game events");
            return StatusCode(500, "An error occurred while retrieving game events");
        }
    }

    /// <summary>
    /// Simulate dice roll for job progression
    /// </summary>
    /// <param name="playerId">The player identifier</param>
    /// <returns>New job level based on dice roll</returns>
    [HttpPost("players/{playerId}/roll-job")]
    public async Task<ActionResult<DiceRollResult>> RollForJob(Guid playerId)
    {
        try
        {
            var result = await _diceService.RollForJobAsync(playerId);
            var response = new DiceRollEducationalResponse
            {
                DiceValue = result.DiceValue,
                NewJob = result.NewJob,
                EducationalExplanation = "Rolling the dice teaches you about probability and career progression. Each job helps you earn income and build reputation!",
                ProgressTip = "Next: Use your new job to earn more and unlock new territories."
            };
            _logger.LogInformation("Player {PlayerId} rolled {DiceValue} and got job {Job}", 
                playerId, result.DiceValue, result.NewJob);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling for job for player: {PlayerId}", playerId);
            return StatusCode(500, "An error occurred while rolling for job");
        }
    }

    /// <summary>
    /// Start a new game session
    /// </summary>
    /// <param name="playerId">The player identifier</param>
    /// <returns>Game session information</returns>
    [HttpPost("players/{playerId}/start-game")]
    public async Task<ActionResult<GameSession>> StartGame(Guid playerId)
    {
        try
        {
            var session = await _gameEngine.StartNewGameAsync(playerId);
            var response = new GameSessionEducationalResponse
            {
                Session = session,
                EducationalExplanation = "Welcome to your new game session! Every turn helps you learn about world leadership, geography, and economics.",
                ProgressTip = "Start by rolling the dice and exploring new territories."
            };
            _logger.LogInformation("Started new game session for player {PlayerId}", playerId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting game for player: {PlayerId}", playerId);
            return StatusCode(500, "An error occurred while starting the game");
        }
    }

    /// <summary>
    /// Advance to next turn
    /// </summary>
    /// <param name="playerId">The player identifier</param>
    /// <returns>Game state update</returns>
    [HttpPost("players/{playerId}/advance-turn")]
    public async Task<ActionResult<GameStateUpdate>> AdvanceTurn(Guid playerId)
    {
        try
        {
            var update = await _gameEngine.AdvanceTurnAsync(playerId);
            var response = new GameStateEducationalResponse
            {
                StateUpdate = update,
                EducationalExplanation = "Advancing your turn helps you learn about decision-making and strategy. Each choice affects your progress as a world leader!",
                ProgressTip = "Review your resources and plan your next move."
            };
            _logger.LogInformation("Advanced turn for player {PlayerId}", playerId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error advancing turn for player: {PlayerId}", playerId);
            return StatusCode(500, "An error occurred while advancing the turn");
        }
    }
}

/// <summary>
/// Game health response for educational platform monitoring
/// </summary>
public sealed record GameHealthResponse
{
    public string Status { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string EducationalContext { get; init; } = string.Empty;
    public string GameMode { get; init; } = string.Empty;
    public List<GameHealthCheck> Checks { get; init; } = new();
    public int TotalChecks { get; init; }
    public int HealthyChecks { get; init; }
    public double OverallScore { get; init; }
    public string Message { get; init; } = string.Empty;
}

/// <summary>
/// Individual game health check result
/// </summary>
public sealed record GameHealthCheck
{
    public string Name { get; init; } = string.Empty;
    public bool IsHealthy { get; init; }
    public string Description { get; init; } = string.Empty;
}
