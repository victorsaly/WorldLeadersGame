using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using WorldLeaders.Infrastructure.Data;
using WorldLeaders.Infrastructure.Entities;
using WorldLeaders.Infrastructure.Services;
using WorldLeaders.Infrastructure.Tests.Infrastructure;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Models;
using WorldLeaders.Shared.Services;
using Xunit.Abstractions;

namespace WorldLeaders.Infrastructure.Tests.Services;

/// <summary>
/// Comprehensive testing for Territory Management service with educational focus
/// Context: Educational game component for 12-year-old geography and economics learning
/// Educational Objective: Validate territory mechanics teach real-world geography and economic concepts
/// Safety Requirements: Age-appropriate content, positive learning experiences, cultural sensitivity
/// </summary>
public class TerritoryServiceTests : ServiceTestBase
{
    private readonly Mock<IExternalDataService> _mockExternalDataService;
    private readonly Mock<ILogger<TerritoryService>> _mockLogger;

    public TerritoryServiceTests(ITestOutputHelper output) : base(output)
    {
        _mockExternalDataService = CreateEducationalMock<IExternalDataService>();
        _mockLogger = CreateEducationalMock<ILogger<TerritoryService>>();
    }

    protected override void ConfigureAdditionalServices(IServiceCollection services)
    {
        services.AddSingleton(_mockExternalDataService.Object);
        services.AddSingleton(_mockLogger.Object);
        services.AddScoped<ITerritoryService, TerritoryService>();
    }

    protected override async Task SeedTestDataAsync(WorldLeadersDbContext dbContext)
    {
        // Create test player
        var testPlayer = new PlayerEntity
        {
            Id = Guid.NewGuid(),
            Username = "test_student",
            DisplayName = "Test Student",
            CurrentJob = JobLevel.Student,
            Income = 1000,
            Reputation = 50,
            Happiness = 75,
            CreatedAt = DateTime.UtcNow,
            LastActiveAt = DateTime.UtcNow
        };

        dbContext.Players.Add(testPlayer);

        // Create educational test territories with real-world data
        var territories = new[]
        {
            new TerritoryEntity
            {
                Id = Guid.NewGuid(),
                CountryName = "Canada",
                CountryCode = "CA",
                GdpInBillions = 2139.0m,
                Tier = 1,
                Cost = 500000,
                ReputationRequired = 40,
                MonthlyIncome = 5000,
                IsAvailable = true,
                OfficialLanguagesJson = """["English", "French"]""",
                CreatedAt = DateTime.UtcNow
            },
            new TerritoryEntity
            {
                Id = Guid.NewGuid(),
                CountryName = "Japan",
                CountryCode = "JP",
                GdpInBillions = 4937.0m,
                Tier = 2,
                Cost = 1200000,
                ReputationRequired = 70,
                MonthlyIncome = 12000,
                IsAvailable = true,
                OfficialLanguagesJson = """["Japanese"]""",
                CreatedAt = DateTime.UtcNow
            },
            new TerritoryEntity
            {
                Id = Guid.NewGuid(),
                CountryName = "Brazil",
                CountryCode = "BR",
                GdpInBillions = 2126.0m,
                Tier = 1,
                Cost = 480000,
                ReputationRequired = 35,
                MonthlyIncome = 4800,
                IsAvailable = false, // Already owned for testing
                OwnedByPlayerId = testPlayer.Id,
                OfficialLanguagesJson = """["Portuguese"]""",
                CreatedAt = DateTime.UtcNow
            }
        };

        dbContext.Territories.AddRange(territories);
        await dbContext.SaveChangesAsync();
    }

    #region Territory Availability Tests

    [Fact]
    public async Task GetAvailableTerritoriesAsync_ReturnsOnlyAvailableTerritories_ForEducationalExploration()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<ITerritoryService>();
            var testPlayer = context.Players.First();
            
