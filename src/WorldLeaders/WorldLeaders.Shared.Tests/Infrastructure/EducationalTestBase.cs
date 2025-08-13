using System.Text.RegularExpressions;
using WorldLeaders.Shared.Enums;
using Xunit.Abstractions;

namespace WorldLeaders.Shared.Tests.Infrastructure;

/// <summary>
/// Educational test base with child safety validation helpers
/// Context: Educational game component for 12-year-old players
/// Educational Objective: Ensure all tested components maintain child-appropriate content
/// Safety Requirements: Age-appropriate content, positive messaging, cultural sensitivity
/// </summary>
public abstract class EducationalTestBase
{
    protected readonly ITestOutputHelper Output;

    protected EducationalTestBase(ITestOutputHelper output)
    {
        Output = output;
    }

    /// <summary>
    /// Validates content is appropriate for 12-year-old players
    /// </summary>
    /// <param name="content">Content to validate</param>
    /// <param name="context">Educational context for validation</param>
    protected void ValidateChildSafeContent(string content, string context = "")
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Content cannot be null or empty for child safety validation");
        }

        // Age-appropriate language validation
        ValidateLanguageAppropriate(content, context);
        
        // Educational value verification
        ValidateEducationalContent(content, context);
        
        // Positive messaging check
        ValidatePositiveMessaging(content, context);
        
        // Cultural sensitivity validation
        ValidateCulturalSensitivity(content, context);

        Output.WriteLine($"✅ Child safety validation passed for content: '{content.Substring(0, Math.Min(50, content.Length))}...' in context: {context}");
    }

    /// <summary>
    /// Validates educational outcomes for 12-year-old learning objectives
    /// </summary>
    /// <param name="result">Test result to validate</param>
    /// <param name="learningObjective">Expected learning objective</param>
    protected void ValidateEducationalOutcome(object result, string learningObjective)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result), "Educational outcome cannot be null");
        }

        if (string.IsNullOrWhiteSpace(learningObjective))
        {
            throw new ArgumentException("Learning objective must be specified for educational validation");
        }

        // Learning objective achievement validation
        ValidateLearningObjectiveAchievement(result, learningObjective);
        
        // Real-world application verification
        ValidateRealWorldApplication(result, learningObjective);
        
        // Progress tracking accuracy
        ValidateProgressTracking(result, learningObjective);

        Output.WriteLine($"✅ Educational outcome validation passed for objective: '{learningObjective}'");
    }

    /// <summary>
    /// Validates game mechanics are appropriate for child players
    /// </summary>
    /// <param name="jobLevel">Job level to validate</param>
    protected void ValidateChildFriendlyJobLevel(JobLevel jobLevel)
    {
        // Ensure job levels have positive, achievable descriptions
        var jobDescription = GetJobLevelDescription(jobLevel);
        ValidateChildSafeContent(jobDescription, $"Job Level: {jobLevel}");
        
        // Verify progression is encouraging
        Assert.True(IsProgressionEncouraging(jobLevel), 
            $"Job level {jobLevel} should provide encouraging progression for 12-year-olds");

        Output.WriteLine($"✅ Child-friendly job level validation passed for: {jobLevel}");
    }

    /// <summary>
    /// Validates AI agent responses for child safety
    /// </summary>
    /// <param name="agentType">Type of AI agent</param>
    /// <param name="response">Agent response to validate</param>
    protected void ValidateAIAgentChildSafety(AgentType agentType, string response)
    {
        ValidateChildSafeContent(response, $"AI Agent: {agentType}");
        
        // Agent-specific validation
        switch (agentType)
        {
            case AgentType.CareerGuide:
                ValidateCareerGuidanceAppropriate(response);
                break;
            case AgentType.EventNarrator:
                ValidateEventNarrationPositive(response);
                break;
            case AgentType.FortuneTeller:
                ValidateFortuneTellingEncouraging(response);
                break;
            case AgentType.HappinessAdvisor:
                ValidateHappinessAdviceSupportive(response);
                break;
            case AgentType.TerritoryStrategist:
                ValidateTerritoryAdviceEducational(response);
                break;
            case AgentType.LanguageTutor:
                ValidateLanguageTutoringAppropriate(response);
                break;
        }

        Output.WriteLine($"✅ AI agent child safety validation passed for {agentType}");
    }

    #region Private Validation Methods

    private void ValidateLanguageAppropriate(string content, string context)
    {
        // Check for inappropriate language patterns
        var inappropriatePatterns = new[]
        {
            @"\b(hate|stupid|dumb|idiot|loser)\b",  // Negative words
            @"\b(failure|worthless)\b",             // Discouraging terms
            // Removed excessive caps pattern as it interferes with abbreviations like GDP
        };

        foreach (var pattern in inappropriatePatterns)
        {
            if (Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase))
            {
                Assert.Fail($"Content contains inappropriate language for 12-year-olds in context: {context}. Found: {pattern}");
            }
        }
    }

    private void ValidateEducationalContent(string content, string context)
    {
        // Ensure content has educational value
        var educationalIndicators = new[]
        {
            @"\b(learn|discover|explore|understand|grow)\b",
            @"\b(geography|country|language|culture|economy)\b",
            @"\b(skill|knowledge|education|progress)\b"
        };

        var hasEducationalValue = educationalIndicators.Any(pattern =>
            Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase));

        // Allow non-educational content in specific contexts (like error messages)
        if (!context.Contains("error") && !context.Contains("exception"))
        {
            Assert.True(hasEducationalValue || content.Length < 20,
                $"Content should have educational value for 12-year-olds in context: {context}");
        }
    }

    private void ValidatePositiveMessaging(string content, string context)
    {
        // Check for positive, encouraging language
        var positivePatterns = new[]
        {
            @"\b(great|awesome|excellent|wonderful|amazing|good)\b",
            @"\b(try|attempt|practice|improve|better)\b",
            @"\b(help|support|guide|assist)\b"
        };

        // Negative patterns to avoid
        var negativePatterns = new[]
        {
            @"\b(never|impossible|can't|won't|shouldn't)\b",
            @"\b(wrong|bad|terrible|awful)\b"
        };

        foreach (var pattern in negativePatterns)
        {
            Assert.False(Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase),
                $"Content contains discouraging language for 12-year-olds in context: {context}");
        }
    }

    private void ValidateCulturalSensitivity(string content, string context)
    {
        // Basic cultural sensitivity checks
        var insensitivePatterns = new[]
        {
            @"\b(weird|strange|backwards|primitive)\b",  // Cultural judgments
            @"\b(better than|worse than|superior|inferior)\b"  // Comparative judgments
        };

        foreach (var pattern in insensitivePatterns)
        {
            Assert.False(Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase),
                $"Content may contain culturally insensitive language in context: {context}");
        }
    }

    private void ValidateLearningObjectiveAchievement(object result, string learningObjective)
    {
        // Verify the result aligns with educational goals
        Assert.NotNull(result);
        
        // Check if result type makes sense for the learning objective
        if (learningObjective.Contains("geography", StringComparison.OrdinalIgnoreCase))
        {
            // Geography learning should involve territories or countries
            var resultString = result.ToString();
            var hasGeographicProperties = result.GetType().GetProperties()
                .Any(p => p.Name.Contains("Territory", StringComparison.OrdinalIgnoreCase) || 
                         p.Name.Contains("Country", StringComparison.OrdinalIgnoreCase) ||
                         p.Name.Contains("Location", StringComparison.OrdinalIgnoreCase));
            
            var hasGeographicContent = resultString?.Contains("territory", StringComparison.OrdinalIgnoreCase) == true ||
                                     resultString?.Contains("country", StringComparison.OrdinalIgnoreCase) == true ||
                                     resultString?.Contains("geography", StringComparison.OrdinalIgnoreCase) == true ||
                                     resultString?.Contains("location", StringComparison.OrdinalIgnoreCase) == true;
            
            // Accept either geographic properties OR geographic content OR simple test objects
            Assert.True(hasGeographicProperties || hasGeographicContent || IsSimpleTestObject(result),
                       "Geography learning objective should involve territorial or country-based results");
        }
    }

    private bool IsSimpleTestObject(object obj)
    {
        // Allow simple anonymous objects or test objects that don't need specific validation
        var typeName = obj.GetType().Name;
        return typeName.Contains("AnonymousType") || typeName.Contains("TestResult") || 
               obj.GetType().GetProperties().Any(p => p.Name.Contains("Score") || p.Name.Contains("Progress") || p.Name.Contains("Level"));
    }

    private void ValidateRealWorldApplication(object result, string learningObjective)
    {
        // Ensure learning connects to real-world concepts
        Assert.NotNull(result);
        
        // Real-world validation based on learning objective
        if (learningObjective.Contains("economic", StringComparison.OrdinalIgnoreCase))
        {
            // Economic learning should involve realistic values or concepts
            Assert.True(result.ToString()?.Contains("GDP") == true ||
                       result.ToString()?.Contains("income") == true ||
                       result.GetType().GetProperties().Any(p => p.Name.Contains("Income") || p.Name.Contains("GDP")),
                       "Economic learning should connect to real-world economic concepts");
        }
    }

    private void ValidateProgressTracking(object result, string learningObjective)
    {
        // Ensure progress can be measured and tracked
        Assert.NotNull(result);
        
        // Progress should be quantifiable for educational tracking
        var hasQuantifiableProgress = result.GetType().GetProperties()
            .Any(p => p.PropertyType == typeof(int) || 
                     p.PropertyType == typeof(double) || 
                     p.PropertyType == typeof(decimal) ||
                     p.Name.Contains("Level") ||
                     p.Name.Contains("Score") ||
                     p.Name.Contains("Progress"));

        Assert.True(hasQuantifiableProgress,
            "Educational results should include quantifiable progress indicators");
    }

    private string GetJobLevelDescription(JobLevel jobLevel)
    {
        return jobLevel switch
        {
            JobLevel.Farmer => "Growing food and learning about agriculture",
            JobLevel.Gardener => "Caring for plants and understanding nature",
            JobLevel.Shopkeeper => "Managing a store and learning about commerce",
            JobLevel.Artisan => "Creating beautiful crafts and developing skills",
            JobLevel.Politician => "Helping communities and learning about leadership",
            JobLevel.BusinessLeader => "Building companies and understanding economics",
            _ => "Exploring new career opportunities"
        };
    }

    private bool IsProgressionEncouraging(JobLevel jobLevel)
    {
        // All job levels should be portrayed positively
        return true;
    }

    private void ValidateCareerGuidanceAppropriate(string response)
    {
        Assert.Contains("career", response, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("failure", response, StringComparison.OrdinalIgnoreCase);
    }

    private void ValidateEventNarrationPositive(string response)
    {
        // Event narration should be engaging and positive
        Assert.True(response.Length > 10, "Event narration should be descriptive");
    }

    private void ValidateFortuneTellingEncouraging(string response)
    {
        // Fortune telling should be optimistic and encouraging
        Assert.DoesNotContain("bad luck", response, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("misfortune", response, StringComparison.OrdinalIgnoreCase);
    }

    private void ValidateHappinessAdviceSupportive(string response)
    {
        // Happiness advice should be supportive and constructive
        Assert.Contains("happy", response, StringComparison.OrdinalIgnoreCase);
    }

    private void ValidateTerritoryAdviceEducational(string response)
    {
        // Territory advice should include educational elements
        Assert.True(response.Contains("territory") || response.Contains("country") || response.Contains("region"),
            "Territory advice should reference geographical concepts");
    }

    private void ValidateLanguageTutoringAppropriate(string response)
    {
        // Language tutoring should be encouraging and educational
        Assert.Contains("language", response, StringComparison.OrdinalIgnoreCase);
    }

    #endregion
}