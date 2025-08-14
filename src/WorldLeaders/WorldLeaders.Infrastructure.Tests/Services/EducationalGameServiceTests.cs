using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using WorldLeaders.Infrastructure.Data;
using WorldLeaders.Infrastructure.Entities;
using WorldLeaders.Infrastructure.Services;
using WorldLeaders.Infrastructure.Tests.Infrastructure;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Services;
using Xunit.Abstractions;

namespace WorldLeaders.Infrastructure.Tests.Services;

/// <summary>
/// Working educational game service tests aligned with actual codebase
/// Context: Educational game component for 12-year-old geography, economics, and language learning
/// Educational Objective: Validate core educational game mechanics work reliably
/// Safety Requirements: Age-appropriate content, positive messaging, cultural sensitivity
/// </summary>
public class EducationalGameServiceTests : ServiceTestBase
{
    public EducationalGameServiceTests(ITestOutputHelper output) : base(output)
    {
    }

    protected override void ConfigureAdditionalServices(IServiceCollection services)
    {
        // Configure test services with educational focus
        services.AddSingleton<ILogger<DiceService>>(Mock.Of<ILogger<DiceService>>());
        services.AddScoped<IDiceService, DiceService>();
    }

    protected override async Task SeedTestDataAsync(WorldLeadersDbContext dbContext)
    {
        // Create test player with educational profile
        var testPlayer = new PlayerEntity
        {
            Id = Guid.NewGuid(),
            Username = "educational_test_student",
            Income = 1000,
            Reputation = 25,
            Happiness = 80,
            CurrentJob = JobLevel.Farmer, // Starting job level
            CurrentGameState = GameState.InProgress,
            GameStartedAt = DateTime.UtcNow.AddDays(-7),
            LastActiveAt = DateTime.UtcNow.AddMinutes(-30),
            CreatedAt = DateTime.UtcNow.AddDays(-7)
        };

        dbContext.Players.Add(testPlayer);
        await dbContext.SaveChangesAsync();
    }

    #region Educational Dice Service Tests

    [Fact]
    public async Task DiceService_RollForJob_GeneratesValidEducationalProgression()
    {
        // Arrange & Act
        await ExecuteWithDbContextAsync(async context =>
        {
            var diceService = GetService<IDiceService>();
            var testPlayer = context.Players.First();

            // Perform dice roll for career progression
            var rollResult = await diceService.RollForJobAsync(testPlayer.Id);

            // Assert educational outcomes
            Assert.NotNull(rollResult);
            Assert.InRange(rollResult.DiceValue, 1, 6);
            Assert.True(rollResult.IncomeChange != 0, "Career progression should affect income");
            Assert.True(rollResult.ReputationChange >= 0, "Reputation should never decrease for positive experience");
            Assert.True(rollResult.HappinessChange > 0, "Happiness should always increase to encourage learning");

            // Validate job progression is educational
            Assert.NotEqual(testPlayer.CurrentJob, rollResult.NewJob);
            
            // Also test ValidateEducationalOutcome with proper types
            ValidateEducationalOutcome(rollResult.NewJob, "career progression learning");
            ValidateEducationalOutcome(rollResult.EncouragingMessage, "positive messaging");
            ValidateEducationalOutcome(rollResult.JobDescription, "career awareness");

            // Validate encouraging message
            Assert.NotEmpty(rollResult.EncouragingMessage);
            ValidateChildSafeContent(rollResult.EncouragingMessage, "Career progression message");

            // Verify no negative language in dice outcomes
            var lowerMessage = rollResult.EncouragingMessage.ToLowerInvariant();
            Assert.DoesNotContain("fail", lowerMessage);
            Assert.DoesNotContain("bad", lowerMessage);
            Assert.DoesNotContain("lose", lowerMessage);

            Output.WriteLine($"✅ Educational Dice Roll: {testPlayer.CurrentJob} → {rollResult.NewJob}");
            Output.WriteLine($"   Dice: {rollResult.DiceValue}, Message: {rollResult.EncouragingMessage}");
            Output.WriteLine($"   Income: {rollResult.IncomeChange:+#;-#;0}, Reputation: {rollResult.ReputationChange:+#;-#;0}, Happiness: {rollResult.HappinessChange:+#;-#;0}");
        });
    }

