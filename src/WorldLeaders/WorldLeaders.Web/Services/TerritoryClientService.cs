using System.Text.Json;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Services;
using WorldLeaders.Shared.Models;

namespace WorldLeaders.Web.Services;

/// <summary>
/// HTTP-based client service for territory operations
/// Context: Educational game client communicating with territory API
/// Educational Objective: Enable territory exploration and acquisition through API calls
/// </summary>
public class TerritoryClientService : ITerritoryService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TerritoryClientService> _logger;

    public TerritoryClientService(IHttpClientFactory httpClientFactory, ILogger<TerritoryClientService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<List<TerritoryDto>> GetAvailableTerritoriesAsync(Guid playerId)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("GameAPI");
            var response = await httpClient.GetAsync($"api/Territory/available/{playerId}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var territories = JsonSerializer.Deserialize<List<TerritoryDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                _logger.LogInformation("Loaded {Count} available territories for player {PlayerId}", 
                    territories?.Count ?? 0, playerId);
                    
                return territories ?? new List<TerritoryDto>();
            }
            else
            {
                _logger.LogWarning("Failed to load available territories. Status: {StatusCode}", response.StatusCode);
                return new List<TerritoryDto>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading available territories for player {PlayerId}", playerId);
            return new List<TerritoryDto>();
        }
    }

    public async Task<List<TerritoryDto>> GetPlayerTerritoriesAsync(Guid playerId)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("GameAPI");
            var response = await httpClient.GetAsync($"api/Territory/owned/{playerId}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var territories = JsonSerializer.Deserialize<List<TerritoryDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                _logger.LogInformation("Loaded {Count} owned territories for player {PlayerId}", 
                    territories?.Count ?? 0, playerId);
                    
                return territories ?? new List<TerritoryDto>();
            }
            else
            {
                _logger.LogWarning("Failed to load owned territories. Status: {StatusCode}", response.StatusCode);
                return new List<TerritoryDto>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading owned territories for player {PlayerId}", playerId);
            return new List<TerritoryDto>();
        }
    }

    public async Task<TerritoryAcquisitionResult> AcquireTerritoryAsync(Guid playerId, Guid territoryId)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("GameAPI");
            var response = await httpClient.PostAsync($"api/Territory/acquire/{playerId}/{territoryId}", null);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TerritoryAcquisitionResult>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                _logger.LogInformation("Territory acquisition result for player {PlayerId}: {Success}", 
                    playerId, result?.Success ?? false);
                    
                return result ?? new TerritoryAcquisitionResult(false, "Acquisition failed", null, null, null, new List<string>());
            }
            else
            {
                _logger.LogWarning("Failed to acquire territory. Status: {StatusCode}", response.StatusCode);
                return new TerritoryAcquisitionResult(false, "Territory acquisition failed", null, null, null, new List<string>());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error acquiring territory {TerritoryId} for player {PlayerId}", territoryId, playerId);
            return new TerritoryAcquisitionResult(false, "An error occurred during territory acquisition", null, null, null, new List<string>());
        }
    }

    public async Task<TerritoryDetailDto> GetTerritoryDetailsAsync(Guid territoryId)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("GameAPI");
            var response = await httpClient.GetAsync($"api/Territory/details/{territoryId}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var details = JsonSerializer.Deserialize<TerritoryDetailDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                _logger.LogInformation("Loaded territory details for {TerritoryId}", territoryId);
                return details ?? CreateDefaultTerritoryDetail(territoryId);
            }
            else
            {
                _logger.LogWarning("Failed to load territory details for {TerritoryId}. Status: {StatusCode}", territoryId, response.StatusCode);
                return CreateDefaultTerritoryDetail(territoryId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading territory details for {TerritoryId}", territoryId);
            return CreateDefaultTerritoryDetail(territoryId);
        }
    }

    public async Task<List<TerritoryDto>> GetTerritoriesByTierAsync(TerritoryTier tier, Guid playerId)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("GameAPI");
            var response = await httpClient.GetAsync($"api/Territory/by-tier/{tier}/{playerId}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var territories = JsonSerializer.Deserialize<List<TerritoryDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                _logger.LogInformation("Loaded {Count} territories for tier {Tier} and player {PlayerId}", 
                    territories?.Count ?? 0, tier, playerId);
                    
                return territories ?? new List<TerritoryDto>();
            }
            else
            {
                _logger.LogWarning("Failed to load territories by tier {Tier}. Status: {StatusCode}", tier, response.StatusCode);
                return new List<TerritoryDto>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading territories by tier {Tier} for player {PlayerId}", tier, playerId);
            return new List<TerritoryDto>();
        }
    }

    public async Task<int> CalculateMonthlyTerritoryIncomeAsync(Guid playerId)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("GameAPI");
            var response = await httpClient.GetAsync($"api/Territory/income/{playerId}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var income = JsonSerializer.Deserialize<int>(content);
                
                _logger.LogInformation("Monthly territory income for player {PlayerId}: {Income}", playerId, income);
                return income;
            }
            else
            {
                _logger.LogWarning("Failed to calculate territory income. Status: {StatusCode}", response.StatusCode);
                return 0;
            }
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
            var httpClient = _httpClientFactory.CreateClient("GameAPI");
            var response = await httpClient.GetAsync($"api/Territory/language-challenges/{playerId}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var challenges = JsonSerializer.Deserialize<List<LanguageChallengeDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                _logger.LogInformation("Loaded {Count} language challenges for player {PlayerId}", 
                    challenges?.Count ?? 0, playerId);
                    
                return challenges ?? new List<LanguageChallengeDto>();
            }
            else
            {
                _logger.LogWarning("Failed to load language challenges. Status: {StatusCode}", response.StatusCode);
                return new List<LanguageChallengeDto>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading language challenges for player {PlayerId}", playerId);
            return new List<LanguageChallengeDto>();
        }
    }

    public async Task<CulturalContextDto> GetTerritoryCulturalContextAsync(Guid territoryId)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("GameAPI");
            var response = await httpClient.GetAsync($"api/Territory/cultural-context/{territoryId}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var culturalContext = JsonSerializer.Deserialize<CulturalContextDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                _logger.LogInformation("Loaded cultural context for territory {TerritoryId}", territoryId);
                return culturalContext ?? CreateDefaultCulturalContext(territoryId);
            }
            else
            {
                _logger.LogWarning("Failed to load cultural context for territory {TerritoryId}. Status: {StatusCode}", territoryId, response.StatusCode);
                return CreateDefaultCulturalContext(territoryId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading cultural context for territory {TerritoryId}", territoryId);
            return CreateDefaultCulturalContext(territoryId);
        }
    }

    private TerritoryDetailDto CreateDefaultTerritoryDetail(Guid territoryId)
    {
        return new TerritoryDetailDto(
            Id: territoryId,
            CountryName: "Unknown Territory",
            CountryCode: "XX",
            OfficialLanguages: new List<string> { "en" },
            GdpInBillions: 0m,
            Tier: TerritoryTier.Small,
            Cost: 1000,
            ReputationRequired: 10,
            MonthlyIncome: 100,
            IsAvailable: false,
            IsOwned: false,
            FlagUrl: "",
            Capital: "Unknown",
            Population: 0,
            Region: "Unknown",
            Subregion: "Unknown",
            Currencies: new List<string> { "USD" },
            EducationalFact: "This territory is currently not available for exploration.",
            GeographicFeatures: new List<string> { "Unknown features" },
            CulturalHighlights: new List<string> { "Discover cultural highlights by exploring!" }
        );
    }

    private CulturalContextDto CreateDefaultCulturalContext(Guid territoryId)
    {
        return new CulturalContextDto(
            TerritoryId: territoryId,
            CountryName: "Unknown Territory",
            HistoricalSignificance: "This territory has a rich history waiting to be discovered!",
            CulturalTraditions: new List<string> { "Unique traditions await discovery" },
            FamousLandmarks: new List<string> { "Amazing landmarks to explore" },
            NotableAchievements: new List<string> { "Great achievements in history" },
            GeographyLesson: "This territory offers wonderful geography learning opportunities!",
            EconomicLesson: "Learn about economics by exploring this territory!",
            EducationalQuizQuestions: new List<string> { "What makes this territory special?" },
            ChildFriendlyDescription: "An exciting territory with lots to learn and discover! 🌍"
        );
    }
}
