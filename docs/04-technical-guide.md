# World Leaders Game - Technical Implementation Guide

## 🏗️ Project Architecture

### Solution Structure
```
WorldLeadersGame.sln
├── src/
│   ├── WorldLeaders.AppHost/           # .NET Aspire Orchestration
│   ├── WorldLeaders.Web/               # Blazor Server Application
│   ├── WorldLeaders.API/               # Game API Services
│   ├── WorldLeaders.Shared/            # Shared Models & Contracts
│   └── WorldLeaders.Infrastructure/    # Data Access & External Services
├── tests/
│   ├── WorldLeaders.Tests.Unit/
│   └── WorldLeaders.Tests.Integration/
└── docs/
    ├── api/
    └── deployment/
```

### Technology Stack Details

#### Backend Services
- **.NET 8**: Latest LTS version for performance and features
- **ASP.NET Core**: Web API and Blazor Server hosting
- **.NET Aspire**: Orchestration and service discovery
- **Entity Framework Core**: Database ORM with PostgreSQL
- **SignalR**: Real-time communication
- **MediatR**: CQRS pattern implementation

#### AI & Speech Services
- **Azure OpenAI Service**: GPT-4 for AI agents
- **Azure Speech Services**: Speech-to-text and pronunciation assessment
- **Azure Cognitive Services**: Content moderation
- **LangChain.NET**: AI workflow orchestration

#### Real-World Data Integration
- **World Bank API**: GDP data for all countries
- **REST Countries API**: Country information and flags
- **Exchange Rates API**: Currency conversion for territory costs
- **Geographic Data**: Country boundaries and regions

#### Frontend Technologies
- **Blazor Server**: Interactive web UI with C#
- **TailwindCSS**: Utility-first CSS framework
- **Chart.js**: Data visualization for progress tracking
- **Howler.js**: Audio management for game sounds

## 🔧 Core Implementation Components

### 1. Game Engine Core

```csharp
// Game State Management
public class GameEngine : IGameEngine
{
    private readonly IGameStateRepository _gameStateRepository;
    private readonly IHubContext<GameHub> _hubContext;
    private readonly ILogger<GameEngine> _logger;

    public async Task<GameResult> ProcessTurnAsync(Guid gameId, PlayerAction action)
    {
        var gameState = await _gameStateRepository.GetAsync(gameId);
        var result = await ExecutePhase(gameState, action);
        
        await _gameStateRepository.UpdateAsync(gameState);
        await NotifyClientsAsync(gameId, result);
        
        return result;
    }
}

// Player Profile Management
public class PlayerProfile
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Income { get; set; }
    public int Reputation { get; set; }
    public int Happiness { get; set; }
    public JobLevel CurrentJob { get; set; }
    public List<Territory> OwnedTerritories { get; set; }
    public LanguageLearningProgress Languages { get; set; }
}
```

### 2. AI Agent Framework

```csharp
// Base AI Agent Interface
public interface IAIAgent
{
    AgentType Type { get; }
    string Personality { get; }
    Task<AgentResponse> GenerateResponseAsync(AgentContext context);
    Task<bool> ValidateResponseAsync(string response);
}

// AI Agent Service Implementation
public class AIAgentService : IAIAgentService
{
    private readonly OpenAIClient _openAIClient;
    private readonly IContentModerator _contentModerator;
    private readonly Dictionary<AgentType, IAIAgent> _agents;

    public async Task<AgentResponse> GetAgentResponseAsync(
        AgentType agentType, 
        GameContext gameContext, 
        string userInput)
    {
        var agent = _agents[agentType];
        var context = BuildAgentContext(gameContext, userInput);
        
        var response = await agent.GenerateResponseAsync(context);
        var isAppropriate = await _contentModerator.ValidateContentAsync(response.Content);
        
        return isAppropriate ? response : GetFallbackResponse(agentType);
    }
}

// Specific Agent Implementation
public class CareerGuideAgent : IAIAgent
{
    public AgentType Type => AgentType.CareerGuide;
    public string Personality => "Encouraging and supportive mentor";

    public async Task<AgentResponse> GenerateResponseAsync(AgentContext context)
    {
        var prompt = $@"
            You are a career guide helping a 12-year-old player in a strategy game.
            Current situation: {context.GameState}
            Player input: {context.UserInput}
            
            Respond as an encouraging mentor who:
            - Celebrates achievements
            - Explains job benefits simply
            - Provides strategic career advice
            - Uses age-appropriate language
            - Maintains excitement about progress
        ";

        return await CallOpenAIAsync(prompt);
    }
}
```

