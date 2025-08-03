using Microsoft.EntityFrameworkCore;
using WorldLeaders.Infrastructure.Entities;
using WorldLeaders.Shared.Enums;
using System.Text.Json;

namespace WorldLeaders.Infrastructure.Data.Seed;

/// <summary>
/// Seed data for territories based on real World Bank GDP data
/// Educational content for teaching geography, economics, and cultural awareness
/// </summary>
public static class TerritorySeeder
{
    /// <summary>
    /// Seeds the database with educational territory data based on real GDP rankings
    /// Designed for 12-year-old learners with age-appropriate difficulty progression
    /// </summary>
    public static void SeedTerritories(ModelBuilder modelBuilder)
    {
        var territories = GetEducationalTerritories();
        
        modelBuilder.Entity<TerritoryEntity>().HasData(territories);
    }

    /// <summary>
    /// Get carefully curated territories for educational gameplay
    /// Based on real GDP data but simplified for child learners
    /// </summary>
    private static List<TerritoryEntity> GetEducationalTerritories()
    {
        return new List<TerritoryEntity>
        {
            // Tier 1: Small Countries (Easy to acquire - Perfect for beginners)
            CreateTerritory("Nepal", "NP", 36.29m, 5000, 10, TerritoryTier.Small, 100, 500, new[] { "ne", "en" }),
            CreateTerritory("Iceland", "IS", 27.84m, 8000, 15, TerritoryTier.Small, 95, 800, new[] { "is", "en" }),
            CreateTerritory("Estonia", "EE", 38.03m, 6000, 12, TerritoryTier.Small, 98, 600, new[] { "et", "en" }),
            CreateTerritory("Luxembourg", "LU", 86.93m, 12000, 20, TerritoryTier.Small, 90, 1200, new[] { "lb", "fr", "de" }),
            CreateTerritory("Malta", "MT", 17.32m, 4500, 8, TerritoryTier.Small, 105, 450, new[] { "mt", "en" }),
            CreateTerritory("Cyprus", "CY", 27.97m, 5500, 10, TerritoryTier.Small, 102, 550, new[] { "el", "tr", "en" }),
            CreateTerritory("Brunei", "BN", 16.84m, 7000, 15, TerritoryTier.Small, 110, 700, new[] { "ms", "en" }),
            CreateTerritory("Jamaica", "JM", 17.94m, 4000, 8, TerritoryTier.Small, 108, 400, new[] { "en" }),

            // Tier 2: Medium Countries (Moderate difficulty - Building skills)
            CreateTerritory("Portugal", "PT", 249.89m, 25000, 30, TerritoryTier.Medium, 60, 2500, new[] { "pt" }),
            CreateTerritory("New Zealand", "NZ", 249.89m, 28000, 35, TerritoryTier.Medium, 55, 2800, new[] { "en", "mi" }),
            CreateTerritory("Ireland", "IE", 498.57m, 35000, 40, TerritoryTier.Medium, 45, 3500, new[] { "en", "ga" }),
            CreateTerritory("Denmark", "DK", 398.30m, 32000, 38, TerritoryTier.Medium, 50, 3200, new[] { "da" }),
            CreateTerritory("Finland", "FI", 297.30m, 30000, 35, TerritoryTier.Medium, 52, 3000, new[] { "fi", "sv" }),
            CreateTerritory("Norway", "NO", 482.17m, 38000, 42, TerritoryTier.Medium, 43, 3800, new[] { "no" }),
            CreateTerritory("Singapore", "SG", 397.72m, 40000, 45, TerritoryTier.Medium, 42, 4000, new[] { "en", "ms", "ta", "zh" }),
            CreateTerritory("Israel", "IL", 481.66m, 36000, 40, TerritoryTier.Medium, 46, 3600, new[] { "he", "ar", "en" }),
            CreateTerritory("Austria", "AT", 477.04m, 35000, 38, TerritoryTier.Medium, 47, 3500, new[] { "de" }),
            CreateTerritory("Belgium", "BE", 594.41m, 42000, 45, TerritoryTier.Medium, 40, 4200, new[] { "nl", "fr", "de" }),

            // Tier 3: Major Powers (High difficulty - Advanced gameplay)
            CreateTerritory("Netherlands", "NL", 1.01m, 65000, 60, TerritoryTier.Major, 25, 6500, new[] { "nl" }),
            CreateTerritory("Switzerland", "CH", 807.71m, 70000, 65, TerritoryTier.Major, 22, 7000, new[] { "de", "fr", "it", "rm" }),
            CreateTerritory("Sweden", "SE", 635.66m, 55000, 55, TerritoryTier.Major, 28, 5500, new[] { "sv" }),
            CreateTerritory("Australia", "AU", 1.55m, 80000, 70, TerritoryTier.Major, 15, 8000, new[] { "en" }),
            CreateTerritory("Canada", "CA", 2.14m, 85000, 75, TerritoryTier.Major, 12, 8500, new[] { "en", "fr" }),
            CreateTerritory("South Korea", "KR", 1.81m, 75000, 68, TerritoryTier.Major, 18, 7500, new[] { "ko" }),
            CreateTerritory("Spain", "ES", 1.40m, 72000, 65, TerritoryTier.Major, 20, 7200, new[] { "es", "ca", "gl", "eu" }),
            CreateTerritory("Italy", "IT", 2.11m, 78000, 70, TerritoryTier.Major, 16, 7800, new[] { "it" }),
            CreateTerritory("France", "FR", 2.94m, 90000, 80, TerritoryTier.Major, 8, 9000, new[] { "fr" }),
            CreateTerritory("United Kingdom", "GB", 3.13m, 95000, 82, TerritoryTier.Major, 6, 9500, new[] { "en", "cy", "gd" }),
            CreateTerritory("Germany", "DE", 4.26m, 100000, 85, TerritoryTier.Major, 4, 10000, new[] { "de" }),
            CreateTerritory("Japan", "JP", 4.94m, 110000, 88, TerritoryTier.Major, 3, 11000, new[] { "ja" }),
            CreateTerritory("China", "CN", 17.73m, 150000, 92, TerritoryTier.Major, 2, 15000, new[] { "zh" }),
            CreateTerritory("United States", "US", 26.95m, 200000, 95, TerritoryTier.Major, 1, 20000, new[] { "en", "es" })
        };
    }

    /// <summary>
    /// Helper method to create territory entities with educational considerations
    /// </summary>
    private static TerritoryEntity CreateTerritory(
        string countryName,
        string countryCode,
        decimal gdpTrillions,
        int cost,
        int reputationRequired,
        TerritoryTier tier,
        int gdpRank,
        int monthlyIncome,
        string[] languages)
    {
        // Convert languages to JSON for database storage
        var languagesJson = JsonSerializer.Serialize(languages);

        return new TerritoryEntity
        {
            Id = Guid.NewGuid(),
            CountryName = countryName,
            CountryCode = countryCode,
            GdpInBillions = gdpTrillions, // Simplified for children
            Cost = cost,
            ReputationRequired = reputationRequired,
            Tier = tier,
            RealGDP = (long)(gdpTrillions * 1000000000), // Convert to actual GDP
            GDPRank = gdpRank,
            MonthlyIncome = monthlyIncome,
            OfficialLanguagesJson = languagesJson,
            IsAvailable = true,
            OwnedByPlayerId = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
    }
}