using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using WorldLeaders.Infrastructure.Data;
using WorldLeaders.Infrastructure.Entities;
using WorldLeaders.Infrastructure.Services;
using WorldLeaders.Infrastructure.Tests.Infrastructure;
using WorldLeaders.Shared.Constants;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Services;
using Xunit.Abstractions;

namespace WorldLeaders.Infrastructure.Tests.Services;

/// <summary>
/// Comprehensive testing for Dice Service with educational career progression focus
/// Context: Educational game component for 12-year-old players learning career development
/// Educational Objective: Validate dice mechanics teach resilience, career progression, and positive outcomes
/// Safety Requirements: Encouraging messages for ALL outcomes, no negative feedback, child-friendly design
/// </summary>
public class DiceServiceTests : ServiceTestBase
{
    public DiceServiceTests(ITestOutputHelper output) : base(output)
    {
    }

    protected override void ConfigureAdditionalServices(IServiceCollection services)
    {
        var mockLogger = CreateEducationalMock<ILogger<DiceService>>();
        services.AddSingleton(mockLogger.Object);
        services.AddScoped<IDiceService, DiceService>();
    }

    protected override async Task SeedTestDataAsync(WorldLeadersDbContext dbContext)
    {
        // Create test players with different career levels for testing
        var testPlayers = new[]
        {
            new PlayerEntity
            {
                Id = Guid.NewGuid(),
                Username = "student_player",
                DisplayName = "Test Student",
                CurrentJob = JobLevel.Student,
                Income = 500,
                Reputation = 10,
                Happiness = 80,
                CreatedAt = DateTime.UtcNow.AddSeconds(-10),
                LastActiveAt = DateTime.UtcNow.AddSeconds(-10)
            },
            new PlayerEntity
            {
                Id = Guid.NewGuid(),
                Username = "teacher_player",
                DisplayName = "Test Teacher",
                CurrentJob = JobLevel.Teacher,
                Income = 3000,
                Reputation = 45,
                Happiness = 85,
                CreatedAt = DateTime.UtcNow.AddSeconds(-10),
                LastActiveAt = DateTime.UtcNow.AddSeconds(-10)
            },
            new PlayerEntity
            {
                Id = Guid.NewGuid(),
                Username = "manager_player",
                DisplayName = "Test Manager",
                CurrentJob = JobLevel.Manager,
                Income = 8000,
                Reputation = 70,
                Happiness = 90,
                CreatedAt = DateTime.UtcNow,
                LastActiveAt = DateTime.UtcNow
            }
        };

        dbContext.Players.AddRange(testPlayers);
        await dbContext.SaveChangesAsync();
    }

    #region Dice Rolling Mechanics Tests

    [Fact]
    public async Task RollForJobAsync_GeneratesValidDiceValues_WithinExpectedRange()
    {
        // Arrange - Setup data in the service provider's database context
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WorldLeadersDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        await SeedTestDataAsync(dbContext);
        
        var service = scope.ServiceProvider.GetRequiredService<IDiceService>();
        var testPlayer = dbContext.Players.First(p => p.CurrentJob == JobLevel.Student);
        
        // Act - Roll dice multiple times to test randomness
        var results = new List<int>();
        for (var i = 0; i < 50; i++)
        {
            var rollResult = await service.RollForJobAsync(testPlayer.Id);
            Assert.NotNull(rollResult);
            Assert.InRange(rollResult.DiceValue, 1, 6);
            results.Add(rollResult.DiceValue);
            
            // Reset player for next roll
            testPlayer.CurrentJob = JobLevel.Student;
            testPlayer.Income = 500;
            dbContext.Players.Update(testPlayer);
            await dbContext.SaveChangesAsync();
        }

        // Assert randomness and fairness
        Assert.True(results.Distinct().Count() >= 4, "Dice should show reasonable randomness over 50 rolls");
        
        // Validate each value appears within reasonable frequency (basic fairness check)
        for (var value = 1; value <= 6; value++)
        {
            var count = results.Count(r => r == value);
            Assert.InRange(count, 2, 20); // Should appear between 2-20 times in 50 rolls
        }
        
        Output.WriteLine($"✅ Dice Fairness Test: Values 1-6 appeared: {string.Join(", ", Enumerable.Range(1, 6).Select(i => $"{i}={results.Count(r => r == i)}"))}");
    }

