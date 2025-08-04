using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Models;

namespace WorldLeaders.Shared.Services;

/// <summary>
/// Service for territory acquisition, management, and educational content
/// Educational Focus: Geography learning, economic strategy, cultural awareness
/// Context: Educational game for 12-year-old players learning world geography and economics
/// </summary>
public interface ITerritoryService
{
    /// <summary>
    /// Get all available territories for acquisition
    /// Educational Objective: Introduce world geography and country diversity
    /// </summary>
    Task<List<TerritoryDto>> GetAvailableTerritoriesAsync(Guid playerId);
    
    /// <summary>
    /// Get territories owned by a specific player
    /// Educational Objective: Track geographic knowledge acquisition and empire building
    /// </summary>
    Task<List<TerritoryDto>> GetPlayerTerritoriesAsync(Guid playerId);
    
    /// <summary>
    /// Attempt to acquire a territory with reputation and income validation
    /// Educational Objective: Teach economic strategy and reputation building
    /// </summary>
    Task<TerritoryAcquisitionResult> AcquireTerritoryAsync(Guid playerId, Guid territoryId);
    
    /// <summary>
    /// Get detailed territory information including cultural context
    /// Educational Objective: Deep geography and cultural learning
    /// </summary>
    Task<TerritoryDetailDto> GetTerritoryDetailsAsync(Guid territoryId);
    
    /// <summary>
    /// Get territories filtered by tier for progressive difficulty
    /// Educational Objective: Scaffold learning from easy to challenging territories
    /// </summary>
    Task<List<TerritoryDto>> GetTerritoriesByTierAsync(TerritoryTier tier, Guid playerId);
    
    /// <summary>
    /// Calculate monthly income from all owned territories
    /// Educational Objective: Teach passive income and economic growth concepts
    /// </summary>
    Task<int> CalculateMonthlyTerritoryIncomeAsync(Guid playerId);
    
    /// <summary>
    /// Get language learning challenges for owned territories
    /// Educational Objective: Connect geography with language learning
    /// </summary>
    Task<List<LanguageChallengeDto>> GetTerritoryLanguageChallengesAsync(Guid playerId);
    
    /// <summary>
    /// Get cultural and historical context for educational content
    /// Educational Objective: Provide rich cultural learning beyond geography
    /// </summary>
    Task<CulturalContextDto> GetTerritoryCulturalContextAsync(Guid territoryId);
}

/// <summary>
/// Result of territory acquisition attempt with educational feedback
/// </summary>
public record TerritoryAcquisitionResult(
    bool Success,
    string Message,
    TerritoryDto? AcquiredTerritory,
    ResourceChangeRecord? ResourceChanges,
    Achievement? UnlockedAchievement,
    List<string> EducationalTips
);

/// <summary>
/// Detailed territory information with educational content
/// </summary>
public record TerritoryDetailDto(
    Guid Id,
    string CountryName,
    string CountryCode,
    List<string> OfficialLanguages,
    decimal GdpInBillions,
    TerritoryTier Tier,
    int Cost,
    int ReputationRequired,
    int MonthlyIncome,
    bool IsAvailable,
    bool IsOwned,
    string FlagUrl,
    string Capital,
    long Population,
    string Region,
    string Subregion,
    List<string> Currencies,
    string EducationalFact,
    List<string> GeographicFeatures,
    List<string> CulturalHighlights
);

/// <summary>
/// Cultural and historical educational content
/// </summary>
public record CulturalContextDto(
    Guid TerritoryId,
    string CountryName,
    string HistoricalSignificance,
    List<string> CulturalTraditions,
    List<string> FamousLandmarks,
    List<string> NotableAchievements,
    string GeographyLesson,
    string EconomicLesson,
    List<string> EducationalQuizQuestions,
    string ChildFriendlyDescription
);