### 3. Speech Recognition & Analysis

```csharp
// Speech Service Integration
public class SpeechAnalysisService : ISpeechAnalysisService
{
    private readonly SpeechConfig _speechConfig;
    private readonly ILogger<SpeechAnalysisService> _logger;

    public async Task<PronunciationResult> AnalyzePronunciationAsync(
        byte[] audioData, 
        string targetText, 
        string language)
    {
        using var audioStream = new MemoryStream(audioData);
        using var audioConfig = AudioConfig.FromStreamInput(audioStream);
        using var recognizer = new SpeechRecognizer(_speechConfig, language, audioConfig);

        var pronunciationConfig = new PronunciationAssessmentConfig(
            targetText,
            GradingSystem.HundredMark,
            Granularity.Phoneme);

        pronunciationConfig.ApplyTo(recognizer);

        var result = await recognizer.RecognizeOnceAsync();
        return ProcessPronunciationResult(result);
    }

    private PronunciationResult ProcessPronunciationResult(SpeechRecognitionResult result)
    {
        var pronunciationResult = PronunciationAssessmentResult.FromResult(result);
        
        return new PronunciationResult
        {
            AccuracyScore = pronunciationResult.AccuracyScore,
            FluencyScore = pronunciationResult.FluencyScore,
            CompletenessScore = pronunciationResult.CompletenessScore,
            PronScore = pronunciationResult.PronScore,
            WordDetails = ExtractWordDetails(pronunciationResult)
        };
    }
}
```

### 4. Blazor Components

```csharp
// Game Dashboard Component
@page "/game/{GameId:guid}"
@inject IGameEngine GameEngine
@inject IAIAgentService AIAgentService
@inject IJSRuntime JSRuntime
@implements IAsyncDisposable

<div class="min-h-screen bg-gradient-to-br from-blue-900 via-purple-900 to-indigo-900">
    <div class="container mx-auto px-4 py-8">
        <!-- Player Stats -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
            <StatCard Title="Income" Value="@gameState.Player.Income" Icon="💰" Color="green" />
            <StatCard Title="Reputation" Value="@gameState.Player.Reputation" Icon="⭐" Color="yellow" />
            <StatCard Title="Happiness" Value="@gameState.Player.Happiness" Icon="😊" Color="pink" />
        </div>

        <!-- Current Phase -->
        <div class="bg-white rounded-xl shadow-2xl p-8 mb-8">
            <h2 class="text-3xl font-bold text-gray-800 mb-4">@GetPhaseTitle()</h2>
            
            @switch (gameState.CurrentPhase)
            {
                case GamePhase.CareerRoll:
                    <DiceRollComponent OnRollComplete="HandleCareerRoll" />
                    break;
                case GamePhase.RandomEvent:
                    <EventCardComponent Event="@currentEvent" OnEventResolved="HandleEventResolution" />
                    break;
                case GamePhase.FortuneTelling:
                    <FortuneTellerComponent GameState="@gameState" OnInsightReceived="HandleFortune" />
                    break;
                case GamePhase.LanguageLearning:
                    <LanguageChallengeComponent OnChallengeComplete="HandleLanguageChallenge" />
                    break;
            }
        </div>

        <!-- AI Agent Chat -->
        <AIAgentChat AgentType="@GetCurrentAgent()" GameContext="@gameState" />
    </div>
</div>

@code {
    [Parameter] public Guid GameId { get; set; }
    
    private GameState gameState = new();
    private GameEvent? currentEvent;
    private HubConnection? hubConnection;

    protected override async Task OnInitializedAsync()
    {
        gameState = await GameEngine.GetGameStateAsync(GameId);
        await SetupSignalRConnection();
    }

    private async Task HandleCareerRoll(int rollResult)
    {
        var action = new CareerRollAction { DiceResult = rollResult };
        await GameEngine.ProcessTurnAsync(GameId, action);
    }

    private async Task SetupSignalRConnection()
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl("/gameHub")
            .Build();

        hubConnection.On<GameState>("GameStateUpdated", async (updatedState) =>
        {
            gameState = updatedState;
            await InvokeAsync(StateHasChanged);
        });

        await hubConnection.StartAsync();
        await hubConnection.SendAsync("JoinGame", GameId);
    }
}
```

