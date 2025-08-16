using WorldLeaders.Shared.Constants;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.Infrastructure.Services;

/// <summary>
/// Multi-layer content moderation service for child safety
/// Ensures all AI content is appropriate for 12-year-old players
/// Context: Educational game component for 12-year-old safety protection
/// </summary>
public class ContentModerationService : IContentModerationService
{
    // Constants for age-appropriate content validation
    private const int MaxAgeAppropriateWordCount = 80; // Approximately 400 characters
    
    // Configurable inappropriate terms - can be updated without code changes
    private readonly string[] _inappropriateTerms = { "stupid", "dumb", "idiot", "hate" };

    /// <summary>
    /// Comprehensive content validation for child safety
    /// Combines multiple validation layers for maximum protection
    /// </summary>
    public async Task<ContentModerationResult> ValidateContentAsync(string content, string context = "")
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return new ContentModerationResult(
                IsApproved: false,
                IsSafe: false,
                IsEducational: false,
                IsAgeAppropriate: false,
                Reason: "Empty content is not allowed",
                ConfidenceScore: 1.0,
                Concerns: new List<string> { "No content provided" }
            );
        }

        var concerns = new List<string>();
        var contentLower = content.ToLowerInvariant();

        // Layer 1: Safety validation - check for prohibited content
        var isSafe = await IsContentSafeAsync(content);
        if (!isSafe)
        {
            concerns.Add("Contains inappropriate content for children");
        }

        // Layer 2: Educational appropriateness
        var isEducational = await IsEducationallyAppropriateAsync(content, context);
        if (!isEducational)
        {
            concerns.Add("Content lacks educational value");
        }

        // Layer 3: Age appropriateness for 12-year-olds (context-sensitive)
        var isAgeAppropriate = await IsAgeAppropriateAsync(content, context);
        if (!isAgeAppropriate)
        {
            concerns.Add("Content may be too complex or inappropriate for 12-year-olds");
        }

        // Layer 4: Positive messaging validation
        var isPositive = ValidatePositiveMessaging(contentLower);
        if (!isPositive)
        {
            concerns.Add("Content should be more encouraging and positive");
        }

        // Layer 5: Length validation
        var isAppropriateLength = content.Length <= AIAgentConstants.MaxResponseLength;
        if (!isAppropriateLength)
        {
            concerns.Add($"Content exceeds maximum length of {AIAgentConstants.MaxResponseLength} characters");
        }

        var isApproved = isSafe && isEducational && isAgeAppropriate && isPositive && isAppropriateLength;
        var confidenceScore = CalculateConfidenceScore(isSafe, isEducational, isAgeAppropriate, isPositive, isAppropriateLength);

        return new ContentModerationResult(
            IsApproved: isApproved,
            IsSafe: isSafe,
            IsEducational: isEducational,
            IsAgeAppropriate: isAgeAppropriate,
            Reason: isApproved ? "Content approved for child viewing" : string.Join("; ", concerns),
            ConfidenceScore: confidenceScore,
            Concerns: concerns
        );
    }

    /// <summary>
    /// Quick validation for real-time chat interactions
    /// </summary>
    public async Task<bool> IsContentSafeAsync(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return false;

        var contentLower = content.ToLowerInvariant();

        // Check for prohibited content
        foreach (var prohibited in AIAgentConstants.ProhibitedContent)
        {
            if (contentLower.Contains(prohibited))
            {
                return false;
            }
        }

        // Additional safety checks
        if (ContainsInappropriateLanguage(contentLower))
            return false;

        if (ContainsScaryContent(contentLower))
            return false;

        if (ContainsNegativeMessaging(contentLower))
            return false;

        return await Task.FromResult(true);
    }

    /// <summary>
    /// Educational appropriateness validation with context-sensitive rules
    /// </summary>
    public async Task<bool> IsEducationallyAppropriateAsync(string content, string educationalContext)
    {
        if (string.IsNullOrWhiteSpace(content))
            return false;

        var contentLower = content.ToLowerInvariant();

        // For display names and usernames, we're more lenient - just check they're not inappropriate
        if (string.IsNullOrEmpty(educationalContext) || educationalContext.ToLowerInvariant().Contains("display") || educationalContext.ToLowerInvariant().Contains("name"))
        {
            // For display names, just ensure they're appropriate for children
            return ValidateDisplayNameAppropriate(contentLower);
        }

        // For AI-generated content, apply stricter educational validation
        var hasEducationalContent = AIAgentConstants.EducationalKeywords
            .Any(keyword => contentLower.Contains(keyword));

        // Context-specific validation
        var isContextAppropriate = ValidateEducationalContext(contentLower, educationalContext);

        // Ensure factual accuracy indicators
        var isFactual = ValidateFactualContent(contentLower);

        return await Task.FromResult(hasEducationalContent && isContextAppropriate && isFactual);
    }

    /// <summary>
    /// Age appropriateness validation for 12-year-old comprehension with context awareness
    /// </summary>
    public async Task<bool> IsAgeAppropriateAsync(string content, string context = "")
    {
        if (string.IsNullOrWhiteSpace(content))
            return false;

        // Check reading level (simplified assessment)
        var words = content.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Age-appropriate length (not too long for 12-year-olds)
        if (words.Length > MaxAgeAppropriateWordCount)
            return false;

        // Check for overly complex vocabulary
        var complexWords = words.Count(word => word.Length > 12);
        if (complexWords > words.Length * 0.1) // More than 10% complex words
            return false;

        var contentLower = content.ToLowerInvariant();

        // For display names and usernames, use more lenient validation
        if (!string.IsNullOrEmpty(context) && (context.ToLowerInvariant().Contains("display") || context.ToLowerInvariant().Contains("name")))
        {
            // For display names, just ensure they're simple and child-appropriate
            return ValidateDisplayNameAgeAppropriate(contentLower);
        }

        // For other content, check for age-appropriate educational concepts
        var hasAgeAppropriateContent = ValidateAgeAppropriateConcepts(contentLower);

        return await Task.FromResult(hasAgeAppropriateContent);
    }

    /// <summary>
    /// Age appropriateness validation for 12-year-old comprehension (interface compatibility)
    /// </summary>
    public async Task<bool> IsAgeAppropriateAsync(string content)
    {
        return await IsAgeAppropriateAsync(content, "");
    }

    /// <summary>
    /// Simple content moderation check (interface compatibility)
    /// </summary>
    public async Task<ContentModerationResult> ModerateContentAsync(string content)
    {
        return await ValidateContentAsync(content);
    }

    /// <summary>
    /// Simple appropriateness check (interface compatibility)
    /// </summary>
    public async Task<bool> IsContentAppropriateModerationAsync(string content)
    {
        var result = await ValidateContentAsync(content);
        return result.IsApproved;
    }

    #region Private Helper Methods

    private bool ContainsInappropriateLanguage(string content)
    {
        // Check for truly inappropriate language for children - be more lenient for educational content
        return _inappropriateTerms.Any(term => content.Contains(term));
    }

    private bool ContainsScaryContent(string content)
    {
        var scaryTerms = new[] { "scary", "frightening", "terrifying", "nightmare", "monster", "ghost", "demon" };
        return scaryTerms.Any(term => content.Contains(term));
    }

    private bool ContainsNegativeMessaging(string content)
    {
        var negativeTerms = new[] { "impossible", "never", "can't do", "failure", "worthless", "hopeless", "give up" };
        return negativeTerms.Any(term => content.Contains(term));
    }

    private bool ValidatePositiveMessaging(string content)
    {
        var positiveIndicators = new[] {
            "great", "awesome", "wonderful", "amazing", "excellent", "fantastic", "good", "super", "brilliant",
            "well done", "keep going", "you can", "let's", "together", "learn", "explore", "discover",
            "exciting", "fun", "interesting", "cool", "nice", "beautiful", "magnificent", "marvelous",
            "success", "achieve", "grow", "improve", "progress", "develop", "potential", "opportunity",
            "adventure", "journey", "path", "forward", "future", "hope", "bright", "strong", "skill"
        };

        // More flexible - if content doesn't contain negative messaging and has educational value, consider it positive
        var hasPositiveWords = positiveIndicators.Any(indicator => content.Contains(indicator));
        var hasNegativeMessaging = ContainsNegativeMessaging(content);

        // Allow content that either has positive words OR doesn't have negative messaging (neutral educational content)
        return hasPositiveWords || !hasNegativeMessaging;
    }

    private bool ValidateEducationalContext(string content, string context)
    {
        if (string.IsNullOrWhiteSpace(context))
            return true; // No specific context required

        var contextLower = context.ToLowerInvariant();

        // Geography context
        if (contextLower.Contains("geography"))
        {
            var geoTerms = new[] { "country", "continent", "capital", "map", "location", "region", "world" };
            return geoTerms.Any(term => content.Contains(term));
        }

        // Economics context
        if (contextLower.Contains("economics"))
        {
            var econTerms = new[] { "money", "income", "business", "economy", "trade", "gdp", "economic" };
            return econTerms.Any(term => content.Contains(term));
        }

        // Language context
        if (contextLower.Contains("language"))
        {
            var langTerms = new[] { "language", "pronunciation", "speak", "culture", "communication" };
            return langTerms.Any(term => content.Contains(term));
        }

        return true; // Allow general educational content
    }

    private bool ValidateFactualContent(string content)
    {
        // Simple validation for factual language indicators
        var factualIndicators = new[] {
            "learn", "study", "discover", "explore", "understand", "practice",
            "develop", "grow", "improve", "progress", "achieve"
        };

        return factualIndicators.Any(indicator => content.Contains(indicator));
    }

    private bool ValidateAgeAppropriateConcepts(string content)
    {
        // Ensure concepts are suitable for 12-year-olds
        var appropriateConcepts = new[] {
            "learn", "school", "friend", "family", "game", "fun", "explore", "discover",
            "country", "world", "language", "culture", "job", "career", "money", "help"
        };

        return appropriateConcepts.Any(concept => content.Contains(concept));
    }

    private bool ValidateDisplayNameAppropriate(string content)
    {
        // For display names, we're more lenient - just ensure they're child-friendly
        // Check it's not inappropriate language
        if (ContainsInappropriateLanguage(content))
            return false;

        // Check it's not scary content
        if (ContainsScaryContent(content))
            return false;

        // Check it's not negative messaging
        if (ContainsNegativeMessaging(content))
            return false;

        // Allow common child-friendly terms even if not explicitly educational
        var childFriendlyTerms = new[] {
            "student", "learner", "explorer", "young", "little", "kid", "child",
            "world", "star", "bright", "smart", "cool", "awesome", "amazing",
            "geography", "history", "science", "math", "art", "music",
            "leader", "captain", "champion", "hero", "friend", "buddy"
        };

        // If it contains any child-friendly terms OR is very short (like "Alex"), it's OK
        return childFriendlyTerms.Any(term => content.Contains(term)) || content.Length <= 10;
    }

    private bool ValidateDisplayNameAgeAppropriate(string content)
    {
        // For display names, age appropriateness is much more lenient
        // Just check basic criteria
        
        // Not too long for a display name
        if (content.Length > 50)
            return false;

        // No complex words for display names
        var words = content.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (words.Any(word => word.Length > 15)) // Very long words not appropriate for display names
            return false;

        // Allow any reasonable display name that passes safety checks
        // (safety checks are already done in other validation layers)
        return true;
    }

    private double CalculateConfidenceScore(bool isSafe, bool isEducational, bool isAgeAppropriate, bool isPositive, bool isAppropriateLength)
    {
        var factors = new[] { isSafe, isEducational, isAgeAppropriate, isPositive, isAppropriateLength };
        var passedCount = factors.Count(f => f);
        return (double)passedCount / factors.Length;
    }

    #endregion
}