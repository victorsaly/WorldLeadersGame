using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
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
/// Comprehensive testing for Player Service with educational achievement focus
/// Context: Educational game component for 12-year-old players tracking learning progress
/// Educational Objective: Validate player management supports achievement motivation and learning tracking
/// Safety Requirements: COPPA-compliant data handling, child privacy protection, positive reinforcement
/// </summary>
public class PlayerServiceTests : ServiceTestBase
{
    public PlayerServiceTests(ITestOutputHelper output) : base(output)
    {
    }

    protected override void ConfigureAdditionalServices(IServiceCollection services)
    {
        var mockLogger = CreateEducationalMock<ILogger<PlayerService>>();
        services.AddSingleton(mockLogger.Object);
        services.AddScoped<IPlayerService, PlayerService>();
    }

    protected override async Task SeedTestDataAsync(WorldLeadersDbContext dbContext)
    {
        // Create test players with diverse progress levels
        var testPlayers = new[]
        {
            new PlayerEntity
            {
                Id = Guid.NewGuid(),
                Username = "beginner_student",
                DisplayName = "Alex Explorer",
                CurrentJob = JobLevel.Student,
                Income = 500,
                Reputation = 15,
                Happiness = 85,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                LastActiveAt = DateTime.UtcNow.AddHours(-2)
            },
            new PlayerEntity
            {
                Id = Guid.NewGuid(),
                Username = "active_learner",
                DisplayName = "Sam Achiever",
                CurrentJob = JobLevel.Teacher,
                Income = 3000,
                Reputation = 65,
                Happiness = 92,
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                LastActiveAt = DateTime.UtcNow.AddMinutes(-30)
            },
            new PlayerEntity
            {
                Id = Guid.NewGuid(),
                Username = "advanced_player",
                DisplayName = "Jordan Leader",
                CurrentJob = JobLevel.Manager,
                Income = 8000,
                Reputation = 88,
                Happiness = 95,
                CreatedAt = DateTime.UtcNow.AddDays(-60),
                LastActiveAt = DateTime.UtcNow.AddMinutes(-5)
            }
        };

        dbContext.Players.AddRange(testPlayers);

        // Create test territories for some players
        var territories = new[]
        {
            new TerritoryEntity
            {
                Id = Guid.NewGuid(),
                CountryName = "Canada",
                CountryCode = "CA",
                GdpInBillions = 2139.0m,
                Tier = (TerritoryTier)1,
                Cost = 500000,
                ReputationRequired = 40,
                MonthlyIncome = 5000,
                IsAvailable = false,
                OwnedByPlayerId = testPlayers[1].Id, // Sam Achiever owns Canada
                OfficialLanguagesJson = JsonSerializer.Serialize(new[] { "English", "French" }),
                CreatedAt = DateTime.UtcNow
            },
            new TerritoryEntity
            {
                Id = Guid.NewGuid(),
                CountryName = "Japan",
                CountryCode = "JP",
                GdpInBillions = 4937.0m,
                Tier = (TerritoryTier)2,
                Cost = 1200000,
                ReputationRequired = 70,
                MonthlyIncome = 12000,
                IsAvailable = false,
                OwnedByPlayerId = testPlayers[2].Id, // Jordan Leader owns Japan
                OfficialLanguagesJson = """["Japanese"]""",
                CreatedAt = DateTime.UtcNow
            }
        };

        dbContext.Territories.AddRange(territories);

        // Create dice roll history for educational tracking
        var diceRolls = new[]
        {
            new DiceRollHistoryEntity
            {
                Id = Guid.NewGuid(),
                PlayerId = testPlayers[1].Id,
                DiceValue = 4,
                PreviousJob = JobLevel.Student,
                NewJob = JobLevel.Teacher,
                IncomeChange = 2500,
                ReputationChange = 25,
                HappinessChange = 10,
                RolledAt = DateTime.UtcNow.AddDays(-10)
            },
            new DiceRollHistoryEntity
            {
                Id = Guid.NewGuid(),
                PlayerId = testPlayers[2].Id,
                DiceValue = 6,
                PreviousJob = JobLevel.Teacher,
                NewJob = JobLevel.Manager,
                IncomeChange = 5000,
                ReputationChange = 30,
                HappinessChange = 15,
                RolledAt = DateTime.UtcNow.AddDays(-5)
            }
        };

        dbContext.DiceRollHistory.AddRange(diceRolls);

        // Create achievements for testing
        var achievements = new[]
        {
            new PlayerAchievementEntity
            {
                Id = Guid.NewGuid(),
                PlayerId = testPlayers[1].Id,
                AchievementId = "first_territory",
                Title = "Geography Explorer",
                Description = "Acquired your first territory and learned about geography! You're exploring how countries develop and expand.",
                IconEmoji = "🌍",
                PointsReward = 100,
                UnlockedAt = DateTime.UtcNow.AddDays(-8)
            },
            new PlayerAchievementEntity
            {
                Id = Guid.NewGuid(),
                PlayerId = testPlayers[2].Id,
                AchievementId = "career_advancement",
                Title = "Career Climber",
                Description = "Advanced to Manager level and discovered different career paths! You're learning how skills help you grow in your profession.",
                IconEmoji = "📈",
                PointsReward = 250,
                UnlockedAt = DateTime.UtcNow.AddDays(-3)
            }
        };

        dbContext.PlayerAchievements.AddRange(achievements);

        await dbContext.SaveChangesAsync();
    }