```csharp
// Speech Recording Component
@inject IJSRuntime JSRuntime
@inject ISpeechAnalysisService SpeechService

<div class="bg-white rounded-lg p-6 shadow-lg">
    <h3 class="text-xl font-bold mb-4">🎤 Pronunciation Practice</h3>
    
    <div class="mb-4">
        <p class="text-lg font-medium text-gray-700">Say: "<span class="text-blue-600">@TargetPhrase</span>"</p>
    </div>

    <div class="flex items-center justify-center space-x-4 mb-6">
        <button @onclick="StartRecording" 
                disabled="@isRecording"
                class="@GetRecordButtonClass()">
            @if (isRecording)
            {
                <span class="animate-pulse">🔴 Recording...</span>
            }
            else
            {
                <span>🎤 Start Recording</span>
            }
        </button>

        @if (isRecording)
        {
            <button @onclick="StopRecording" 
                    class="bg-red-500 hover:bg-red-600 text-white px-6 py-3 rounded-lg">
                ⏹️ Stop
            </button>
        }
    </div>

    @if (pronunciationResult != null)
    {
        <div class="border-t pt-4">
            <h4 class="font-bold mb-2">Your Results:</h4>
            <div class="grid grid-cols-2 gap-4">
                <ScoreCard Title="Accuracy" Score="@pronunciationResult.AccuracyScore" />
                <ScoreCard Title="Fluency" Score="@pronunciationResult.FluencyScore" />
            </div>
            
            <div class="mt-4">
                <h5 class="font-medium mb-2">Word Analysis:</h5>
                @foreach (var word in pronunciationResult.WordDetails)
                {
                    <span class="inline-block px-3 py-1 rounded-full text-sm mr-2 mb-2 @GetWordClass(word.AccuracyScore)">
                        @word.Word (@word.AccuracyScore%)
                    </span>
                }
            </div>
        </div>
    }
</div>

@code {
    [Parameter] public string TargetPhrase { get; set; } = "";
    [Parameter] public string Language { get; set; } = "en-US";
    [Parameter] public EventCallback<PronunciationResult> OnAnalysisComplete { get; set; }

    private bool isRecording = false;
    private PronunciationResult? pronunciationResult;

    private async Task StartRecording()
    {
        isRecording = true;
        await JSRuntime.InvokeVoidAsync("startRecording");
    }

    private async Task StopRecording()
    {
        isRecording = false;
        var audioData = await JSRuntime.InvokeAsync<byte[]>("stopRecording");
        
        pronunciationResult = await SpeechService.AnalyzePronunciationAsync(
            audioData, TargetPhrase, Language);
            
        await OnAnalysisComplete.InvokeAsync(pronunciationResult);
    }

    private string GetWordClass(double accuracy) => accuracy switch
    {
        >= 80 => "bg-green-200 text-green-800",
        >= 60 => "bg-yellow-200 text-yellow-800",
        _ => "bg-red-200 text-red-800"
    };
}
```

### 5. Database Models

