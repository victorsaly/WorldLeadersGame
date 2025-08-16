using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WorldLeaders.Infrastructure.Configuration;
using WorldLeaders.Infrastructure.Data;
using WorldLeaders.Infrastructure.Services;
using WorldLeaders.Infrastructure.Tests.Infrastructure;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Services;
using Xunit.Abstractions;

namespace WorldLeaders.Infrastructure.Tests.Services;

/// <summary>
/// Comprehensive testing for Child Safety Validator with COPPA compliance focus
/// Context: Educational game safety system for 12-year-old players
/// Educational Objective: Validate comprehensive content protection and age-appropriate experiences
/// Safety Requirements: Multi-layer validation, cultural sensitivity, educational appropriateness
/// </summary>
public class ChildSafetyValidatorTests : ServiceTestBase
{
    private readonly Mock<IContentModerationService> _mockContentModerationService;
    private readonly Mock<ILogger<ChildSafetyValidator>> _mockLogger;
    private readonly Mock<IOptions<ChildSafetyOptions>> _mockOptions;

    public ChildSafetyValidatorTests(ITestOutputHelper output) : base(output)
    {
        _mockContentModerationService = CreateEducationalMock<IContentModerationService>();
        _mockLogger = CreateEducationalMock<ILogger<ChildSafetyValidator>>();
        _mockOptions = new Mock<IOptions<ChildSafetyOptions>>();
        
        // Setup child safety options
        _mockOptions.Setup(x => x.Value).Returns(new ChildSafetyOptions
        {
            Enabled = true,
            ChildAgeThreshold = 13,
            RequireParentalConsent = true,
            ChildSessionTimeoutMinutes = 30,
            SessionWarningMinutes = 5,
            LogAllEvents = true,
            ContentModerationLevel = "Strict",
            EnforceGdprCompliance = true,
            DataRetentionDays = 365
        });
    }