    [Fact]
    public async Task DiceService_ProvidesConsistentEducationalExperience_AcrossMultipleRolls()
    {
        // Arrange & Act
        var rollResults = new List<string>();

        await ExecuteWithDbContextAsync(async context =>
        {
            var diceService = GetService<IDiceService>();
            var testPlayer = context.Players.First();

            // Perform multiple rolls to test consistency
            for (int i = 0; i < 5; i++)
            {
                var rollResult = await diceService.RollForJobAsync(testPlayer.Id);
                rollResults.Add(rollResult.EncouragingMessage);

                // Reset player for next test
                testPlayer.CurrentJob = JobLevel.Farmer;
                testPlayer.Happiness = 80;
                testPlayer.Reputation = 25;
                context.Players.Update(testPlayer);
                await context.SaveChangesAsync();
            }
        });

        // Assert all messages are educational and child-safe
        Assert.All(rollResults, message =>
        {
            Assert.NotEmpty(message);
            ValidateChildSafeContent(message, "Dice roll message");

            // All messages should be encouraging
            var lowerMessage = message.ToLowerInvariant();
            Assert.True(
                lowerMessage.Contains("great") ||
                lowerMessage.Contains("awesome") ||
                lowerMessage.Contains("fantastic") ||
                lowerMessage.Contains("excellent") ||
                lowerMessage.Contains("wonderful"),
                $"Message should be encouraging: {message}");
        });

        Output.WriteLine($"✅ Consistent Educational Experience: {rollResults.Count} encouraging messages validated");
    }

    [Fact]
    public void DiceService_JobDescriptions_ProvideEducationalCareerInformation()
    {
        // Arrange
        var diceService = GetService<IDiceService>();
        var allJobLevels = Enum.GetValues<JobLevel>();

        // Act & Assert
        foreach (var jobLevel in allJobLevels)
        {
            var description = diceService.GetJobDescription(jobLevel);

            Assert.NotEmpty(description);
            ValidateChildSafeContent(description, $"Job description for {jobLevel}");
            ValidateEducationalOutcome(description, "career awareness and exploration");

            // Should contain educational career information
            var lowerDescription = description.ToLowerInvariant();
            Assert.True(
                lowerDescription.Contains("help") ||
                lowerDescription.Contains("work") ||
                lowerDescription.Contains("skill") ||
                lowerDescription.Contains("learn") ||
                lowerDescription.Contains("grow"),
                $"Job description should be educational for {jobLevel}: {description}");

            Output.WriteLine($"✅ {jobLevel}: {description}");
        }
    }

    [Fact]
    public void DiceService_EncouragingMessages_ArePositiveForAllOutcomes()
    {
        // Arrange
        var diceService = GetService<IDiceService>();
        var jobLevels = Enum.GetValues<JobLevel>();

        // Act & Assert
        foreach (int diceValue in Enumerable.Range(1, 6))
        {
            foreach (var jobLevel in jobLevels)
            {
                var message = diceService.GetEncouragingMessage(diceValue, jobLevel);

                Assert.NotEmpty(message);
                ValidateChildSafeContent(message, $"Encouraging message for dice {diceValue}, job {jobLevel}");

                // Must be encouraging and positive
                var lowerMessage = message.ToLowerInvariant();
                Assert.True(
                    lowerMessage.Contains("great") ||
                    lowerMessage.Contains("awesome") ||
                    lowerMessage.Contains("fantastic") ||
                    lowerMessage.Contains("excellent") ||
                    lowerMessage.Contains("wonderful") ||
                    lowerMessage.Contains("amazing"),
                    $"Message must be encouraging for dice {diceValue}, job {jobLevel}: {message}");

                // Should never contain negative words
                Assert.DoesNotContain("bad", lowerMessage);
                Assert.DoesNotContain("fail", lowerMessage);
                Assert.DoesNotContain("wrong", lowerMessage);
                Assert.DoesNotContain("terrible", lowerMessage);
            }
        }

        Output.WriteLine("✅ All dice outcomes provide positive, encouraging messages");
    }

