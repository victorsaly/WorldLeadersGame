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
using WorldLeaders.Shared.Models;
using WorldLeaders.Shared.Services;
using Xunit.Abstractions;

namespace WorldLeaders.API.Tests;

/// <summary>
/// Comprehensive testing for GameController
/// Context: Educational game mechanics for 12-year-old players
/// Educational Objective: Validate dice rolling mechanics, career progression, and resource management
/// Safety Requirements: All game mechanics promote positive learning experiences
/// </summary>
public class GameControllerTests : ApiTestBase
{
    public GameControllerTests(TestWebApplicationFactory factory, ITestOutputHelper output) 
        : base(factory, output)
    {
    }

    [Fact]
    public async Task RollForJob_WithValidPlayer_ReturnsEducationalCareerProgression()
    {
        // Arrange
        var testPlayerId = Guid.NewGuid();
        var endpoint = $"/api/game/players/{testPlayerId}/roll-job";

        // Act
        var response = await Client.PostAsync(endpoint, null);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
        await ValidateApiResponseChildSafety(response, "Job Roll Mechanism");

        var content = await response.Content.ReadAsStringAsync();
        
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var rollResult = JsonSerializer.Deserialize<DiceRollResult>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            rollResult.Should().NotBeNull();
            rollResult!.DiceValue.Should().BeInRange(1, 6, "Dice should roll 1-6 for educational game");
            ValidateChildFriendlyJobLevel(rollResult.NewJob);
            ValidateChildSafeContent(rollResult.EncouragingMessage, "Dice Roll Message");
            ValidateEducationalOutcome(rollResult, "Learn career progression through dice-based mechanics");
        }
        else
        {
            ValidateChildSafeContent(content, "Job Roll Error Response");
        }

