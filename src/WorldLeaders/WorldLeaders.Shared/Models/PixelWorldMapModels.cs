using System.ComponentModel.DataAnnotations;
using WorldLeaders.Shared.Enums;

namespace WorldLeaders.Shared.Models;

/// <summary>
/// Interactive Pixel Art World Map for educational geography learning
/// Context: 32-bit pixel art style map for 12-year-old players
/// Educational Objective: Visual geography learning with cultural discovery
/// </summary>
public class PixelWorldMap
{
    /// <summary>
    /// Unique identifier for the map instance
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Map identifier for different map types
    /// </summary>
    [Required]
    [StringLength(50)]
    public string MapId { get; set; } = "world-map-v1";
    
    /// <summary>
    /// Pixel width for 32-bit era resolution (16:10 aspect ratio)
    /// </summary>
    [Range(800, 2560)]
    public int PixelWidth { get; set; } = 1280;
    
    /// <summary>
    /// Pixel height for 32-bit era resolution
    /// </summary>
    [Range(600, 1440)]
    public int PixelHeight { get; set; } = 640;
    
    /// <summary>
    /// Path to the sprite sheet containing map assets
    /// </summary>
    [Required]
    public string SpriteSheetPath { get; set; } = "/assets/maps/world-map-sprite.svg";
    
    /// <summary>
    /// List of interactive territories on the map
    /// </summary>
    public List<InteractiveTerritory> Territories { get; set; } = new();
    
    /// <summary>
    /// Current viewport settings for the map
    /// </summary>
    public MapViewport CurrentViewport { get; set; } = new();
    
    /// <summary>
    /// Whether the map is optimized for mobile/tablet devices
    /// </summary>
    public bool MobileOptimized { get; set; } = true;
    
    /// <summary>
    /// Current zoom level of the map
    /// </summary>
    public ZoomLevel CurrentZoom { get; set; } = ZoomLevel.World;
    
    /// <summary>
    /// Child designer's green theme configuration
    /// </summary>
    public string ThemeColor { get; set; } = "#2ea44f";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Interactive territory representation on the pixel map
/// Context: Child-friendly territory interaction for geography learning
/// </summary>
public class InteractiveTerritory
{
    /// <summary>
    /// Unique identifier for the interactive territory
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ISO 3166-1 alpha-2 country code
    /// </summary>
    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string CountryCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Full country name for display
    /// </summary>
    [Required]
    [StringLength(100)]
    public string CountryName { get; set; } = string.Empty;
    
    /// <summary>
    /// Position coordinates on the pixel map
    /// </summary>
    public PixelCoordinate Position { get; set; } = new();
    
    /// <summary>
    /// Interactive boundary definition for click detection
    /// </summary>
    public PixelBounds Boundaries { get; set; } = new();
    
    /// <summary>
    /// Current status of the territory (available, owned, locked)
    /// </summary>
    public TerritoryStatus Status { get; set; } = TerritoryStatus.Available;
    
    /// <summary>
    /// Educational information about the territory
    /// </summary>
    public EducationalInfo Education { get; set; } = new();
    
    /// <summary>
    /// Pixel art flag representation
    /// </summary>
    public PixelFlag Flag { get; set; } = new();
    
    /// <summary>
    /// Famous landmarks in the territory
    /// </summary>
    public List<PixelLandmark> Landmarks { get; set; } = new();
    
    /// <summary>
    /// Cultural information for educational discovery
    /// </summary>
    public CulturalInfo Culture { get; set; } = new();
    
    /// <summary>
    /// Whether the territory can be acquired
    /// </summary>
    public bool IsAvailable { get; set; } = true;
    
    /// <summary>
    /// Whether the territory is owned by a player
    /// </summary>
    public bool IsOwned { get; set; } = false;
    
    /// <summary>
    /// Touch radius for mobile-friendly interaction (minimum 44px)
    /// </summary>
    [Range(44, 100)]
    public int TouchRadius { get; set; } = 44;
    
    /// <summary>
    /// Reference to the actual territory entity
    /// </summary>
    public Guid? TerritoryId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Pixel coordinate system for map positioning
/// </summary>
public class PixelCoordinate
{
    /// <summary>
    /// X coordinate in pixels from left edge
    /// </summary>
    [Range(0, 2560)]
    public int X { get; set; }
    
    /// <summary>
    /// Y coordinate in pixels from top edge
    /// </summary>
    [Range(0, 1440)]
    public int Y { get; set; }
    
    /// <summary>
    /// Optional real-world latitude for educational accuracy
    /// </summary>
    [Range(-90, 90)]
    public double? Latitude { get; set; }
    
    /// <summary>
    /// Optional real-world longitude for educational accuracy
    /// </summary>
    [Range(-180, 180)]
    public double? Longitude { get; set; }
}

/// <summary>
/// Boundary definition for interactive territories
/// </summary>
public class PixelBounds
{
    /// <summary>
    /// Top-left corner of the boundary
    /// </summary>
    public PixelCoordinate TopLeft { get; set; } = new();
    
    /// <summary>
    /// Bottom-right corner of the boundary
    /// </summary>
    public PixelCoordinate BottomRight { get; set; } = new();
    
