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

    public GameController(
        ILogger<GameController> logger,
        IGameEngine gameEngine,
        IPlayerService playerService,
        IDiceService diceService)
    {
        _logger = logger;
        _gameEngine = gameEngine;
        _playerService = playerService;
        _diceService = diceService;
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
            // TODO: Implement territory retrieval from service layer
            var territories = new List<TerritoryDto>
            {
                new TerritoryDto(
                    Id: Guid.NewGuid(),
                    CountryName: "Nepal",
                    CountryCode: "NP",
                    OfficialLanguages: new List<string> { "Nepali", "Maithili" },
                    GdpInBillions: 36.3m,
                    Tier: TerritoryTier.Small,
                    Cost: 5000,
                    ReputationRequired: 10,
                    MonthlyIncome: 200,
                    IsAvailable: true,
                    IsOwned: false
                ),
                new TerritoryDto(
                    Id: Guid.NewGuid(),
                    CountryName: "Canada",
                    CountryCode: "CA",
                    OfficialLanguages: new List<string> { "English", "French" },
                    GdpInBillions: 2139.8m,
                    Tier: TerritoryTier.Medium,
                    Cost: 50000,
                    ReputationRequired: 40,
                    MonthlyIncome: 2000,
                    IsAvailable: true,
                    IsOwned: false
                )
            };

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
    public async Task<ActionResult<IEnumerable<GameEventDto>>> GetGameEvents()
    {
        try
        {
            // TODO: Implement event retrieval from service layer
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