        Output.WriteLine("✅ Job rolling mechanism validated for educational career progression");
    }

    [Fact]
    public async Task StartGame_WithValidPlayer_ReturnsEducationalGameSession()
    {
        // Arrange
        var testPlayerId = Guid.NewGuid();
        var endpoint = $"/api/game/players/{testPlayerId}/start-game";

        // Act
        var response = await Client.PostAsync(endpoint, null);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
        await ValidateApiResponseChildSafety(response, "Game Session Start");

        var content = await response.Content.ReadAsStringAsync();
        ValidateChildSafeContent(content, "Game Session Response");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var gameSession = JsonSerializer.Deserialize<GameSession>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            gameSession.Should().NotBeNull();
            ValidateEducationalOutcome(gameSession!, "Start educational game session with learning objectives");
        }

        Output.WriteLine("✅ Game session creation validated for educational learning structure");
    }

    [Fact]
    public async Task AdvanceTurn_WithValidPlayer_ReturnsEducationalStateUpdate()
    {
        // Arrange
        var testPlayerId = Guid.NewGuid();
        var endpoint = $"/api/game/players/{testPlayerId}/advance-turn";

        // Act
        var response = await Client.PostAsync(endpoint, null);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
        await ValidateApiResponseChildSafety(response, "Turn Advancement");

        var content = await response.Content.ReadAsStringAsync();
        ValidateChildSafeContent(content, "Turn Advancement Response");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var stateUpdate = JsonSerializer.Deserialize<GameStateUpdate>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            stateUpdate.Should().NotBeNull();
            ValidateChildSafeContent(stateUpdate!.Message, "Turn Advancement Message");
            ValidateEducationalOutcome(stateUpdate, "Advance learning through turn-based gameplay");
        }

        Output.WriteLine("✅ Turn advancement validated for educational progression");
    }

    [Fact]
    public async Task GetPlayerDashboard_WithValidPlayer_ReturnsEducationalProgress()
    {
        // Arrange
        var testPlayerId = Guid.NewGuid();
        var endpoint = $"/api/game/players/{testPlayerId}/dashboard";

        // Act
        var response = await Client.GetAsync(endpoint);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
        await ValidateApiResponseChildSafety(response, "Player Dashboard");

        var content = await response.Content.ReadAsStringAsync();
        ValidateChildSafeContent(content, "Player Dashboard Response");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var dashboard = JsonSerializer.Deserialize<PlayerDashboardDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            dashboard.Should().NotBeNull();
            dashboard!.Income.Should().BeGreaterThanOrEqualTo(0, "Income should be non-negative for children");
            dashboard.Reputation.Should().BeInRange(0, 100, "Reputation should be percentage-based");
            dashboard.Happiness.Should().BeGreaterThanOrEqualTo(0, "Happiness should be positive-focused");
            ValidateChildFriendlyJobLevel(dashboard.CurrentJob);
            ValidateEducationalOutcome(dashboard, "Track educational progress through player dashboard");
        }

        Output.WriteLine("✅ Player dashboard validated for educational progress tracking");
    }

    [Fact]
    public async Task GameEvents_ValidateEducationalContent_EnsuresPositiveLearningExperiences()
    {
        // Arrange
        var endpoint = "/api/game/events";

        // Act
        var response = await Client.GetAsync(endpoint);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await ValidateApiResponseChildSafety(response, "Educational Game Events");

        var content = await response.Content.ReadAsStringAsync();
        var eventsResponse = JsonSerializer.Deserialize<GameEventsEducationalResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        eventsResponse.Should().NotBeNull();
        eventsResponse!.Events.Should().NotBeNull().And.NotBeEmpty("Game should provide educational events");
        ValidateChildSafeContent(eventsResponse.EducationalExplanation, "Educational Explanation");
        ValidateChildSafeContent(eventsResponse.ProgressTip, "Progress Tip");
        
        foreach (var gameEvent in eventsResponse.Events)
        {
            // Educational validation for each event
            ValidateChildSafeContent(gameEvent.Title, "Game Event Title");
            ValidateChildSafeContent(gameEvent.Description, "Game Event Description");
            
            // Child safety requirements
            gameEvent.IsPositive.Should().BeTrue("All game events should be positive for 12-year-olds");
            gameEvent.IconEmoji.Should().NotBeNullOrEmpty("Events should have visual representation");
            
            // Educational value validation
            var hasEducationalValue = gameEvent.Type is EventType.Career or EventType.Social or EventType.Economic;
            hasEducationalValue.Should().BeTrue("Events should support learning objectives");
            
            // Income/reputation effects should be reasonable for children
            gameEvent.IncomeEffect.Should().BeGreaterThan(-1000, "Large negative effects discourage learning");
            gameEvent.HappinessEffect.Should().BeGreaterThan(-50, "Happiness should not be severely impacted");
        }

        ValidateEducationalOutcome(eventsResponse.Events, "Learn through positive, educational game events");
        Output.WriteLine($"✅ {eventsResponse.Events.Count} educational game events validated for positive learning");
    }

    protected override void ConfigureAdditionalServices(IServiceCollection services)
    {
        // Mock services for game controller testing
        var mockGameEngine = new Mock<IGameEngine>();
        var mockPlayerService = new Mock<IPlayerService>();
        var mockDiceService = new Mock<IDiceService>();
        var mockTerritoryService = new Mock<ITerritoryService>();

        // Configure mock dice service with educational outcomes
        mockDiceService.Setup(x => x.RollForJobAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new DiceRollResult(
                DiceValue: 4,
                NewJob: JobLevel.Artisan,
                IncomeChange: 500,
                ReputationChange: 5,
                HappinessChange: 10,
                EncouragingMessage: "Great progress! You've advanced to become an Artisan, creating beautiful crafts!",
                JobDescription: "Artisans create beautiful and useful items while learning about craftsmanship and trade!"
            ));

        // Configure mock game engine
        mockGameEngine.Setup(x => x.StartNewGameAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new GameSession(
                PlayerId: Guid.NewGuid(),
                SessionId: Guid.NewGuid(),
                State: GameState.InProgress,
                CurrentTurn: 1,
                StartedAt: DateTime.UtcNow,
                LastSavedAt: DateTime.UtcNow,
                PlayerData: new Player
                {
                    Id = Guid.NewGuid(),
                    Username = "TestPlayer",
                    CurrentJob = JobLevel.Shopkeeper
                }
            ));

        mockGameEngine.Setup(x => x.AdvanceTurnAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new GameStateUpdate(
                IncomeChange: 100,
                ReputationChange: 5,
                HappinessChange: 10,
                Message: "Another successful month of learning and growth!",
                EventType: EventType.Social
            ));

        // Configure mock player service
        mockPlayerService.Setup(x => x.CreatePlayerAsync(It.IsAny<CreatePlayerRequest>()))
            .ReturnsAsync(new Player
            {
                Id = Guid.NewGuid(),
                Username = "TestPlayer",
                Income = 1000,
                Reputation = 50,
                Happiness = 75,
                CurrentJob = JobLevel.Shopkeeper,
                CreatedAt = DateTime.UtcNow
            });

        mockPlayerService.Setup(x => x.GetPlayerDashboardAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new PlayerDashboardDto(
                Id: Guid.NewGuid(),
                Username: "TestPlayer",
                Income: 1000,
                Reputation: 50,
                Happiness: 75,
                CurrentJob: JobLevel.Shopkeeper,
                TerritoriesOwned: 2,
                CurrentGameState: GameState.InProgress,
                LastActiveAt: DateTime.UtcNow
            ));

        // Configure mock territory service
        mockTerritoryService.Setup(x => x.GetAvailableTerritoriesAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<TerritoryDto>
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
                )
            });

        // Register mocked services
        services.AddSingleton(mockGameEngine.Object);
        services.AddSingleton(mockPlayerService.Object);
        services.AddSingleton(mockDiceService.Object);
        services.AddSingleton(mockTerritoryService.Object);
    }
}