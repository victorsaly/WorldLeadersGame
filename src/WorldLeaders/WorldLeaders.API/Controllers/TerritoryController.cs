using Microsoft.AspNetCore.Mvc;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.API.Controllers;

/// <summary>
/// Territory controller for educational geography and economics learning
/// Context: Educational game for 12-year-old players learning world geography, economics, and culture
/// Educational Objective: Territory acquisition, cultural awareness, language learning
/// Safety: All responses validated for child-appropriate content
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TerritoryController : ControllerBase
{
    private readonly ILogger<TerritoryController> _logger;
    private readonly ITerritoryService _territoryService;
    private readonly IPlayerService _playerService;

    public TerritoryController(
        ILogger<TerritoryController> logger,
        ITerritoryService territoryService,
        IPlayerService playerService)
    {
        _logger = logger;
        _territoryService = territoryService;
        _playerService = playerService;
    }

    /// <summary>
    /// Get all territories available for acquisition
    /// Educational Objective: Introduce world geography and country diversity
    /// </summary>
    /// <param name="playerId">Player identifier</param>
    /// <returns>List of available territories with educational information</returns>
    [HttpGet("available/{playerId:guid}")]
    public async Task<ActionResult<List<TerritoryDto>>> GetAvailableTerritories(Guid playerId)
    {
        try
        {
            var territories = await _territoryService.GetAvailableTerritoriesAsync(playerId);
            _logger.LogInformation("Retrieved {Count} available territories for player {PlayerId}", 
                territories.Count, playerId);
            return Ok(territories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available territories for player {PlayerId}", playerId);
            return StatusCode(500, "Unable to retrieve territories. Please try again later.");
        }
    }

    /// <summary>
    /// Get territories owned by a specific player
    /// Educational Objective: Track geographic knowledge acquisition and empire building
    /// </summary>
    /// <param name="playerId">Player identifier</param>
    /// <returns>List of player-owned territories</returns>
    [HttpGet("owned/{playerId:guid}")]
    public async Task<ActionResult<List<TerritoryDto>>> GetPlayerTerritories(Guid playerId)
    {
        try
        {
            var territories = await _territoryService.GetPlayerTerritoriesAsync(playerId);
            _logger.LogInformation("Retrieved {Count} owned territories for player {PlayerId}", 
                territories.Count, playerId);
            return Ok(territories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting player territories for {PlayerId}", playerId);
            return StatusCode(500, "Unable to retrieve your territories. Please try again later.");
        }
    }

    /// <summary>
    /// Attempt to acquire a territory with educational validation
    /// Educational Objective: Teach economic strategy, reputation building, and geography
    /// </summary>
    /// <param name="playerId">Player identifier</param>
    /// <param name="territoryId">Territory to acquire</param>
    /// <returns>Acquisition result with educational feedback</returns>
    [HttpPost("acquire/{playerId:guid}/{territoryId:guid}")]
    public async Task<ActionResult<TerritoryAcquisitionResult>> AcquireTerritory(Guid playerId, Guid territoryId)
    {
        try
        {
            var result = await _territoryService.AcquireTerritoryAsync(playerId, territoryId);
            
            if (result.Success)
            {
                _logger.LogInformation("Player {PlayerId} successfully acquired territory {TerritoryId}", 
                    playerId, territoryId);
                return Ok(result);
            }
            else
            {
                _logger.LogInformation("Player {PlayerId} failed to acquire territory {TerritoryId}: {Message}", 
                    playerId, territoryId, result.Message);
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error acquiring territory {TerritoryId} for player {PlayerId}", 
                territoryId, playerId);
            return StatusCode(500, "Unable to acquire territory. Please try again later.");
        }
    }

    /// <summary>
    /// Get detailed territory information with cultural and educational content
    /// Educational Objective: Deep geography and cultural learning
    /// </summary>
    /// <param name="territoryId">Territory identifier</param>
    /// <returns>Detailed territory information with educational content</returns>
    [HttpGet("details/{territoryId:guid}")]
    public async Task<ActionResult<TerritoryDetailDto>> GetTerritoryDetails(Guid territoryId)
    {
        try
        {
            var details = await _territoryService.GetTerritoryDetailsAsync(territoryId);
            _logger.LogInformation("Retrieved details for territory {TerritoryId}", territoryId);
            return Ok(details);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Territory {TerritoryId} not found", territoryId);
            return NotFound("Territory not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting territory details for {TerritoryId}", territoryId);
            return StatusCode(500, "Unable to retrieve territory details. Please try again later.");
        }
    }

    /// <summary>
    /// Get territories by tier for progressive learning
    /// Educational Objective: Scaffold learning from easy to challenging territories
    /// </summary>
    /// <param name="tier">Territory tier (Small, Medium, Major)</param>
    /// <param name="playerId">Player identifier</param>
    /// <returns>Territories filtered by tier</returns>
    [HttpGet("tier/{tier}/{playerId:guid}")]
    public async Task<ActionResult<List<TerritoryDto>>> GetTerritoriesByTier(TerritoryTier tier, Guid playerId)
    {
        try
        {
            var territories = await _territoryService.GetTerritoriesByTierAsync(tier, playerId);
            _logger.LogInformation("Retrieved {Count} territories for tier {Tier} and player {PlayerId}", 
                territories.Count, tier, playerId);
            return Ok(territories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting territories by tier {Tier} for player {PlayerId}", 
                tier, playerId);
            return StatusCode(500, "Unable to retrieve territories. Please try again later.");
        }
    }

    /// <summary>
    /// Calculate monthly territory income for economic education
    /// Educational Objective: Teach passive income and economic growth concepts
    /// </summary>
    /// <param name="playerId">Player identifier</param>
    /// <returns>Total monthly income from territories</returns>
    [HttpGet("income/{playerId:guid}")]
    public async Task<ActionResult<int>> GetTerritoryIncome(Guid playerId)
    {
        try
        {
            var income = await _territoryService.CalculateMonthlyTerritoryIncomeAsync(playerId);
            _logger.LogInformation("Calculated territory income {Income} for player {PlayerId}", 
                income, playerId);
            return Ok(income);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating territory income for player {PlayerId}", playerId);
            return StatusCode(500, "Unable to calculate income. Please try again later.");
        }
    }

    /// <summary>
    /// Get language learning challenges for owned territories
    /// Educational Objective: Connect geography with language learning
    /// </summary>
    /// <param name="playerId">Player identifier</param>
    /// <returns>Language challenges for owned territories</returns>
    [HttpGet("language-challenges/{playerId:guid}")]
    public async Task<ActionResult<List<LanguageChallengeDto>>> GetLanguageChallenges(Guid playerId)
    {
        try
        {
            var challenges = await _territoryService.GetTerritoryLanguageChallengesAsync(playerId);
            _logger.LogInformation("Retrieved {Count} language challenges for player {PlayerId}", 
                challenges.Count, playerId);
            return Ok(challenges);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting language challenges for player {PlayerId}", playerId);
            return StatusCode(500, "Unable to retrieve language challenges. Please try again later.");
        }
    }

    /// <summary>
    /// Get cultural context for educational learning
    /// Educational Objective: Provide rich cultural learning beyond geography
    /// </summary>
    /// <param name="territoryId">Territory identifier</param>
    /// <returns>Cultural and historical educational content</returns>
    [HttpGet("cultural-context/{territoryId:guid}")]
    public async Task<ActionResult<CulturalContextDto>> GetCulturalContext(Guid territoryId)
    {
        try
        {
            var context = await _territoryService.GetTerritoryCulturalContextAsync(territoryId);
            _logger.LogInformation("Retrieved cultural context for territory {TerritoryId}", territoryId);
            return Ok(context);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Territory {TerritoryId} not found for cultural context", territoryId);
            return NotFound("Territory not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cultural context for territory {TerritoryId}", territoryId);
            return StatusCode(500, "Unable to retrieve cultural context. Please try again later.");
        }
    }

    /// <summary>
    /// Get player statistics with territory information for educational analytics
    /// Educational Objective: Show learning progress and achievements
    /// </summary>
    /// <param name="playerId">Player identifier</param>
    /// <returns>Player statistics including territory count and learning metrics</returns>
    [HttpGet("player-stats/{playerId:guid}")]
    public async Task<ActionResult<TerritoryPlayerStats>> GetPlayerTerritoryStats(Guid playerId)
    {
        try
        {
            var playerTerritories = await _territoryService.GetPlayerTerritoriesAsync(playerId);
            var monthlyIncome = await _territoryService.CalculateMonthlyTerritoryIncomeAsync(playerId);
            
            var stats = new TerritoryPlayerStats(
                playerId,
                playerTerritories.Count,
                monthlyIncome,
                playerTerritories.GroupBy(t => t.Tier).ToDictionary(g => g.Key, g => g.Count()),
                playerTerritories.SelectMany(t => t.OfficialLanguages).Distinct().Count(),
                playerTerritories.Select(t => GetContinent(t.CountryCode)).Distinct().Count()
            );

            _logger.LogInformation("Retrieved territory statistics for player {PlayerId}", playerId);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting territory statistics for player {PlayerId}", playerId);
            return StatusCode(500, "Unable to retrieve statistics. Please try again later.");
        }
    }

    /// <summary>
    /// Helper method to determine continent from country code for educational statistics
    /// </summary>
    private string GetContinent(string countryCode) => countryCode.ToUpper() switch
    {
        "US" or "CA" or "MX" or "GT" or "BZ" or "CR" or "PA" or "CU" or "JM" or "HT" or "DO" or "PR" or "TT" or "BB" or "GY" or "SR" or "BR" or "AR" or "CL" or "PE" or "EC" or "CO" or "VE" or "BO" or "PY" or "UY" => "Americas",
        "GB" or "FR" or "DE" or "IT" or "ES" or "PT" or "NL" or "BE" or "CH" or "AT" or "SE" or "NO" or "DK" or "FI" or "IS" or "IE" or "PL" or "CZ" or "SK" or "HU" or "RO" or "BG" or "GR" or "HR" or "SI" or "EE" or "LV" or "LT" or "LU" or "MT" or "CY" => "Europe",
        "CN" or "JP" or "IN" or "KR" or "TH" or "VN" or "PH" or "MY" or "SG" or "ID" or "MM" or "LA" or "KH" or "BN" or "NP" or "BT" or "LK" or "MV" or "MN" or "KZ" or "UZ" or "TJ" or "KG" or "AF" or "PK" or "BD" or "TR" or "IR" or "IQ" or "SY" or "JO" or "LB" or "IL" or "SA" or "AE" or "OM" or "YE" or "QA" or "BH" or "KW" => "Asia",
        "ZA" or "NG" or "EG" or "KE" or "ET" or "UG" or "TZ" or "ZW" or "ZM" or "MW" or "MZ" or "BW" or "NA" or "SZ" or "LS" or "MG" or "MU" or "SC" or "KM" or "DZ" or "LY" or "TN" or "MA" or "EH" or "ML" or "BF" or "NE" or "TD" or "SD" or "SS" or "CF" or "CM" or "GQ" or "GA" or "CG" or "CD" or "AO" or "GH" or "TG" or "BJ" or "CI" or "LR" or "SL" or "GN" or "GW" or "SN" or "GM" or "CV" or "ST" or "ER" or "DJ" or "SO" or "RW" or "BI" => "Africa",
        "AU" or "NZ" or "FJ" or "PG" or "SB" or "VU" or "NC" or "PF" or "WS" or "TO" or "TV" or "KI" or "NR" or "PW" or "FM" or "MH" => "Oceania",
        _ => "Unknown"
    };
}

/// <summary>
/// Player territory statistics for educational progress tracking
/// </summary>
public record TerritoryPlayerStats(
    Guid PlayerId,
    int TotalTerritories,
    int MonthlyTerritoryIncome,
    Dictionary<TerritoryTier, int> TerritoriesByTier,
    int LanguagesExplored,
    int ContinentsRepresented
);