namespace WorldLeaders.Shared.Enums;

/// <summary>
/// Represents the job levels in the World Leaders Game.
/// Determined by dice roll: 1-2 = Basic, 3-4 = Skilled, 5-6 = Leadership
/// </summary>
public enum JobLevel
{
    Farmer = 1,
    Gardener = 2,
    Shopkeeper = 3,
    Artisan = 4,
    Politician = 5,
    BusinessLeader = 6
}

/// <summary>
/// Types of AI agents available for player assistance
/// </summary>
public enum AgentType
{
    CareerGuide,
    EventNarrator,
    FortuneTeller,
    HappinessAdvisor,
    TerritoryStrategist,
    LanguageTutor
}

/// <summary>
/// Territory tiers based on real-world GDP data
/// </summary>
public enum TerritoryTier
{
    Small = 1,      // GDP rank 100+
    Medium = 2,     // GDP rank 30-100
    Major = 3       // GDP rank 1-30
}

/// <summary>
/// Game event types that can affect player stats
/// </summary>
public enum EventType
{
    Career,
    Economic,
    Social,
    International,
    Natural
}

/// <summary>
/// Current state of the game session
/// </summary>
public enum GameState
{
    NotStarted,
    InProgress,
    Won,
    Lost,
    Paused
}
