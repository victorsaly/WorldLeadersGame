using Microsoft.EntityFrameworkCore;
using WorldLeaders.Infrastructure.Entities;
using WorldLeaders.Shared.Enums;

namespace WorldLeaders.Infrastructure.Data.Seed;

/// <summary>
/// Seed data for character personas - Child Designer Vision Implementation
/// Context: Educational game component for 12-year-old players
/// Educational Objective: Visual character selection enhances engagement and identity
/// Safety Requirements: Age-appropriate, diverse, positive role models
/// </summary>
public static class CharacterPersonaSeeder
{
    /// <summary>
    /// Seeds the database with the six child-friendly character personas
    /// designed by our 12-year-old creative director
    /// </summary>
    public static void SeedCharacterPersonas(ModelBuilder modelBuilder)
    {
        var personas = GetDefaultCharacterPersonas();
        
        modelBuilder.Entity<CharacterPersonaEntity>().HasData(personas);
    }

    /// <summary>
    /// Get the default character personas for the educational game
    /// Each persona represents positive leadership qualities and educational values
    /// </summary>
    private static List<CharacterPersonaEntity> GetDefaultCharacterPersonas()
    {
        return new List<CharacterPersonaEntity>
        {
            new CharacterPersonaEntity
            {
                Id = Guid.Parse("a3b1c2d3-4e5f-6789-abcd-0123456789ab"),
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
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new CharacterPersonaEntity
            {
                Id = Guid.Parse("b7f8e1d2-9c4a-5b6e-def0-123456789def"),
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
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new CharacterPersonaEntity
            {
                Id = Guid.Parse("c5e9f3a1-7d2b-4c8f-9a12-3456789abc12"),
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
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new CharacterPersonaEntity
            {
                Id = Guid.Parse("d8a4b6e2-1f5c-4a9b-8e73-456789def456"),
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
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new CharacterPersonaEntity
            {
                Id = Guid.Parse("e7c2f1a5-6b8d-4e9a-bc12-56789abcdef7"),
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
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new CharacterPersonaEntity
            {
                Id = Guid.Parse("f1b5d8e3-2a6c-4f9e-8b12-6789abcdef89"),
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
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
    }
}