            return await service.GetAvailableTerritoriesAsync(testPlayer.Id);
        });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count >= 2, "Should return available territories for educational exploration");
        
        // Validate educational content
        foreach (var territory in result)
        {
            Assert.True(territory.IsAvailable, "All returned territories should be available");
            Assert.False(territory.IsOwned, "Available territories should not be owned");
            Assert.NotEmpty(territory.CountryName);
            Assert.NotEmpty(territory.CountryCode);
            Assert.True(territory.Cost > 0, "Territory should have realistic cost");
            Assert.True(territory.GdpInBillions > 0, "Territory should have real GDP data");
            
            ValidateEducationalOutcome(territory, "geography learning through territory exploration");
            
            Output.WriteLine($"✅ Available Territory: {territory.CountryName} - GDP: ${territory.GdpInBillions}B, Cost: ${territory.Cost:N0}");
        }
    }

    [Fact]
    public async Task GetAvailableTerritoriesAsync_IncludesEducationalGeographyData()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<ITerritoryService>();
            var testPlayer = context.Players.First();
            
            return await service.GetAvailableTerritoriesAsync(testPlayer.Id);
        });

        // Assert
        var canadaTerritory = result.FirstOrDefault(t => t.CountryCode == "CA");
        Assert.NotNull(canadaTerritory);
        
        // Validate real-world educational data
        Assert.Equal("Canada", canadaTerritory.CountryName);
        Assert.True(canadaTerritory.GdpInBillions > 2000, "Canada should have realistic GDP data");
        Assert.Contains("English", canadaTerritory.OfficialLanguages);
        Assert.Contains("French", canadaTerritory.OfficialLanguages);
        
        Output.WriteLine($"✅ Educational Geography: {canadaTerritory.CountryName} speaks {string.Join(", ", canadaTerritory.OfficialLanguages)}");
    }

    #endregion

    #region Territory Acquisition Tests

    [Fact]
    public async Task AcquireTerritoryAsync_SucceedsWithSufficientFunds_AndReputation()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<ITerritoryService>();
            var testPlayer = context.Players.First();
            
            // Update player to have sufficient funds and reputation
            testPlayer.Income = 600000; // More than territory cost
            testPlayer.Reputation = 50; // More than required
            context.Players.Update(testPlayer);
            await context.SaveChangesAsync();
            
            var canadaTerritory = context.Territories.First(t => t.CountryCode == "CA");
            
            return await service.AcquireTerritoryAsync(testPlayer.Id, canadaTerritory.Id);
        });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success, "Territory acquisition should succeed with sufficient resources");
        Assert.NotEmpty(result.Message);
        
        ValidateChildSafeContent(result.Message, "Territory acquisition success message");
        ValidateEducationalOutcome(result, "economic strategy and geography learning");
        
        Output.WriteLine($"✅ Territory Acquisition Success: {result.Message}");
    }

    [Fact]
    public async Task AcquireTerritoryAsync_FailsWithInsufficientFunds_WithEncouragingMessage()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<ITerritoryService>();
            var testPlayer = context.Players.First();
            
            // Set player to have insufficient funds
            testPlayer.Income = 100000; // Less than territory cost
            testPlayer.Reputation = 80; // Sufficient reputation
            context.Players.Update(testPlayer);
            await context.SaveChangesAsync();
            
            var canadaTerritory = context.Territories.First(t => t.CountryCode == "CA");
            
            return await service.AcquireTerritoryAsync(testPlayer.Id, canadaTerritory.Id);
        });

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success, "Territory acquisition should fail with insufficient funds");
        Assert.NotEmpty(result.Message);
        
        // Validate encouraging, educational message for children
        ValidateChildSafeContent(result.Message, "Territory acquisition failure message");
        
        var lowerMessage = result.Message.ToLowerInvariant();
        Assert.True(
            lowerMessage.Contains("save") || 
            lowerMessage.Contains("earn") || 
            lowerMessage.Contains("try") ||
            lowerMessage.Contains("keep"),
            "Failure message should be encouraging and educational");
            
        Output.WriteLine($"✅ Encouraging Failure Message: {result.Message}");
    }

    [Fact]
    public async Task AcquireTerritoryAsync_FailsWithInsufficientReputation_WithGuidance()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<ITerritoryService>();
            var testPlayer = context.Players.First();
            
            // Set player to have insufficient reputation
            testPlayer.Income = 600000; // Sufficient funds
            testPlayer.Reputation = 20; // Less than required
            context.Players.Update(testPlayer);
            await context.SaveChangesAsync();
            
            var canadaTerritory = context.Territories.First(t => t.CountryCode == "CA");
            
            return await service.AcquireTerritoryAsync(testPlayer.Id, canadaTerritory.Id);
        });

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success, "Territory acquisition should fail with insufficient reputation");
        
        // Validate educational guidance message
        ValidateChildSafeContent(result.Message, "Territory acquisition reputation failure message");
        
        var lowerMessage = result.Message.ToLowerInvariant();
        Assert.True(
            lowerMessage.Contains("reputation") || 
            lowerMessage.Contains("build") || 
            lowerMessage.Contains("improve") ||
            lowerMessage.Contains("work"),
            "Reputation failure message should provide educational guidance");
            
        Output.WriteLine($"✅ Educational Guidance Message: {result.Message}");
    }

    #endregion

    #region Player Territory Management Tests

    [Fact]
    public async Task GetPlayerTerritoriesAsync_ReturnsOwnedTerritories_WithEducationalData()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<ITerritoryService>();
            var testPlayer = context.Players.First();
            
            return await service.GetPlayerTerritoriesAsync(testPlayer.Id);
        });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count >= 1, "Player should have owned territories from seed data");
        
        foreach (var territory in result)
        {
            Assert.True(territory.IsOwned, "All returned territories should be owned");
            Assert.NotEmpty(territory.CountryName);
            Assert.True(territory.MonthlyIncome > 0, "Owned territories should provide income");
            
            ValidateEducationalOutcome(territory, "geography mastery and economic understanding");
            
            Output.WriteLine($"✅ Owned Territory: {territory.CountryName} - Monthly Income: ${territory.MonthlyIncome:N0}");
        }
    }

    [Fact]
    public async Task CalculateMonthlyTerritoryIncomeAsync_SumsIncomeCorrectly_ForEducationalTracking()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<ITerritoryService>();
            var testPlayer = context.Players.First();
            
            return await service.CalculateMonthlyTerritoryIncomeAsync(testPlayer.Id);
        });

        // Assert
        Assert.True(result > 0, "Player should have positive territory income from owned territories");
        
        // Validate realistic income calculation
        Assert.True(result >= 4000, "Territory income should be realistic for educational economics learning");
        
        Output.WriteLine($"✅ Total Monthly Territory Income: ${result:N0}");
    }

    #endregion

    #region Territory Details and Educational Content Tests

    [Fact]
    public async Task GetTerritoryDetailsAsync_ProvidesComprehensiveEducationalInformation()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<ITerritoryService>();
            var canadaTerritory = context.Territories.First(t => t.CountryCode == "CA");
            
            // Mock external data service to provide educational information
            _mockExternalDataService
                .Setup(x => x.GetCountryInformationAsync("CA"))
                .ReturnsAsync(new CountryInfo
                {
                    Capital = "Ottawa",
                    Population = 38000000,
                    Region = "North America",
                    FlagUrl = "https://flagcdn.com/w320/ca.png"
                });
            
            return await service.GetTerritoryDetailsAsync(canadaTerritory.Id);
        });

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Canada", result.CountryName);
        Assert.Equal("CA", result.CountryCode);
        
        // Validate educational geographical information
        Assert.Equal("Ottawa", result.Capital);
        Assert.True(result.Population > 30000000, "Population should be realistic");
        Assert.Equal("North America", result.Region);
        Assert.NotEmpty(result.FlagUrl);
        
        // Validate economic education data
        Assert.True(result.GdpInBillions > 2000, "GDP should be realistic for educational learning");
        Assert.True(result.Cost > 0, "Territory cost should be realistic");
        Assert.True(result.MonthlyIncome > 0, "Monthly income should be positive");
        
        ValidateEducationalOutcome(result, "comprehensive geography and economics learning");
        
        Output.WriteLine($"✅ Educational Territory Details: {result.CountryName}, Capital: {result.Capital}, Population: {result.Population:N0}");
    }

    [Fact]
    public async Task GetTerritoryDetailsAsync_IncludesLanguageLearningOpportunities()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<ITerritoryService>();
            var canadaTerritory = context.Territories.First(t => t.CountryCode == "CA");
            
            return await service.GetTerritoryDetailsAsync(canadaTerritory.Id);
        });

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.OfficialLanguages);
        Assert.Contains("English", result.OfficialLanguages);
        Assert.Contains("French", result.OfficialLanguages);
        
        // Validate language learning educational value
        ValidateEducationalOutcome(result.OfficialLanguages, "language learning and cultural awareness");
        
        Output.WriteLine($"✅ Language Learning Opportunity: {result.CountryName} speaks {string.Join(", ", result.OfficialLanguages)}");
    }

    #endregion

    #region Tier System and Educational Progression Tests

    [Fact]
    public async Task TerritoryTierSystem_ProvideProgressiveEducationalChallenges()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<ITerritoryService>();
            var testPlayer = context.Players.First();
            
            var availableTerritories = await service.GetAvailableTerritoriesAsync(testPlayer.Id);
            return availableTerritories.GroupBy(t => t.Tier).ToList();
        });

        // Assert
        Assert.NotEmpty(result);
        
        foreach (var tierGroup in result)
        {
            var tier = tierGroup.Key;
            var territories = tierGroup.ToList();
            
            Assert.True(territories.All(t => t.Tier == tier), $"All territories in tier {tier} should have consistent tier");
            
            // Validate educational progression - higher tiers should be more challenging
            if (tier > 1)
            {
                Assert.True(
                    territories.All(t => t.ReputationRequired >= 50),
                    $"Tier {tier} territories should require higher reputation for educational progression");
            }
            
            Output.WriteLine($"✅ Educational Tier {tier}: {territories.Count} territories, avg reputation required: {territories.Average(t => t.ReputationRequired):F0}");
        }
    }

    #endregion

    #region Real-World Data Integration Tests

    [Fact]
    public async Task TerritoryService_IntegratesRealWorldGDPData_ForEducationalAccuracy()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<ITerritoryService>();
            var testPlayer = context.Players.First();
            
            return await service.GetAvailableTerritoriesAsync(testPlayer.Id);
        });

        // Assert
        Assert.NotEmpty(result);
        
        foreach (var territory in result)
        {
            // Validate realistic GDP data for educational accuracy
            Assert.True(territory.GdpInBillions > 0, $"{territory.CountryName} should have positive GDP");
            Assert.True(territory.GdpInBillions < 30000, $"{territory.CountryName} GDP should be realistic");
            
            // Validate cost scaling relative to GDP for educational economics
            var costToGdpRatio = (double)territory.Cost / (double)territory.GdpInBillions;
            Assert.True(costToGdpRatio > 100 && costToGdpRatio < 1000, 
                $"{territory.CountryName} cost should scale appropriately with GDP for educational realism");
                
            Output.WriteLine($"✅ Real-World Data: {territory.CountryName} - GDP: ${territory.GdpInBillions:N0}B, Cost: ${territory.Cost:N0}");
        }
    }

    #endregion

    #region Performance and Reliability Tests

    [Fact]
    public async Task TerritoryService_RespondsWithinPerformanceTargets()
    {
        // Arrange
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        // Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<ITerritoryService>();
            var testPlayer = context.Players.First();
            
            return await service.GetAvailableTerritoriesAsync(testPlayer.Id);
        });
        
        stopwatch.Stop();

        // Assert
        Assert.NotEmpty(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 500, 
            $"Territory service should respond within 500ms for child-friendly experience. Actual: {stopwatch.ElapsedMilliseconds}ms");
            
        Output.WriteLine($"✅ Performance: Territory service responded in {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task TerritoryService_HandlesInvalidRequests_Gracefully()
    {
        // Arrange & Act
        await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<ITerritoryService>();
            var invalidPlayerId = Guid.NewGuid();
            var invalidTerritoryId = Guid.NewGuid();

            // Test invalid player ID
            var availableTerritories = await service.GetAvailableTerritoriesAsync(invalidPlayerId);
            Assert.NotNull(availableTerritories);
            Assert.Empty(availableTerritories);

            // Test invalid territory acquisition
            var acquisitionResult = await service.AcquireTerritoryAsync(invalidPlayerId, invalidTerritoryId);
            Assert.NotNull(acquisitionResult);
            Assert.False(acquisitionResult.Success);
            ValidateChildSafeContent(acquisitionResult.Message, "Error message for invalid territory acquisition");

            Output.WriteLine("✅ Service handles invalid requests gracefully with child-safe messages");
        });
    }

    #endregion

    #region Cultural Sensitivity and Educational Values Tests

    [Fact]
    public async Task TerritoryService_PromotesCulturalAwareness_ThroughDiverseRepresentation()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<ITerritoryService>();
            var testPlayer = context.Players.First();
            
            return await service.GetAvailableTerritoriesAsync(testPlayer.Id);
        });

        // Assert
        Assert.NotEmpty(result);
        
        // Validate diverse geographical representation
        var regions = result.Select(t => t.CountryCode.Substring(0, 1)).Distinct().ToList();
        Assert.True(regions.Count > 1, "Should include territories from diverse regions for cultural awareness");
        
        // Validate language diversity for educational value
        var allLanguages = result.SelectMany(t => t.OfficialLanguages).Distinct().ToList();
        Assert.True(allLanguages.Count >= 3, "Should include diverse languages for cultural learning");
        
        Output.WriteLine($"✅ Cultural Diversity: {allLanguages.Count} languages across {result.Count} territories");
        foreach (var language in allLanguages)
        {
            Output.WriteLine($"   - Language: {language}");
        }
    }

    #endregion
}