```csharp
// Entity Framework Models
public class GameStateEntity
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public GamePhase CurrentPhase { get; set; }
    public string GameData { get; set; } // JSON serialized game state
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public PlayerEntity Player { get; set; }
    public List<GameEventEntity> Events { get; set; }
}

public class PlayerEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Income { get; set; }
    public int Reputation { get; set; }
    public int Happiness { get; set; }
    public JobLevel CurrentJob { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<TerritoryOwnershipEntity> OwnedTerritories { get; set; }
    public List<LanguageProgressEntity> LanguageProgress { get; set; }
}

public class TerritoryEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int RequiredReputation { get; set; }
    public decimal Cost { get; set; }
    public int IncomeBonus { get; set; }
    public string Region { get; set; }
    public string ImageUrl { get; set; }
    public string CountryCode { get; set; } // ISO 3166-1 alpha-2
    public decimal GDPValue { get; set; } // Real GDP data from World Bank
    public int Population { get; set; }
    public string Currency { get; set; }
    public string[] Languages { get; set; } // Languages spoken in this territory
    public int DifficultyLevel { get; set; } // 1-6 based on GDP and strategic value
    public bool IsUnlocked { get; set; }
    public string FlagUrl { get; set; }
}

// Real-World Data Service
public interface IWorldDataService
{
    Task<List<CountryGDPData>> GetGDPDataAsync();
    Task<CountryInfo> GetCountryInfoAsync(string countryCode);
    Task<decimal> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency);
    Task RefreshWorldDataAsync(); // Updates GDP data periodically
}

public class CountryGDPData
{
    public string CountryCode { get; set; }
    public string CountryName { get; set; }
    public decimal GDPNominal { get; set; } // In USD
    public decimal GDPPerCapita { get; set; }
    public int Population { get; set; }
    public int WorldRank { get; set; } // GDP ranking
    public string[] OfficialLanguages { get; set; }
    public string Currency { get; set; }
    public string Region { get; set; }
    public string FlagUrl { get; set; }
}

// Database Context
public class GameDbContext : DbContext
{
    public DbSet<GameStateEntity> GameStates { get; set; }
    public DbSet<PlayerEntity> Players { get; set; }
    public DbSet<TerritoryEntity> Territories { get; set; }
    public DbSet<GameEventEntity> GameEvents { get; set; }
    public DbSet<LanguageProgressEntity> LanguageProgress { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerEntity>()
            .HasMany(p => p.OwnedTerritories)
            .WithOne(t => t.Player)
            .HasForeignKey(t => t.PlayerId);

        modelBuilder.Entity<GameEventEntity>()
            .Property(e => e.EventType)
            .HasConversion<string>();

        // Seed initial territories and events
        SeedInitialData(modelBuilder);
    }

    private void SeedInitialData(ModelBuilder modelBuilder)
    {
        // Seed territories based on real GDP data
        var territories = new[]
        {
            // Tier 1: Lower GDP countries (easier to acquire)
            new TerritoryEntity 
            { 
                Id = Guid.NewGuid(), Name = "Nepal", CountryCode = "NP", 
                GDPValue = 36290000000m, RequiredReputation = 10, Cost = 5000m,
                IncomeBonus = 500, DifficultyLevel = 1, Region = "Asia"
            },
            new TerritoryEntity 
            { 
                Id = Guid.NewGuid(), Name = "Jamaica", CountryCode = "JM", 
                GDPValue = 15720000000m, RequiredReputation = 15, Cost = 8000m,
                IncomeBonus = 750, DifficultyLevel = 2, Region = "Caribbean"
            },
            
            // Tier 2: Medium GDP countries
            new TerritoryEntity 
            { 
                Id = Guid.NewGuid(), Name = "Canada", CountryCode = "CA", 
                GDPValue = 2140000000000m, RequiredReputation = 40, Cost = 50000m,
                IncomeBonus = 5000, DifficultyLevel = 4, Region = "North America"
            },
            new TerritoryEntity 
            { 
                Id = Guid.NewGuid(), Name = "Australia", CountryCode = "AU", 
                GDPValue = 1550000000000m, RequiredReputation = 45, Cost = 45000m,
                IncomeBonus = 4500, DifficultyLevel = 4, Region = "Oceania"
            },
            
            // Tier 3: High GDP countries (hardest to acquire)
            new TerritoryEntity 
            { 
                Id = Guid.NewGuid(), Name = "United States", CountryCode = "US", 
                GDPValue = 25460000000000m, RequiredReputation = 85, Cost = 200000m,
                IncomeBonus = 15000, DifficultyLevel = 6, Region = "North America"
            },
            new TerritoryEntity 
            { 
                Id = Guid.NewGuid(), Name = "China", CountryCode = "CN", 
                GDPValue = 17730000000000m, RequiredReputation = 80, Cost = 150000m,
                IncomeBonus = 12000, DifficultyLevel = 6, Region = "Asia"
            },
            new TerritoryEntity 
            { 
                Id = Guid.NewGuid(), Name = "Germany", CountryCode = "DE", 
                GDPValue = 4260000000000m, RequiredReputation = 60, Cost = 75000m,
                IncomeBonus = 7500, DifficultyLevel = 5, Region = "Europe"
            }
        };

        modelBuilder.Entity<TerritoryEntity>().HasData(territories);
    }
}
```

### 6. Aspire Orchestration

```csharp
// Program.cs in AppHost
var builder = DistributedApplication.CreateBuilder(args);

// Add services
var postgres = builder.AddPostgres("postgres")
                     .WithDataVolume()
                     .WithPgAdmin();

var gameDb = postgres.AddDatabase("gamedb");

var redis = builder.AddRedis("redis")
                  .WithDataVolume();

// Add OpenAI service
var openai = builder.AddConnectionString("openai");

// Add Azure Speech service
var speech = builder.AddConnectionString("speech");

// Add the main web application
var web = builder.AddProject<Projects.WorldLeaders_Web>("web")
                .WithReference(gameDb)
                .WithReference(redis)
                .WithReference(openai)
                .WithReference(speech)
                .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

// Add API service
var api = builder.AddProject<Projects.WorldLeaders_API>("api")
                .WithReference(gameDb)
                .WithReference(redis)
                .WithReference(openai)
                .WithReference(speech);

web.WithReference(api);

builder.Build().Run();
```

