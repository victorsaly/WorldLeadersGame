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
    /// Get comprehensive territories for educational gameplay representing global diversity
    /// Based on real GDP data but simplified for child learners
    /// Expanded to include countries from all continents for enhanced geography education
    /// </summary>
    private static List<TerritoryEntity> GetEducationalTerritories()
    {
        return new List<TerritoryEntity>
        {
            // Tier 1: Small Countries (Easy to acquire - Perfect for beginners)
            // Africa
            CreateTerritory("Rwanda", "RW", 11.07m, 2500, 5, TerritoryTier.Small, 140, 250, new[] { "rw", "en", "fr" }),
            CreateTerritory("Botswana", "BW", 18.34m, 3500, 8, TerritoryTier.Small, 120, 350, new[] { "en", "tn" }),
            CreateTerritory("Mauritius", "MU", 14.18m, 3000, 7, TerritoryTier.Small, 130, 300, new[] { "en", "fr" }),
            CreateTerritory("Seychelles", "SC", 1.74m, 1500, 5, TerritoryTier.Small, 170, 150, new[] { "en", "fr" }),
            CreateTerritory("Cape Verde", "CV", 2.07m, 1800, 6, TerritoryTier.Small, 160, 180, new[] { "pt" }),
            
            // Asia
            CreateTerritory("Nepal", "NP", 36.29m, 5000, 10, TerritoryTier.Small, 100, 500, new[] { "ne", "en" }),
            CreateTerritory("Bhutan", "BT", 2.53m, 2000, 6, TerritoryTier.Small, 165, 200, new[] { "dz", "en" }),
            CreateTerritory("Maldives", "MV", 5.94m, 2500, 7, TerritoryTier.Small, 155, 250, new[] { "dv", "en" }),
            CreateTerritory("Brunei", "BN", 16.84m, 7000, 15, TerritoryTier.Small, 110, 700, new[] { "ms", "en" }),
            CreateTerritory("Mongolia", "MN", 15.29m, 3200, 8, TerritoryTier.Small, 125, 320, new[] { "mn" }),
            CreateTerritory("Laos", "LA", 19.23m, 3500, 9, TerritoryTier.Small, 118, 350, new[] { "lo" }),
            CreateTerritory("Cambodia", "KH", 27.18m, 4000, 10, TerritoryTier.Small, 105, 400, new[] { "km" }),
            
            // Europe
            CreateTerritory("Iceland", "IS", 27.84m, 8000, 15, TerritoryTier.Small, 95, 800, new[] { "is", "en" }),
            CreateTerritory("Estonia", "EE", 38.03m, 6000, 12, TerritoryTier.Small, 98, 600, new[] { "et", "en" }),
            CreateTerritory("Luxembourg", "LU", 86.93m, 12000, 20, TerritoryTier.Small, 90, 1200, new[] { "lb", "fr", "de" }),
            CreateTerritory("Malta", "MT", 17.32m, 4500, 8, TerritoryTier.Small, 105, 450, new[] { "mt", "en" }),
            CreateTerritory("Cyprus", "CY", 27.97m, 5500, 10, TerritoryTier.Small, 102, 550, new[] { "el", "tr", "en" }),
            CreateTerritory("Latvia", "LV", 40.87m, 6200, 12, TerritoryTier.Small, 92, 620, new[] { "lv" }),
            CreateTerritory("Lithuania", "LT", 65.58m, 7500, 14, TerritoryTier.Small, 85, 750, new[] { "lt" }),
            CreateTerritory("Slovenia", "SI", 63.64m, 7200, 13, TerritoryTier.Small, 88, 720, new[] { "sl" }),
            CreateTerritory("Slovakia", "SK", 115.28m, 9500, 16, TerritoryTier.Small, 75, 950, new[] { "sk" }),
            CreateTerritory("Croatia", "HR", 70.97m, 8000, 14, TerritoryTier.Small, 82, 800, new[] { "hr" }),
            CreateTerritory("Serbia", "RS", 63.07m, 7300, 13, TerritoryTier.Small, 89, 730, new[] { "sr" }),
            CreateTerritory("Bosnia and Herzegovina", "BA", 24.53m, 4800, 9, TerritoryTier.Small, 115, 480, new[] { "bs", "hr", "sr" }),
            CreateTerritory("Albania", "AL", 18.26m, 4200, 8, TerritoryTier.Small, 122, 420, new[] { "sq" }),
            CreateTerritory("North Macedonia", "MK", 13.86m, 3800, 8, TerritoryTier.Small, 135, 380, new[] { "mk" }),
            CreateTerritory("Montenegro", "ME", 6.18m, 2800, 7, TerritoryTier.Small, 150, 280, new[] { "sr", "bs", "sq", "hr" }),
            
            // Americas
            CreateTerritory("Jamaica", "JM", 17.94m, 4000, 8, TerritoryTier.Small, 108, 400, new[] { "en" }),
            CreateTerritory("Trinidad and Tobago", "TT", 28.78m, 5000, 10, TerritoryTier.Small, 103, 500, new[] { "en" }),
            CreateTerritory("Barbados", "BB", 5.42m, 2600, 7, TerritoryTier.Small, 152, 260, new[] { "en" }),
            CreateTerritory("Bahamas", "BS", 12.83m, 3600, 8, TerritoryTier.Small, 138, 360, new[] { "en" }),
            CreateTerritory("Belize", "BZ", 2.95m, 2200, 6, TerritoryTier.Small, 163, 220, new[] { "en" }),
            CreateTerritory("Guyana", "GY", 8.04m, 3000, 7, TerritoryTier.Small, 145, 300, new[] { "en" }),
            CreateTerritory("Suriname", "SR", 3.27m, 2300, 6, TerritoryTier.Small, 162, 230, new[] { "nl" }),
            CreateTerritory("Paraguay", "PY", 40.71m, 5800, 11, TerritoryTier.Small, 93, 580, new[] { "es", "gn" }),
            CreateTerritory("Uruguay", "UY", 59.32m, 7000, 13, TerritoryTier.Small, 90, 700, new[] { "es" }),
            CreateTerritory("Costa Rica", "CR", 68.37m, 7800, 14, TerritoryTier.Small, 80, 780, new[] { "es" }),
            CreateTerritory("Panama", "PA", 75.49m, 8200, 15, TerritoryTier.Small, 78, 820, new[] { "es" }),
            
            // Oceania
            CreateTerritory("Fiji", "FJ", 5.54m, 2700, 7, TerritoryTier.Small, 151, 270, new[] { "en", "fj", "hi" }),
            CreateTerritory("Papua New Guinea", "PG", 26.59m, 4500, 9, TerritoryTier.Small, 112, 450, new[] { "en", "tpi", "ho" }),
            CreateTerritory("Samoa", "WS", 0.85m, 1200, 4, TerritoryTier.Small, 185, 120, new[] { "sm", "en" }),
            CreateTerritory("Vanuatu", "VU", 0.96m, 1300, 4, TerritoryTier.Small, 180, 130, new[] { "bi", "en", "fr" }),

            // Tier 2: Medium Countries (Moderate difficulty - Building skills)
            // Africa
            CreateTerritory("South Africa", "ZA", 419.01m, 32000, 35, TerritoryTier.Medium, 40, 3200, new[] { "af", "en", "nr", "nso", "ss", "st", "tn", "ts", "ve", "xh", "zu" }),
            CreateTerritory("Nigeria", "NG", 440.78m, 33000, 36, TerritoryTier.Medium, 38, 3300, new[] { "en", "ha", "ig", "yo" }),
            CreateTerritory("Egypt", "EG", 469.44m, 34000, 37, TerritoryTier.Medium, 35, 3400, new[] { "ar" }),
            CreateTerritory("Morocco", "MA", 132.73m, 18000, 25, TerritoryTier.Medium, 65, 1800, new[] { "ar", "ber" }),
            CreateTerritory("Kenya", "KE", 112.77m, 16000, 22, TerritoryTier.Medium, 68, 1600, new[] { "en", "sw" }),
            CreateTerritory("Ethiopia", "ET", 127.09m, 17500, 24, TerritoryTier.Medium, 66, 1750, new[] { "am", "om", "aa", "so", "ti" }),
            CreateTerritory("Ghana", "GH", 76.98m, 13000, 20, TerritoryTier.Medium, 72, 1300, new[] { "en" }),
            CreateTerritory("Tunisia", "TN", 48.29m, 9500, 16, TerritoryTier.Medium, 85, 950, new[] { "ar", "fr" }),
            
            // Asia
            CreateTerritory("Malaysia", "MY", 432.26m, 32500, 36, TerritoryTier.Medium, 39, 3250, new[] { "ms", "en", "zh", "ta" }),
            CreateTerritory("Thailand", "TH", 534.85m, 35500, 38, TerritoryTier.Medium, 28, 3550, new[] { "th" }),
            CreateTerritory("Philippines", "PH", 394.09m, 31000, 34, TerritoryTier.Medium, 41, 3100, new[] { "en", "tl" }),
            CreateTerritory("Vietnam", "VN", 408.80m, 31500, 35, TerritoryTier.Medium, 37, 3150, new[] { "vi" }),
            CreateTerritory("Bangladesh", "BD", 460.20m, 33500, 36, TerritoryTier.Medium, 36, 3350, new[] { "bn" }),
            CreateTerritory("Pakistan", "PK", 374.94m, 30000, 33, TerritoryTier.Medium, 44, 3000, new[] { "ur", "en" }),
            CreateTerritory("Sri Lanka", "LK", 80.73m, 13500, 21, TerritoryTier.Medium, 71, 1350, new[] { "si", "ta", "en" }),
            CreateTerritory("Myanmar", "MM", 76.09m, 13000, 20, TerritoryTier.Medium, 73, 1300, new[] { "my" }),
            CreateTerritory("Kazakhstan", "KZ", 220.62m, 24000, 28, TerritoryTier.Medium, 55, 2400, new[] { "kk", "ru" }),
            CreateTerritory("Uzbekistan", "UZ", 80.39m, 13500, 21, TerritoryTier.Medium, 70, 1350, new[] { "uz", "ru" }),
            
            // Europe
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
            CreateTerritory("Czech Republic", "CZ", 295.72m, 29500, 32, TerritoryTier.Medium, 53, 2950, new[] { "cs" }),
            CreateTerritory("Romania", "RO", 284.09m, 28500, 31, TerritoryTier.Medium, 54, 2850, new[] { "ro" }),
            CreateTerritory("Hungary", "HU", 181.85m, 22000, 26, TerritoryTier.Medium, 58, 2200, new[] { "hu" }),
            CreateTerritory("Poland", "PL", 688.18m, 42500, 46, TerritoryTier.Medium, 24, 4250, new[] { "pl" }),
            CreateTerritory("Greece", "GR", 219.07m, 23500, 27, TerritoryTier.Medium, 56, 2350, new[] { "el" }),
            CreateTerritory("Bulgaria", "BG", 84.06m, 14000, 21, TerritoryTier.Medium, 69, 1400, new[] { "bg" }),
            
            // Americas
            CreateTerritory("Chile", "CL", 317.18m, 29000, 32, TerritoryTier.Medium, 51, 2900, new[] { "es" }),
            CreateTerritory("Peru", "PE", 242.63m, 24500, 29, TerritoryTier.Medium, 49, 2450, new[] { "es", "qu", "ay" }),
            CreateTerritory("Colombia", "CO", 343.84m, 30000, 33, TerritoryTier.Medium, 48, 3000, new[] { "es" }),
            CreateTerritory("Argentina", "AR", 630.69m, 41000, 44, TerritoryTier.Medium, 26, 4100, new[] { "es" }),
            CreateTerritory("Ecuador", "EC", 115.05m, 16500, 23, TerritoryTier.Medium, 67, 1650, new[] { "es", "qu" }),
            CreateTerritory("Bolivia", "BO", 43.74m, 8500, 15, TerritoryTier.Medium, 95, 850, new[] { "es", "qu", "ay" }),
            CreateTerritory("Dominican Republic", "DO", 113.46m, 16000, 22, TerritoryTier.Medium, 64, 1600, new[] { "es" }),
            CreateTerritory("Guatemala", "GT", 85.99m, 14500, 21, TerritoryTier.Medium, 74, 1450, new[] { "es" }),

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
            CreateTerritory("United States", "US", 26.95m, 200000, 95, TerritoryTier.Major, 1, 20000, new[] { "en", "es" }),
            CreateTerritory("India", "IN", 3.74m, 105000, 86, TerritoryTier.Major, 5, 10500, new[] { "hi", "en", "bn", "te", "mr", "ta", "ur", "gu", "kn", "ml", "or", "pa", "as", "mai", "bh" }),
            CreateTerritory("Brazil", "BR", 2.61m, 88000, 78, TerritoryTier.Major, 9, 8800, new[] { "pt" }),
            CreateTerritory("Russia", "RU", 2.24m, 82000, 74, TerritoryTier.Major, 11, 8200, new[] { "ru" }),
            CreateTerritory("Mexico", "MX", 1.79m, 76000, 67, TerritoryTier.Major, 13, 7600, new[] { "es" }),
            CreateTerritory("Indonesia", "ID", 1.42m, 73000, 66, TerritoryTier.Major, 17, 7300, new[] { "id", "jv", "su" }),
            CreateTerritory("Turkey", "TR", 819.04m, 58000, 58, TerritoryTier.Major, 19, 5800, new[] { "tr" }),
            CreateTerritory("Taiwan", "TW", 790.73m, 56000, 56, TerritoryTier.Major, 21, 5600, new[] { "zh", "nan", "hak" }),
            CreateTerritory("Saudi Arabia", "SA", 833.54m, 59000, 59, TerritoryTier.Major, 23, 5900, new[] { "ar" })
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