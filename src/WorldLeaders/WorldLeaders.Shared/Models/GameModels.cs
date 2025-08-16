using System.ComponentModel.DataAnnotations;
using WorldLeaders.Shared.Enums;

namespace WorldLeaders.Shared.Models;

/// <summary>
/// Character Persona entity for visual player representation - Child Designer Vision
/// </summary>
public class CharacterPersona
{
    /// <summary>
    /// Unique identifier for the character persona
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Character name (e.g., "Young Explorer", "Brave Leader")
    /// </summary>
    [Required]
    [StringLength(50, ErrorMessage = "Character name cannot exceed 50 characters")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Brief description of the character's personality
    /// </summary>
    [Required]
    [StringLength(100, ErrorMessage = "Description cannot exceed 100 characters")]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Character's main personality trait
    /// </summary>
    [Required]
    [StringLength(30, ErrorMessage = "Trait cannot exceed 30 characters")]
    public string PersonalityTrait { get; set; } = string.Empty;
    
    /// <summary>
    /// Special ability that makes this character unique
    /// </summary>
    [Required]
    [StringLength(50, ErrorMessage = "Special ability cannot exceed 50 characters")]
    public string SpecialAbility { get; set; } = string.Empty;
    
    /// <summary>
    /// Persona type categorization
    /// </summary>
    public PersonaType Type { get; set; }
    
    /// <summary>
    /// File path to 32x32 pixel art sprite
    /// </summary>
    [Required]
    public string PixelArtSprite32 { get; set; } = string.Empty;
    
    /// <summary>
    /// File path to 64x64 pixel art sprite
    /// </summary>
    [Required]
    public string PixelArtSprite64 { get; set; } = string.Empty;
    
    /// <summary>
    /// Primary color theme for UI customization
    /// </summary>
    [Required]
    public string PrimaryColor { get; set; } = "#2ea44f"; // Default retro green
    
    /// <summary>
    /// Whether this character is appropriate for 12-year-old players
    /// </summary>
    public bool IsChildFriendly { get; set; } = true;
    
    /// <summary>
    /// Display order for character selection screen
    /// </summary>
    [Range(0, 99, ErrorMessage = "Sort order must be between 0 and 99")]
    public int SortOrder { get; set; }
    
    /// <summary>
    /// Whether this persona is available for selection
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}

/// <summary>
/// Core player entity for the World Leaders educational game
/// </summary>
public class Player
{
    /// <summary>
    /// Unique identifier for the player
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Player's chosen username for the game (optional with persona system)
    /// </summary>
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public string? Username { get; set; }
    
    /// <summary>
    /// Display name for the player (can be different from username for child safety)
    /// </summary>
    [StringLength(50, ErrorMessage = "Display name cannot exceed 50 characters")]
    public string DisplayName { get; set; } = string.Empty;
    
    /// <summary>
    /// Selected character persona ID - replaces username as primary identity
    /// </summary>
    [Required]
    public Guid CharacterPersonaId { get; set; }
    
    /// <summary>
    /// Navigation property to selected character persona
    /// </summary>
    public CharacterPersona? CharacterPersona { get; set; }
    
    /// <summary>
    /// Current monthly income from job and territories
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Income cannot be negative")]
    public int Income { get; set; }
    
    /// <summary>
    /// Reputation percentage (0-100) affecting territory acquisition ability
    /// </summary>
    [Range(0, 100, ErrorMessage = "Reputation must be between 0 and 100")]
    public int Reputation { get; set; }
    
    /// <summary>
    /// Population happiness percentage (0-100) - game over if reaches 0
    /// </summary>
    [Range(0, 100, ErrorMessage = "Happiness must be between 0 and 100")]
    public int Happiness { get; set; } = 50; // Start at neutral happiness
    
    /// <summary>
    /// Current job level determined by dice roll
    /// </summary>
    public JobLevel CurrentJob { get; set; } = JobLevel.Farmer;
    
    /// <summary>
    /// List of territories owned by the player
    /// </summary>
    public List<Territory> OwnedTerritories { get; set; } = new();
    
    // Additional game state properties
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
    /// <summary>
    /// Unique identifier for the territory
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Name of the country (e.g., "United States", "Nepal", "Canada")
    /// </summary>
    [Required]
    [StringLength(100, ErrorMessage = "Country name cannot exceed 100 characters")]
    public string CountryName { get; set; } = string.Empty;
    
    /// <summary>
    /// ISO 3166-1 alpha-2 country code (e.g., "US", "NP", "CA")
    /// </summary>
    [Required]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "Country code must be exactly 2 characters")]
    public string CountryCode { get; set; } = string.Empty;
    
    /// <summary>
    /// GDP in billions of USD for educational economics learning
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "GDP must be positive")]
    public decimal GdpInBillions { get; set; }
    
    /// <summary>
    /// Cost to purchase this territory in game currency
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Cost must be positive")]
    public int Cost { get; set; }
    
    /// <summary>
    /// Minimum reputation percentage (0-100) required to acquire this territory
    /// </summary>
    [Range(0, 100, ErrorMessage = "Reputation required must be between 0 and 100")]
    public int ReputationRequired { get; set; }
    
    /// <summary>
    /// Official languages spoken in this territory for language learning challenges
    /// </summary>
    public List<string> OfficialLanguages { get; set; } = new();
    
    // Additional properties for enhanced gameplay
    public TerritoryTier Tier { get; set; }
    public long RealGDP { get; set; } // Real GDP in USD (for detailed data)
    public int GDPRank { get; set; } // World ranking
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