## 🚀 Development Workflow

### 1. Setup Development Environment
```bash
# Clone and setup
git clone <repository-url>
cd WorldLeadersGame

# Install .NET Aspire workload
dotnet workload install aspire

# Restore packages
dotnet restore

# Set up local secrets
dotnet user-secrets init --project src/WorldLeaders.Web
dotnet user-secrets set "OpenAI:ApiKey" "your-api-key" --project src/WorldLeaders.Web
dotnet user-secrets set "Speech:ApiKey" "your-speech-key" --project src/WorldLeaders.Web
```

### 2. Run with Aspire
```bash
# Start all services
dotnet run --project src/WorldLeaders.AppHost

# Access Aspire dashboard
# Open browser to: https://localhost:15888
```

### 3. Database Migration
```bash
# Add migration
dotnet ef migrations add InitialCreate --project src/WorldLeaders.Infrastructure

# Update database
dotnet ef database update --project src/WorldLeaders.Infrastructure
```

This technical implementation guide provides the foundation for building a robust, scalable, and engaging educational game that combines modern .NET technologies with AI-powered learning experiences.

## 🌍 Real-World Data Integration

### World Data Service Implementation

```csharp
// World Data Service for GDP and Country Information
public class WorldDataService : IWorldDataService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<WorldDataService> _logger;
    private readonly IConfiguration _configuration;

    public WorldDataService(HttpClient httpClient, IMemoryCache cache, 
                           ILogger<WorldDataService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<List<CountryGDPData>> GetGDPDataAsync()
    {
        const string cacheKey = "world_gdp_data";
        
        if (_cache.TryGetValue(cacheKey, out List<CountryGDPData> cachedData))
        {
            return cachedData;
        }

        try
        {
            // World Bank API for GDP data
            var worldBankUrl = "https://api.worldbank.org/v2/country/all/indicator/NY.GDP.MKTP.CD" +
                              "?format=json&date=2023&per_page=300";
            
            var worldBankResponse = await _httpClient.GetStringAsync(worldBankUrl);
            var worldBankData = JsonSerializer.Deserialize<WorldBankGDPResponse>(worldBankResponse);

            // REST Countries API for additional country info
            var countriesUrl = "https://restcountries.com/v3.1/all?fields=name,cca2,population,languages,currencies,flags";
            var countriesResponse = await _httpClient.GetStringAsync(countriesUrl);
            var countriesData = JsonSerializer.Deserialize<List<CountryInfoResponse>>(countriesResponse);

            var gdpData = ProcessGDPData(worldBankData, countriesData);
            
            // Cache for 24 hours
            _cache.Set(cacheKey, gdpData, TimeSpan.FromHours(24));
            
            return gdpData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch GDP data");
            return GetFallbackGDPData();
        }
    }

    public async Task<CountryInfo> GetCountryInfoAsync(string countryCode)
    {
        var cacheKey = $"country_info_{countryCode}";
        
        if (_cache.TryGetValue(cacheKey, out CountryInfo cachedInfo))
        {
            return cachedInfo;
        }

        try
        {
            var url = $"https://restcountries.com/v3.1/alpha/{countryCode}";
            var response = await _httpClient.GetStringAsync(url);
            var countryData = JsonSerializer.Deserialize<List<CountryInfoResponse>>(response);
            
            var countryInfo = MapToCountryInfo(countryData.FirstOrDefault());
            
            _cache.Set(cacheKey, countryInfo, TimeSpan.FromHours(12));
            return countryInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch country info for {CountryCode}", countryCode);
            return new CountryInfo { CountryCode = countryCode, Name = "Unknown" };
        }
    }

    private List<CountryGDPData> ProcessGDPData(WorldBankGDPResponse worldBankData, 
                                               List<CountryInfoResponse> countriesData)
    {
        var gdpEntries = worldBankData.Data?.Where(d => d.Value.HasValue) ?? [];
        var countriesDict = countriesData.ToDictionary(c => c.Cca2, c => c);

        var processedData = gdpEntries
            .Select(entry =>
            {
                countriesDict.TryGetValue(entry.Country.Id, out var countryInfo);
                
                return new CountryGDPData
                {
                    CountryCode = entry.Country.Id,
                    CountryName = entry.Country.Value,
                    GDPNominal = entry.Value ?? 0,
                    Population = countryInfo?.Population ?? 0,
                    OfficialLanguages = countryInfo?.Languages?.Values.ToArray() ?? [],
                    Currency = countryInfo?.Currencies?.Keys.FirstOrDefault() ?? "USD",
                    FlagUrl = countryInfo?.Flags?.Png ?? "",
                    Region = DetermineRegion(entry.Country.Id)
                };
            })
            .OrderByDescending(c => c.GDPNominal)
            .ToList();

        // Assign world rankings
        for (int i = 0; i < processedData.Count; i++)
        {
            processedData[i].WorldRank = i + 1;
            processedData[i].GDPPerCapita = processedData[i].Population > 0 
                ? processedData[i].GDPNominal / processedData[i].Population 
                : 0;
        }

        return processedData;
    }

    private string DetermineRegion(string countryCode) => countryCode switch
    {
        "US" or "CA" or "MX" => "North America",
        "BR" or "AR" or "CL" or "PE" or "CO" => "South America",
        "GB" or "DE" or "FR" or "IT" or "ES" or "NL" or "BE" or "CH" => "Europe",
        "CN" or "JP" or "IN" or "KR" or "ID" or "TH" or "VN" or "PH" => "Asia",
        "NG" or "EG" or "ZA" or "KE" or "GH" or "MA" => "Africa",
        "AU" or "NZ" or "FJ" => "Oceania",
        _ => "Other"
    };

    private List<CountryGDPData> GetFallbackGDPData()
    {
        // Fallback data in case API is unavailable
        return new List<CountryGDPData>
        {
            new() { CountryCode = "US", CountryName = "United States", GDPNominal = 25460000000000m, WorldRank = 1 },
            new() { CountryCode = "CN", CountryName = "China", GDPNominal = 17730000000000m, WorldRank = 2 },
            new() { CountryCode = "JP", CountryName = "Japan", GDPNominal = 4940000000000m, WorldRank = 3 },
            new() { CountryCode = "DE", CountryName = "Germany", GDPNominal = 4260000000000m, WorldRank = 4 },
            new() { CountryCode = "IN", CountryName = "India", GDPNominal = 3740000000000m, WorldRank = 5 },
            new() { CountryCode = "GB", CountryName = "United Kingdom", GDPNominal = 3130000000000m, WorldRank = 6 },
            new() { CountryCode = "FR", CountryName = "France", GDPNominal = 2940000000000m, WorldRank = 7 },
            new() { CountryCode = "CA", CountryName = "Canada", GDPNominal = 2140000000000m, WorldRank = 10 }
        };
    }
}

// Territory Pricing Algorithm Based on Real GDP
public class TerritoryPricingService : ITerritoryPricingService
{
    public TerritoryPricing CalculateTerritoryValue(CountryGDPData countryData)
    {
        // Base cost calculation using GDP ranking and value
        var baseMultiplier = countryData.WorldRank switch
        {
            <= 10 => 50000m,    // Top 10 economies (USA, China, Japan, etc.)
            <= 25 => 25000m,    // Major economies (Australia, Spain, etc.)
            <= 50 => 15000m,    // Medium economies (Argentina, Thailand, etc.)
            <= 100 => 8000m,    // Smaller economies (Kenya, Uruguay, etc.)
            _ => 3000m          // Developing economies
        };

        var gdpFactor = Math.Log10((double)(countryData.GDPNominal / 1000000000m + 1));
        var populationFactor = Math.Log10((double)(countryData.Population / 1000000 + 1));
        
        var cost = baseMultiplier * (decimal)gdpFactor * 0.5m + 
                   baseMultiplier * (decimal)populationFactor * 0.3m;

        var reputationRequired = countryData.WorldRank switch
        {
            <= 5 => 90,      // Superpowers
            <= 15 => 70,     // Major powers
            <= 30 => 50,     // Regional powers
            <= 60 => 30,     // Medium countries
            _ => 15          // Smaller countries
        };

        var incomeBonus = (int)(cost * 0.1m); // 10% of cost as monthly income

        return new TerritoryPricing
        {
            Cost = Math.Round(cost, 0),
            RequiredReputation = reputationRequired,
            IncomeBonus = incomeBonus,
            DifficultyLevel = CalculateDifficultyLevel(countryData.WorldRank),
            StrategicValue = CalculateStrategicValue(countryData)
        };
    }

    private int CalculateDifficultyLevel(int worldRank) => worldRank switch
    {
        <= 5 => 6,    // Hardest
        <= 15 => 5,
        <= 30 => 4,
        <= 60 => 3,
        <= 100 => 2,
        _ => 1        // Easiest
    };

    private int CalculateStrategicValue(CountryGDPData countryData)
    {
        // Consider GDP, population, and regional importance
        var gdpScore = Math.Min(countryData.WorldRank <= 10 ? 50 : 25, 50);
        var populationScore = countryData.Population > 100000000 ? 25 : 10;
        var languageScore = countryData.OfficialLanguages.Length * 5;
        
        return Math.Min(gdpScore + populationScore + languageScore, 100);
    }
}

public class TerritoryPricing
{
    public decimal Cost { get; set; }
    public int RequiredReputation { get; set; }
    public int IncomeBonus { get; set; }
    public int DifficultyLevel { get; set; }
    public int StrategicValue { get; set; }
}
```

