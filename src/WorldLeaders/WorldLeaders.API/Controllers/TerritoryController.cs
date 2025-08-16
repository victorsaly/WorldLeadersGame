using Microsoft.AspNetCore.Mvc;
using WorldLeaders.Shared.DTOs;
// using WorldLeaders.Shared.Services; // Duplicate directive removed
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

            // Educational explanation for 12-year-olds
            var explanation = "Explore the world! Each territory represents a real country. Discover new places, learn about their cultures, and see how geography shapes our planet. Every country has its own flag, language, and unique story. Try to collect territories from different continents and learn something new about each one!";

            var response = new AvailableTerritoriesEducationalResponse
            {
                Territories = territories,
                EducationalExplanation = explanation,
                ProgressTip = $"You have {territories.Count} territories available. Try to collect at least one from each continent!"
            };
            return Ok(response);
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
            _logger.LogInformation("Retrieved {Count} owned territories for player {PlayerId}", territories.Count, playerId);
            var response = new PlayerTerritoriesEducationalResponse
            {
                Territories = territories,
                EducationalExplanation = "These are the territories you own! Each country teaches you about its geography, culture, and economy. Try to expand your collection and learn something new from every place.",
                ProgressTip = $"You currently own {territories.Count} territories. Aim to collect more and explore new continents!"
            };
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting player territories for {PlayerId}", playerId);
            return StatusCode(500, "Unable to retrieve your territories. Please try again later.");
        }
    }
/// <summary>
/// Educational response for available territories
/// </summary>


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
            var response = new TerritoryAcquisitionEducationalResponse
            {
                Result = result,
                EducationalExplanation = result.Success
                    ? "Great job! Acquiring a territory teaches you about economic strategy and reputation. Each new country expands your knowledge of the world."
                    : "Not every attempt succeeds, but every try helps you learn about strategy and planning. Keep going!",
                ProgressTip = result.Success
                    ? "Your territory collection is growing. Try to acquire territories from different tiers for a balanced empire!"
                    : "Review your reputation and resources, then try again for a new territory."
            };
            if (result.Success)
                return Ok(response);
            else
                return BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error acquiring territory {TerritoryId} for player {PlayerId}", territoryId, playerId);
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
            var response = new TerritoryDetailEducationalResponse
            {
                Details = details,
                EducationalExplanation = "Learn about this country's geography, history, and culture. Every detail helps you understand the world better!",
                ProgressTip = "Explore the details of each territory to unlock new learning achievements."
            };
            return Ok(response);
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
            _logger.LogInformation("Retrieved {Count} territories for tier {Tier} and player {PlayerId}", territories.Count, tier, playerId);
            var response = new TerritoriesByTierEducationalResponse
            {
                Territories = territories,
                Tier = tier,
                EducationalExplanation = "Territory tiers help you learn progressively. Start with small countries and work your way up to major powers!",
                ProgressTip = $"You have {territories.Count} territories in the {tier} tier. Try to collect all tiers for a complete learning experience!"
            };
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting territories by tier {Tier} for player {PlayerId}", tier, playerId);
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
            _logger.LogInformation("Calculated territory income {Income} for player {PlayerId}", income, playerId);
            var response = new TerritoryIncomeEducationalResponse
            {
                Income = income,
                EducationalExplanation = "Monthly income from your territories teaches you about passive income and economic growth. Use your earnings to expand your empire!",
                ProgressTip = $"Your current monthly income is {income}. Try to increase it by acquiring more valuable territories!"
            };
            return Ok(response);
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
            _logger.LogInformation("Retrieved {Count} language challenges for player {PlayerId}", challenges.Count, playerId);
            var response = new LanguageChallengesEducationalResponse
            {
                Challenges = challenges,
                EducationalExplanation = "Language challenges help you connect geography with language learning. Practice new words and discover the languages spoken in your territories!",
                ProgressTip = $"You have {challenges.Count} language challenges. Try to complete them all for a language mastery badge!"
            };
            return Ok(response);
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
            var response = new CulturalContextEducationalResponse
            {
                Context = context,
                EducationalExplanation = "Cultural context helps you appreciate the history, traditions, and achievements of each country. Explore and celebrate global diversity!",
                ProgressTip = "Learn about the cultural context of every territory to unlock the World Explorer achievement."
            };
            return Ok(response);
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
            var continentsRepresented = playerTerritories.Select(t => GetContinent(t.CountryCode)).Distinct().Count();
            var stats = new PlayerTerritoryStatsEducationalResponse
            {
                PlayerId = playerId,
                TotalTerritories = playerTerritories.Count,
                MonthlyTerritoryIncome = monthlyIncome,
                TerritoriesByTier = playerTerritories.GroupBy(t => t.Tier).ToDictionary(g => g.Key, g => g.Count()),
                LanguagesExplored = playerTerritories.SelectMany(t => t.OfficialLanguages).Distinct().Count(),
                ContinentsRepresented = continentsRepresented,
                EducationalExplanation = "Your territory stats show your progress as a world leader! Track your achievements and set new learning goals.",
                ProgressTip = $"You have explored {playerTerritories.Count} territories, {monthlyIncome} monthly income, and {continentsRepresented} continents. Keep learning and expanding!"
            };
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