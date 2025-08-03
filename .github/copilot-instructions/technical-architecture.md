# 🏗️ Technical Architecture - World Leaders Game

**Module Purpose**: .NET Aspire, Blazor, and technical implementation patterns for educational game development.

**Use This Module**: When implementing technical features, setting up infrastructure, or making architectural decisions.

---

## 🎯 Technology Stack

### Core Technologies
- **.NET 8 LTS** with **ASP.NET Core** - Stable foundation for educational applications
- **.NET Aspire** - Orchestration and service discovery for microservices
- **Blazor Server** - Interactive web UI with server-side rendering
- **TailwindCSS** - Utility-first styling with child-friendly design system
- **Entity Framework Core** with **PostgreSQL** - Robust data persistence
- **SignalR** - Real-time updates for engaging gameplay

### AI & External Services
- **Azure OpenAI Service** (GPT-4) - Educational AI agent personalities
- **Azure Speech Services** - Speech-to-text and pronunciation assessment
- **Azure Cognitive Services** - Content moderation for child safety
- **World Bank API** - Real GDP data for territory pricing
- **REST Countries API** - Country information and cultural data

## 🏛️ Project Structure

### Solution Architecture
```
src/
├── WorldLeaders.AppHost/           # .NET Aspire orchestration
│   ├── Program.cs                  # Service discovery and configuration
│   └── appsettings.json           # Environment-specific settings
├── WorldLeaders.Web/               # Blazor Server application
│   ├── Components/                 # Blazor components
│   │   ├── Game/                   # Game-specific UI components
│   │   ├── Shared/                 # Reusable UI components
│   │   └── Layout/                 # Application layout components
│   ├── Pages/                      # Blazor pages and routing
│   ├── Services/                   # Client-side service implementations
│   └── wwwroot/                    # Static assets (CSS, JS, images)
├── WorldLeaders.API/               # Game API services
│   ├── Controllers/                # REST API controllers
│   ├── Hubs/                       # SignalR hubs for real-time communication
│   └── Services/                   # Business logic services
├── WorldLeaders.Shared/            # Shared contracts and models
│   ├── Models/                     # Domain models and game entities
│   ├── DTOs/                       # Data transfer objects
│   ├── Enums/                      # Shared enumerations
│   └── Constants/                  # Application constants
└── WorldLeaders.Infrastructure/    # Data access and external services
    ├── Data/                       # Entity Framework context and configuration
    ├── Entities/                   # Database entity definitions
    ├── Services/                   # External service integrations
    └── Migrations/                 # Database schema migrations
```

## 💻 Development Guidelines

### Coding Standards (CRITICAL)
- **C# Conventions**: Follow Microsoft C# coding standards
- **Async/Await**: Use async patterns for all I/O operations
- **Dependency Injection**: Use built-in DI container exclusively
- **Error Handling**: Comprehensive try-catch with structured logging
- **Child Safety**: Always validate and sanitize user inputs

### Version Management Guidelines (CRITICAL)
```xml
<!-- CORRECT: LTS versions matching .NET 8 target framework -->
<TargetFramework>net8.0</TargetFramework>
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.8" />
<PackageReference Include="Microsoft.AspNetCore.SignalR.Client" Version="8.0.8" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.8" />

<!-- INCORRECT: Latest versions that may conflict with target framework -->
<TargetFramework>net8.0</TargetFramework>
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.7" />
<PackageReference Include="Microsoft.AspNetCore.SignalR.Client" Version="9.0.7" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.0" />
```

**Why LTS Versions Matter**:
- **Educational Stability**: Consistent behavior for students and teachers
- **Production Reliability**: Proven track record in enterprise environments
- **Long-term Support**: Extended support lifecycle and security updates
- **Deployment Confidence**: Reduced risk of version conflicts

## 🎮 Domain Models & Entities

