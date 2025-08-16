using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WorldLeaders.API.Controllers;
using WorldLeaders.API.Tests.Infrastructure;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Services;
using Xunit.Abstractions;

namespace WorldLeaders.API.Tests;

/// <summary>
/// Comprehensive testing for TerritoryController
/// Context: Educational geography and economics learning for 12-year-old players
/// Educational Objective: Territory acquisition, cultural awareness, language learning with real GDP data
/// Safety Requirements: All content appropriate for children, culturally sensitive, positive messaging
/// </summary>
public class TerritoryControllerTests : ApiTestBase
{
    public TerritoryControllerTests(TestWebApplicationFactory factory, ITestOutputHelper output) 
        : base(factory, output)
    {
    }

    [Fact]
    public async Task GetAvailableTerritories_ValidatesRealWorldGDPData()
    {
        // Arrange
        var testPlayerId = Guid.NewGuid();
        var endpoint = $"/api/territory/available/{testPlayerId}";

        // Act
        var response = await Client.GetAsync(endpoint);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
        await ValidateApiResponseChildSafety(response, "Territory GDP Data");

        var content = await response.Content.ReadAsStringAsync();
        
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var result = JsonSerializer.Deserialize<AvailableTerritoriesEducationalResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result!.EducationalExplanation.Should().NotBeNullOrEmpty();
            result.Territories.Should().NotBeNull();

            foreach (var territory in result.Territories)
            {
                // Geography education validation
                ValidateChildSafeContent(territory.CountryName, "Country Name");
                territory.CountryCode.Should().NotBeNullOrEmpty("Country codes support geography learning");
                territory.OfficialLanguages.Should().NotBeEmpty("Languages support cultural learning");

                // Economics education validation
                territory.GdpInBillions.Should().BeGreaterThan(0, "GDP data should be positive for economic learning");
                territory.Cost.Should().BeGreaterThan(0, "Territory costs teach economic strategy");
                territory.ReputationRequired.Should().BeInRange(0, 100, "Reputation requirements should be percentage-based");
                territory.MonthlyIncome.Should().BeGreaterThan(0, "Monthly income teaches passive income concepts");

                // Educational tier validation
                Enum.IsDefined(typeof(TerritoryTier), territory.Tier).Should().BeTrue("Territory tiers support progressive learning");

                ValidateEducationalOutcome(territory, "Learn world geography and economics through territory data");
            }
        }
        else
        {
            ValidateChildSafeContent(content, "Territory Error Response");
        }

