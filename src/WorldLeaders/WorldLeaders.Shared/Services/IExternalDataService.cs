namespace WorldLeaders.Shared.Services;

/// <summary>
/// Service for integrating real-world data from external APIs
/// Educational Focus: Authentic, up-to-date country information for learning
/// Context: Educational game requiring real-world accuracy for 12-year-old players
/// Safety: All external data validated for child-appropriate content
/// </summary>
public interface IExternalDataService
{
    /// <summary>
    /// Get country information from REST Countries API
    /// Educational Objective: Provide authentic country data for geography learning
    /// </summary>
    Task<CountryInfo?> GetCountryInfoAsync(string countryCode);
    
    /// <summary>
    /// Get multiple countries information efficiently
    /// Educational Objective: Bulk data retrieval for territory management
    /// </summary>
    Task<List<CountryInfo>> GetCountriesInfoAsync(List<string> countryCodes);
    
    /// <summary>
    /// Get country flag URL for visual learning
    /// Educational Objective: Visual recognition of world flags
    /// </summary>
    Task<string> GetCountryFlagUrlAsync(string countryCode);
    
    /// <summary>
    /// Validate and moderate external content for child safety
    /// Safety: COPPA compliance and age-appropriate content filtering
    /// </summary>
    Task<bool> ValidateContentForChildSafetyAsync(string content);
}

/// <summary>
/// Country information from external APIs with educational focus
/// </summary>
public record CountryInfo(
    string Name,
    string Code,
    string Capital,
    long Population,
    string Region,
    string Subregion,
    List<string> Languages,
    List<string> Currencies,
    string FlagUrl,
    List<string> Timezones,
    string FlagDescription,
    List<string> Borders
);