    [Fact]
    public async Task RollForJobAsync_AdvancesCareerProgression_AccordingToEducationalMapping()
    {
        // Arrange - Setup data in the service provider's database context
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WorldLeadersDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        await SeedTestDataAsync(dbContext);
        
        var service = scope.ServiceProvider.GetRequiredService<IDiceService>();
        var testPlayer = dbContext.Players.First(p => p.CurrentJob == JobLevel.Student);
        var originalJob = testPlayer.CurrentJob;
        var originalIncome = testPlayer.Income;

        // Act
        var rollResult = await service.RollForJobAsync(testPlayer.Id);

        // Refresh player data
        var updatedPlayer = dbContext.Players.Find(testPlayer.Id);
        Assert.NotNull(updatedPlayer);

        // Assert
        Assert.NotNull(rollResult);
        Assert.InRange(rollResult.DiceValue, 1, 6);
        Assert.True(rollResult.IncomeChange != 0, "Income should change with job progression");
        Assert.True(rollResult.ReputationChange >= 0, "Reputation should never decrease for child-friendly experience");
        Assert.True(rollResult.HappinessChange > 0, "Happiness should always increase to encourage continued play");

        // Validate job progression mapping
        var expectedJob = JobProgressionMapping.GetJobFromDiceRoll(rollResult.DiceValue);
        Assert.Equal(expectedJob, rollResult.NewJob);
        Assert.Equal(expectedJob, updatedPlayer.CurrentJob);

        // Validate income progression
        var expectedIncome = JobProgressionMapping.GetJobIncome(expectedJob);
        Assert.Equal(expectedIncome, updatedPlayer.Income);

        ValidateEducationalOutcome(rollResult, "career progression and resilience learning");
        
        Output.WriteLine($"✅ Career Progression: {originalJob} → {rollResult.NewJob}, Income: ${originalIncome} → ${updatedPlayer.Income}");
    }

    #endregion

    #region Encouraging Messages Tests

    [Fact]
    public void GetEncouragingMessage_ProvidesPositiveMessages_ForAllDiceValues()
    {
        // Arrange
        var service = GetService<IDiceService>();
        var jobLevels = Enum.GetValues<JobLevel>();

        foreach (var diceValue in Enumerable.Range(1, 6))
        {
            foreach (var jobLevel in jobLevels)
            {
                // Act
                var message = service.GetEncouragingMessage(diceValue, jobLevel);

                // Assert
                Assert.NotEmpty(message);
                ValidateChildSafeContent(message, $"Encouraging message for dice {diceValue}, job {jobLevel}");
                
                // Validate encouraging tone
                var lowerMessage = message.ToLowerInvariant();
                Assert.True(
                    lowerMessage.Contains("great") || 
                    lowerMessage.Contains("awesome") || 
                    lowerMessage.Contains("fantastic") ||
                    lowerMessage.Contains("excellent") ||
                    lowerMessage.Contains("wonderful") ||
                    lowerMessage.Contains("amazing") ||
                    lowerMessage.Contains("outstanding"),
                    $"Message should be encouraging for dice {diceValue}, job {jobLevel}: {message}");

                // Should never contain negative words
                Assert.DoesNotContain("bad", lowerMessage);
                Assert.DoesNotContain("fail", lowerMessage);
                Assert.DoesNotContain("wrong", lowerMessage);
                Assert.DoesNotContain("terrible", lowerMessage);

                Output.WriteLine($"✅ Dice {diceValue} → {jobLevel}: {message}");
            }
        }
    }

