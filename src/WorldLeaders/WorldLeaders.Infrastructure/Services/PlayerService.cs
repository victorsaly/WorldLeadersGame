using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WorldLeaders.Infrastructure.Data;
using WorldLeaders.Infrastructure.Entities;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Models;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.Infrastructure.Services;

/// <summary>
/// Player service implementation for profile management and educational tracking
/// Context: Educational game for 12-year-old players with achievement motivation
/// Safety: COPPA-compliant data handling, child privacy protection
/// </summary>
public class PlayerService : IPlayerService
{
    private readonly WorldLeadersDbContext _context;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(WorldLeadersDbContext context, ILogger<PlayerService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Player> CreatePlayerAsync(CreatePlayerRequest request)
    {
        try
        {
            // Validate username for child safety
            if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Length < 3)
            {
                throw new ArgumentException("Username must be at least 3 characters long");
            }

            // Check if username already exists
            var existingPlayer = await _context.Players
                .FirstOrDefaultAsync(p => p.Username == request.Username && !p.IsDeleted);

            if (existingPlayer != null)
            {
                throw new InvalidOperationException("Username already exists");
            }

            // Create new player with educational starting values
            var playerEntity = new PlayerEntity
            {
                Username = request.Username,
                Income = 1000, // Start as farmer
                Reputation = 0, // Build reputation through gameplay
                Happiness = 50, // Start with neutral happiness
                CurrentJob = JobLevel.Farmer,
                CurrentGameState = GameState.InProgress, // Game starts immediately
                GameStartedAt = DateTime.UtcNow,
                LastActiveAt = DateTime.UtcNow
            };

            _context.Players.Add(playerEntity);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created new player: {Username} with ID: {PlayerId}",
                request.Username, playerEntity.Id);

            return MapToPlayer(playerEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating player: {Username}", request.Username);
            throw;
        }
    }

    public async Task<Player?> GetPlayerAsync(Guid playerId)
    {
        try
        {
            var playerEntity = await _context.Players
                .FirstOrDefaultAsync(p => p.Id == playerId && !p.IsDeleted);

            return playerEntity == null ? null : MapToPlayer(playerEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting player {PlayerId}", playerId);
            return null;
        }
    }

    public async Task<Player> UpdatePlayerAsync(Player player)
    {
        try
        {
            var existingPlayer = await _context.Players.FindAsync(player.Id);
            if (existingPlayer == null)
            {
                throw new ArgumentException($"Player {player.Id} not found");
            }

            // Update player data
            existingPlayer.Username = player.Username;
            existingPlayer.Income = player.Income;
            existingPlayer.Reputation = Math.Clamp(player.Reputation, 0, 100);
            existingPlayer.Happiness = Math.Clamp(player.Happiness, 0, 100);
            existingPlayer.CurrentJob = player.CurrentJob;
            existingPlayer.CurrentGameState = player.CurrentGameState;
            existingPlayer.LastActiveAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated player {PlayerId}", player.Id);

            return MapToPlayer(existingPlayer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating player {PlayerId}", player.Id);
            throw;
        }
    }

    public async Task<PlayerDashboardDto> GetPlayerDashboardAsync(Guid playerId)
    {
        try
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
            {
                throw new ArgumentException($"Player {playerId} not found");
            }

            var territoriesOwned = await _context.Territories
                .CountAsync(t => t.OwnedByPlayerId == playerId);

            return new PlayerDashboardDto(
                player.Id,
                player.Username,
                player.Income,
                player.Reputation,
                player.Happiness,
                player.CurrentJob,
                territoriesOwned,
                player.CurrentGameState,
                player.LastActiveAt
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard for player {PlayerId}", playerId);
            throw;
        }
    }

    public async Task UpdatePlayerStatsAsync(Guid playerId, ResourceChange change)
    {
        try
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
            {
                throw new ArgumentException($"Player {playerId} not found");
            }

            // Apply changes with proper validation
            player.Income = Math.Max(0, player.Income + change.IncomeChange);
            player.Reputation = Math.Clamp(player.Reputation + change.ReputationChange, 0, 100);
            player.Happiness = Math.Clamp(player.Happiness + change.HappinessChange, 0, 100);
            player.LastActiveAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated stats for player {PlayerId}: {Change}", playerId, change);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stats for player {PlayerId}", playerId);
            throw;
        }
    }

    public async Task<List<Achievement>> GetPlayerAchievementsAsync(Guid playerId)
    {
        try
        {
            var achievements = await _context.PlayerAchievements
                .Where(a => a.PlayerId == playerId)
                .OrderByDescending(a => a.UnlockedAt)
                .ToListAsync();

            return achievements.Select(a => new Achievement(
                a.AchievementId,
                a.Title,
                a.Description,
                a.IconEmoji,
                a.PointsReward,
                a.UnlockedAt,
                true
            )).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting achievements for player {PlayerId}", playerId);
            return new List<Achievement>();
        }
    }