### Core Game Entities
```csharp
public class Player
{
    public Guid Id { get; set; }
    public string Username { get; set; }  // No real names for child privacy
    public int Income { get; set; }       // Monthly earnings from job + territories
    public int Reputation { get; set; }   // 0-100% scale for territory purchases
    public int Happiness { get; set; }    // Population satisfaction 0-100%
    public JobLevel CurrentJob { get; set; }
    public List<Territory> OwnedTerritories { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }   // Soft delete for child data protection
}

public class Territory
{
    public Guid Id { get; set; }
    public string CountryName { get; set; }
    public string CountryCode { get; set; }  // ISO 3166-1 alpha-2
    public decimal GdpInBillions { get; set; }  // Real World Bank data
    public int Cost { get; set; }              // Calculated from GDP
    public int ReputationRequired { get; set; } // Required reputation %
    public List<string> OfficialLanguages { get; set; }  // For language learning
    public string CulturalContext { get; set; }  // Educational information
    public DateTime DataLastUpdated { get; set; }
}

public enum JobLevel
{
    Farmer = 1,         // Entry level, basic income
    Gardener = 2,       // Entry level, slightly higher income
    Shopkeeper = 3,     // Middle level, moderate income
    Artisan = 4,        // Middle level, higher moderate income
    Politician = 5,     // Leadership level, high income
    BusinessLeader = 6  // Leadership level, highest income
}

public enum AgentType
{
    CareerGuide,        // Encouraging mentor for job progression
    EventNarrator,      // Dramatic storyteller for random events
    FortuneTeller,      // Mystical advisor for strategic insights
    HappinessAdvisor,   // Caring diplomat for population management
    TerritoryStrategist, // Strategic advisor for expansion planning
    LanguageTutor       // Patient teacher for pronunciation practice
}
```

### Database Configuration
```csharp
public class GameDbContext : DbContext
{
    public DbSet<Player> Players { get; set; }
    public DbSet<Territory> Territories { get; set; }
    public DbSet<GameEvent> GameEvents { get; set; }
    public DbSet<AIConversation> AIConversations { get; set; }  // For safety monitoring

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Soft delete filter for child data protection
        modelBuilder.Entity<Player>().HasQueryFilter(p => !p.IsDeleted);
        
        // JSON column for complex data
        modelBuilder.Entity<Territory>()
            .Property(t => t.OfficialLanguages)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            );

        // Indexes for performance
        modelBuilder.Entity<Territory>().HasIndex(t => t.CountryCode).IsUnique();
        modelBuilder.Entity<Player>().HasIndex(p => p.Username).IsUnique();
    }
}
```

## 🔄 Service Layer Architecture

### AI Integration Pattern
```csharp
public interface IAIAgentService
{
    Task<AgentResponse> GenerateResponseAsync(
        AgentType agentType, 
        GameContext context, 
        string userInput);
    Task<bool> ValidateContentSafetyAsync(string content);
    Task<string> GetFallbackResponseAsync(AgentType agentType);
}

public class AIAgentService : IAIAgentService
{
    private readonly IOpenAIService _openAIService;
    private readonly IContentModerationService _contentModerator;
    private readonly ILogger<AIAgentService> _logger;

    public async Task<AgentResponse> GenerateResponseAsync(
        AgentType agentType, 
        GameContext context, 
        string userInput)
    {
        try
        {
            // Generate AI response with educational context
            var response = await _openAIService.GenerateResponseAsync(
                GetAgentPrompt(agentType), 
                context, 
                userInput);

            // Validate content safety for children
            var isAppropriate = await ValidateContentSafetyAsync(response.Content);
            
            if (isAppropriate)
            {
                await LogSuccessfulInteractionAsync(agentType, response);
                return response;
            }
            
            // Use safe fallback if content fails validation
            var fallbackContent = await GetFallbackResponseAsync(agentType);
            await LogContentModerationEventAsync(agentType, response.Content, fallbackContent);
            
            return new AgentResponse { Content = fallbackContent, IsGenerated = false };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI agent response generation failed for {AgentType}", agentType);
            return new AgentResponse 
            { 
                Content = await GetFallbackResponseAsync(agentType), 
                IsGenerated = false 
            };
        }
    }
}
```

