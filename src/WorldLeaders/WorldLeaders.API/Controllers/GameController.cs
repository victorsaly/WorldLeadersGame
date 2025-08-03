using Microsoft.AspNetCore.Mvc;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;

namespace WorldLeaders.API.Controllers;

/// <summary>
/// Game controller for the World Leaders educational game API
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;

    public GameController(ILogger<GameController> logger)
    {
        _logger = logger;
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
            // TODO: Implement player creation logic with service layer
            var playerId = Guid.NewGuid();
            
            var playerDashboard = new PlayerDashboardDto(
                Id: playerId,
                Username: request.Username,
                Income: 1000, // Start with farmer income
                Reputation: 0,
                Happiness: 50, // Start with neutral happiness
                CurrentJob: JobLevel.Farmer,
                TerritoriesOwned: 0,
                CurrentGameState: GameState.NotStarted,
                LastActiveAt: DateTime.UtcNow
            );

            _logger.LogInformation("Created new player: {Username} with ID: {PlayerId}", request.Username, playerId);
            return Ok(playerDashboard);
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
            // TODO: Implement dashboard data retrieval from service layer
            var dashboard = new PlayerDashboardDto(
                Id: playerId,
                Username: "SamplePlayer",
                Income: 2500,
                Reputation: 25,
                Happiness: 65,
                CurrentJob: JobLevel.Artisan,
                TerritoriesOwned: 2,
                CurrentGameState: GameState.InProgress,
                LastActiveAt: DateTime.UtcNow
            );

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
    public async Task<ActionResult<GameStateUpdate>> RollForJob(Guid playerId)
    {
        try
        {
            var random = new Random();
            var diceRoll = random.Next(1, 7); // 1-6 dice roll
            
            JobLevel newJob = diceRoll switch
            {
                1 or 2 => diceRoll == 1 ? JobLevel.Farmer : JobLevel.Gardener,
                3 or 4 => diceRoll == 3 ? JobLevel.Shopkeeper : JobLevel.Artisan,
                5 or 6 => diceRoll == 5 ? JobLevel.Politician : JobLevel.BusinessLeader,
                _ => JobLevel.Farmer
            };

            int incomeChange = newJob switch
            {
                JobLevel.Farmer => 1000,
                JobLevel.Gardener => 1200,
                JobLevel.Shopkeeper => 2000,
                JobLevel.Artisan => 2500,
                JobLevel.Politician => 4000,
                JobLevel.BusinessLeader => 5000,
                _ => 1000
            };

            var update = new GameStateUpdate(
                IncomeChange: incomeChange,
                ReputationChange: 0,
                HappinessChange: 5, // Players are happy with new jobs
                Message: $"🎲 You rolled a {diceRoll}! Your new job is {newJob}. Monthly income: ${incomeChange:N0}",
                EventType: EventType.Career
            );

            _logger.LogInformation("Player {PlayerId} rolled {DiceRoll} and got job {Job}", playerId, diceRoll, newJob);
            return Ok(update);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling for job for player: {PlayerId}", playerId);
            return StatusCode(500, "An error occurred while rolling for job");
        }
    }
}