    public async Task<bool> AwardAchievementAsync(Guid playerId, string achievementId)
    {
        try
        {
            // Check if player already has this achievement
            var existingAchievement = await _context.PlayerAchievements
                .FirstOrDefaultAsync(a => a.PlayerId == playerId && a.AchievementId == achievementId);

            if (existingAchievement != null)
            {
                return false; // Already has this achievement
            }

            // Get achievement definition
            var achievementDef = GetAchievementDefinition(achievementId);
            if (achievementDef == null)
            {
                return false;
            }

            // Award the achievement
            var achievementEntity = new PlayerAchievementEntity
            {
                PlayerId = playerId,
                AchievementId = achievementId,
                Title = achievementDef.Title,
                Description = achievementDef.Description,
                IconEmoji = achievementDef.IconEmoji,
                PointsReward = achievementDef.PointsReward,
                UnlockedAt = DateTime.UtcNow
            };

            _context.PlayerAchievements.Add(achievementEntity);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Awarded achievement {AchievementId} to player {PlayerId}",
                achievementId, playerId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error awarding achievement {AchievementId} to player {PlayerId}",
                achievementId, playerId);
            return false;
        }
    }

    public async Task<PlayerAnalytics> GetPlayerAnalyticsAsync(Guid playerId)
    {
        try
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
            {
                throw new ArgumentException($"Player {playerId} not found");
            }

            // Get dice roll statistics
            var diceRolls = await _context.DiceRollHistory
                .Where(d => d.PlayerId == playerId)
                .ToListAsync();

            var jobHistory = diceRolls
                .GroupBy(d => d.ResultingJob)
                .ToDictionary(g => g.Key, g => g.Count());

            var territoriesAcquired = await _context.Territories
                .CountAsync(t => t.OwnedByPlayerId == playerId);

            var languagesChallenged = await _context.LanguageProgress
                .Where(l => l.PlayerId == playerId)
                .CountAsync();

            return new PlayerAnalytics(
                playerId,
                diceRolls.Count,
                jobHistory,
                territoriesAcquired,
                languagesChallenged,
                CalculateAverageSessionLength(player),
                player.CreatedAt,
                player.LastActiveAt,
                GetLearningObjectivesMet(player, territoriesAcquired, diceRolls.Count)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting analytics for player {PlayerId}", playerId);
            throw;
        }
    }

    private Player MapToPlayer(PlayerEntity entity)
    {
        return new Player
        {
            Id = entity.Id,
            Username = entity.Username,
            Income = entity.Income,
            Reputation = entity.Reputation,
            Happiness = entity.Happiness,
            CurrentJob = entity.CurrentJob,
            CurrentGameState = entity.CurrentGameState,
            GameStartedAt = entity.GameStartedAt,
            LastActiveAt = entity.LastActiveAt,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            IsDeleted = entity.IsDeleted
        };
    }

    private Achievement? GetAchievementDefinition(string achievementId)
    {
        // Pre-defined educational achievements
        var achievements = new Dictionary<string, Achievement>
        {
            ["first_roll"] = new("first_roll", "First Dice Roll", "Rolled your first dice! Every journey begins with a single step.", "🎲", 100, null, false),
            ["job_master"] = new("job_master", "Job Master", "Experienced all job levels! You understand different careers.", "💼", 500, null, false),
            ["territory_owner"] = new("territory_owner", "Territory Owner", "Acquired your first territory! You're building your empire.", "🏴", 250, null, false),
            ["world_leader"] = new("world_leader", "World Leader", "Achieved high reputation and multiple territories!", "👑", 1000, null, false),
            ["happy_people"] = new("happy_people", "People's Champion", "Maintained high happiness for your population!", "😊", 300, null, false)
        };

        return achievements.TryGetValue(achievementId, out var achievement) ? achievement : null;
    }

    private double CalculateAverageSessionLength(PlayerEntity player)
    {
        // Simplified calculation - in real implementation, would track session data
        var daysSinceStart = (DateTime.UtcNow - player.CreatedAt).TotalDays;
        return daysSinceStart > 0 ? Math.Max(1, daysSinceStart) : 1;
    }

    private List<string> GetLearningObjectivesMet(PlayerEntity player, int territories, int diceRolls)
    {
        var objectives = new List<string>();

        if (diceRolls > 0) objectives.Add("Career exploration through dice mechanics");
        if (player.Reputation > 25) objectives.Add("Understanding reputation building");
        if (player.Happiness > 60) objectives.Add("Learning population happiness management");
        if (territories > 0) objectives.Add("Geography learning through territory acquisition");
        if (player.Income > 2000) objectives.Add("Basic economics through income progression");

        return objectives;
    }
}