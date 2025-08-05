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

            _logger.LogInformation("Created new player: {Username} with ID: {PlayerId}", request.Username, player.Id);
            return Ok(dashboard);
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
            return Ok(dashboard);
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
            
            _logger.LogInformation("Retrieved {Count} available territories for educational game", territories.Count);
            return Ok(territories);
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

            _logger.LogInformation("Retrieved {Count} sample game events for educational gameplay", events.Count);
            return Ok(events);
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
            
            _logger.LogInformation("Player {PlayerId} rolled {DiceValue} and got job {Job}", 
                playerId, result.DiceValue, result.NewJob);
            
            return Ok(result);
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
            
            _logger.LogInformation("Started new game session for player {PlayerId}", playerId);
            
            return Ok(session);
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
            
            _logger.LogInformation("Advanced turn for player {PlayerId}", playerId);
            
            return Ok(update);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error advancing turn for player: {PlayerId}", playerId);
            return StatusCode(500, "An error occurred while advancing the turn");
        }
    }
}