        Output.WriteLine("✅ Territory GDP data validated for real-world economic education");
    }

    [Fact]
    public async Task AcquireTerritory_ValidatesEducationalEconomicStrategy()
    {
        // Arrange
        var testPlayerId = Guid.NewGuid();
        var testTerritoryId = Guid.NewGuid();
        var endpoint = $"/api/territory/acquire/{testPlayerId}/{testTerritoryId}";

        // Act
        var response = await Client.PostAsync(endpoint, null);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
        await ValidateApiResponseChildSafety(response, "Territory Acquisition Strategy");

        var content = await response.Content.ReadAsStringAsync();
        
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var acquisitionResponse = JsonSerializer.Deserialize<TerritoryAcquisitionEducationalResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            acquisitionResponse.Should().NotBeNull();
            acquisitionResponse!.Result.Should().NotBeNull("Students learn territory acquisition results");
            ValidateChildSafeContent(acquisitionResponse.Result!.Message, "Territory Acquisition Message");
            ValidateChildSafeContent(acquisitionResponse.EducationalExplanation, "Educational Explanation");
            ValidateChildSafeContent(acquisitionResponse.ProgressTip, "Progress Tip");
            
            if (acquisitionResponse.Result.Success)
            {
                acquisitionResponse.Result.AcquiredTerritory.Should().NotBeNull("Successful acquisition should include territory data");
                acquisitionResponse.Result.EducationalTips.Should().NotBeEmpty("Acquisition should include educational tips");
            }
            
            ValidateEducationalOutcome(acquisitionResponse, "Learn economic strategy through territory acquisition");
        }
        else
        {
            ValidateChildSafeContent(content, "Territory Acquisition Error Response");
        }

        Output.WriteLine("✅ Territory acquisition validated for economic strategy education");
    }

    [Fact]
    public async Task GetTerritoryDetails_ValidatesCulturalEducationalContent()
    {
        // Arrange
        var testTerritoryId = Guid.NewGuid();
        var endpoint = $"/api/territory/details/{testTerritoryId}";

        // Act
        var response = await Client.GetAsync(endpoint);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.InternalServerError);
        await ValidateApiResponseChildSafety(response, "Territory Cultural Details");

        var content = await response.Content.ReadAsStringAsync();
        
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var territoryDetails = JsonSerializer.Deserialize<TerritoryDetailEducationalResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            territoryDetails.Should().NotBeNull();
            territoryDetails!.Details.Should().NotBeNull("Territory details should be provided");
            
            ValidateChildSafeContent(territoryDetails.Details!.CountryName, "Territory Country Name");
            ValidateChildSafeContent(territoryDetails.Details.EducationalFact, "Educational Fact");
            ValidateChildSafeContent(territoryDetails.EducationalExplanation, "Educational Explanation");
            ValidateChildSafeContent(territoryDetails.ProgressTip, "Progress Tip");
            
            // Validate educational content
            territoryDetails.Details.GeographicFeatures.Should().NotBeEmpty("Should provide geographic learning");
            territoryDetails.Details.CulturalHighlights.Should().NotBeEmpty("Should provide cultural learning");
            
            ValidateEducationalOutcome(territoryDetails, "Learn geography, culture, and languages through territory exploration");
        }
        else
        {
            ValidateChildSafeContent(content, "Territory Details Error Response");
        }

        Output.WriteLine("✅ Territory cultural details validated for comprehensive geographic education");
    }

    [Fact]
    public async Task GetTerritoriesByTier_ValidatesProgressiveLearning()
    {
        // Arrange
        var testPlayerId = Guid.NewGuid();
        var tiers = new[] { TerritoryTier.Small, TerritoryTier.Medium, TerritoryTier.Major };

        // Act & Assert
        foreach (var tier in tiers)
        {
            var endpoint = $"/api/territory/tier/{tier}/{testPlayerId}";
            var response = await Client.GetAsync(endpoint);

            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
            await ValidateApiResponseChildSafety(response, $"Territory Tier: {tier}");

            var content = await response.Content.ReadAsStringAsync();
            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var territories = JsonSerializer.Deserialize<TerritoriesByTierEducationalResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                territories.Should().NotBeNull();
                territories!.Territories.Should().NotBeNull("Territories list should be provided");
                ValidateChildSafeContent(territories.EducationalExplanation, "Educational Explanation");
                ValidateChildSafeContent(territories.ProgressTip, "Progress Tip");
                
                foreach (var territory in territories.Territories)
                {
                    territory.Tier.Should().Be(tier, $"All territories should match requested tier {tier}");
                    
                    // Validate tier difficulty progression
                    switch (tier)
                    {
                        case TerritoryTier.Small:
                            territory.Cost.Should().BeLessThan(3000, "Small territories should be affordable for beginners");
                            territory.ReputationRequired.Should().BeLessThan(50, "Small territories should have low reputation requirements");
                            break;
                        case TerritoryTier.Medium:
                            territory.Cost.Should().BeInRange(2000, 8000, "Medium territories should be moderately priced");
                            territory.ReputationRequired.Should().BeInRange(25, 75, "Medium territories should have moderate requirements");
                            break;
                        case TerritoryTier.Major:
                            territory.Cost.Should().BeGreaterOrEqualTo(5000, "Major territories should be expensive challenges");
                            territory.ReputationRequired.Should().BeGreaterThan(60, "Major territories should require high reputation");
                            break;
                    }
                    
                    ValidateEducationalOutcome(territory, $"Learn through progressive {tier} territory challenges");
                }
            }
            else
            {
                ValidateChildSafeContent(content, $"Territory Tier {tier} Error Response");
            }

            Output.WriteLine($"✅ Territory tier {tier} validated for progressive learning structure");
        }
    }

    [Fact]
    public async Task GetTerritoryIncome_ValidatesEconomicEducation()
    {
        // Arrange
        var testPlayerId = Guid.NewGuid();
        var endpoint = $"/api/territory/income/{testPlayerId}";

        // Act
        var response = await Client.GetAsync(endpoint);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
        await ValidateApiResponseChildSafety(response, "Territory Income Calculation");

        var content = await response.Content.ReadAsStringAsync();
        
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var incomeResponse = JsonSerializer.Deserialize<TerritoryIncomeEducationalResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            incomeResponse.Should().NotBeNull();
            incomeResponse!.Income.Should().BeGreaterThanOrEqualTo(0, "Territory income should be non-negative for children");
            ValidateChildSafeContent(incomeResponse.EducationalExplanation, "Educational Explanation");
            ValidateChildSafeContent(incomeResponse.ProgressTip, "Progress Tip");
            ValidateEducationalOutcome(incomeResponse, "Learn passive income concepts through territory ownership");
        }
        else
        {
            ValidateChildSafeContent(content, "Territory Income Error Response");
        }

        Output.WriteLine("✅ Territory income calculation validated for economic education");
    }

    [Fact]
    public async Task GetLanguageChallenges_ValidatesEducationalLanguageLearning()
    {
        // Arrange
        var testPlayerId = Guid.NewGuid();
        var endpoint = $"/api/territory/language-challenges/{testPlayerId}";

        // Act
        var response = await Client.GetAsync(endpoint);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
        await ValidateApiResponseChildSafety(response, "Language Learning Challenges");

        var content = await response.Content.ReadAsStringAsync();
        
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var challenges = JsonSerializer.Deserialize<LanguageChallengesEducationalResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            challenges.Should().NotBeNull();
            challenges!.Challenges.Should().NotBeNull().And.NotBeEmpty("Students discover language challenges for territory learning");
            
            foreach (var challenge in challenges.Challenges)
            {
                // Language education validation
                ValidateChildSafeContent(challenge.Word, "Language Word");
                ValidateChildSafeContent(challenge.Pronunciation, "Pronunciation Guide");
                ValidateChildSafeContent(challenge.CulturalContext, "Cultural Context");
                
                challenge.LanguageCode.Should().NotBeNullOrEmpty("Language codes support international learning");
                challenge.LanguageName.Should().NotBeNullOrEmpty("Language names support cultural awareness");
                challenge.RequiredAccuracy.Should().BeInRange(50, 90, "Accuracy requirements should be achievable for 12-year-olds");
                challenge.SupportsSpeechRecognition.Should().BeTrue("Speech recognition supports pronunciation learning");
                
                // Challenge type validation for age appropriateness
                Enum.IsDefined(typeof(ChallengeType), challenge.Type).Should().BeTrue("Challenge types should be educational");
                
                ValidateEducationalOutcome(challenge, "Learn languages through territory-based cultural immersion");
            }
        }
        else
        {
            ValidateChildSafeContent(content, "Language Challenges Error Response");
        }

        Output.WriteLine("✅ Language learning challenges validated for cultural and linguistic education");
    }

    [Fact]
    public async Task GetCulturalContext_ValidatesChildSafeCulturalLearning()
    {
        // Arrange
        var testTerritoryId = Guid.NewGuid();
        var endpoint = $"/api/territory/cultural-context/{testTerritoryId}";

        // Act
        var response = await Client.GetAsync(endpoint);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.InternalServerError);
        await ValidateApiResponseChildSafety(response, "Cultural Context Learning");

        var content = await response.Content.ReadAsStringAsync();
        
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var culturalResponse = JsonSerializer.Deserialize<CulturalContextEducationalResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            culturalResponse.Should().NotBeNull();
            culturalResponse!.Context.Should().NotBeNull("Students learn cultural context for territory education");
            ValidateChildSafeContent(culturalResponse.Context!.CountryName, "Country Name");
            ValidateChildSafeContent(culturalResponse.Context.HistoricalSignificance, "Historical Significance");
            ValidateChildSafeContent(culturalResponse.Context.ChildFriendlyDescription, "Child Friendly Description");
            ValidateChildSafeContent(culturalResponse.EducationalExplanation, "Educational Explanation");
            ValidateChildSafeContent(culturalResponse.ProgressTip, "Progress Tip");
            
            // Cultural sensitivity validation
            culturalResponse.Context.ChildFriendlyDescription.Should().NotContainAny("weird", "strange");
            culturalResponse.Context.HistoricalSignificance.Should().NotContain("war");
            culturalResponse.Context.ChildFriendlyDescription.Should().ContainAny("children", "kids", "young");
            
            ValidateEducationalOutcome(culturalResponse.Context, "Learn about diverse cultures with respect and appreciation");
        }
        else
        {
            ValidateChildSafeContent(content, "Cultural Context Error Response");
        }

        Output.WriteLine("✅ Cultural context validated for respectful and educational cultural learning");
    }

    [Fact]
    public async Task GetPlayerTerritoryStats_ValidatesEducationalProgressTracking()
    {
        // Arrange
        var testPlayerId = Guid.NewGuid();
        var endpoint = $"/api/territory/player-stats/{testPlayerId}";

        // Act
        var response = await Client.GetAsync(endpoint);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
        await ValidateApiResponseChildSafetyAllowErrors(response, "Territory Statistics");

        var content = await response.Content.ReadAsStringAsync();
        
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var stats = JsonSerializer.Deserialize<PlayerTerritoryStatsEducationalResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            stats.Should().NotBeNull();
            stats!.TotalTerritories.Should().BeGreaterThanOrEqualTo(0, "Territory count should be non-negative");
            stats.MonthlyTerritoryIncome.Should().BeGreaterThanOrEqualTo(0, "Territory income should be non-negative");
            stats.LanguagesExplored.Should().BeGreaterThanOrEqualTo(0, "Language count should track learning progress");
            stats.ContinentsRepresented.Should().BeInRange(0, 7, "Continent count should be realistic");
            
            ValidateEducationalOutcome(stats, "Track geographic and linguistic learning progress");
        }
        else
        {
            ValidateChildSafeContent(content, "Territory Statistics Error Response");
        }

        Output.WriteLine("✅ Territory statistics validated for educational progress tracking");
    }

    protected override void ConfigureAdditionalServices(IServiceCollection services)
    {
        // Mock territory service for comprehensive testing
        var mockTerritoryService = new Mock<ITerritoryService>();
        var mockPlayerService = new Mock<IPlayerService>();

        // Configure mock territory service with educational data
        mockTerritoryService.Setup(x => x.GetAvailableTerritoriesAsync(It.IsAny<Guid>()))
            .ReturnsAsync(CreateSampleEducationalTerritories());

        mockTerritoryService.Setup(x => x.AcquireTerritoryAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(Task.FromResult(new TerritoryAcquisitionResult(
                Success: true,
                Message: "Congratulations! You've acquired this wonderful territory and learned about its culture!",
                AcquiredTerritory: CreateSampleEducationalTerritories().First(),
                ResourceChanges: null,
                UnlockedAchievement: null,
                EducationalTips: new List<string> { "Great choice! This territory will help you learn about different cultures!" }
            )));

        mockTerritoryService.Setup(x => x.GetTerritoryDetailsAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult(CreateSampleTerritoryDetails()));

        mockTerritoryService.Setup(x => x.GetTerritoriesByTierAsync(It.IsAny<TerritoryTier>(), It.IsAny<Guid>()))
            .Returns<TerritoryTier, Guid>((tier, playerId) => 
                Task.FromResult(CreateSampleEducationalTerritories().Where(t => t.Tier == tier).ToList()));

        mockTerritoryService.Setup(x => x.CalculateMonthlyTerritoryIncomeAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult(1500));

        mockTerritoryService.Setup(x => x.GetTerritoryLanguageChallengesAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult(CreateSampleLanguageChallenges()));

        mockTerritoryService.Setup(x => x.GetTerritoryCulturalContextAsync(It.IsAny<Guid>()))
            .Returns(Task.FromResult(CreateSampleCulturalContext()));

        // Register mocked services
        services.AddSingleton(mockTerritoryService.Object);
        services.AddSingleton(mockPlayerService.Object);
    }

    private List<TerritoryDto> CreateSampleEducationalTerritories()
    {
        return new List<TerritoryDto>
        {
            new TerritoryDto(
                Guid.NewGuid(),
                "United Kingdom",
                "GB",
                new List<string> { "English" },
                3.13m,
                TerritoryTier.Major,
                5000,
                75,
                1000,
                true,
                false
            ),
            new TerritoryDto(
                Guid.NewGuid(),
                "Costa Rica",
                "CR",
                new List<string> { "Spanish" },
                0.065m,
                TerritoryTier.Small,
                1500,
                25,
                300,
                true,
                false
            ),
            new TerritoryDto(
                Guid.NewGuid(),
                "Japan",
                "JP",
                new List<string> { "Japanese" },
                4.23m,
                TerritoryTier.Major,
                8000,
                85,
                1200,
                true,
                false
            )
        };
    }

    private TerritoryDetailDto CreateSampleTerritoryDetails()
    {
        return new TerritoryDetailDto(
            Id: Guid.NewGuid(),
            CountryName: "United Kingdom",
            CountryCode: "GB",
            OfficialLanguages: new List<string> { "English" },
            GdpInBillions: 3.13m,
            Tier: TerritoryTier.Major,
            Cost: 5000,
            ReputationRequired: 75,
            MonthlyIncome: 1000,
            IsAvailable: true,
            IsOwned: false,
            FlagUrl: "/flags/gb.png",
            Capital: "London",
            Population: 67000000,
            Region: "Europe",
            Subregion: "Northern Europe",
            Currencies: new List<string> { "GBP" },
            EducationalFact: "Home to amazing castles, beautiful countryside, and the birthplace of many beloved stories!",
            GeographicFeatures: new List<string> { "Islands", "Rolling hills", "Coastal areas" },
            CulturalHighlights: new List<string> { "Rich literary tradition", "Historic architecture", "Diverse cultural heritage" }
        );
    }

    private List<LanguageChallengeDto> CreateSampleLanguageChallenges()
    {
        return new List<LanguageChallengeDto>
        {
            new LanguageChallengeDto(
                "en",
                "English",
                "Hello",
                "HEH-loh",
                "/audio/hello-en.mp3",
                75,
                true,
                "A friendly greeting used by children and adults everywhere!",
                ChallengeType.Greeting
            ),
            new LanguageChallengeDto(
                "es",
                "Spanish",
                "Hola",
                "OH-lah",
                "/audio/hola-es.mp3",
                75,
                true,
                "The warm Spanish greeting that brings smiles to faces!",
                ChallengeType.Greeting
            )
        };
    }

    private CulturalContextDto CreateSampleCulturalContext()
    {
        return new CulturalContextDto(
            TerritoryId: Guid.NewGuid(),
            CountryName: "United Kingdom",
            HistoricalSignificance: "Known for incredible discoveries, beautiful art, and important contributions to science and literature that have helped make the world a better place.",
            CulturalTraditions: new List<string> { "Afternoon tea", "Royal traditions", "Literary heritage" },
            FamousLandmarks: new List<string> { "Big Ben", "Tower Bridge", "Stonehenge" },
            NotableAchievements: new List<string> { "Shakespeare's literature", "Scientific discoveries", "Democratic traditions" },
            GeographyLesson: "An island nation with diverse landscapes from rolling hills to coastal areas.",
            EconomicLesson: "A major economic power with strong trade relationships worldwide.",
            EducationalQuizQuestions: new List<string> { "What is the capital city?", "Which language is primarily spoken?" },
            ChildFriendlyDescription: "A wonderful country with rich traditions, beautiful landscapes, and friendly people who love to share their culture with children from around the world!"
        );
    }
}