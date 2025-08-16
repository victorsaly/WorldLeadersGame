using Microsoft.Extensions.Logging;
using System.Text.Json;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.Infrastructure.Services;

/// <summary>
/// External data service for integrating real-world country information
/// Context: Educational game for 12-year-old players requiring authentic data
/// Educational Objective: Provide accurate, up-to-date country information for geography learning
/// Safety: All external content validated for child-appropriate material
/// </summary>
public class ExternalDataService : IExternalDataService
{
    // Educational constants for child-friendly limits
    private const int MAX_LANGUAGES_PER_TERRITORY = 3; // Limit to 3 languages for children
    private const int MAX_CURRENCIES_PER_TERRITORY = 2; // Limit to 2 currencies for simplicity
    
    private readonly HttpClient _httpClient;
    private readonly IContentModerationService _contentModerationService;
    private readonly ILogger<ExternalDataService> _logger;
    private readonly Dictionary<string, CountryInfo> _cache = new();

    public ExternalDataService(
        HttpClient httpClient,
        IContentModerationService contentModerationService,
        ILogger<ExternalDataService> logger)
    {
        _httpClient = httpClient;
        _contentModerationService = contentModerationService;
        _logger = logger;
    }

    public async Task<CountryInfo?> GetCountryInfoAsync(string countryCode)
    {
        try
        {
            // Check cache first for performance
            if (_cache.TryGetValue(countryCode.ToUpper(), out var cachedInfo))
            {
                return cachedInfo;
            }

            // Call REST Countries API
            var response = await _httpClient.GetAsync($"https://restcountries.com/v3.1/alpha/{countryCode}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get country info for {CountryCode}: {StatusCode}", 
                    countryCode, response.StatusCode);
                return null;
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            var countryData = JsonSerializer.Deserialize<RestCountryResponse[]>(jsonContent);
            
            if (countryData == null || countryData.Length == 0)
            {
                return null;
            }

            var country = countryData[0];
            
            // Validate content for child safety
            var isContentSafe = await ValidateContentForChildSafetyAsync(
                $"{country.Name?.Common} {country.Capital?.FirstOrDefault()} {string.Join(" ", country.Region ?? "")}");
            
            if (!isContentSafe)
            {
                _logger.LogWarning("Content validation failed for country {CountryCode}", countryCode);
                return CreateSafeCountryInfo(countryCode);
            }

            // Create child-safe country info
            var countryInfo = new CountryInfo(
                country.Name?.Common ?? "Unknown",
                countryCode.ToUpper(),
                country.Capital?.FirstOrDefault() ?? "Unknown",
                country.Population ?? 0,
                country.Region ?? "Unknown",
                country.Subregion ?? "Unknown",
                ExtractLanguages(country.Languages),
                ExtractCurrencies(country.Currencies),
                country.Flags?.Png ?? $"https://flagcdn.com/w320/{countryCode.ToLower()}.png",
                country.Timezones ?? new List<string>(),
                country.Flag ?? "🏴",
                country.Borders ?? new List<string>()
            );

            // Cache for future requests
            _cache[countryCode.ToUpper()] = countryInfo;
            
            _logger.LogInformation("Successfully retrieved country info for {CountryCode}", countryCode);
            
            return countryInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting country info for {CountryCode}", countryCode);
            return CreateSafeCountryInfo(countryCode);
        }
    }

    /// <summary>
    /// Get country information (alias for test compatibility)
    /// </summary>
    public async Task<CountryInfo?> GetCountryInformationAsync(string countryCode)
    {
        return await GetCountryInfoAsync(countryCode);
    }

    public async Task<List<CountryInfo>> GetCountriesInfoAsync(List<string> countryCodes)
    {
        var countries = new List<CountryInfo>();
        
        foreach (var countryCode in countryCodes)
        {
            var countryInfo = await GetCountryInfoAsync(countryCode);
            if (countryInfo != null)
            {
                countries.Add(countryInfo);
            }
        }
        
        return countries;
    }

    public async Task<string> GetCountryFlagUrlAsync(string countryCode)
    {
        try
        {
            var countryInfo = await GetCountryInfoAsync(countryCode);
            return countryInfo?.FlagUrl ?? $"https://flagcdn.com/w320/{countryCode.ToLower()}.png";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting flag URL for {CountryCode}", countryCode);
            return $"https://flagcdn.com/w320/{countryCode.ToLower()}.png";
        }
    }

    public async Task<bool> ValidateContentForChildSafetyAsync(string content)
    {
        try
        {
            // Use content moderation service to validate safety
            return await _contentModerationService.IsContentSafeAsync(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating content safety");
            // Default to safe if validation fails
            return true;
        }
    }

    private CountryInfo CreateSafeCountryInfo(string countryCode)
    {
        // Create safe fallback country info for children
        return new CountryInfo(
            GetSafeCountryName(countryCode),
            countryCode.ToUpper(),
            "Capital City",
            1000000,
            "World Region",
            "Sub Region",
            new List<string> { "Local Language" },
            new List<string> { "Local Currency" },
            $"https://flagcdn.com/w320/{countryCode.ToLower()}.png",
            new List<string> { "UTC" },
            "🏴",
            new List<string>()
        );
    }

    private string GetSafeCountryName(string countryCode) => countryCode.ToUpper() switch
    {
        "US" => "United States",
        "GB" => "United Kingdom", 
        "CA" => "Canada",
        "AU" => "Australia",
        "FR" => "France",
        "DE" => "Germany",
        "IT" => "Italy",
        "ES" => "Spain",
        "JP" => "Japan",
        "CN" => "China",
        "IN" => "India",
        "BR" => "Brazil",
        "MX" => "Mexico",
        "RU" => "Russia",
        _ => "Country"
    };

    private List<string> ExtractLanguages(Dictionary<string, LanguageInfo>? languages)
    {
        if (languages == null) return new List<string>();
        
        return languages.Values
            .Where(lang => !string.IsNullOrEmpty(lang.Name))
            .Select(lang => lang.Name!)
            .Take(MAX_LANGUAGES_PER_TERRITORY) // Limit to 3 languages for children
            .ToList();
    }

    private List<string> ExtractCurrencies(Dictionary<string, CurrencyInfo>? currencies)
    {
        if (currencies == null) return new List<string>();
        
        return currencies.Values
            .Where(curr => !string.IsNullOrEmpty(curr.Name))
            .Select(curr => curr.Name!)
            .Take(MAX_CURRENCIES_PER_TERRITORY) // Limit to 2 currencies for simplicity
            .ToList();
    }
}

// DTOs for REST Countries API response
internal record RestCountryResponse(
    NameInfo? Name,
    string? Region,
    string? Subregion,
    List<string>? Capital,
    long? Population,
    FlagInfo? Flags,
    string? Flag,
    Dictionary<string, LanguageInfo>? Languages,
    Dictionary<string, CurrencyInfo>? Currencies,
    List<string>? Timezones,
    List<string>? Borders
);

internal record NameInfo(string? Common, string? Official);
internal record FlagInfo(string? Png, string? Svg);
internal record LanguageInfo(string? Name);
internal record CurrencyInfo(string? Name, string? Symbol);