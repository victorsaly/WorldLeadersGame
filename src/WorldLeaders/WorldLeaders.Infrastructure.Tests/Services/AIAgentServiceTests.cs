using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WorldLeaders.Infrastructure.Configuration;
using WorldLeaders.Infrastructure.Services;
using WorldLeaders.Infrastructure.Tests.Infrastructure;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Services;
using Xunit.Abstractions;

namespace WorldLeaders.Infrastructure.Tests.Services;

/// <summary>
/// Comprehensive testing for AI Agent services with educational focus
/// Context: Educational game component for 12-year-old geography, economics, and language learning
/// Educational Objective: Validate AI agents provide safe, encouraging, and educational interactions
/// Safety Requirements: Age-appropriate content, positive messaging, cultural sensitivity
/// </summary>
public class AIAgentServiceTests : ServiceTestBase
{
    private readonly Mock<IContentModerationService> _mockContentModerationService;
    private readonly Mock<ILogger<AIAgentService>> _mockLogger;
    private readonly Mock<ILogger<CloudAIAgentService>> _mockCloudLogger;
    private readonly Mock<IOptions<AzureAIOptions>> _mockAzureOptions;

    public AIAgentServiceTests(ITestOutputHelper output) : base(output)
    {
        _mockContentModerationService = CreateEducationalMock<IContentModerationService>();
        _mockLogger = CreateEducationalMock<ILogger<AIAgentService>>();
        _mockCloudLogger = CreateEducationalMock<ILogger<CloudAIAgentService>>();
        _mockAzureOptions = new Mock<IOptions<AzureAIOptions>>();
        
        // Setup Azure options for testing
        _mockAzureOptions.Setup(x => x.Value).Returns(new AzureAIOptions
        {
            Endpoint = "https://test-endpoint.openai.azure.com/",
            ApiKey = "test-key-12345678901234567890123456789012",
            DeploymentName = "test-deployment"
        });
    }

    protected override void ConfigureAdditionalServices(IServiceCollection services)
    {
        // Configure AI Agent services
        services.AddSingleton(_mockContentModerationService.Object);
        services.AddSingleton(_mockLogger.Object);
        services.AddSingleton(_mockCloudLogger.Object);
        services.AddSingleton(_mockAzureOptions.Object);
        services.AddScoped<IAIAgentService, AIAgentService>();
    }

    #region CareerGuideAgent Tests

    [Fact]
    public async Task CareerGuideAgent_GeneratesEncouragingResponse_ForCareerExploration()
    {
        // Arrange
        var service = GetService<IAIAgentService>();
        var context = "12-year-old exploring career options";
        var input = "I want to become a teacher";
        
        // Setup content moderation to approve educational content
        _mockContentModerationService
            .Setup(x => x.IsContentAppropriateModerationAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var response = await service.GenerateResponseAsync(AgentType.CareerGuide, context, input);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsGenerated, "AI agent should generate response for valid educational input");
        Assert.False(string.IsNullOrEmpty(response.Content), "Response content should not be empty");
        
        // Validate educational content
        ValidateChildSafeContent(response.Content, "CareerGuide response");
        ValidateEducationalOutcome(response, "career guidance and encouragement");
        
        // Specific career guide validations
        Assert.Contains("teach", response.Content.ToLowerInvariant());
        Assert.True(response.Content.Length > 10, "Response should be substantial enough to be helpful");
        
        Output.WriteLine($"✅ CareerGuide Response: {response.Content}");
    }

    [Fact]
    public async Task CareerGuideAgent_ProvidesPositiveEncouragement_ForAllCareerChoices()
    {
        // Arrange
        var service = GetService<IAIAgentService>();
        var careerChoices = new[] { "scientist", "artist", "engineer", "doctor", "chef", "writer" };
        
        _mockContentModerationService
            .Setup(x => x.IsContentAppropriateModerationAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        foreach (var career in careerChoices)
        {
            // Act
            var response = await service.GenerateResponseAsync(
                AgentType.CareerGuide, 
                "12-year-old career exploration", 
                $"I want to be a {career}");

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsGenerated);
            ValidateChildSafeContent(response.Content, $"CareerGuide response for {career}");
            
            // Should contain encouraging language
            var lowerContent = response.Content.ToLowerInvariant();
            Assert.True(
                lowerContent.Contains("great") || 
                lowerContent.Contains("wonderful") || 
                lowerContent.Contains("amazing") || 
                lowerContent.Contains("excellent"),
                $"Response for {career} should contain encouraging language");
                
            Output.WriteLine($"✅ {career}: {response.Content}");
        }
    }

