using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WorldLeaders.Infrastructure.Data;
using WorldLeaders.Infrastructure.Entities;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Models;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.Infrastructure.Services;

/// <summary>
/// Core game engine implementation for educational World Leaders game
/// Context: Educational game for 12-year-old players learning geography, economics, and languages
/// Safety: Child-appropriate content, positive messaging, educational value
/// </summary>
public class GameEngine : IGameEngine
{
    private readonly WorldLeadersDbContext _context;
    private readonly IDiceService _diceService;
    private readonly ILogger<GameEngine> _logger;

    public GameEngine(
        WorldLeadersDbContext context,
        IDiceService diceService,
        ILogger<GameEngine> logger)
    {
        _context = context;
        _diceService = diceService;
        _logger = logger;
    }

    public async Task<GameSession> StartNewGameAsync(Guid playerId)
    {
        try
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
            {
                throw new ArgumentException($"Player {playerId} not found");
            }

            // Create new game session
            var session = new GameSessionEntity
            {
                PlayerId = playerId,
                State = GameState.InProgress,
                CurrentTurn = 1,
                StartedAt = DateTime.UtcNow,
                LastSavedAt = DateTime.UtcNow,
                GameDataJson = JsonSerializer.Serialize(new { InitialState = true })
            };

            _context.GameSessions.Add(session);

            // Update player state
            player.CurrentGameState = GameState.InProgress;
            player.GameStartedAt = DateTime.UtcNow;
            player.LastActiveAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Started new game session for player {PlayerId}", playerId);

            return new GameSession(
                playerId,
                session.Id,
                GameState.InProgress,
                1,
                DateTime.UtcNow,
                DateTime.UtcNow,
                MapToPlayer(player)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting new game for player {PlayerId}", playerId);
            throw;
        }
    }

    public async Task<GameSession?> LoadGameSessionAsync(Guid playerId)
    {
        try
        {
            var session = await _context.GameSessions
                .Where(s => s.PlayerId == playerId && !s.IsDeleted)
                .OrderByDescending(s => s.LastSavedAt)
                .FirstOrDefaultAsync();

            if (session == null)
            {
                return null;
            }

            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
            {
                return null;
            }

            return new GameSession(
                playerId,
                session.Id,
                session.State,
                session.CurrentTurn,
                session.StartedAt,
                session.LastSavedAt,
                MapToPlayer(player)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading game session for player {PlayerId}", playerId);
            return null;
        }
    }

    public async Task<bool> SaveGameSessionAsync(GameSession session)
    {
        try
        {
            var existingSession = await _context.GameSessions.FindAsync(session.SessionId);
            if (existingSession == null)
            {
                return false;
            }

            existingSession.State = session.State;
            existingSession.CurrentTurn = session.CurrentTurn;
            existingSession.LastSavedAt = DateTime.UtcNow;
            existingSession.GameDataJson = JsonSerializer.Serialize(session.PlayerData);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Saved game session {SessionId} for player {PlayerId}", 
                session.SessionId, session.PlayerId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving game session {SessionId}", session.SessionId);
            return false;
        }
    }

    public async Task<GameStateUpdate> AdvanceTurnAsync(Guid playerId)
    {
        try
        {
            var session = await LoadGameSessionAsync(playerId);
            if (session == null)
            {
                throw new InvalidOperationException("No active game session found");
            }

            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
            {
                throw new ArgumentException($"Player {playerId} not found");
            }

            // Advance turn counter
            var sessionEntity = await _context.GameSessions.FindAsync(session.SessionId);
            if (sessionEntity != null)
            {
                sessionEntity.CurrentTurn++;
                sessionEntity.LastSavedAt = DateTime.UtcNow;
            }

            // Apply monthly income
            int monthlyIncome = CalculateMonthlyIncome(player);
            player.Income += monthlyIncome;
            player.LastActiveAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Advanced turn for player {PlayerId} to turn {Turn}", 
                playerId, sessionEntity?.CurrentTurn);

            return new GameStateUpdate(
                monthlyIncome,
                0,
                2, // Small happiness boost for completing a turn
                $"🗓️ Month completed! You earned ${monthlyIncome:N0} from your job and territories. Keep going!",
                EventType.Economic
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error advancing turn for player {PlayerId}", playerId);
            throw;
        }
    }

    public async Task<DiceRollResult> ProcessDiceRollAsync(Guid playerId)
    {
        try
        {
            return await _diceService.RollForJobAsync(playerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing dice roll for player {PlayerId}", playerId);
            throw;
        }
    }

    public async Task<bool> ValidateGameStateAsync(Guid playerId)
    {
        try
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
            {
                return false;
            }

            // Validate game state integrity
            bool isValid = player.Income >= 0 &&
                          player.Reputation >= 0 && player.Reputation <= 100 &&
                          player.Happiness >= 0 && player.Happiness <= 100;

            if (!isValid)
            {
                _logger.LogWarning("Invalid game state detected for player {PlayerId}", playerId);
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating game state for player {PlayerId}", playerId);
            return false;
        }
    }

    public async Task<GameResult?> CheckGameCompletionAsync(Guid playerId)
    {
        try
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
            {
                return null;
            }

            var territoriesOwned = await _context.Territories
                .CountAsync(t => t.OwnedByPlayerId == playerId);

            var achievements = await _context.PlayerAchievements
                .Where(a => a.PlayerId == playerId)
                .ToListAsync();

            // Check win conditions
            if (territoriesOwned >= 5 && player.Reputation >= 80)
            {
                return new GameResult(
                    true,
                    true,
                    "Congratulations! You've become a world leader by owning 5+ territories with high reputation!",
                    CalculateFinalScore(player, territoriesOwned, achievements.Count),
                    achievements.Select(a => a.Title).ToList()
                );
            }

            // Check loss conditions
            if (player.Happiness <= 0)
            {
                return new GameResult(
                    true,
                    false,
                    "Game over! Your population's happiness reached zero. Remember, good leaders care for their people!",
                    CalculateFinalScore(player, territoriesOwned, achievements.Count),
                    achievements.Select(a => a.Title).ToList()
                );
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking game completion for player {PlayerId}", playerId);
            return null;
        }
    }

    public async Task<GameStateUpdate> ProcessResourceChangeAsync(Guid playerId, ResourceChange change)
    {
        try
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
            {
                throw new ArgumentException($"Player {playerId} not found");
            }

            // Apply changes with validation
            player.Income = Math.Max(0, player.Income + change.IncomeChange);
            player.Reputation = Math.Clamp(player.Reputation + change.ReputationChange, 0, 100);
            player.Happiness = Math.Clamp(player.Happiness + change.HappinessChange, 0, 100);
            player.LastActiveAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Processed resource change for player {PlayerId}: {Change}", 
                playerId, change);

            return new GameStateUpdate(
                change.IncomeChange,
                change.ReputationChange,
                change.HappinessChange,
                change.Reason,
                EventType.Economic
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing resource change for player {PlayerId}", playerId);
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

    private int CalculateMonthlyIncome(PlayerEntity player)
    {
        int baseIncome = JobProgressionMapping.GetJobIncome(player.CurrentJob);
        
        // Add territory income (if any)
        // This would be calculated from owned territories
        // For now, return base job income
        
        return baseIncome;
    }

    private int CalculateFinalScore(PlayerEntity player, int territoriesOwned, int achievements)
    {
        return player.Income + 
               (player.Reputation * 100) + 
               (player.Happiness * 50) + 
               (territoriesOwned * 1000) + 
               (achievements * 500);
    }
}