    #region Player Creation and Profile Management Tests

    [Fact]
    public async Task CreatePlayerAsync_CreatesValidPlayer_WithEducationalDefaults()
    {
        // Arrange
        var createRequest = new CreatePlayerRequest(
            "new_student_123",
            Guid.NewGuid()
        );

        // Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IPlayerService>();
            return await service.CreatePlayerAsync(createRequest);
        });

        // Assert
        Assert.NotNull(result);
        Assert.Equal("new_student_123", result.Username);
        Assert.Equal("Taylor Learner", result.DisplayName);
        Assert.Equal(JobLevel.Student, result.CurrentJob);
        Assert.True(result.Income > 0, "New player should start with positive income");
        Assert.True(result.Reputation >= 0, "New player should start with non-negative reputation");
        Assert.True(result.Happiness > 50, "New player should start with positive happiness");
        Assert.True(result.Id != Guid.Empty, "Player should have valid ID");

        ValidateEducationalOutcome(result, "positive onboarding experience for new learners");
        
        Output.WriteLine($"✅ New Player Created: {result.DisplayName} ({result.Username}) - Job: {result.CurrentJob}, Income: ${result.Income}");
    }

    [Fact]
    public async Task CreatePlayerAsync_ValidatesChildSafeUsernames()
    {
        // Arrange & Act
        var inappropriateRequest = new CreatePlayerRequest(
            "contact_me_123",  // Contains contact-related word
            Guid.NewGuid()
        );

        await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IPlayerService>();
            
            // This should either succeed with a sanitized username or fail gracefully
            try
            {
                var result = await service.CreatePlayerAsync(inappropriateRequest);
                
                // If it succeeds, validate the result is child-safe
                ValidateChildSafeContent(result.Username ?? "", "Player username");
                ValidateChildSafeContent(result.DisplayName, "Player display name");
            }
            catch (ArgumentException ex)
            {
                // Acceptable to reject inappropriate usernames
                ValidateChildSafeContent(ex.Message, "Username validation error message");
            }
        });
        
        Output.WriteLine("✅ Service handles potentially inappropriate usernames safely");
    }

    #endregion

    #region Player Retrieval and Profile Tests

    [Fact]
    public async Task GetPlayerAsync_RetrievesExistingPlayer_WithCompleteProfile()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IPlayerService>();
            var testPlayer = context.Players.First(p => p.Username == "active_learner");
            
            return await service.GetPlayerAsync(testPlayer.Id);
        });

        // Assert
        Assert.NotNull(result);
        Assert.Equal("active_learner", result.Username);
        Assert.Equal("Sam Achiever", result.DisplayName);
        Assert.Equal(JobLevel.Teacher, result.CurrentJob);
        Assert.True(result.Income > 0);
        Assert.True(result.Reputation > 0);
        Assert.True(result.Happiness > 0);

        ValidateEducationalOutcome(result, "player progress tracking and motivation");
        
        Output.WriteLine($"✅ Player Retrieved: {result.DisplayName} - Job: {result.CurrentJob}, Rep: {result.Reputation}, Happy: {result.Happiness}");
    }

    [Fact]
    public async Task GetPlayerAsync_ReturnsNull_ForNonExistentPlayer()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IPlayerService>();
            var nonExistentId = Guid.NewGuid();
            
            return await service.GetPlayerAsync(nonExistentId);
        });

        // Assert
        Assert.Null(result);
        
        Output.WriteLine("✅ Service handles non-existent player gracefully");
    }

    #endregion

    #region Player Dashboard and Analytics Tests

    [Fact]
    public async Task GetPlayerDashboardAsync_ProvidesEducationalOverview()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IPlayerService>();
            var testPlayer = context.Players.First(p => p.Username == "active_learner");
            
            return await service.GetPlayerDashboardAsync(testPlayer.Id);
        });

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Player);
        Assert.Equal("Sam Achiever", result.Player.DisplayName);
        
        // Validate educational dashboard components
        Assert.NotNull(result.Territories);
        Assert.NotEmpty(result.Territories); // Should have Canada
        Assert.NotNull(result.RecentAchievements);
        Assert.NotNull(result.LearningProgress);
        
        // Validate territory information
        var canadaTerritory = result.Territories.FirstOrDefault(t => t.CountryCode == "CA");
        Assert.NotNull(canadaTerritory);
        Assert.Equal("Canada", canadaTerritory.CountryName);
        Assert.True(canadaTerritory.IsOwned);

        ValidateEducationalOutcome(result, "comprehensive learning progress overview");
        
        Output.WriteLine($"✅ Dashboard: {result.Player.DisplayName} owns {result.Territories.Count} territories, {result.RecentAchievements.Count} achievements");
    }

    [Fact]
    public async Task GetPlayerAnalyticsAsync_TracksEducationalProgress()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IPlayerService>();
            var testPlayer = context.Players.First(p => p.Username == "advanced_player");
            
            return await service.GetPlayerAnalyticsAsync(testPlayer.Id);
        });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.TotalPlayTime > TimeSpan.Zero, "Should track meaningful play time");
        Assert.True(result.DiceRollsCount >= 0, "Should track dice roll history");
        Assert.True(result.TerritoriesCount >= 0, "Should track territory acquisitions");
        Assert.True(result.AchievementsCount >= 0, "Should track achievements earned");
        
        // Validate educational metrics
        Assert.NotNull(result.LearningObjectivesMet);
        Assert.NotEmpty(result.LearningObjectivesMet); // Advanced player should have met some objectives
        
        // Educational objectives should be meaningful for 12-year-olds
        Output.WriteLine($"🔍 DEBUG: Found {result.LearningObjectivesMet.Count} learning objectives:");
        for (int i = 0; i < result.LearningObjectivesMet.Count; i++)
        {
            var objective = result.LearningObjectivesMet[i];
            Output.WriteLine($"   {i + 1}. '{objective}'");
        }
        
        // Debug: Test regex patterns manually for first objective
        if (result.LearningObjectivesMet.Count > 0)
        {
            var testContent = result.LearningObjectivesMet[0];
            Output.WriteLine($"🔬 Testing regex patterns for: '{testContent}'");
            
            var educationalPatterns = new[]
            {
                @"\b(learn|learner|discover|explore|explorer|understand|grow)\b",
                @"\b(geography|country|language|culture|economy)\b", 
                @"\b(skill|knowledge|education|progress|advanced)\b",
                @"\b(student|teacher|guide|helper|builder)\b"
            };
            
            for (int p = 0; p < educationalPatterns.Length; p++)
            {
                var matches = System.Text.RegularExpressions.Regex.IsMatch(testContent, educationalPatterns[p], System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                Output.WriteLine($"   Pattern {p + 1}: {educationalPatterns[p]} -> {matches}");
            }
        }
        
        // Now validate each one
        for (int i = 0; i < result.LearningObjectivesMet.Count; i++)
        {
            var objective = result.LearningObjectivesMet[i];
            try 
            {
                ValidateChildSafeContent(objective, "Learning objective description");
                ValidateEducationalOutcome(objective, "specific educational achievement");
                Output.WriteLine($"✅ Objective {i + 1} passed validation");
            }
            catch (Exception ex)
            {
                Output.WriteLine($"❌ Objective {i + 1} failed validation: {ex.Message}");
                throw; // Re-throw to fail the test
            }
        }

        Output.WriteLine($"✅ Analytics: {result.DiceRollsCount} rolls, {result.TerritoriesCount} territories, {result.AchievementsCount} achievements");
        foreach (var objective in result.LearningObjectivesMet.Take(3))
        {
            Output.WriteLine($"   📚 Learning: {objective}");
        }
    }

    #endregion

    #region Achievement System Tests

    [Fact]
    public async Task GetPlayerAchievementsAsync_ReturnsOrderedAchievements()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IPlayerService>();
            var testPlayer = context.Players.First(p => p.Username == "active_learner");
            
            return await service.GetPlayerAchievementsAsync(testPlayer.Id);
        });

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        
        // Validate achievement structure
        foreach (var achievement in result)
        {
            Assert.NotEmpty(achievement.Title);
            Assert.NotEmpty(achievement.Description);
            Assert.NotEmpty(achievement.IconEmoji);
            Assert.True(achievement.PointsReward > 0, "Achievement should have positive point reward");
            Assert.True(achievement.IsUnlocked, "Retrieved achievements should be unlocked");
            
            ValidateChildSafeContent(achievement.Title, "Achievement title");
            ValidateChildSafeContent(achievement.Description, "Achievement description");
            ValidateEducationalOutcome(achievement, "learning motivation and recognition");
        }
        
        // Validate chronological order (most recent first)
        if (result.Count > 1)
        {
            for (var i = 1; i < result.Count; i++)
            {
                Assert.True(result[i - 1].UnlockedAt >= result[i].UnlockedAt,
                    "Achievements should be ordered by unlock date (most recent first)");
            }
        }

        Output.WriteLine($"✅ Achievements: {result.Count} unlocked achievements");
        foreach (var achievement in result.Take(2))
        {
            Output.WriteLine($"   🏆 {achievement.IconEmoji} {achievement.Title}: {achievement.Description}");
        }
    }

    [Fact]
    public async Task AwardAchievementAsync_GrantsNewAchievement_WithEducationalValue()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IPlayerService>();
            var testPlayer = context.Players.First(p => p.Username == "beginner_student");
            
            return await service.AwardAchievementAsync(testPlayer.Id, "first_dice_roll");
        });

        // Assert
        Assert.True(result, "Should successfully award new achievement");
        
        // Verify achievement was actually saved
        await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IPlayerService>();
            var testPlayer = context.Players.First(p => p.Username == "beginner_student");
            var achievements = await service.GetPlayerAchievementsAsync(testPlayer.Id);
            
            var newAchievement = achievements.FirstOrDefault(a => a.AchievementId == "first_dice_roll");
            Assert.NotNull(newAchievement);
            ValidateEducationalOutcome(newAchievement, "achievement-based learning motivation");
        });

        Output.WriteLine("✅ Achievement awarded successfully with educational motivation");
    }

    [Fact]
    public async Task AwardAchievementAsync_PreventsDuplicateAchievements()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IPlayerService>();
            var testPlayer = context.Players.First(p => p.Username == "active_learner");
            
            // Try to award achievement that player already has
            return await service.AwardAchievementAsync(testPlayer.Id, "first_territory");
        });

        // Assert
        Assert.False(result, "Should not award duplicate achievement");
        
        Output.WriteLine("✅ Service prevents duplicate achievement awards correctly");
    }

    #endregion

    #region Player Update and Profile Management Tests

    [Fact]
    public async Task UpdatePlayerAsync_UpdatesPlayerProfile_WithValidation()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IPlayerService>();
            var testPlayer = context.Players.First(p => p.Username == "beginner_student");
            
            var originalPlayer = await service.GetPlayerAsync(testPlayer.Id);
            Assert.NotNull(originalPlayer);
            
            // Update player with new values
            originalPlayer.DisplayName = "Alex Advanced Explorer";
            originalPlayer.Reputation = 25;
            originalPlayer.Happiness = 90;
            
            return await service.UpdatePlayerAsync(originalPlayer);
        });

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Alex Advanced Explorer", result.DisplayName);
        Assert.Equal(25, result.Reputation);
        Assert.Equal(90, result.Happiness);
        
        ValidateChildSafeContent(result.DisplayName, "Updated player display name");
        ValidateEducationalOutcome(result, "player profile customization and ownership");

        Output.WriteLine($"✅ Player Profile Updated: {result.DisplayName} - Rep: {result.Reputation}, Happy: {result.Happiness}");
    }

    [Fact]
    public async Task UpdatePlayerAsync_ValidatesStatBounds_ForGameBalance()
    {
        // Arrange & Act
        await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IPlayerService>();
            var testPlayer = context.Players.First(p => p.Username == "beginner_student");
            
            var originalPlayer = await service.GetPlayerAsync(testPlayer.Id);
            Assert.NotNull(originalPlayer);
            
            // Try to update with out-of-bounds values
            originalPlayer.Reputation = 150; // Over 100
            originalPlayer.Happiness = -10;  // Below 0
            
            var result = await service.UpdatePlayerAsync(originalPlayer);
            
            // Assert bounds are enforced
            Assert.InRange(result.Reputation, 0, 100);
            Assert.InRange(result.Happiness, 0, 100);
        });

        Output.WriteLine("✅ Service enforces stat bounds for balanced gameplay");
    }

    #endregion

    #region Educational Progress Tracking Tests

    [Fact]
    public async Task PlayerService_TracksGeographyLearning_ThroughTerritoryOwnership()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IPlayerService>();
            var testPlayer = context.Players.First(p => p.Username == "active_learner");
            
            var analytics = await service.GetPlayerAnalyticsAsync(testPlayer.Id);
            
            // Assert geography learning tracking
            Assert.True(analytics.TerritoriesCount > 0, "Player should own territories for geography learning");
            
            var geographyObjectives = analytics.LearningObjectivesMet
                .Where(obj => obj.ToLowerInvariant().Contains("geography"))
                .ToList();
                
            Assert.NotEmpty(geographyObjectives);
            
            foreach (var objective in geographyObjectives)
            {
                ValidateEducationalOutcome(objective, "geography knowledge acquisition");
            }

            return analytics;
        });

        Output.WriteLine($"✅ Geography Learning: {result.TerritoriesCount} territories owned");
        var geoObjectives = result.LearningObjectivesMet.Where(obj => obj.ToLowerInvariant().Contains("geography"));
        foreach (var objective in geoObjectives)
        {
            Output.WriteLine($"   🌍 {objective}");
        }
    }

    [Fact]
    public async Task PlayerService_TracksEconomicLearning_ThroughIncomeProgression()
    {
        // Arrange & Act
        var result = await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IPlayerService>();
            var testPlayer = context.Players.First(p => p.Username == "advanced_player");
            
            var analytics = await service.GetPlayerAnalyticsAsync(testPlayer.Id);
            
            // Assert economic learning tracking
            var economicObjectives = analytics.LearningObjectivesMet
                .Where(obj => obj.ToLowerInvariant().Contains("economic") || 
                            obj.ToLowerInvariant().Contains("income"))
                .ToList();
                
            Assert.NotEmpty(economicObjectives);
            
            foreach (var objective in economicObjectives)
            {
                ValidateEducationalOutcome(objective, "economic concept understanding");
            }

            return analytics;
        });

        Output.WriteLine("✅ Economic Learning tracked through income progression");
        var ecoObjectives = result.LearningObjectivesMet.Where(obj => 
            obj.ToLowerInvariant().Contains("economic") || obj.ToLowerInvariant().Contains("income"));
        foreach (var objective in ecoObjectives)
        {
            Output.WriteLine($"   💰 {objective}");
        }
    }

    #endregion

    #region Performance and Reliability Tests

    [Fact]
    public async Task PlayerService_RespondsWithinPerformanceTargets()
    {
        // Arrange
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IPlayerService>();
            var testPlayer = context.Players.First();
            
            await service.GetPlayerDashboardAsync(testPlayer.Id);
        });
        
        stopwatch.Stop();

        // Assert
        Assert.True(stopwatch.ElapsedMilliseconds < 500, 
            $"Player dashboard should load within 500ms for child-friendly experience. Actual: {stopwatch.ElapsedMilliseconds}ms");

        Output.WriteLine($"✅ Performance: Player dashboard loaded in {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task PlayerService_HandlesLargeDataSets_Efficiently()
    {
        // Arrange & Act - Test with multiple players and achievements
        await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IPlayerService>();
            
            // Get all players and their data
            var players = await context.Players.ToListAsync();
            var allPlayerTasks = players.Select(async p => 
            {
                var dashboard = await service.GetPlayerDashboardAsync(p.Id);
                var achievements = await service.GetPlayerAchievementsAsync(p.Id);
                var analytics = await service.GetPlayerAnalyticsAsync(p.Id);
                
                return new { Dashboard = dashboard, Achievements = achievements, Analytics = analytics };
            });
            
            var results = await Task.WhenAll(allPlayerTasks);
            
            // Assert all operations completed successfully
            Assert.All(results, result =>
            {
                Assert.NotNull(result.Dashboard);
                Assert.NotNull(result.Achievements);
                Assert.NotNull(result.Analytics);
            });
        });

        Output.WriteLine("✅ Service handles multiple concurrent player data requests efficiently");
    }

    #endregion

    #region COPPA Compliance and Child Safety Tests

    [Fact]
    public async Task PlayerService_ProtectsChildData_AccordingToCOPPA()
    {
        // Arrange & Act
        await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IPlayerService>();
            var testPlayer = context.Players.First();
            
            var player = await service.GetPlayerAsync(testPlayer.Id);
            Assert.NotNull(player);
            
            // Assert no personal information is exposed
            Assert.DoesNotContain("@", player.Username);
            Assert.DoesNotContain("email", player.Username?.ToLowerInvariant() ?? "");
            Assert.DoesNotContain("phone", player.Username?.ToLowerInvariant() ?? "");
            
            ValidateChildSafeContent(player.Username ?? "", "Player username for COPPA compliance");
            ValidateChildSafeContent(player.DisplayName, "Player display name for COPPA compliance");
            
            // Validate that only educational data is tracked
            var analytics = await service.GetPlayerAnalyticsAsync(testPlayer.Id);
            Assert.NotNull(analytics);
            
            // All tracked data should be educational and game-related
            foreach (var objective in analytics.LearningObjectivesMet)
            {
                ValidateEducationalOutcome(objective, "COPPA-compliant educational tracking");
            }
        });

        Output.WriteLine("✅ Service protects child data according to COPPA requirements");
    }

    [Fact]
    public async Task PlayerService_ProvidesPositiveExperience_ForAllPlayerLevels()
    {
        // Arrange & Act
        await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IPlayerService>();
            
            foreach (var testPlayer in context.Players)
            {
                var dashboard = await service.GetPlayerDashboardAsync(testPlayer.Id);
                Assert.NotNull(dashboard);
                
                // Every player should have positive experience indicators
                Assert.True(dashboard.Player.Happiness > 0, "Every player should have positive happiness");
                Assert.True(dashboard.Player.Income > 0, "Every player should have positive income");
                Assert.True(dashboard.Player.Reputation >= 0, "Every player should have non-negative reputation");
                
                // Learning progress should always show positive elements
                Assert.NotNull(dashboard.LearningProgress);
                
                ValidateEducationalOutcome(dashboard, "positive learning experience for all skill levels");
                
                Output.WriteLine($"✅ Positive Experience: {dashboard.Player.DisplayName} - Happy: {dashboard.Player.Happiness}, Income: ${dashboard.Player.Income}");
            }
        });
    }

    #endregion
}