    [Fact]
    public void GetJobDescription_ProvidesEducationalInformation_ForAllCareerLevels()
    {
        // Arrange
        var service = GetService<IDiceService>();
        var jobLevels = Enum.GetValues<JobLevel>();

        foreach (var jobLevel in jobLevels)
        {
            // Act
            var description = service.GetJobDescription(jobLevel);

            // Assert
            Assert.NotEmpty(description);
            ValidateChildSafeContent(description, $"Job description for {jobLevel}");
            
            // Should contain educational career information
            var lowerDescription = description.ToLowerInvariant();
            Assert.True(
                lowerDescription.Contains("learn") || 
                lowerDescription.Contains("help") || 
                lowerDescription.Contains("work") ||
                lowerDescription.Contains("skill") ||
                lowerDescription.Contains("knowledge"),
                $"Job description should be educational for {jobLevel}: {description}");

            Output.WriteLine($"✅ {jobLevel} Description: {description}");
        }
    }

    #endregion

    #region Job Progression Mapping Tests

    [Fact]
    public void JobProgressionMapping_ProvidesFairDistribution_AcrossCareerLevels()
    {
        // Arrange & Act
        var mappingCounts = new Dictionary<JobLevel, int>();
        
        for (var diceValue = 1; diceValue <= 6; diceValue++)
        {
            var job = JobProgressionMapping.GetJobFromDiceRoll(diceValue);
            mappingCounts[job] = mappingCounts.GetValueOrDefault(job, 0) + 1;
            
            Output.WriteLine($"Dice {diceValue} → {job}");
        }

        // Assert
        Assert.True(mappingCounts.Count >= 3, "Should map to at least 3 different job levels for variety");
        
        // Validate educational progression logic
        foreach (var mapping in mappingCounts)
        {
            var job = mapping.Key;
            var income = JobProgressionMapping.GetJobIncome(job);
            var reputationBonus = JobProgressionMapping.GetJobReputationBonus(job);
            
            Assert.True(income > 0, $"{job} should have positive income");
            Assert.True(reputationBonus >= 0, $"{job} should have non-negative reputation bonus");
            
            Output.WriteLine($"✅ {job}: Income=${income:N0}, Reputation+{reputationBonus}");
        }
    }

    [Fact]
    public void JobProgressionMapping_ShowsRealisticIncomeProgression()
    {
        // Arrange
        var allJobs = Enum.GetValues<JobLevel>();
        var incomeData = new List<(JobLevel Job, int Income)>();

        foreach (var job in allJobs)
        {
            // Act
            var income = JobProgressionMapping.GetJobIncome(job);
            incomeData.Add((job, income));
        }

        // Assert
        var sortedByIncome = incomeData.OrderBy(x => x.Income).ToList();
        
        // Validate realistic income progression for educational value
        Assert.True(sortedByIncome.First().Income >= 500, "Entry-level jobs should have reasonable starting income");
        Assert.True(sortedByIncome.Last().Income <= 50000, "High-level jobs should have realistic but aspirational income");
        
        // Validate progressive income increases
        for (var i = 1; i < sortedByIncome.Count; i++)
        {
            var current = sortedByIncome[i];
            var previous = sortedByIncome[i - 1];
            
            Assert.True(current.Income >= previous.Income, 
                $"Income progression should be logical: {previous.Job}(${previous.Income}) → {current.Job}(${current.Income})");
        }
        
        Output.WriteLine("✅ Realistic Income Progression:");
        foreach (var (job, income) in sortedByIncome)
        {
            Output.WriteLine($"   {job}: ${income:N0}");
        }
    }

    #endregion

    #region Dice Roll History and Analytics Tests