### Enhanced Game Components with Real Data

```csharp
// World Map Component with Real Countries
@page "/worldmap"
@inject IWorldDataService WorldDataService
@inject ITerritoryPricingService PricingService
@inject IJSRuntime JSRuntime

<div class="world-map-container bg-blue-900 min-h-screen p-8">
    <h1 class="text-4xl font-bold text-white text-center mb-8">🌍 World Leaders Game</h1>
    
    <div class="filters mb-6 flex justify-center space-x-4">
        <select @onchange="FilterByRegion" class="bg-white rounded px-4 py-2">
            <option value="">All Regions</option>
            <option value="North America">North America</option>
            <option value="Europe">Europe</option>
            <option value="Asia">Asia</option>
            <option value="Africa">Africa</option>
            <option value="South America">South America</option>
            <option value="Oceania">Oceania</option>
        </select>
        
        <select @onchange="FilterByDifficulty" class="bg-white rounded px-4 py-2">
            <option value="">All Difficulty Levels</option>
            <option value="1">⭐ Easy (Small countries)</option>
            <option value="2">⭐⭐ Medium</option>
            <option value="3">⭐⭐⭐ Hard</option>
            <option value="4">⭐⭐⭐⭐ Very Hard</option>
            <option value="5">⭐⭐⭐⭐⭐ Expert</option>
            <option value="6">⭐⭐⭐⭐⭐⭐ Master (Superpowers)</option>
        </select>
    </div>

    <div class="countries-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        @foreach (var country in filteredCountries)
        {
            <div class="country-card bg-white rounded-lg shadow-lg overflow-hidden 
                        @(country.IsOwned ? "border-4 border-green-500" : "") 
                        @(country.CanAfford ? "hover:shadow-xl cursor-pointer" : "opacity-75")">
                
                <div class="country-header bg-gradient-to-r from-blue-500 to-purple-600 text-white p-4">
                    <div class="flex items-center justify-between">
                        <img src="@country.FlagUrl" alt="@country.Name flag" class="w-8 h-6 rounded"/>
                        <span class="text-sm font-bold">#@country.WorldRank</span>
                    </div>
                    <h3 class="text-lg font-bold mt-2">@country.Name</h3>
                </div>

                <div class="country-info p-4">
                    <div class="stats mb-3">
                        <div class="flex justify-between mb-1">
                            <span class="text-sm text-gray-600">GDP:</span>
                            <span class="text-sm font-bold">$@FormatCurrency(country.GDPNominal)</span>
                        </div>
                        <div class="flex justify-between mb-1">
                            <span class="text-sm text-gray-600">Population:</span>
                            <span class="text-sm font-bold">@FormatNumber(country.Population)</span>
                        </div>
                        <div class="flex justify-between mb-1">
                            <span class="text-sm text-gray-600">Languages:</span>
                            <span class="text-sm">@string.Join(", ", country.OfficialLanguages.Take(2))</span>
                        </div>
                    </div>

                    <div class="difficulty-indicator mb-3">
                        <span class="text-sm text-gray-600">Difficulty: </span>
                        @for (int i = 1; i <= 6; i++)
                        {
                            <span class="@(i <= country.DifficultyLevel ? "text-yellow-500" : "text-gray-300")">⭐</span>
                        }
                    </div>

                    <div class="cost-info bg-gray-50 rounded p-3 mb-3">
                        <div class="flex justify-between items-center mb-2">
                            <span class="text-sm font-medium">Cost:</span>
                            <span class="text-lg font-bold text-green-600">$@FormatCurrency(country.Cost)</span>
                        </div>
                        <div class="flex justify-between items-center mb-2">
                            <span class="text-sm font-medium">Reputation needed:</span>
                            <span class="text-sm font-bold">@country.RequiredReputation%</span>
                        </div>
                        <div class="flex justify-between items-center">
                            <span class="text-sm font-medium">Monthly income:</span>
                            <span class="text-sm font-bold text-blue-600">+$@FormatCurrency(country.IncomeBonus)</span>
                        </div>
                    </div>

                    @if (country.IsOwned)
                    {
                        <div class="owned-badge bg-green-500 text-white text-center py-2 rounded">
                            ✅ OWNED
                        </div>
                    }
                    else if (country.CanAfford)
                    {
                        <button @onclick="() => PurchaseCountry(country)" 
                                class="purchase-btn bg-blue-500 hover:bg-blue-600 text-white font-bold py-2 px-4 rounded w-full">
                            🏛️ Purchase
                        </button>
                    }
                    else
                    {
                        <div class="cannot-afford bg-gray-400 text-white text-center py-2 rounded">
                            @GetBlockingReason(country)
                        </div>
                    }
                </div>
            </div>
        }
    </div>
</div>

@code {
    private List<EnhancedCountryData> allCountries = new();
    private List<EnhancedCountryData> filteredCountries = new();
    private PlayerProfile currentPlayer = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadCountriesData();
        FilterCountries();
    }

    private async Task LoadCountriesData()
    {
        var gdpData = await WorldDataService.GetGDPDataAsync();
        
        allCountries = gdpData.Select(country =>
        {
            var pricing = PricingService.CalculateTerritoryValue(country);
            
            return new EnhancedCountryData
            {
                CountryCode = country.CountryCode,
                Name = country.CountryName,
                GDPNominal = country.GDPNominal,
                Population = country.Population,
                WorldRank = country.WorldRank,
                OfficialLanguages = country.OfficialLanguages,
                FlagUrl = country.FlagUrl,
                Region = country.Region,
                Cost = pricing.Cost,
                RequiredReputation = pricing.RequiredReputation,
                IncomeBonus = pricing.IncomeBonus,
                DifficultyLevel = pricing.DifficultyLevel,
                StrategicValue = pricing.StrategicValue,
                IsOwned = currentPlayer.OwnedTerritories?.Any(t => t.CountryCode == country.CountryCode) ?? false,
                CanAfford = CanPlayerAfford(pricing, currentPlayer)
            };
        }).ToList();
    }

    private bool CanPlayerAfford(TerritoryPricing pricing, PlayerProfile player)
    {
        return player.Income >= pricing.Cost && player.Reputation >= pricing.RequiredReputation;
    }

    private string GetBlockingReason(EnhancedCountryData country)
    {
        if (currentPlayer.Reputation < country.RequiredReputation)
            return $"Need {country.RequiredReputation}% reputation";
        if (currentPlayer.Income < country.Cost)
            return "Insufficient funds";
        return "Cannot purchase";
    }

    private async Task PurchaseCountry(EnhancedCountryData country)
    {
        // Implementation for purchasing country
        // This would integrate with the game engine
        await JSRuntime.InvokeVoidAsync("showPurchaseConfirmation", country.Name, country.Cost);
    }

    private string FormatCurrency(decimal amount)
    {
        if (amount >= 1_000_000_000_000m)
            return $"{amount / 1_000_000_000_000m:F1}T";
        if (amount >= 1_000_000_000m)
            return $"{amount / 1_000_000_000m:F1}B";
        if (amount >= 1_000_000m)
            return $"{amount / 1_000_000m:F1}M";
        return $"{amount:F0}";
    }

    private string FormatNumber(long number)
    {
        if (number >= 1_000_000_000)
            return $"{number / 1_000_000_000.0:F1}B";
        if (number >= 1_000_000)
            return $"{number / 1_000_000.0:F1}M";
        return $"{number:N0}";
    }
}

public class EnhancedCountryData : CountryGDPData
{
    public decimal Cost { get; set; }
    public int RequiredReputation { get; set; }
    public int IncomeBonus { get; set; }
    public int DifficultyLevel { get; set; }
    public int StrategicValue { get; set; }
    public bool IsOwned { get; set; }
    public bool CanAfford { get; set; }
}
```

This enhancement adds real-world GDP data integration that makes the game both educational and realistic, exactly as your son envisioned. Countries with higher GDP (like the USA and China) will be more expensive and require higher reputation, while smaller countries can be acquired earlier in the game.