    #endregion

    #region EventNarratorAgent Tests

    [Fact]
    public async Task EventNarratorAgent_GeneratesChildSafeStories_ForGameEvents()
    {
        // Arrange
        var service = GetService<IAIAgentService>();
        var context = "Territory acquisition celebration";
        var input = "Player just acquired their first territory";
        
        _mockContentModerationService
            .Setup(x => x.IsContentAppropriateModerationAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var response = await service.GenerateResponseAsync(AgentType.EventNarrator, context, input);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsGenerated);
        ValidateChildSafeContent(response.Content, "EventNarrator story");
        ValidateEducationalOutcome(response, "engaging narrative and geographical learning");
        
        // Story should be engaging and appropriate
        Assert.True(response.Content.Length > 20, "Story should be substantial");
        Assert.DoesNotContain("war", response.Content.ToLowerInvariant());
        Assert.DoesNotContain("conflict", response.Content.ToLowerInvariant());
        
        Output.WriteLine($"✅ EventNarrator Story: {response.Content}");
    }

    #endregion

    #region FortuneTellerAgent Tests

    [Fact]
    public async Task FortuneTellerAgent_ProvidesPositiveStrategicGuidance()
    {
        // Arrange
        var service = GetService<IAIAgentService>();
        var context = "Strategic planning for territory expansion";
        var input = "What should I do next in my leadership journey?";
        
        _mockContentModerationService
            .Setup(x => x.IsContentAppropriateModerationAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var response = await service.GenerateResponseAsync(AgentType.FortuneTeller, context, input);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsGenerated);
        ValidateChildSafeContent(response.Content, "FortuneTeller guidance");
        ValidateEducationalOutcome(response, "strategic thinking and future planning");
        
        // Should provide forward-looking, positive guidance
        var lowerContent = response.Content.ToLowerInvariant();
        Assert.True(
            lowerContent.Contains("future") || 
            lowerContent.Contains("next") || 
            lowerContent.Contains("plan") ||
            lowerContent.Contains("opportunity"),
            "FortuneTeller should provide future-focused guidance");
            
        Output.WriteLine($"✅ FortuneTeller Guidance: {response.Content}");
    }

    #endregion

    #region HappinessAdvisorAgent Tests

    [Fact]
    public async Task HappinessAdvisorAgent_ProvidesDiplomaticAdvice_ForPopulationManagement()
    {
        // Arrange
        var service = GetService<IAIAgentService>();
        var context = "Population happiness management";
        var input = "How can I keep my citizens happy?";
        
        _mockContentModerationService
            .Setup(x => x.IsContentAppropriateModerationAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var response = await service.GenerateResponseAsync(AgentType.HappinessAdvisor, context, input);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsGenerated);
        ValidateChildSafeContent(response.Content, "HappinessAdvisor advice");
        ValidateEducationalOutcome(response, "leadership and population management");
        
        // Should focus on positive leadership concepts
        var lowerContent = response.Content.ToLowerInvariant();
        Assert.True(
            lowerContent.Contains("happy") || 
            lowerContent.Contains("care") || 
            lowerContent.Contains("help") ||
            lowerContent.Contains("listen"),
            "HappinessAdvisor should focus on positive leadership");
            
        Output.WriteLine($"✅ HappinessAdvisor Advice: {response.Content}");
    }

    #endregion

    #region TerritoryStrategistAgent Tests

    [Fact]
    public async Task TerritoryStrategistAgent_ProvidesEducationalExpansionPlanning()
    {
        // Arrange
        var service = GetService<IAIAgentService>();
        var context = "Territory expansion strategy";
        var input = "Which territory should I acquire next?";
        
        _mockContentModerationService
            .Setup(x => x.IsContentAppropriateModerationAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var response = await service.GenerateResponseAsync(AgentType.TerritoryStrategist, context, input);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsGenerated);
        ValidateChildSafeContent(response.Content, "TerritoryStrategist planning");
        ValidateEducationalOutcome(response, "strategic thinking and geographical knowledge");
        
        // Should provide strategic, educational guidance
        var lowerContent = response.Content.ToLowerInvariant();
        Assert.True(
            lowerContent.Contains("strategy") || 
            lowerContent.Contains("consider") || 
            lowerContent.Contains("think") ||
            lowerContent.Contains("plan"),
            "TerritoryStrategist should provide strategic guidance");
            
        Output.WriteLine($"✅ TerritoryStrategist Planning: {response.Content}");
    }