    #endregion

    #region Educational Data Validation Tests

    [Fact]
    public async Task GameEntities_MaintainEducationalIntegrity()
    {
        // Arrange & Act
        await ExecuteWithDbContextAsync(async context =>
        {
            var testPlayer = context.Players.First();

            // Assert player entity maintains educational data integrity
            Assert.True(testPlayer.Income > 0, "Players should always have positive income for motivation");
            Assert.InRange(testPlayer.Reputation, 0, 100);
            Assert.InRange(testPlayer.Happiness, 0, 100);
            Assert.NotEmpty(testPlayer.Username);

            // Validate educational state progression
            Assert.True(Enum.IsDefined<JobLevel>(testPlayer.CurrentJob), "Job level should be valid educational career");
            Assert.True(Enum.IsDefined<GameState>(testPlayer.CurrentGameState), "Game state should be valid");

            ValidateEducationalOutcome(testPlayer, "player progress tracking and educational motivation");

            Output.WriteLine($"✅ Educational Player Data: {testPlayer.Username}");
            Output.WriteLine($"   Job: {testPlayer.CurrentJob}, Income: {testPlayer.Income}");
            Output.WriteLine($"   Reputation: {testPlayer.Reputation}, Happiness: {testPlayer.Happiness}");
        });
    }

    #endregion

    #region Performance and Reliability Tests

    [Fact]
    public async Task EducationalServices_RespondWithinChildFriendlyTimeframes()
    {
        // Arrange
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        await ExecuteWithDbContextAsync(async context =>
        {
            var diceService = GetService<IDiceService>();
            var testPlayer = context.Players.First();

            await diceService.RollForJobAsync(testPlayer.Id);
        });

        stopwatch.Stop();

        // Assert
        Assert.True(stopwatch.ElapsedMilliseconds < 500,
            $"Educational services should respond within 500ms for child-friendly experience. Actual: {stopwatch.ElapsedMilliseconds}ms");

        Output.WriteLine($"✅ Performance: Educational service responded in {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task EducationalServices_HandleErrorsGracefully()
    {
        // Arrange & Act
        await ExecuteWithDbContextAsync(async context =>
        {
            var diceService = GetService<IDiceService>();
            var invalidPlayerId = Guid.NewGuid();

            // Test error handling
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await diceService.RollForJobAsync(invalidPlayerId);
            });

            // Assert error handling is child-safe
            Assert.NotNull(exception);
            Assert.NotEmpty(exception.Message);

            Output.WriteLine($"✅ Graceful Error Handling: {exception.GetType().Name}");
        });
    }

    #endregion

    #region Educational Value Validation Tests

    [Fact]
    public async Task EducationalGameMechanics_TeachRealWorldConcepts()
    {
        // Arrange & Act
        await ExecuteWithDbContextAsync(async context =>
        {
            var diceService = GetService<IDiceService>();
            var testPlayer = context.Players.First();

            // Test multiple career progressions
            var progressions = new List<(JobLevel From, JobLevel To, int Income)>();

            for (int i = 0; i < 3; i++)
            {
                var originalJob = testPlayer.CurrentJob;
                var originalIncome = testPlayer.Income;

                var rollResult = await diceService.RollForJobAsync(testPlayer.Id);

                // Refresh player
                var updatedPlayer = context.Players.Find(testPlayer.Id);
                Assert.NotNull(updatedPlayer);

                progressions.Add((originalJob, updatedPlayer.CurrentJob, updatedPlayer.Income));

                // Reset for next test
                testPlayer.CurrentJob = JobLevel.Farmer;
                testPlayer.Income = 1000;
                context.Players.Update(testPlayer);
                await context.SaveChangesAsync();
            }

            // Assert educational value
            Assert.All(progressions, progression =>
            {
                Assert.True(progression.Income > 0, "All jobs should provide positive income for economic education");
                ValidateEducationalOutcome(progression, "career progression and economic understanding");
            });

            Output.WriteLine("✅ Educational Game Mechanics:");
            foreach (var (from, to, income) in progressions)
            {
                Output.WriteLine($"   {from} → {to} (Income: {income})");
            }
        });
    }

    #endregion
}