using WorldLeaders.Shared.Enums;

namespace WorldLeaders.Shared.Models;

/// <summary>
/// Core player entity for the World Leaders educational game
/// </summary>
public class Player
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public int Income { get; set; }
    public int Reputation { get; set; } // 0-100%
    public int Happiness { get; set; } = 50; // Start at neutral happiness
    public JobLevel CurrentJob { get; set; } = JobLevel.Farmer;
    public List<Territory> OwnedTerritories { get; set; } = new();
    public GameState CurrentGameState { get; set; } = GameState.NotStarted;
    public DateTime GameStartedAt { get; set; }
    public DateTime LastActiveAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}

/// <summary>
/// Territory entity representing countries/regions that can be acquired
/// </summary>
public class Territory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty; // ISO 3166-1 alpha-2
    public string PrimaryLanguage { get; set; } = string.Empty;
    public TerritoryTier Tier { get; set; }
    public long RealGDP { get; set; } // Real GDP in USD
    public int GDPRank { get; set; } // World ranking
    public int PurchaseCost { get; set; }
    public int ReputationRequired { get; set; }
    public int MonthlyIncome { get; set; } // Income generated when owned
    public bool IsAvailable { get; set; } = true;
    public Guid? OwnedByPlayerId { get; set; }
    public DateTime? AcquiredAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}

/// <summary>
/// Game events that can randomly affect player stats
/// </summary>
public class GameEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EventType Type { get; set; }
    public int IncomeEffect { get; set; } // Can be positive or negative
    public int ReputationEffect { get; set; }
    public int HappinessEffect { get; set; }
    public bool IsPositive { get; set; }
    public string IconEmoji { get; set; } = "🎲";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}

/// <summary>
/// Player's interaction history with AI agents
/// </summary>
public class AIInteraction
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
/// Language learning progress for territory acquisition
/// </summary>
public class LanguageProgress
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PlayerId { get; set; }
    public string LanguageCode { get; set; } = string.Empty; // ISO 639-1
    public string LanguageName { get; set; } = string.Empty;
    public int AccuracyPercentage { get; set; }
    public int ChallengesCompleted { get; set; }
    public bool HasPassed { get; set; } // 70%+ accuracy required
    public DateTime LastPracticeAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