    #endregion

    #region LanguageTutorAgent Tests

    [Fact]
    public async Task LanguageTutorAgent_ProvidesEducationalPronunciationGuidance()
    {
        // Arrange
        var service = GetService<IAIAgentService>();
        var context = "Language learning for territory languages";
        var input = "How do I pronounce 'Bonjour' in French?";
        
        _mockContentModerationService
            .Setup(x => x.IsContentAppropriateModerationAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var response = await service.GenerateResponseAsync(AgentType.LanguageTutor, context, input);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsGenerated);
        ValidateChildSafeContent(response.Content, "LanguageTutor guidance");
        ValidateEducationalOutcome(response, "language learning and pronunciation");
        
        // Should provide educational language guidance
        var lowerContent = response.Content.ToLowerInvariant();
        Assert.True(
            lowerContent.Contains("bonjour") || 
            lowerContent.Contains("french") || 
            lowerContent.Contains("pronounce") ||
            lowerContent.Contains("sound"),
            "LanguageTutor should provide pronunciation guidance");
            
        Output.WriteLine($"✅ LanguageTutor Guidance: {response.Content}");
    }

    #endregion

    #region Agent Personality Tests

    [Fact]
    public async Task GetAgentPersonality_ReturnsAppropriatePersonalityInfo_ForAllAgents()
    {
        // Arrange
        var service = GetService<IAIAgentService>();
        var agentTypes = Enum.GetValues<AgentType>();

        foreach (var agentType in agentTypes)
        {
            // Act
            var personality = await service.GetAgentPersonalityAsync(agentType);

            // Assert
            Assert.NotNull(personality);
            Assert.False(string.IsNullOrEmpty(personality.Name), $"{agentType} should have a name");
            Assert.False(string.IsNullOrEmpty(personality.Description), $"{agentType} should have a description");
            Assert.False(string.IsNullOrEmpty(personality.Greeting), $"{agentType} should have a greeting");
            
            // Validate child-safe personality content
            ValidateChildSafeContent(personality.Name, $"{agentType} personality name");
            ValidateChildSafeContent(personality.Description, $"{agentType} personality description");
            ValidateChildSafeContent(personality.Greeting, $"{agentType} personality greeting");
            
            Output.WriteLine($"✅ {agentType} Personality: {personality.Name} - {personality.Description}");
        }
    }

    #endregion

    #region Safety and Fallback Tests

    [Fact]
    public async Task ValidateResponseSafety_RejectsInappropriateContent()
    {
        // Arrange
        var service = GetService<IAIAgentService>();
        var inappropriateContent = "This content contains personal information like email@example.com";
        
        // Act
        var isValid = await service.ValidateResponseSafetyAsync(inappropriateContent, AgentType.CareerGuide);

        // Assert
        Assert.False(isValid, "Service should reject content with personal information");
        
        Output.WriteLine("✅ Safety validation correctly rejected inappropriate content");
    }

    [Fact]
    public async Task GetSafeFallbackResponse_ProvidesChildSafeDefault_WhenAIFails()
    {
        // Arrange
        var service = GetService<IAIAgentService>();
        var agentTypes = Enum.GetValues<AgentType>();

        foreach (var agentType in agentTypes)
        {
            // Act
            var fallback = await service.GetSafeFallbackResponseAsync(agentType, "test context");

            // Assert
            Assert.NotNull(fallback);
            Assert.False(string.IsNullOrEmpty(fallback.Content), $"{agentType} should have fallback content");
            ValidateChildSafeContent(fallback.Content, $"{agentType} fallback response");
            
            Output.WriteLine($"✅ {agentType} Fallback: {fallback.Content}");
        }
    }

    #endregion

    #region Content Moderation Integration Tests

    [Fact]
    public async Task AIAgentService_IntegratesWithContentModeration_ForAllResponses()
    {
        // Arrange
        var service = GetService<IAIAgentService>();
        var testInput = "Tell me about geography";
        
        // Setup content moderation to be called
        _mockContentModerationService
            .Setup(x => x.IsContentAppropriateModerationAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var response = await service.GenerateResponseAsync(AgentType.CareerGuide, "test", testInput);

        // Assert
        Assert.NotNull(response);
        
        // Verify content moderation was called
        _mockContentModerationService.Verify(
            x => x.IsContentAppropriateModerationAsync(It.IsAny<string>()),
            Times.AtLeastOnce,
            "Content moderation should be called for AI responses");
            
        Output.WriteLine("✅ Content moderation integration verified");
    }

    #endregion
}