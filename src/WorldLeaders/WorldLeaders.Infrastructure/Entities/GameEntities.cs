using WorldLeaders.Shared.Enums;

namespace WorldLeaders.Infrastructure.Entities;

/// <summary>
/// Database entity for Player
/// </summary>
public class PlayerEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public int Income { get; set; }
    public int Reputation { get; set; }
    public int Happiness { get; set; }
    public JobLevel CurrentJob { get; set; }
    public GameState CurrentGameState { get; set; }
    public DateTime GameStartedAt { get; set; }
    public DateTime LastActiveAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}

/// <summary>
/// Database entity for Territory
/// </summary>
public class TerritoryEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CountryName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public decimal GdpInBillions { get; set; }
    public int Cost { get; set; }
    public int ReputationRequired { get; set; }
    public string OfficialLanguagesJson { get; set; } = string.Empty; // JSON serialized List<string>
    
    // Additional properties for enhanced gameplay
    public TerritoryTier Tier { get; set; }
    public long RealGDP { get; set; }
    public int GDPRank { get; set; }
    public int MonthlyIncome { get; set; }
    public bool IsAvailable { get; set; } = true;
    public Guid? OwnedByPlayerId { get; set; }
    public DateTime? AcquiredAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}

/// <summary>
/// Database entity for GameEvent
/// </summary>
public class GameEventEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EventType Type { get; set; }
    public int IncomeEffect { get; set; }
    public int ReputationEffect { get; set; }
    public int HappinessEffect { get; set; }
    public bool IsPositive { get; set; }
    public string IconEmoji { get; set; } = "🎲";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}

/// <summary>
/// Database entity for AIInteraction
/// </summary>
public class AIInteractionEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PlayerId { get; set; }
    public AgentType AgentType { get; set; }
    public string PlayerInput { get; set; } = string.Empty;
    public string AgentResponse { get; set; } = string.Empty;
    public bool WasHelpful { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Database entity for LanguageProgress
/// </summary>
public class LanguageProgressEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PlayerId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string LanguageName { get; set; } = string.Empty;
    public int AccuracyPercentage { get; set; }
    public int ChallengesCompleted { get; set; }
    public bool HasPassed { get; set; }
    public DateTime LastPracticeAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Database entity for GameSession - tracks active game state
/// </summary>
public class GameSessionEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PlayerId { get; set; }
    public GameState State { get; set; }
    public int CurrentTurn { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastSavedAt { get; set; } = DateTime.UtcNow;
    public string GameDataJson { get; set; } = string.Empty; // Serialized game state
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}

/// <summary>
/// Database entity for DiceRollHistory - tracks educational dice interactions
/// </summary>
public class DiceRollHistoryEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PlayerId { get; set; }
    public int DiceValue { get; set; }
    public JobLevel ResultingJob { get; set; }
    public int IncomeChange { get; set; }
    public int ReputationChange { get; set; }
    public int HappinessChange { get; set; }
    public string PlayerReaction { get; set; } = string.Empty;
    public DateTime RolledAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Database entity for PlayerAchievement - tracks educational milestones
/// </summary>
public class PlayerAchievementEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PlayerId { get; set; }
    public string AchievementId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconEmoji { get; set; } = string.Empty;
    public int PointsReward { get; set; }
    public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
}