    /// <summary>
    /// Optional polygon points for complex shapes
    /// </summary>
    public List<PixelCoordinate> PolygonPoints { get; set; } = new();
    
    /// <summary>
    /// Whether to use polygon points or rectangle bounds
    /// </summary>
    public bool UsePolygon { get; set; } = false;
}

/// <summary>
/// Educational information about territories for child learning
/// </summary>
public class EducationalInfo
{
    /// <summary>
    /// Capital city of the territory
    /// </summary>
    [StringLength(100)]
    public string Capital { get; set; } = string.Empty;
    
    /// <summary>
    /// Population in millions (simplified for children)
    /// </summary>
    [Range(0, 10000)]
    public decimal PopulationMillions { get; set; }
    
    /// <summary>
    /// Child-friendly fun facts about the country
    /// </summary>
    public List<string> FunFacts { get; set; } = new();
    
    /// <summary>
    /// Famous foods from the country
    /// </summary>
    public List<string> FamousFoods { get; set; } = new();
    
    /// <summary>
    /// Traditional celebrations and holidays
    /// </summary>
    public List<string> Celebrations { get; set; } = new();
    
    /// <summary>
    /// Geographical features (mountains, rivers, etc.)
    /// </summary>
    public List<string> GeographicalFeatures { get; set; } = new();
    
    /// <summary>
    /// Child-appropriate economic context
    /// </summary>
    [StringLength(200)]
    public string EconomyDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// Climate description for educational context
    /// </summary>
    [StringLength(200)]
    public string ClimateDescription { get; set; } = string.Empty;
}

/// <summary>
/// Pixel art flag representation for territories
/// </summary>
public class PixelFlag
{
    /// <summary>
    /// Path to 16x12 pixel flag sprite
    /// </summary>
    [Required]
    public string SpritePath { get; set; } = string.Empty;
    
    /// <summary>
    /// Alternative text for accessibility
    /// </summary>
    [Required]
    [StringLength(100)]
    public string AltText { get; set; } = string.Empty;
    
    /// <summary>
    /// Primary colors in the flag for educational context
    /// </summary>
    public List<string> PrimaryColors { get; set; } = new();
    
    /// <summary>
    /// Meaning or symbolism of the flag (child-appropriate)
    /// </summary>
    [StringLength(300)]
    public string Symbolism { get; set; } = string.Empty;
}

/// <summary>
/// Pixel art landmark representation
/// </summary>
public class PixelLandmark
{
    /// <summary>
    /// Unique identifier for the landmark
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Name of the landmark
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Position on the territory map
    /// </summary>
    public PixelCoordinate Position { get; set; } = new();
    
    /// <summary>
    /// Path to 24x24 pixel landmark sprite
    /// </summary>
    [Required]
    public string SpritePath { get; set; } = string.Empty;
    
    /// <summary>
    /// Child-friendly description of the landmark
    /// </summary>
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of landmark for categorization
    /// </summary>
    public LandmarkType Type { get; set; } = LandmarkType.Monument;
    
    /// <summary>
    /// Whether this landmark is famous worldwide
    /// </summary>
    public bool IsWorldFamous { get; set; } = false;
}

/// <summary>
/// Cultural information for educational discovery
/// </summary>
public class CulturalInfo
{
    /// <summary>
    /// Traditional clothing or costumes
    /// </summary>
    public List<string> TraditionalClothing { get; set; } = new();
    
    /// <summary>
    /// Popular sports in the country
    /// </summary>
    public List<string> PopularSports { get; set; } = new();
    
    /// <summary>
    /// Traditional musical instruments
    /// </summary>
    public List<string> MusicalInstruments { get; set; } = new();
    
    /// <summary>
    /// Common greetings in local language(s)
    /// </summary>
    public Dictionary<string, string> CommonGreetings { get; set; } = new();
    
    /// <summary>
    /// Traditional stories or legends (child-appropriate)
    /// </summary>
    public List<string> TraditionalStories { get; set; } = new();
    
    /// <summary>
    /// Art and craft traditions
    /// </summary>
    public List<string> ArtTraditions { get; set; } = new();
}

/// <summary>
/// Map viewport configuration for zoom and navigation
/// </summary>
public class MapViewport
{
    /// <summary>
    /// Current center point of the viewport
    /// </summary>
    public PixelCoordinate Center { get; set; } = new() { X = 640, Y = 320 };
    
    /// <summary>
    /// Current zoom scale (1.0 = normal, 2.0 = 200% zoom)
    /// </summary>
    [Range(0.5, 5.0)]
    public double ZoomScale { get; set; } = 1.0;
    
    /// <summary>
    /// Visible area width in pixels
    /// </summary>
    [Range(400, 2560)]
    public int ViewportWidth { get; set; } = 1280;
    
    /// <summary>
    /// Visible area height in pixels
    /// </summary>
    [Range(300, 1440)]
    public int ViewportHeight { get; set; } = 640;
    
    /// <summary>
    /// Whether smooth zoom animations are enabled
    /// </summary>
    public bool SmoothZoom { get; set; } = true;
    
    /// <summary>
    /// Animation duration in milliseconds
    /// </summary>
    [Range(100, 1000)]
    public int AnimationDuration { get; set; } = 300;
}