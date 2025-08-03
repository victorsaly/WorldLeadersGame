using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Models;

namespace WorldLeaders.Shared.Services;

/// <summary>
/// Service for managing player profiles, statistics, and achievements
/// Educational focus: Track learning progress and encourage continued engagement
/// </summary>
public interface IPlayerService
{
    /// <summary>
    /// Create a new player profile
    /// </summary>
    Task<Player> CreatePlayerAsync(CreatePlayerRequest request);
    
    /// <summary>
    /// Get player by ID
    /// </summary>
    Task<Player?> GetPlayerAsync(Guid playerId);
    
    /// <summary>
    /// Update player profile
    /// </summary>
    Task<Player> UpdatePlayerAsync(Player player);
    
    /// <summary>
    /// Get player dashboard data
    /// </summary>
    Task<PlayerDashboardDto> GetPlayerDashboardAsync(Guid playerId);
    
    /// <summary>
    /// Update player statistics
    /// </summary>
    Task UpdatePlayerStatsAsync(Guid playerId, ResourceChange change);
    
    /// <summary>
    /// Get player achievements
    /// </summary>
    Task<List<Achievement>> GetPlayerAchievementsAsync(Guid playerId);
    
    /// <summary>
    /// Award achievement to player
    /// </summary>
    Task<bool> AwardAchievementAsync(Guid playerId, string achievementId);
    
    /// <summary>
    /// Get player learning analytics
    /// </summary>
    Task<PlayerAnalytics> GetPlayerAnalyticsAsync(Guid playerId);
}

/// <summary>
/// Player achievement system for educational motivation
/// </summary>
public record Achievement(
    string Id,
    string Title,
    string Description,
    string IconEmoji,
    int PointsReward,
    DateTime? UnlockedAt,
    bool IsUnlocked
);

/// <summary>
/// Player learning and engagement analytics
/// </summary>
public record PlayerAnalytics(
    Guid PlayerId,
    int TotalDiceRolls,
    Dictionary<JobLevel, int> JobHistory,
    int TerritoriesAcquired,
    int LanguagesChallenged,
    double AverageSessionLength,
    DateTime FirstGameDate,
    DateTime LastActiveDate,
    List<string> LearningObjectivesMet
);