    protected override void ConfigureAdditionalServices(IServiceCollection services)
    {
        // Create fresh mocks for dependency injection since constructor fields aren't initialized yet
        var mockContentModerationService = new Mock<IContentModerationService>();
        var mockLogger = new Mock<ILogger<ChildSafetyValidator>>();
        var mockOptions = new Mock<IOptions<ChildSafetyOptions>>();
        
        // Setup content moderation service to return different results based on content patterns
        mockContentModerationService
            .Setup(x => x.ValidateContentAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((string content, string context) =>
            {
                // For these tests, the external moderation service should approve content
                // The ChildSafetyValidator itself should detect personal information patterns
                return new ContentModerationResult(
                    IsApproved: true,  // External service approves
                    IsSafe: true,
                    IsEducational: true,
                    IsAgeAppropriate: true,
                    Reason: "External moderation passed",
                    ConfidenceScore: 0.9,
                    Concerns: new List<string>()
                );
            });
            
        // Also setup the ModerateContentAsync method used in some tests
        mockContentModerationService
            .Setup(x => x.ModerateContentAsync(It.IsAny<string>()))
            .ReturnsAsync((string content) =>
            {
                return new ContentModerationResult(
                    IsApproved: true,  // External service approves
                    IsSafe: true,
                    IsEducational: true,
                    IsAgeAppropriate: true,
                    Reason: "External moderation passed",
                    ConfidenceScore: 0.9,
                    Concerns: new List<string>()
                );
            });
            
        // Setup default child safety options
        mockOptions.Setup(x => x.Value).Returns(new ChildSafetyOptions
        {
            Enabled = true,
            ChildAgeThreshold = 13,
            RequireParentalConsent = true,
            ContentModerationLevel = "Strict",
            LogAllEvents = false  // Disable logging for tests
        });
        
        services.AddSingleton(mockContentModerationService.Object);
        services.AddSingleton(mockLogger.Object);
        services.AddSingleton(mockOptions.Object);
        services.AddScoped<IChildSafetyValidator, ChildSafetyValidator>();
    }

    #region Basic Content Validation Tests

    [Fact]
    public async Task ValidateContentAsync_ApprovesEducationalContent()
    {
        // Arrange
        var educationalContent = "Learning about geography is fun! Countries like Canada have amazing landscapes and diverse cultures.";
        
        _mockContentModerationService
            .Setup(x => x.ModerateContentAsync(It.IsAny<string>()))
            .ReturnsAsync(new ContentModerationResult(
                IsApproved: true,
                IsSafe: true,
                IsEducational: true,
                IsAgeAppropriate: true,
                Reason: "Educational geography content",
                ConfidenceScore: 0.95,
                Concerns: new List<string>()
            )
            {
                Categories = new[] { "Educational", "Geography" }
            });

        // Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var validator = GetService<IChildSafetyValidator>();
            return await validator.ValidateContentAsync(educationalContent, "GameContent");
        });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsApproved, "Educational content should be approved");
        Assert.True(result.ConfidenceScore > 0.7, "High confidence for clearly educational content");
        Assert.Contains("educational", result.Reason.ToLowerInvariant());
        
        ValidateEducationalOutcome(result, "content safety validation for educational material");
        
        Output.WriteLine($"✅ Educational Content Approved: {result.Reason} (Confidence: {result.ConfidenceScore:P0})");
    }

    [Fact]
    public async Task ValidateContentAsync_RejectsInappropriateContent()
    {
        // Arrange
        var inappropriateContent = "Give me your email address and phone number so we can meet.";
        
        _mockContentModerationService
            .Setup(x => x.ModerateContentAsync(It.IsAny<string>()))
            .ReturnsAsync(new ContentModerationResult(
                IsApproved: false,
                IsSafe: false,
                IsEducational: false,
                IsAgeAppropriate: false,
                Reason: "Inappropriate content detected",
                ConfidenceScore: 0.95,
                Concerns: new List<string> { "Personal Information", "Contact Request" }
            )
            {
                Categories = new[] { "Personal Information", "Contact Request" }
            });

        // Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var validator = GetService<IChildSafetyValidator>();
            return await validator.ValidateContentAsync(inappropriateContent, "ChatMessage");
        });

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsApproved, "Content with personal information should be rejected");
        Assert.True(result.ConfidenceScore > 0.7, "High confidence for clearly inappropriate content");
        Assert.Contains("inappropriate", result.Reason.ToLowerInvariant());
        
        Output.WriteLine($"✅ Inappropriate Content Rejected: {result.Reason} (Confidence: {result.ConfidenceScore:P0})");
    }

    #endregion

    #region Personal Information Detection Tests

    [Theory]
    [InlineData("My email is student@school.edu", "Email address")]
    [InlineData("Call me at 555-123-4567", "Phone number")]
    [InlineData("I live at 123 Main Street", "Home address")]
    [InlineData("My password is secret123", "Password information")]
    [InlineData("Contact me privately", "Contact request")]
    public async Task ValidateContentAsync_DetectsPersonalInformation(string content, string expectedViolation)
    {
        // Arrange
        _mockContentModerationService
            .Setup(x => x.ModerateContentAsync(It.IsAny<string>()))
            .ReturnsAsync(new ContentModerationResult(
                IsApproved: true,
                IsSafe: true, 
                IsEducational: true,
                IsAgeAppropriate: true,
                Reason: "Content approved",
                ConfidenceScore: 0.8,
                Concerns: new List<string>()
            ));

        // Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var validator = GetService<IChildSafetyValidator>();
            return await validator.ValidateContentAsync(content, "UserInput");
        });

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsApproved, $"Content with {expectedViolation} should be rejected");
        
        var hasWarning = result.Warnings.Any(w => 
            w.ToLowerInvariant().Contains("personal") || 
            w.ToLowerInvariant().Contains("information"));
        Assert.True(hasWarning, "Should include warning about personal information");
        
        Output.WriteLine($"✅ {expectedViolation} Detected and Rejected: {content}");
    }

    #endregion

    #region Educational Content Recognition Tests

    [Theory]
    [InlineData("I learned that Canada's capital is Ottawa", "Geography learning")]
    [InlineData("Economics teaches us about supply and demand", "Economics education")]
    [InlineData("Speaking French helps me understand Quebec", "Language learning")]
    [InlineData("Being a leader means helping others", "Leadership development")]
    public async Task ValidateContentAsync_RecognizesEducationalValue(string content, string educationalArea)
    {
        // Arrange
        _mockContentModerationService
            .Setup(x => x.ModerateContentAsync(It.IsAny<string>()))
            .ReturnsAsync(new ContentModerationResult(IsApproved: true, IsSafe: true, IsEducational: true, IsAgeAppropriate: true, Reason: "Content approved", ConfidenceScore: 0.9, Concerns: new List<string>()));

        // Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var validator = GetService<IChildSafetyValidator>();
            return await validator.ValidateContentAsync(content, "GameContent");
        });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsApproved, $"Educational content about {educationalArea} should be approved");
        Assert.True(result.ConfidenceScore > 0.7, "Educational content should have high confidence");
        
        ValidateEducationalOutcome(result, $"educational content recognition for {educationalArea}");
        
        Output.WriteLine($"✅ {educationalArea} Recognized: {content}");
    }

    #endregion

    #region Content Length and Format Validation Tests

    [Fact]
    public async Task ValidateContentAsync_RejectsExcessivelyLongContent()
    {
        // Arrange
        var longContent = new string('A', 1500); // Exceeds typical limit
        
        _mockContentModerationService
            .Setup(x => x.ModerateContentAsync(It.IsAny<string>()))
            .ReturnsAsync(new ContentModerationResult(IsApproved: true, IsSafe: true, IsEducational: true, IsAgeAppropriate: true, Reason: "Content approved", ConfidenceScore: 0.8, Concerns: new List<string>()));

        // Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var validator = GetService<IChildSafetyValidator>();
            return await validator.ValidateContentAsync(longContent, "UserInput");
        });

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsApproved, "Excessively long content should be rejected");
        Assert.Contains("length", result.Reason.ToLowerInvariant());
        
        Output.WriteLine($"✅ Long Content Rejected: {longContent.Length} characters exceeds limit");
    }

    [Theory]
    [InlineData("", "Empty content")]
    [InlineData("   ", "Whitespace only")]
    [InlineData("\n\t\r", "Control characters only")]
    public async Task ValidateContentAsync_RejectsEmptyOrMeaninglessContent(string content, string description)
    {
        // Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var validator = GetService<IChildSafetyValidator>();
            return await validator.ValidateContentAsync(content, "UserInput");
        });

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsApproved, $"{description} should be rejected");
        
        Output.WriteLine($"✅ {description} Appropriately Rejected");
    }

    #endregion

    #region Cultural Sensitivity Tests

    [Theory]
    [InlineData("Learning about different cultures is amazing!", "Cultural appreciation")]
    [InlineData("Every country has unique traditions", "Cultural diversity")]
    [InlineData("Respecting different languages makes us better leaders", "Cultural inclusion")]
    public async Task ValidateContentAsync_PromotesCulturalSensitivity(string content, string culturalAspect)
    {
        // Arrange
        _mockContentModerationService
            .Setup(x => x.ModerateContentAsync(It.IsAny<string>()))
            .ReturnsAsync(new ContentModerationResult(IsApproved: true, IsSafe: true, IsEducational: true, IsAgeAppropriate: true, Reason: "Content approved", ConfidenceScore: 0.9, Concerns: new List<string>()));

        // Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var validator = GetService<IChildSafetyValidator>();
            return await validator.ValidateContentAsync(content, "EducationalContent");
        });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsApproved, $"Content promoting {culturalAspect} should be approved");
        
        ValidateEducationalOutcome(result, $"cultural sensitivity for {culturalAspect}");
        
        Output.WriteLine($"✅ Cultural Content Approved: {culturalAspect} - {content}");
    }

    #endregion

    #region Age-Appropriate Language Tests

    [Theory]
    [InlineData("That's awesome! I love learning new things!", "Enthusiastic and age-appropriate")]
    [InlineData("Wow, this game is so cool and educational!", "Positive educational excitement")]
    [InlineData("I'm excited to explore more countries!", "Adventure and learning motivation")]
    public async Task ValidateContentAsync_ApprovesAgeAppropriateLanguage(string content, string languageType)
    {
        // Arrange
        _mockContentModerationService
            .Setup(x => x.ModerateContentAsync(It.IsAny<string>()))
            .ReturnsAsync(new ContentModerationResult(IsApproved: true, IsSafe: true, IsEducational: true, IsAgeAppropriate: true, Reason: "Content approved", ConfidenceScore: 0.9, Concerns: new List<string>()));

        // Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var validator = GetService<IChildSafetyValidator>();
            return await validator.ValidateContentAsync(content, "PlayerMessage");
        });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsApproved, $"{languageType} language should be approved");
        
        ValidateChildSafeContent(content, "Age-appropriate language validation");
        
        Output.WriteLine($"✅ Age-Appropriate Language: {languageType} - {content}");
    }

    #endregion

    #region Validation Type-Specific Tests

    [Fact]
    public async Task ValidateContentAsync_AppliesDifferentRules_ForDifferentContentTypes()
    {
        // Arrange
        var educationalContent = "Geography helps us understand our world better!";
        
        _mockContentModerationService
            .Setup(x => x.ModerateContentAsync(It.IsAny<string>()))
            .ReturnsAsync(new ContentModerationResult(IsApproved: true, IsSafe: true, IsEducational: true, IsAgeAppropriate: true, Reason: "Content approved", ConfidenceScore: 0.9, Concerns: new List<string>()));

        // Act & Assert different content types
        var contentTypes = new[] { "GameContent", "PlayerMessage", "AIResponse", "Achievement" };
        
        foreach (var contentType in contentTypes)
        {
            var result = await ExecuteWithDbContextAsync(async context =>
            {
                var validator = GetService<IChildSafetyValidator>();
                return await validator.ValidateContentAsync(educationalContent, contentType);
            });

            Assert.NotNull(result);
            Assert.True(result.IsApproved, $"Educational content should be approved for {contentType}");
            
            Output.WriteLine($"✅ Content Type {contentType}: Approved with confidence {result.ConfidenceScore:P0}");
        }
    }

    #endregion

    #region Integration with Content Moderation Service Tests

    [Fact]
    public async Task ValidateContentAsync_IntegratesWithExternalModeration()
    {
        // Arrange
        var testContent = "This is a test message for educational game validation.";
        
        _mockContentModerationService
            .Setup(x => x.ModerateContentAsync(testContent))
            .ReturnsAsync(new ContentModerationResult(
                IsApproved: true,
                IsSafe: true,
                IsEducational: true,
                IsAgeAppropriate: true,
                Reason: "Content approved for educational use",
                ConfidenceScore: 0.85,
                Concerns: new List<string>()
            )
            {
                Categories = new[] { "Educational", "Safe" }
            });

        // Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var validator = GetService<IChildSafetyValidator>();
            return await validator.ValidateContentAsync(testContent, "TestContent");
        });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsApproved);
        
        // Verify external service was called
        _mockContentModerationService.Verify(
            x => x.ModerateContentAsync(testContent),
            Times.Once,
            "External content moderation service should be called");
            
        Output.WriteLine("✅ Integration with external content moderation service verified");
    }

    [Fact]
    public async Task ValidateContentAsync_HandlesModerationServiceFailure_Gracefully()
    {
        // Arrange
        var testContent = "Test content for service failure scenario.";
        
        _mockContentModerationService
            .Setup(x => x.ModerateContentAsync(It.IsAny<string>()))
            .ThrowsAsync(new HttpRequestException("Service temporarily unavailable"));

        // Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var validator = GetService<IChildSafetyValidator>();
            return await validator.ValidateContentAsync(testContent, "TestContent");
        });

        // Assert
        Assert.NotNull(result);
        // Should fall back to internal validation with appropriate confidence
        Assert.True(result.ConfidenceScore < 0.8, "Should have lower confidence when external service fails");
        
        Output.WriteLine($"✅ Service failure handled gracefully: {result.Reason} (Confidence: {result.ConfidenceScore:P0})");
    }

    #endregion

    #region Performance and Scalability Tests

    [Fact]
    public async Task ValidateContentAsync_PerformsWithinAcceptableTimeframe()
    {
        // Arrange
        var testContent = "Performance test content for child safety validation system.";
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        _mockContentModerationService
            .Setup(x => x.ModerateContentAsync(It.IsAny<string>()))
            .ReturnsAsync(new ContentModerationResult(IsApproved: true, IsSafe: true, IsEducational: true, IsAgeAppropriate: true, Reason: "Content approved", ConfidenceScore: 0.9, Concerns: new List<string>()));

        // Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var validator = GetService<IChildSafetyValidator>();
            return await validator.ValidateContentAsync(testContent, "PerformanceTest");
        });
        
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 500, 
            $"Validation should complete within 500ms for real-time experience. Actual: {stopwatch.ElapsedMilliseconds}ms");
            
        Output.WriteLine($"✅ Performance: Validation completed in {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task ValidateContentAsync_HandlesConcurrentRequests_Efficiently()
    {
        // Arrange
        var testContents = Enumerable.Range(1, 10)
            .Select(i => $"Concurrent test content number {i} for educational game validation.")
            .ToArray();
            
        _mockContentModerationService
            .Setup(x => x.ModerateContentAsync(It.IsAny<string>()))
            .ReturnsAsync(new ContentModerationResult(IsApproved: true, IsSafe: true, IsEducational: true, IsAgeAppropriate: true, Reason: "Content approved", ConfidenceScore: 0.9, Concerns: new List<string>()));

        // Act
        var results = await ExecuteWithDbContextAsync(async context =>
        {
            var validator = GetService<IChildSafetyValidator>();
            
            var tasks = testContents.Select(content => 
                validator.ValidateContentAsync(content, "ConcurrencyTest"));
                
            return await Task.WhenAll(tasks);
        });

        // Assert
        Assert.All(results, result =>
        {
            Assert.NotNull(result);
            Assert.True(result.IsApproved);
        });
        
        Output.WriteLine($"✅ Concurrency: Successfully validated {results.Length} concurrent requests");
    }

    #endregion

    #region Educational Appropriateness Tests

    [Theory]
    [InlineData("Let's learn about world capitals together!", "Collaborative learning")]
    [InlineData("Making mistakes helps us learn and grow!", "Growth mindset")]
    [InlineData("Every student can become a great leader!", "Positive reinforcement")]
    [InlineData("Exploring different cultures makes us wiser!", "Cultural learning")]
    public async Task ValidateContentAsync_RecognizesEducationallyAppropriateMessages(string content, string educationalValue)
    {
        // Arrange
        _mockContentModerationService
            .Setup(x => x.ModerateContentAsync(It.IsAny<string>()))
            .ReturnsAsync(new ContentModerationResult(IsApproved: true, IsSafe: true, IsEducational: true, IsAgeAppropriate: true, Reason: "Content approved", ConfidenceScore: 0.9, Concerns: new List<string>()));

        // Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var validator = GetService<IChildSafetyValidator>();
            return await validator.ValidateContentAsync(content, "EducationalMessage");
        });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsApproved, $"Content with {educationalValue} should be approved");
        Assert.True(result.ConfidenceScore > 0.7, "Educational content should have high confidence");
        
        ValidateEducationalOutcome(result, educationalValue);
        
        Output.WriteLine($"✅ Educational Content: {educationalValue} - {content}");
    }

    #endregion

    #region Edge Cases and Boundary Tests

    [Fact]
    public async Task ValidateContentAsync_HandlesUnicodeAndInternationalContent()
    {
        // Arrange
        var internationalContent = "¡Aprender español es divertido! 学习中文很有趣！ Français est magnifique!";
        
        _mockContentModerationService
            .Setup(x => x.ModerateContentAsync(It.IsAny<string>()))
            .ReturnsAsync(new ContentModerationResult(IsApproved: true, IsSafe: true, IsEducational: true, IsAgeAppropriate: true, Reason: "Content approved", ConfidenceScore: 0.85, Concerns: new List<string>()));

        // Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var validator = GetService<IChildSafetyValidator>();
            return await validator.ValidateContentAsync(internationalContent, "LanguageLearning");
        });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsApproved, "International educational content should be approved");
        
        ValidateEducationalOutcome(result, "multilingual educational content support");
        
        Output.WriteLine($"✅ International Content Supported: {internationalContent}");
    }

    [Fact]
    public async Task ValidateContentAsync_HandlesMixedContentScenarios()
    {
        // Arrange
        var mixedContent = "I love learning geography! My favorite country is Canada because it's so diverse.";
        
        _mockContentModerationService
            .Setup(x => x.ModerateContentAsync(It.IsAny<string>()))
            .ReturnsAsync(new ContentModerationResult(IsApproved: true, IsSafe: true, IsEducational: true, IsAgeAppropriate: true, Reason: "Content approved", ConfidenceScore: 0.9, Concerns: new List<string>()));

        // Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var validator = GetService<IChildSafetyValidator>();
            return await validator.ValidateContentAsync(mixedContent, "StudentResponse");
        });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsApproved, "Mixed educational and personal preference content should be approved");
        
        // Should recognize both educational value and personal expression
        ValidateEducationalOutcome(result, "balanced educational and personal expression");
        
        Output.WriteLine($"✅ Mixed Content Handled: Educational value with personal preference");
    }

    #endregion
}