### Real-Time Communication
```csharp
public class GameHub : Hub
{
    private readonly IGameStateService _gameStateService;
    private readonly ILogger<GameHub> _logger;

    public async Task JoinGameSession(string playerId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Player-{playerId}");
        _logger.LogInformation("Player {PlayerId} joined game session", playerId);
    }

    public async Task UpdateGameState(GameStateUpdate update)
    {
        // Validate update is appropriate for children
        if (await ValidateGameStateUpdateAsync(update))
        {
            await _gameStateService.UpdateGameStateAsync(update);
            await Clients.Group($"Player-{update.PlayerId}")
                .SendAsync("GameStateUpdated", update);
        }
    }

    private async Task<bool> ValidateGameStateUpdateAsync(GameStateUpdate update)
    {
        // Ensure all game state changes maintain child safety
        return await _contentModerator.ValidateGameContentAsync(update);
    }
}
```

## 🎨 Blazor Component Architecture

### Component Guidelines
```csharp
@using WorldLeaders.Shared.Models
@using WorldLeaders.Shared.Enums
@inherits ComponentBase
@inject IJSRuntime JSRuntime
@inject ILogger<GameComponent> Logger

@*
Context: Educational game component for 12-year-old players
Educational Goal: [Specific learning objective]
Child-UX: Large buttons, clear feedback, encouraging messages
*@

<div class="game-component p-6 bg-gradient-to-br from-purple-400 to-blue-500 rounded-lg shadow-lg">
    <h2 class="text-2xl font-bold text-white mb-4">@Title</h2>
    
    @if (IsLoading)
    {
        <div class="loading-spinner">
            <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-white"></div>
            <p class="text-white mt-2">Loading your adventure...</p>
        </div>
    }
    else
    {
        @ChildContent
    }
</div>

@code {
    [Parameter] public string Title { get; set; } = "";
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public EventCallback<string> OnAction { get; set; }
    
    private bool IsLoading { get; set; } = true;
    
    protected override async Task OnInitializedAsync()
    {
        // Simulate loading for smooth user experience
        await Task.Delay(500);
        IsLoading = false;
        StateHasChanged();
    }
    
    protected async Task HandleChildAction(string action)
    {
        // Log educational interactions
        Logger.LogInformation("Child performed action: {Action} in component: {Component}", 
            action, GetType().Name);
        
        // Notify parent component
        await OnAction.InvokeAsync(action);
    }
}
```

### Child-Friendly UI Patterns
```css
/* TailwindCSS configuration for child-friendly design */
.btn-child-friendly {
    @apply px-8 py-4 text-lg font-bold rounded-xl shadow-lg transform transition-all duration-300;
    @apply hover:scale-105 hover:shadow-xl active:scale-95;
    @apply bg-gradient-to-r from-blue-500 to-purple-600 text-white;
    @apply border-4 border-white border-opacity-20;
}

.educational-card {
    @apply bg-white rounded-2xl shadow-xl p-6 border-4 border-opacity-10;
    @apply transform transition-all duration-300 hover:scale-102;
    @apply border-gradient-to-r from-yellow-400 to-pink-400;
}

.progress-meter {
    @apply w-full bg-gray-200 rounded-full h-6 overflow-hidden;
    @apply border-2 border-gray-300 shadow-inner;
}

.progress-fill {
    @apply bg-gradient-to-r from-green-400 to-blue-500 h-full rounded-full;
    @apply transition-all duration-1000 ease-out;
    @apply shadow-lg;
}
```

## 📊 Performance Guidelines

### Optimization Targets
- **Page Load**: < 2 seconds initial load (critical for child engagement)
- **API Response**: < 500ms for most operations
- **AI Response**: < 3 seconds for agent interactions
- **Speech Processing**: < 2 seconds for pronunciation analysis

