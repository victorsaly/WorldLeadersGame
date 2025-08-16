namespace WorldLeaders.Shared.Enums;

/// <summary>
/// Represents the job levels in the World Leaders Game.
/// Determined by dice roll: 1-2 = Basic, 3-4 = Skilled, 5-6 = Leadership
/// </summary>
public enum JobLevel
{
    Student = 0,        // Entry level for educational progression
    Farmer = 1,
    Gardener = 2,
    Shopkeeper = 3,
    Artisan = 4,
    Politician = 5,
    BusinessLeader = 6,
    Teacher = 7,        // Educational profession
    Manager = 8         // Leadership progression
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

/// <summary>
/// User roles in the educational platform
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Guest user (temporary exploration without registration, enhanced safety features)
    /// </summary>
    Guest = 0,

    /// <summary>
    /// Student (typically 12+ years old, requires enhanced safety features)
    /// </summary>
    Student = 1,

    /// <summary>
    /// Teacher (can oversee student accounts, create educational content)
    /// </summary>
    Teacher = 2,

    /// <summary>
    /// Administrator (full system access, can manage all accounts)
    /// </summary>
    Admin = 3
}

/// <summary>
/// Types of safety events for child protection monitoring
/// </summary>
public enum SafetyEventType
{
    /// <summary>
    /// Content was flagged by moderation system
    /// </summary>
    ContentFlagged = 1,

    /// <summary>
    /// User session exceeded time limits
    /// </summary>
    SessionTimeout = 2,

    /// <summary>
    /// Parental consent was required or updated
    /// </summary>
    ParentalConsent = 3,

    /// <summary>
    /// GDPR data request or deletion
    /// </summary>
    GdprRequest = 4,

    /// <summary>
    /// User attempted unauthorized access
    /// </summary>
    UnauthorizedAccess = 5,

    /// <summary>
    /// Child safety policy violation
    /// </summary>
    PolicyViolation = 6
}

/// <summary>
/// Character persona types for visual player representation - Child Designer Vision
/// </summary>
public enum PersonaType
{
    /// <summary>
    /// Curious adventurer with backpack - loves discovering new places
    /// </summary>
    YoungExplorer = 1,
    
    /// <summary>
    /// Confident leader with cape - natural leadership qualities
    /// </summary>
    BraveLeader = 2,
    
    /// <summary>
    /// Thoughtful learner with books - wisdom and knowledge focused
    /// </summary>
    WiseScholar = 3,
    
    /// <summary>
    /// Peaceful negotiator with flowers - diplomacy and cooperation
    /// </summary>
    FriendlyDiplomat = 4,
    
    /// <summary>
    /// Imaginative creator with paintbrush - creativity and arts
    /// </summary>
    CreativeArtist = 5,
    
    /// <summary>
    /// Smart innovator with gadgets - technology and problem solving
    /// </summary>
    TechInventor = 6
}

/// <summary>
/// Severity levels for safety events
/// </summary>
public enum SafetyEventSeverity
{
    /// <summary>
    /// Informational - no action required
    /// </summary>
    Info = 1,

    /// <summary>
    /// Low risk - monitor but continue
    /// </summary>
    Low = 2,

    /// <summary>
    /// Medium risk - may require intervention
    /// </summary>
    Medium = 3,

    /// <summary>
    /// High risk - immediate action required
    /// </summary>
    High = 4,

    /// <summary>
    /// Critical risk - suspend account/session
    /// </summary>
    Critical = 5
}

/// <summary>
/// Zoom levels for the interactive pixel world map
/// </summary>
public enum ZoomLevel
{
    /// <summary>
    /// Global view showing all continents
    /// </summary>
    World = 1,

    /// <summary>
    /// Continental view showing countries in a continent
    /// </summary>
    Continent = 2,

    /// <summary>
    /// Regional view showing neighboring countries
    /// </summary>
    Region = 3,

    /// <summary>
    /// Country view showing landmarks and cities
    /// </summary>
    Country = 4
}

/// <summary>
/// Status of territories on the interactive map
/// </summary>
public enum TerritoryStatus
{
    /// <summary>
    /// Territory is locked and cannot be acquired yet
    /// </summary>
    Locked = 1,

    /// <summary>
    /// Territory is available for acquisition
    /// </summary>
    Available = 2,

    /// <summary>
    /// Territory is owned by the current player
    /// </summary>
    Owned = 3,

    /// <summary>
    /// Territory is owned by another player (multiplayer mode)
    /// </summary>
    OtherPlayer = 4,

    /// <summary>
    /// Territory is being contested (future feature)
    /// </summary>
    Contested = 5
}

/// <summary>
/// Types of landmarks for educational discovery
/// </summary>
public enum LandmarkType
{
    /// <summary>
    /// Historical monuments and structures
    /// </summary>
    Monument = 1,

    /// <summary>
    /// Natural wonders and geographical features
    /// </summary>
    NaturalWonder = 2,

    /// <summary>
    /// Museums and cultural sites
    /// </summary>
    Museum = 3,

    /// <summary>
    /// Religious or spiritual sites
    /// </summary>
    Religious = 4,

    /// <summary>
    /// Modern architectural marvels
    /// </summary>
    Modern = 5,

    /// <summary>
    /// Parks and recreational areas
    /// </summary>
    Park = 6,

    /// <summary>
    /// UNESCO World Heritage sites
    /// </summary>
    WorldHeritage = 7
}
