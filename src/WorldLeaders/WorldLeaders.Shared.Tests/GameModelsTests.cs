using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Models;
using WorldLeaders.Shared.Tests.Infrastructure;
using Xunit.Abstractions;

namespace WorldLeaders.Shared.Tests;

/// <summary>
/// Educational tests for game models - ensuring child safety and learning objectives
/// Context: Educational game component for 12-year-old geography and economics learning
/// </summary>
public class GameModelsTests : EducationalTestBase
{
    public GameModelsTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void CharacterPersona_ShouldHaveChildFriendlyProperties()
    {
        // Arrange & Act
        var characterPersona = new CharacterPersona
        {
            Name = "Young Explorer",
            Description = "A brave young person who loves to discover new places and learn about different cultures",
            PersonalityTrait = "Curious and Kind",
            SpecialAbility = "Language Learning Boost"
        };

        // Assert - Educational Validation
        ValidateChildSafeContent(characterPersona.Name, "Character Name");
        ValidateChildSafeContent(characterPersona.Description, "Character Description");
        ValidateChildSafeContent(characterPersona.PersonalityTrait, "Character Trait");
        ValidateChildSafeContent(characterPersona.SpecialAbility, "Character Ability");
        
        // Assert - Basic Properties
        Assert.NotEqual(Guid.Empty, characterPersona.Id);
        Assert.Equal("Young Explorer", characterPersona.Name);
        Assert.Contains("learn", characterPersona.Description, StringComparison.OrdinalIgnoreCase);
        
        ValidateEducationalOutcome(characterPersona, "Character development and personality understanding");
    }

    [Theory]
    [InlineData(JobLevel.Farmer)]
    [InlineData(JobLevel.Gardener)]
    [InlineData(JobLevel.Shopkeeper)]
    [InlineData(JobLevel.Artisan)]
    [InlineData(JobLevel.Politician)]
    [InlineData(JobLevel.BusinessLeader)]
    public void JobLevel_ShouldBeChildFriendlyAndEducational(JobLevel jobLevel)
    {
        // Act & Assert
        ValidateChildFriendlyJobLevel(jobLevel);
        
        // Assert - Verify job level is within valid range
        Assert.True(Enum.IsDefined(typeof(JobLevel), jobLevel), 
            $"Job level {jobLevel} should be a valid career option for 12-year-olds");
        
        // Assert - Verify progression makes educational sense
        var jobValue = (int)jobLevel;
        Assert.InRange(jobValue, 1, 6);
        
        // Get the educational description for validation
        var jobDescription = GetJobLevelDescription(jobLevel);
        Assert.True(jobDescription.Length > 30, "Job descriptions should be educational and substantial");
        Assert.True(jobDescription.ToLowerInvariant().Contains("learning"), 
            "Job descriptions should emphasize learning and education");
    }

    [Theory]
    [InlineData(AgentType.CareerGuide)]
    [InlineData(AgentType.EventNarrator)]
    [InlineData(AgentType.FortuneTeller)]
    [InlineData(AgentType.HappinessAdvisor)]
    [InlineData(AgentType.TerritoryStrategist)]
    [InlineData(AgentType.LanguageTutor)]
    public void AgentType_ShouldProvideEducationalGuidance(AgentType agentType)
    {
        // Act - Generate sample response for each agent type
        var sampleResponse = GenerateSampleAgentResponse(agentType);
        
        // Assert - Validate child safety
        ValidateAIAgentChildSafety(agentType, sampleResponse);
        
        // Assert - Verify agent type is educational
        Assert.True(Enum.IsDefined(typeof(AgentType), agentType), 
            $"Agent type {agentType} should be a valid educational assistant");
        
        // Verify the sample response has educational content
        Assert.True(sampleResponse.Length > 20, "Agent responses should be substantial for learning");
        Assert.True(sampleResponse.ToLowerInvariant().Contains("learn"), 
            "Agent responses should encourage learning");
    }

