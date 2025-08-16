namespace WorldLeaders.Shared.Services;

/// <summary>
/// Multi-layer content moderation service for child safety
/// Ensures all AI content is appropriate for 12-year-old players
/// </summary>
public interface IContentModerationService
{
    /// <summary>
    /// Comprehensive content validation for child safety
    /// Combines multiple validation layers for maximum protection
    /// </summary>
    /// <param name="content">The content to validate</param>
    /// <param name="context">Additional context for validation</param>
    /// <returns>Content safety validation result</returns>
    Task<ContentModerationResult> ValidateContentAsync(string content, string context = "");
    
    /// <summary>
    /// Moderate content with comprehensive analysis (alias for test compatibility)
    /// </summary>
    /// <param name="content">The content to moderate</param>
    /// <returns>Content moderation result</returns>
    Task<ContentModerationResult> ModerateContentAsync(string content);

    /// <summary>
    /// Quick validation for real-time chat interactions
    /// </summary>
    /// <param name="content">The content to validate</param>
    /// <returns>True if content is safe for immediate display</returns>
    Task<bool> IsContentSafeAsync(string content);
    
    /// <summary>
    /// Quick appropriateness check for moderation
    /// </summary>
    /// <param name="content">The content to check</param>
    /// <returns>True if content is appropriate</returns>
    Task<bool> IsContentAppropriateModerationAsync(string content);

    /// <summary>
    /// Educational appropriateness validation
    /// Ensures content aligns with geography, economics, and language learning goals
    /// </summary>
    /// <param name="content">The content to validate</param>
    /// <param name="educationalContext">The learning context (geography, economics, language)</param>
    /// <returns>True if content is educationally appropriate</returns>
    Task<bool> IsEducationallyAppropriateAsync(string content, string educationalContext);

    /// <summary>
    /// Age appropriateness validation for 12-year-old comprehension
    /// </summary>
    /// <param name="content">The content to validate</param>
    /// <returns>True if content is appropriate for 12-year-old understanding</returns>
    Task<bool> IsAgeAppropriateAsync(string content);
}

/// <summary>
/// Result of content moderation validation
/// </summary>
public record ContentModerationResult(
    bool IsApproved,
    bool IsSafe,
    bool IsEducational,
    bool IsAgeAppropriate,
    string Reason,
    double ConfidenceScore,
    List<string> Concerns
)
{
    /// <summary>
    /// Whether content is appropriate for children (alias for test compatibility)
    /// </summary>
    public bool IsAppropriate => IsApproved && IsSafe && IsAgeAppropriate;
    
    /// <summary>
    /// Categories of content validation
    /// </summary>
    public string[] Categories { get; init; } = Array.Empty<string>();
    
    /// <summary>
    /// Warnings about the content
    /// </summary>
    public List<string> Warnings { get; init; } = new();
};

/// <summary>
/// Content validation categories for detailed analysis
/// </summary>
public enum ContentCategory
{
    Safe,
    Educational,
    AgeAppropriate,
    Encouraging,
    Respectful,
    FactuallyAccurate
}