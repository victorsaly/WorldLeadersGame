using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Models;

namespace WorldLeaders.Shared.Services;

/// <summary>
/// Core game engine for managing game state, progression, and mechanics
/// Educational game for 12-year-old players learning geography, economics, and languages
/// </summary>
public interface IGameEngine
{
    /// <summary>
    /// Start a new game session for a player
    /// </summary>
    Task<GameSession> StartNewGameAsync(Guid playerId);
    
    /// <summary>
    /// Load an existing game session
    /// </summary>
    Task<GameSession?> LoadGameSessionAsync(Guid playerId);
    
    /// <summary>
    /// Save current game state
    /// </summary>
    Task<bool> SaveGameSessionAsync(GameSession session);
    
    /// <summary>
    /// Advance to the next turn/phase in the game
    /// </summary>
    Task<GameStateUpdate> AdvanceTurnAsync(Guid playerId);
    
    /// <summary>
    /// Process a dice roll for job progression
    /// </summary>
    Task<DiceRollResult> ProcessDiceRollAsync(Guid playerId);
    
    /// <summary>
    /// Validate game state integrity
    /// </summary>
    Task<bool> ValidateGameStateAsync(Guid playerId);
    
    /// <summary>
    /// Check win/loss conditions
    /// </summary>
    Task<GameResult?> CheckGameCompletionAsync(Guid playerId);
    
    /// <summary>
    /// Process resource changes (income, reputation, happiness)
    /// </summary>
    Task<GameStateUpdate> ProcessResourceChangeAsync(Guid playerId, ResourceChange change);
}

/// <summary>
/// Game session representing current game state
/// </summary>
public record GameSession(
    Guid PlayerId,
    Guid SessionId,
    GameState State,
    int CurrentTurn,
    DateTime StartedAt,
    DateTime LastSavedAt,
    Player PlayerData
);

/// <summary>
/// Result of a dice roll operation
/// </summary>
public record DiceRollResult(
    int DiceValue,
    JobLevel NewJob,
    int IncomeChange,
    int ReputationChange,
    int HappinessChange,
    string EncouragingMessage,
    string JobDescription
)
{
    /// <summary>
    /// Alias for EncouragingMessage to support test compatibility
    /// </summary>
    public string Message => EncouragingMessage;
};

/// <summary>
/// Game completion result
/// </summary>
public record GameResult(
    bool IsCompleted,
    bool IsWon,
    string Reason,
    int FinalScore,
    List<string> Achievements
);