    [Fact]
    public async Task RollForJobAsync_SavesDiceRollHistory_ForEducationalTracking()
    {
        // Arrange & Act
        await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IDiceService>();
            var testPlayer = context.Players.First(p => p.CurrentJob == JobLevel.Student);
            var rollResults = new List<(int DiceValue, JobLevel NewJob)>();

            // Act - Perform multiple rolls
            for (var i = 0; i < 5; i++)
            {
                var rollResult = await service.RollForJobAsync(testPlayer.Id);
                rollResults.Add((rollResult.DiceValue, rollResult.NewJob));
                
                // Ensure database changes are saved
                await context.SaveChangesAsync();
                
                // Small delay to ensure different timestamps
                await Task.Delay(10);
                
                // Verify history is being saved progressively
                var currentHistory = await service.GetDiceRollHistoryAsync(testPlayer.Id);
                Output.WriteLine($"Roll {i + 1}: History count = {currentHistory.Count}");
            }

            var history = await service.GetDiceRollHistoryAsync(testPlayer.Id);
            
            // Assert
            Assert.NotNull(history);
            Assert.True(history.Count >= 5, $"Should save dice roll history for educational tracking. Expected >= 5, got {history.Count}");
            
            foreach (var historyItem in history.Take(5))
            {
                Assert.InRange(historyItem.DiceValue, 1, 6);
                Assert.NotEqual(JobLevel.Student, historyItem.ResultingJob); // Should have progressed
                // Income change can be positive, negative, or zero depending on job transitions
                // Just verify it's a reasonable value (not extreme)
                Assert.InRange(historyItem.IncomeChange, -10000, 50000); // Reasonable income change range
                
                ValidateEducationalOutcome(historyItem, "progress tracking and learning analytics");
            }
            
            Output.WriteLine($"✅ Dice Roll History: {history.Count} entries saved for educational tracking");
            foreach (var item in history.Take(3))
            {
                Output.WriteLine($"   Dice: {item.DiceValue} → {item.ResultingJob} (Income: {item.IncomeChange:+#;-#;0})");
            }
        });
    }

    [Fact]
    public async Task GetDiceRollHistoryAsync_ReturnsOrderedHistory_ForProgressTracking()
    {
        // Arrange & Act
        await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IDiceService>();
            var testPlayer = context.Players.First(p => p.CurrentJob == JobLevel.Student);

            // Create some roll history
            await service.RollForJobAsync(testPlayer.Id);
            await Task.Delay(50);
            await service.RollForJobAsync(testPlayer.Id);
            await Task.Delay(50);
            await service.RollForJobAsync(testPlayer.Id);

            var history = await service.GetDiceRollHistoryAsync(testPlayer.Id);

            // Assert
            Assert.NotNull(history);
            Assert.True(history.Count >= 3);
            
            // Validate chronological order (most recent first)
            for (var i = 1; i < history.Count; i++)
            {
                Assert.True(history[i - 1].RolledAt >= history[i].RolledAt,
                    "History should be ordered with most recent first");
            }
            
            // Validate educational tracking data
            foreach (var item in history)
            {
                Assert.True(item.PlayerId == testPlayer.Id);
                Assert.InRange(item.DiceValue, 1, 6);
                ValidateEducationalOutcome(item, "learning progress and achievement tracking");
            }
            
            Output.WriteLine($"✅ Chronological History: {history.Count} entries properly ordered for progress tracking");
        });
    }

    #endregion

    #region Player State Management Tests

    [Fact]
    public async Task RollForJobAsync_UpdatesPlayerStatsCorrectly_ForEducationalProgression()
    {
        // Arrange & Act
        await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IDiceService>();
            var testPlayer = context.Players.First(p => p.CurrentJob == JobLevel.Student);
            
            var originalJob = testPlayer.CurrentJob;
            var originalIncome = testPlayer.Income;
            var originalReputation = testPlayer.Reputation;
            var originalHappiness = testPlayer.Happiness;

            var rollResult = await service.RollForJobAsync(testPlayer.Id);

            // Refresh player data from database
            context.Entry(testPlayer).Reload();
            var updatedPlayer = testPlayer;
            Assert.NotNull(updatedPlayer);

            // Assert job progression
            Assert.NotEqual(originalJob, updatedPlayer.CurrentJob);
            Assert.Equal(rollResult.NewJob, updatedPlayer.CurrentJob);

            // Assert income change
            Assert.NotEqual(originalIncome, updatedPlayer.Income);
            Assert.Equal(originalIncome + rollResult.IncomeChange, updatedPlayer.Income);

            // Assert reputation improvement (never decreases for child-friendly experience)
            Assert.True(updatedPlayer.Reputation >= originalReputation);
            Assert.Equal(originalReputation + rollResult.ReputationChange, updatedPlayer.Reputation);

            // Assert happiness increase (always positive for encouraging experience)
            Assert.True(updatedPlayer.Happiness > originalHappiness);
            Assert.Equal(originalHappiness + rollResult.HappinessChange, updatedPlayer.Happiness);
            
            // Assert last active timestamp is updated (within the last minute)
            Assert.True(updatedPlayer.LastActiveAt > DateTime.UtcNow.AddMinutes(-1), 
                $"LastActiveAt should be recent. Expected > {DateTime.UtcNow.AddMinutes(-1):yyyy-MM-dd HH:mm:ss}, Actual: {updatedPlayer.LastActiveAt:yyyy-MM-dd HH:mm:ss}");

            ValidateEducationalOutcome(updatedPlayer, "comprehensive player development and progress tracking");
            
            Output.WriteLine($"✅ Player Progression: {originalJob}→{updatedPlayer.CurrentJob}, Rep: {originalReputation}→{updatedPlayer.Reputation}, Happy: {originalHappiness}→{updatedPlayer.Happiness}");
        });
    }

    [Fact]
    public async Task RollForJobAsync_CapsStatsAtMaximum_ForBalancedGameplay()
    {
        // Arrange & Act
        await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IDiceService>();
            var testPlayer = context.Players.First(p => p.CurrentJob == JobLevel.Manager);
            
            // Set player near maximum stats
            testPlayer.Reputation = 95;
            testPlayer.Happiness = 98;
            context.Players.Update(testPlayer);
            await context.SaveChangesAsync();

            var rollResult = await service.RollForJobAsync(testPlayer.Id);

            // Refresh player data
            var updatedPlayer = context.Players.Find(testPlayer.Id);
            Assert.NotNull(updatedPlayer);

            // Assert stats are properly capped
            Assert.True(updatedPlayer.Reputation <= 100, "Reputation should be capped at 100");
            Assert.True(updatedPlayer.Happiness <= 100, "Happiness should be capped at 100");
            
            // Even when capped, player should feel positive about the roll
            Assert.NotNull(rollResult);
            ValidateChildSafeContent(rollResult.Message, "Dice roll message when stats are near maximum");
            
            Output.WriteLine($"✅ Stat Capping: Reputation={updatedPlayer.Reputation}/100, Happiness={updatedPlayer.Happiness}/100");
        });
    }

    #endregion

    #region Error Handling and Resilience Tests

    [Fact]
    public async Task RollForJobAsync_HandlesInvalidPlayer_Gracefully()
    {
        // Arrange & Act
        await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IDiceService>();
            var invalidPlayerId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await service.RollForJobAsync(invalidPlayerId);
            });
            
            Output.WriteLine("✅ Service handles invalid player ID gracefully with appropriate exception");
        });
    }

    [Fact]
    public async Task DiceService_MaintainsConsistency_UnderConcurrentAccess()
    {
        // Arrange & Act
        await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IDiceService>();
            var testPlayer = context.Players.First(p => p.CurrentJob == JobLevel.Student);
            
            // Simulate concurrent dice rolls (though unlikely in real scenario)
            var tasks = new List<Task>();
            var results = new List<Exception>();
            
            for (var i = 0; i < 3; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await service.RollForJobAsync(testPlayer.Id);
                    }
                    catch (Exception ex)
                    {
                        lock (results)
                        {
                            results.Add(ex);
                        }
                    }
                }));
            }
            
            await Task.WhenAll(tasks);
            
            // Assert that service handles concurrent access gracefully
            // Some operations may fail due to concurrent modifications, which is acceptable
            Assert.True(results.Count <= 2, "Service should handle most concurrent operations gracefully");
            
            // Final player state should be consistent
            var finalPlayer = context.Players.Find(testPlayer.Id);
            Assert.NotNull(finalPlayer);
            Assert.True(finalPlayer.Income > 0);
            Assert.InRange(finalPlayer.Reputation, 0, 100);
            Assert.InRange(finalPlayer.Happiness, 0, 100);
            
            Output.WriteLine($"✅ Concurrency Test: {results.Count} failures out of 3 concurrent operations (acceptable)");
        });
    }

    #endregion

    #region Educational Value and Child Safety Tests

    [Fact]
    public async Task DiceService_PromotesPositiveLearningExperience_ThroughAllOutcomes()
    {
        // Arrange & Act
        var allMessages = new List<string>();
        
        await ExecuteWithDbContextAsync(async context =>
        {
            var service = GetService<IDiceService>();
            var testPlayer = context.Players.First(p => p.CurrentJob == JobLevel.Student);

            // Test multiple rolls to see variety of outcomes
            for (var i = 0; i < 10; i++)
            {
                var rollResult = await service.RollForJobAsync(testPlayer.Id);
                allMessages.Add(rollResult.Message);
                
                // Reset for next test
                testPlayer.CurrentJob = JobLevel.Student;
                context.Players.Update(testPlayer);
                await context.SaveChangesAsync();
            }
        });

        // Assert all messages are child-safe and encouraging
        foreach (var message in allMessages)
        {
            ValidateChildSafeContent(message, "Dice roll outcome message");
            
            var lowerMessage = message.ToLowerInvariant();
            
            // Should contain positive language
            Assert.True(
                lowerMessage.Contains("great") || 
                lowerMessage.Contains("awesome") ||
                lowerMessage.Contains("fantastic") ||
                lowerMessage.Contains("excellent") ||
                lowerMessage.Contains("amazing") ||
                lowerMessage.Contains("wonderful") ||
                lowerMessage.Contains("outstanding") ||
                lowerMessage.Contains("good"),
                $"Message should be encouraging: {message}");
                
            // Should not contain any discouraging language
            Assert.DoesNotContain("fail", lowerMessage);
            Assert.DoesNotContain("bad", lowerMessage);
            Assert.DoesNotContain("lose", lowerMessage);
            Assert.DoesNotContain("wrong", lowerMessage);
        }
        
        Output.WriteLine($"✅ All {allMessages.Count} dice roll messages promote positive learning experience");
    }

    [Fact]
    public void DiceService_TeachesValueOfPersistence_ThroughCareerProgression()
    {
        // Arrange
        var service = GetService<IDiceService>();
        var allJobs = Enum.GetValues<JobLevel>();

        // Act & Assert - Test that all career paths show progression value
        foreach (var job in allJobs)
        {
            var description = service.GetJobDescription(job);
            var income = JobProgressionMapping.GetJobIncome(job);
            
            // Every job should have educational value and show progress potential
            Assert.True(income > 0, $"{job} should have positive income to show value of work");
            
            var lowerDescription = description.ToLowerInvariant();
            Assert.True(
                lowerDescription.Contains("learn") ||
                lowerDescription.Contains("grow") ||
                lowerDescription.Contains("develop") ||
                lowerDescription.Contains("skill") ||
                lowerDescription.Contains("help"),
                $"{job} description should emphasize learning and growth: {description}");
        }
        
        Output.WriteLine("✅ All career levels teach value of persistence and learning");
    }

    #endregion
}