### Caching Strategy
```csharp
public class CachingService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;

    // Frequently accessed game data in memory
    public async Task<Territory[]> GetTerritoriesAsync()
    {
        return await _memoryCache.GetOrCreateAsync("territories", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
            return await _territoryService.GetAllTerritoriesAsync();
        });
    }

    // World Bank GDP data (24-hour TTL)
    public async Task<GDPData> GetGDPDataAsync(string countryCode)
    {
        var cacheKey = $"gdp-{countryCode}";
        var cachedData = await _distributedCache.GetStringAsync(cacheKey);
        
        if (cachedData != null)
        {
            return JsonSerializer.Deserialize<GDPData>(cachedData);
        }

        var gdpData = await _worldBankService.GetGDPDataAsync(countryCode);
        var serializedData = JsonSerializer.Serialize(gdpData);
        
        await _distributedCache.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
        });

        return gdpData;
    }
}
```

## 🧪 Testing Architecture

### Unit Testing Patterns
```csharp
[TestClass]
public class AIAgentServiceTests
{
    private Mock<IOpenAIService> _mockOpenAI;
    private Mock<IContentModerationService> _mockContentModerator;
    private AIAgentService _aiAgentService;

    [TestInitialize]
    public void Setup()
    {
        _mockOpenAI = new Mock<IOpenAIService>();
        _mockContentModerator = new Mock<IContentModerationService>();
        _aiAgentService = new AIAgentService(_mockOpenAI.Object, _mockContentModerator.Object);
    }

    [TestMethod]
    public async Task GenerateResponse_WithAppropriateContent_ReturnsOriginalResponse()
    {
        // Arrange
        var agentType = AgentType.CareerGuide;
        var context = new GameContext { PlayerAge = 12 };
        var userInput = "I want to be a teacher";
        var expectedResponse = "Teaching is a wonderful career that helps others learn!";

        _mockOpenAI.Setup(x => x.GenerateResponseAsync(It.IsAny<string>(), context, userInput))
            .ReturnsAsync(new AgentResponse { Content = expectedResponse });
        
        _mockContentModerator.Setup(x => x.ValidateAsync(expectedResponse))
            .ReturnsAsync(true);

        // Act
        var result = await _aiAgentService.GenerateResponseAsync(agentType, context, userInput);

        // Assert
        Assert.AreEqual(expectedResponse, result.Content);
        Assert.IsTrue(result.IsGenerated);
    }

    [TestMethod]
    public async Task GenerateResponse_WithInappropriateContent_ReturnsFallbackResponse()
    {
        // Arrange
        var agentType = AgentType.CareerGuide;
        var context = new GameContext { PlayerAge = 12 };
        var userInput = "I want to be a teacher";
        var inappropriateResponse = "Some inappropriate content";
        var fallbackResponse = "Every career offers great learning opportunities!";

        _mockOpenAI.Setup(x => x.GenerateResponseAsync(It.IsAny<string>(), context, userInput))
            .ReturnsAsync(new AgentResponse { Content = inappropriateResponse });
        
        _mockContentModerator.Setup(x => x.ValidateAsync(inappropriateResponse))
            .ReturnsAsync(false);

        // Act
        var result = await _aiAgentService.GenerateResponseAsync(agentType, context, userInput);

        // Assert
        Assert.AreEqual(fallbackResponse, result.Content);
        Assert.IsFalse(result.IsGenerated);
    }
}
```

## 📚 Cross-Module Relationships

### This Module Connects To:
- **[core-principles.md](./core-principles.md)**: LTS-first development and child safety principles
- **[educational-game-development.md](./educational-game-development.md)**: Game mechanics implementation
- **[ai-safety-and-child-protection.md](./ai-safety-and-child-protection.md)**: Content moderation integration
- **[ui-ux-guidelines.md](./ui-ux-guidelines.md)**: Child-friendly Blazor component design

### Architecture Pattern:
```
Core Principles
↓
Technical Architecture (this module)
↓
+ Educational Game Development + AI Safety
↓ 
= Safe, scalable, educational game platform
```

---

**Remember**: Technical excellence serves educational outcomes. Every architectural decision should support safe, engaging learning experiences for 12-year-old students.