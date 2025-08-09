using WorldLeaders.Shared.Models;
using WorldLeaders.Shared.Enums;

namespace WorldLeaders.Shared.Services;

/// <summary>
/// Service for managing character personas - Child Designer Vision Implementation
/// Context: Educational game component for 12-year-old players
/// Educational Objective: Visual character selection enhances engagement and identity
/// Safety Requirements: Age-appropriate, diverse, positive role models
/// </summary>
public interface ICharacterPersonaService
{
    /// <summary>
    /// Get all available character personas for selection
    /// </summary>
    Task<List<CharacterPersona>> GetAvailablePersonasAsync();
    
    /// <summary>
    /// Get a specific character persona by ID
    /// </summary>
    Task<CharacterPersona?> GetPersonaByIdAsync(Guid personaId);
    
    /// <summary>
    /// Get default character personas for new game setup
    /// </summary>
    List<CharacterPersona> GetDefaultPersonas();
}

/// <summary>
/// Implementation of character persona management
/// </summary>
public class CharacterPersonaService : ICharacterPersonaService
{
    private readonly List<CharacterPersona> _defaultPersonas;

    public CharacterPersonaService()
    {
        _defaultPersonas = InitializeDefaultPersonas();
    }

    public async Task<List<CharacterPersona>> GetAvailablePersonasAsync()
    {
        // In a real implementation, this would query a database
        // For now, return the default personas
        await Task.CompletedTask;
        return _defaultPersonas.Where(p => p.IsActive && p.IsChildFriendly).ToList();
    }

    public async Task<CharacterPersona?> GetPersonaByIdAsync(Guid personaId)
    {
        await Task.CompletedTask;
        return _defaultPersonas.FirstOrDefault(p => p.Id == personaId);
    }

    public List<CharacterPersona> GetDefaultPersonas()
    {
        return _defaultPersonas.ToList();
    }

    /// <summary>
    /// Initialize the six child-friendly character personas designed by our 12-year-old creative director
    /// </summary>
    private List<CharacterPersona> InitializeDefaultPersonas()
    {
        return new List<CharacterPersona>
        {
            new CharacterPersona
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Young Explorer",
                Description = "Curious adventurer who loves discovering new places and cultures",
                PersonalityTrait = "Curious & Adventurous",
                SpecialAbility = "Quick Territory Discovery",
                Type = PersonaType.YoungExplorer,
                PixelArtSprite32 = "/assets/characters/young-explorer-32x32.png",
                PixelArtSprite64 = "/assets/characters/young-explorer-64x64.png",
                PrimaryColor = "#2ea44f", // Retro green
                SortOrder = 1,
                IsChildFriendly = true,
                IsActive = true
            },
            new CharacterPersona
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Brave Leader",
                Description = "Confident leader who inspires others and makes tough decisions",
                PersonalityTrait = "Confident & Inspiring",
                SpecialAbility = "Enhanced Reputation Gain",
                Type = PersonaType.BraveLeader,
                PixelArtSprite32 = "/assets/characters/brave-leader-32x32.png",
                PixelArtSprite64 = "/assets/characters/brave-leader-64x64.png",
                PrimaryColor = "#3b82f6", // Retro blue
                SortOrder = 2,
                IsChildFriendly = true,
                IsActive = true
            },
            new CharacterPersona
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Wise Scholar",
                Description = "Thoughtful learner who values knowledge and understanding",
                PersonalityTrait = "Thoughtful & Wise",
                SpecialAbility = "Language Learning Bonus",
                Type = PersonaType.WiseScholar,
                PixelArtSprite32 = "/assets/characters/wise-scholar-32x32.png",
                PixelArtSprite64 = "/assets/characters/wise-scholar-64x64.png",
                PrimaryColor = "#8b5cf6", // Retro purple
                SortOrder = 3,
                IsChildFriendly = true,
                IsActive = true
            },
            new CharacterPersona
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Friendly Diplomat",
                Description = "Peaceful negotiator who builds bridges between people",
                PersonalityTrait = "Peaceful & Diplomatic",
                SpecialAbility = "Happiness Boost",
                Type = PersonaType.FriendlyDiplomat,
                PixelArtSprite32 = "/assets/characters/friendly-diplomat-32x32.png",
                PixelArtSprite64 = "/assets/characters/friendly-diplomat-64x64.png",
                PrimaryColor = "#86efac", // Retro green bright
                SortOrder = 4,
                IsChildFriendly = true,
                IsActive = true
            },
            new CharacterPersona
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "Creative Artist",
                Description = "Imaginative creator who sees beauty and possibility everywhere",
                PersonalityTrait = "Creative & Imaginative",
                SpecialAbility = "Cultural Connection",
                Type = PersonaType.CreativeArtist,
                PixelArtSprite32 = "/assets/characters/creative-artist-32x32.png",
                PixelArtSprite64 = "/assets/characters/creative-artist-64x64.png",
                PrimaryColor = "#f97316", // Retro orange
                SortOrder = 5,
                IsChildFriendly = true,
                IsActive = true
            },
            new CharacterPersona
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Name = "Tech Inventor",
                Description = "Smart innovator who loves solving problems with technology",
                PersonalityTrait = "Innovative & Logical",
                SpecialAbility = "Economic Efficiency",
                Type = PersonaType.TechInventor,
                PixelArtSprite32 = "/assets/characters/tech-inventor-32x32.png",
                PixelArtSprite64 = "/assets/characters/tech-inventor-64x64.png",
                PrimaryColor = "#eab308", // Retro yellow
                SortOrder = 6,
                IsChildFriendly = true,
                IsActive = true
            }
        };
    }
}