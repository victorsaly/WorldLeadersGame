using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WorldLeaders.Infrastructure.Data;
using WorldLeaders.Infrastructure.Entities;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Models;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.Infrastructure.Services;

/// <summary>
/// Territory service implementation for educational world geography game
/// Context: Educational game for 12-year-old players learning geography, economics, and strategy
/// Educational Objectives: Geography mastery, economic understanding, cultural awareness
/// Safety: Age-appropriate content and positive learning experiences
/// </summary>
public class TerritoryService : ITerritoryService
{
    // Educational constants for child-friendly limits
    private const int MAX_LANGUAGE_CHALLENGES = 5; // Limit challenges to avoid overwhelming children
    private const int CHILD_ACCURACY_REQUIREMENT = 70; // 70% accuracy required for children
    
    private readonly WorldLeadersDbContext _context;
    private readonly IExternalDataService _externalDataService;
    private readonly IPlayerService _playerService;
    private readonly ILogger<TerritoryService> _logger;

    public TerritoryService(
        WorldLeadersDbContext context,
        IExternalDataService externalDataService,
        IPlayerService playerService,
        ILogger<TerritoryService> logger)
    {
        _context = context;
        _externalDataService = externalDataService;
        _playerService = playerService;
        _logger = logger;
    }

    public async Task<List<TerritoryDto>> GetAvailableTerritoriesAsync(Guid playerId)
    {
        try
        {
            var availableTerritories = await _context.Territories
                .Where(t => t.IsAvailable && !t.IsDeleted && t.OwnedByPlayerId == null)
                .OrderBy(t => t.Tier)
                .ThenBy(t => t.Cost)
                .ToListAsync();

            return availableTerritories.Select(t => MapToTerritoryDto(t, false)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available territories for player {PlayerId}", playerId);
            return new List<TerritoryDto>();
        }
    }

    public async Task<List<TerritoryDto>> GetPlayerTerritoriesAsync(Guid playerId)
    {
        try
        {
            var playerTerritories = await _context.Territories
                .Where(t => t.OwnedByPlayerId == playerId && !t.IsDeleted)
                .OrderBy(t => t.AcquiredAt)
                .ToListAsync();

            return playerTerritories.Select(t => MapToTerritoryDto(t, true)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting territories for player {PlayerId}", playerId);
            return new List<TerritoryDto>();
        }
    }

    public async Task<TerritoryAcquisitionResult> AcquireTerritoryAsync(Guid playerId, Guid territoryId)
    {
        try
        {
            // Get player and territory data
            var player = await _context.Players.FindAsync(playerId);
            var territory = await _context.Territories.FindAsync(territoryId);

            if (player == null)
            {
                return new TerritoryAcquisitionResult(false, "Player not found", null, null, null, 
                    new List<string> { "Make sure you're logged in properly!" });
            }

            if (territory == null || !territory.IsAvailable || territory.OwnedByPlayerId != null)
            {
                return new TerritoryAcquisitionResult(false, "Territory not available", null, null, null,
                    new List<string> { "Try looking for other available territories!" });
            }

            // Educational validation: Check reputation requirement
            if (player.Reputation < territory.ReputationRequired)
            {
                var tips = new List<string>
                {
                    $"You need {territory.ReputationRequired}% reputation to acquire {territory.CountryName}",
                    "Build reputation by completing dice challenges and helping your people!",
                    "Try acquiring smaller territories first to build your reputation"
                };
                return new TerritoryAcquisitionResult(false, 
                    $"Reputation too low! Need {territory.ReputationRequired}%, have {player.Reputation}%", 
                    null, null, null, tips);
            }

            // Educational validation: Check if player has enough money
            if (player.Income < territory.Cost)
            {
                var tips = new List<string>
                {
                    $"You need {territory.Cost:C} to acquire {territory.CountryName}",
                    "Earn more income by improving your job through dice rolling!",
                    "Each territory generates monthly income once acquired"
                };
                return new TerritoryAcquisitionResult(false, 
                    $"Not enough money! Need {territory.Cost:C}, have {player.Income:C}", 
                    null, null, null, tips);
            }

            // Successful acquisition - Educational economics lesson
            territory.OwnedByPlayerId = playerId;
            territory.AcquiredAt = DateTime.UtcNow;
            territory.IsAvailable = false;

            // Apply cost and reputation bonus for successful acquisition
            var resourceChange = new ResourceChangeRecord(
                -territory.Cost, // Cost deducted
                5, // Reputation bonus for successful acquisition
                10, // Happiness bonus for expanding territory
                $"Acquired {territory.CountryName}! Your people are proud of this expansion.",
                EventType.International,
                DateTime.UtcNow
            );

            player.Income -= territory.Cost;
            player.Reputation = Math.Min(100, player.Reputation + 5);
            player.Happiness = Math.Min(100, player.Happiness + 10);
            player.LastActiveAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Check for achievements
            var achievement = await CheckForTerritoryAchievements(playerId);
            
            var educationalTips = new List<string>
            {
                $"🎉 Welcome to {territory.CountryName}! You're now learning about this amazing country.",
                $"💰 This territory will generate ${territory.MonthlyIncome} monthly income!",
                $"🌍 {territory.CountryName} is in the {GetTierDescription(territory.Tier)} tier.",
                "🎯 Try learning the languages spoken here for extra bonuses!"
            };

            var territoryDto = MapToTerritoryDto(territory, true);

            _logger.LogInformation("Player {PlayerId} successfully acquired territory {TerritoryName}", 
                playerId, territory.CountryName);

            return new TerritoryAcquisitionResult(true, 
                $"Successfully acquired {territory.CountryName}!", 
                territoryDto, resourceChange, achievement, educationalTips);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error acquiring territory {TerritoryId} for player {PlayerId}", 
                territoryId, playerId);
            return new TerritoryAcquisitionResult(false, "An error occurred during acquisition", 
                null, null, null, new List<string> { "Please try again later!" });
        }
    }

    public async Task<TerritoryDetailDto> GetTerritoryDetailsAsync(Guid territoryId)
    {
        try
        {
            var territory = await _context.Territories.FindAsync(territoryId);
            if (territory == null)
            {
                throw new ArgumentException($"Territory {territoryId} not found");
            }

            // Get additional data from external API
            var countryInfo = await _externalDataService.GetCountryInfoAsync(territory.CountryCode);
            
            var languages = DeserializeLanguages(territory.OfficialLanguagesJson);
            
            return new TerritoryDetailDto(
                territory.Id,
                territory.CountryName,
                territory.CountryCode,
                languages,
                territory.GdpInBillions,
                territory.Tier,
                territory.Cost,
                territory.ReputationRequired,
                territory.MonthlyIncome,
                territory.IsAvailable,
                territory.OwnedByPlayerId != null,
                countryInfo?.FlagUrl ?? $"https://flagcdn.com/w320/{territory.CountryCode.ToLower()}.png",
                countryInfo?.Capital ?? "Unknown",
                countryInfo?.Population ?? 0,
                countryInfo?.Region ?? "Unknown",
                countryInfo?.Subregion ?? "Unknown",
                countryInfo?.Currencies ?? new List<string>(),
                GenerateEducationalFact(territory),
                GenerateGeographicFeatures(territory),
                GenerateCulturalHighlights(territory)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting territory details for {TerritoryId}", territoryId);
            throw;
        }
    }

    public async Task<List<TerritoryDto>> GetTerritoriesByTierAsync(TerritoryTier tier, Guid playerId)
    {
        try
        {
            var territories = await _context.Territories
                .Where(t => t.Tier == tier && !t.IsDeleted)
                .OrderBy(t => t.Cost)
                .ToListAsync();

            return territories.Select(t => MapToTerritoryDto(t, t.OwnedByPlayerId == playerId)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tier {Tier} territories for player {PlayerId}", tier, playerId);
            return new List<TerritoryDto>();
        }
    }

    public async Task<int> CalculateMonthlyTerritoryIncomeAsync(Guid playerId)
    {
        try
        {
            var totalIncome = await _context.Territories
                .Where(t => t.OwnedByPlayerId == playerId && !t.IsDeleted)
                .SumAsync(t => t.MonthlyIncome);

            return totalIncome;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating territory income for player {PlayerId}", playerId);
            return 0;
        }
    }

    public async Task<List<LanguageChallengeDto>> GetTerritoryLanguageChallengesAsync(Guid playerId)
    {
        try
        {
            var playerTerritories = await _context.Territories
                .Where(t => t.OwnedByPlayerId == playerId && !t.IsDeleted)
                .ToListAsync();

            var challenges = new List<LanguageChallengeDto>();

            foreach (var territory in playerTerritories)
            {
                var languages = DeserializeLanguages(territory.OfficialLanguagesJson);
                foreach (var languageCode in languages)
                {
                    // Create simplified language challenges for children
                    challenges.Add(new LanguageChallengeDto(
                        languageCode,
                        GetLanguageName(languageCode),
                        GetSimpleWord(territory.CountryName, languageCode),
                        GetPronunciation(territory.CountryName, languageCode),
                        $"/audio/{languageCode}/{territory.CountryCode.ToLower()}.mp3",
                        CHILD_ACCURACY_REQUIREMENT // 70% accuracy required for children
                    ));
                }
            }

            return challenges.Take(MAX_LANGUAGE_CHALLENGES).ToList(); // Limit to 5 challenges to avoid overwhelming children
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting language challenges for player {PlayerId}", playerId);
            return new List<LanguageChallengeDto>();
        }
    }

    public async Task<CulturalContextDto> GetTerritoryCulturalContextAsync(Guid territoryId)
    {
        try
        {
            var territory = await _context.Territories.FindAsync(territoryId);
            if (territory == null)
            {
                throw new ArgumentException($"Territory {territoryId} not found");
            }

            return new CulturalContextDto(
                territoryId,
                territory.CountryName,
                GenerateHistoricalSignificance(territory),
                GenerateCulturalTraditions(territory),
                GenerateFamousLandmarks(territory),
                GenerateNotableAchievements(territory),
                GenerateGeographyLesson(territory),
                GenerateEconomicLesson(territory),
                GenerateEducationalQuizQuestions(territory),
                GenerateChildFriendlyDescription(territory)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cultural context for territory {TerritoryId}", territoryId);
            throw;
        }
    }

    private TerritoryDto MapToTerritoryDto(TerritoryEntity entity, bool isOwned)
    {
        var languages = DeserializeLanguages(entity.OfficialLanguagesJson);
        
        return new TerritoryDto(
            entity.Id,
            entity.CountryName,
            entity.CountryCode,
            languages,
            entity.GdpInBillions,
            entity.Tier,
            entity.Cost,
            entity.ReputationRequired,
            entity.MonthlyIncome,
            entity.IsAvailable,
            isOwned
        );
    }

    private List<string> DeserializeLanguages(string languagesJson)
    {
        try
        {
            return JsonSerializer.Deserialize<List<string>>(languagesJson) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private async Task<Achievement?> CheckForTerritoryAchievements(Guid playerId)
    {
        var territoryCount = await _context.Territories.CountAsync(t => t.OwnedByPlayerId == playerId);
        
        if (territoryCount == 1)
        {
            await _playerService.AwardAchievementAsync(playerId, "first_territory");
            return new Achievement("first_territory", "First Territory", "You acquired your first territory! Welcome to world leadership!", "🏴", 250, DateTime.UtcNow, true);
        }
        
        if (territoryCount == 5)
        {
            await _playerService.AwardAchievementAsync(playerId, "regional_leader");
            return new Achievement("regional_leader", "Regional Leader", "You now control 5 territories! You're becoming a true world leader!", "🌍", 500, DateTime.UtcNow, true);
        }

        return null;
    }

    private string GetTierDescription(TerritoryTier tier) => tier switch
    {
        TerritoryTier.Small => "Small Nation",
        TerritoryTier.Medium => "Medium Power",
        TerritoryTier.Major => "Major Power",
        _ => "Unknown"
    };

    private string GenerateEducationalFact(TerritoryEntity territory)
    {
        // Educational facts appropriate for 12-year-olds
        return territory.CountryCode switch
        {
            "US" => "The United States has 50 states and is the world's largest economy!",
            "CN" => "China is home to over 1.4 billion people and built the Great Wall!",
            "JP" => "Japan is famous for sushi, anime, and amazing technology!",
            "DE" => "Germany is known for cars like BMW and Mercedes, and invented the printing press!",
            "GB" => "The United Kingdom includes England, Scotland, Wales, and Northern Ireland!",
            "FR" => "France gave the world the Eiffel Tower and delicious pastries!",
            "IT" => "Italy is shaped like a boot and gave us pizza and pasta!",
            "BR" => "Brazil is home to the Amazon rainforest and carnival celebrations!",
            "AU" => "Australia is both a country and a continent with unique animals like kangaroos!",
            "IN" => "India is known for colorful festivals, spicy food, and the beautiful Taj Mahal!",
            _ => $"{territory.CountryName} is a wonderful country with rich culture and history!"
        };
    }

    private List<string> GenerateGeographicFeatures(TerritoryEntity territory)
    {
        // Age-appropriate geographic features
        return territory.CountryCode switch
        {
            "US" => new() { "Rocky Mountains", "Grand Canyon", "Great Lakes", "Yellowstone National Park" },
            "CN" => new() { "Great Wall", "Himalayas", "Yellow River", "Gobi Desert" },
            "BR" => new() { "Amazon River", "Amazon Rainforest", "Atlantic Coast", "Pantanal Wetlands" },
            "AU" => new() { "Great Barrier Reef", "Uluru (Ayers Rock)", "Outback Desert", "Blue Mountains" },
            "RU" => new() { "Siberian Tundra", "Lake Baikal", "Ural Mountains", "Volga River" },
            _ => new() { "Mountains", "Rivers", "Forests", "Coastlines" }
        };
    }

    private List<string> GenerateCulturalHighlights(TerritoryEntity territory)
    {
        // Child-friendly cultural highlights
        return territory.CountryCode switch
        {
            "JP" => new() { "Cherry blossom festivals", "Traditional tea ceremonies", "Anime and manga", "Martial arts" },
            "MX" => new() { "Colorful Day of the Dead celebrations", "Mariachi music", "Ancient Mayan pyramids", "Delicious tacos" },
            "IN" => new() { "Bollywood movies", "Holi color festival", "Yoga and meditation", "Beautiful saris" },
            "EG" => new() { "Ancient pyramids", "Mummies and pharaohs", "Hieroglyphic writing", "River Nile" },
            "FR" => new() { "Art museums", "Fashion and style", "Delicious cuisine", "Historic castles" },
            _ => new() { "Traditional festivals", "Local cuisine", "Music and dance", "Art and crafts" }
        };
    }

    private string GetLanguageName(string languageCode) => languageCode switch
    {
        "en" => "English",
        "es" => "Spanish", 
        "fr" => "French",
        "de" => "German",
        "it" => "Italian",
        "pt" => "Portuguese",
        "ru" => "Russian",
        "zh" => "Chinese",
        "ja" => "Japanese",
        "ko" => "Korean",
        "ar" => "Arabic",
        "hi" => "Hindi",
        _ => "Local Language"
    };

    private string GetSimpleWord(string countryName, string languageCode)
    {
        // Simple greetings in different languages for children
        return languageCode switch
        {
            "es" => "Hola", // Hello in Spanish
            "fr" => "Bonjour", // Hello in French  
            "de" => "Guten Tag", // Hello in German
            "it" => "Ciao", // Hello in Italian
            "pt" => "Olá", // Hello in Portuguese
            "zh" => "Nǐ hǎo", // Hello in Chinese
            "ja" => "Konnichiwa", // Hello in Japanese
            "ko" => "Annyeonghaseyo", // Hello in Korean
            "ru" => "Privet", // Hello in Russian
            "ar" => "Marhaba", // Hello in Arabic
            "hi" => "Namaste", // Hello in Hindi
            _ => "Hello"
        };
    }

    private string GetPronunciation(string countryName, string languageCode)
    {
        return languageCode switch
        {
            "es" => "OH-lah",
            "fr" => "bon-ZHOOR",
            "de" => "GOO-ten tahk",
            "it" => "chow",
            "pt" => "oh-LAH",
            "zh" => "nee how",
            "ja" => "kon-nee-chee-wah",
            "ko" => "ahn-nyeong-hah-say-yo",
            "ru" => "pree-VEHT",
            "ar" => "mar-HAH-bah",
            "hi" => "nah-mas-TAY",
            _ => "HEH-loh"
        };
    }

    // Additional cultural context methods for educational content
    private string GenerateHistoricalSignificance(TerritoryEntity territory) =>
        $"{territory.CountryName} has a rich history spanning thousands of years with many important contributions to world culture and knowledge.";

    private List<string> GenerateCulturalTraditions(TerritoryEntity territory) =>
        new() { "Traditional festivals", "Folk music and dance", "Local crafts", "Family celebrations" };

    private List<string> GenerateFamousLandmarks(TerritoryEntity territory) =>
        new() { "Historic monuments", "Natural wonders", "Cultural sites", "Important buildings" };

    private List<string> GenerateNotableAchievements(TerritoryEntity territory) =>
        new() { "Scientific discoveries", "Artistic contributions", "Sports achievements", "Cultural innovations" };

    private string GenerateGeographyLesson(TerritoryEntity territory) =>
        $"Learn about {territory.CountryName}'s location, climate, and natural features that make it unique!";

    private string GenerateEconomicLesson(TerritoryEntity territory) =>
        $"Discover how {territory.CountryName} creates wealth through its industries, resources, and trade!";

    private List<string> GenerateEducationalQuizQuestions(TerritoryEntity territory) =>
        new() { 
            $"What continent is {territory.CountryName} located on?",
            $"What languages are spoken in {territory.CountryName}?",
            $"What is the capital city of {territory.CountryName}?"
        };

    private string GenerateChildFriendlyDescription(TerritoryEntity territory) =>
        $"{territory.CountryName} is an amazing place where people live, work, and play! It has beautiful landscapes, interesting culture, and friendly people who have created wonderful traditions over many years.";
}