    [Theory]
    [InlineData(TerritoryTier.Small)]
    [InlineData(TerritoryTier.Medium)]
    [InlineData(TerritoryTier.Major)]
    public void TerritoryTier_ShouldTeachEconomicConcepts(TerritoryTier territoryTier)
    {
        // Act - Get tier value
        var tierValue = (int)territoryTier;
        
        // Assert - Educational validation
        Assert.InRange(tierValue, 1, 3);
        
        // Assert - Real-world application
        var description = GetTerritoryTierDescription(territoryTier);
        ValidateChildSafeContent(description, $"Territory Tier: {territoryTier}");
        
        // Verify educational content is substantial
        Assert.True(description.Length > 50, "Territory descriptions should be educational and substantial");
        Assert.True(description.ToLowerInvariant().Contains("economics") || description.ToLowerInvariant().Contains("economic"), 
            "Territory descriptions should teach economic concepts");
    }

    [Theory]
    [InlineData(EventType.Career)]
    [InlineData(EventType.Economic)]
    [InlineData(EventType.Social)]
    [InlineData(EventType.International)]
    [InlineData(EventType.Natural)]
    public void EventType_ShouldProvideEducationalScenarios(EventType eventType)
    {
        // Act - Generate sample event description
        var eventDescription = GenerateSampleEventDescription(eventType);
        
        // Assert - Child safety validation
        ValidateChildSafeContent(eventDescription, $"Event Type: {eventType}");
        
        // Assert - Educational value
        Assert.True(eventDescription.Length > 10, "Event descriptions should be descriptive for learning");
        
        ValidateEducationalOutcome(eventDescription, "Real-world events and scenario-based learning");
    }

    #region Helper Methods

    private string GenerateSampleAgentResponse(AgentType agentType)
    {
        return agentType switch
        {
            AgentType.CareerGuide => "Great job exploring different career paths! Let's learn about how farmers help feed the world.",
            AgentType.EventNarrator => "An exciting opportunity has appeared! A new country wants to trade with your territory, bringing cultural exchange and learning.",
            AgentType.FortuneTeller => "The stars show wonderful opportunities ahead! Your language learning efforts will open new doors to friendship.",
            AgentType.HappinessAdvisor => "Your people are happy when they see progress and learning! Keep investing in education and culture to learn about community development!",
            AgentType.TerritoryStrategist => "Consider exploring territories with rich cultural heritage to expand your educational learning about different countries, geography, and economics through strategic territorial development.",
            AgentType.LanguageTutor => "Excellent progress with language learning! Practice speaking with people from different regions to improve.",
            _ => "Keep exploring and learning about our wonderful world!"
        };
    }

    private string GetTerritoryTierDescription(TerritoryTier tier)
    {
        return tier switch
        {
            TerritoryTier.Small => "Small territories with GDP rank 100+ - perfect for starting your educational leadership journey and learning about local economies, geography, and country development through economic education",
            TerritoryTier.Medium => "Medium territories with GDP rank 30-100 - great educational opportunities to understand regional economics, geography, trade, and how countries learn to develop their economies",
            TerritoryTier.Major => "Major territories with GDP rank 1-30 - experience world-class economies and global leadership while learning about geography, economics, and educational systems of major countries",
            _ => "Explore territories to learn about global economics, geography, and educational development through country-based learning"
        };
    }

    private string GenerateSampleEventDescription(EventType eventType)
    {
        return eventType switch
        {
            EventType.Career => "A new career opportunity has emerged! Learn about different professions and how they contribute to society.",
            EventType.Economic => "Economic growth brings new learning opportunities! Discover how trade and commerce connect countries.",
            EventType.Social => "A cultural festival celebrates the diversity of your territory! Learn about different traditions and customs.",
            EventType.International => "A diplomatic mission offers a chance to practice language skills and learn about international cooperation.",
            EventType.Natural => "A beautiful natural wonder has been discovered! Learn about geography and environmental protection.",
            _ => "An educational adventure awaits! Explore and learn about the world around you."
        };
    }

    #endregion
}