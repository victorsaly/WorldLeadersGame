using WorldLeaders.Web.Components;
using WorldLeaders.Web.Services;
using WorldLeaders.Web.Handlers;
using WorldLeaders.Shared.Services;
using WorldLeaders.Infrastructure.Configuration;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add configuration sources
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure Blazor Server options for detailed error reporting
builder.Services.Configure<Microsoft.AspNetCore.Components.Server.CircuitOptions>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.DetailedErrors = true;
    }
});

// Add session services for authentication with memory-based storage for development
// Temporarily disable sessions to resolve Redis dependency issue
// TODO: Re-enable sessions once Redis configuration is resolved
// builder.Services.AddDistributedMemoryCache();
// builder.Services.AddSession(options =>
// {
//     options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
//     options.Cookie.HttpOnly = true;
//     options.Cookie.IsEssential = true;
//     options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() 
//         ? CookieSecurePolicy.SameAsRequest 
//         : CookieSecurePolicy.Always;
//     options.Cookie.Name = "WorldLeaders.Session";
// });

// Add HTTP context accessor for session access
builder.Services.AddHttpContextAccessor();

// Add Blazor Server authentication services (simplified for development)
builder.Services.AddAuthenticationCore();
builder.Services.AddScoped<SimpleAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => 
    provider.GetRequiredService<SimpleAuthenticationStateProvider>());

// Configure security headers and options
builder.Services.AddAntiforgery(options =>
{
    // Use environment-appropriate secure policy: Always for production, SameAsRequest for development
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() 
        ? CookieSecurePolicy.SameAsRequest 
        : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.HttpOnly = true;
});

// Add security services
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

// Register HTTP-based client services for API communication
builder.Services.AddScoped<ITerritoryService, TerritoryClientService>();
builder.Services.AddScoped<ISpeechRecognitionService, SpeechRecognitionClientService>();
builder.Services.AddScoped<IAuthenticationClientService, AuthenticationClientService>();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();

// Register character persona service for retro character selection
builder.Services.AddScoped<ICharacterPersonaService, CharacterPersonaService>();

// Register authentication handler
builder.Services.AddTransient<JwtAuthenticationHandler>();

// Add HttpClient for API communication with production-ready configuration
var apiConfiguration = builder.Configuration.GetSection("ApiSettings");
var apiBaseUrl = GetApiBaseUrl(builder.Configuration, builder.Environment);
var apiTimeout = TimeSpan.FromSeconds(apiConfiguration.GetValue<int>("TimeoutSeconds", 30));

builder.Services.AddHttpClient("GameAPI", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = apiTimeout;
    client.DefaultRequestHeaders.Add("User-Agent", "WorldLeaders-Web/1.0");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    
    // Add API key if available for production
    var apiKey = Environment.GetEnvironmentVariable("API_KEY");
    if (!string.IsNullOrEmpty(apiKey))
    {
        client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
    }
})
.AddHttpMessageHandler<JwtAuthenticationHandler>() // Automatically add JWT tokens
.ConfigurePrimaryHttpMessageHandler(() => 
{
    var handler = new HttpClientHandler();
    
    // Only allow self-signed certificates in development
    if (builder.Environment.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }
    
    return handler;
});

// Add SignalR client factory for real-time updates
builder.Services.AddSingleton<IHubConnectionFactory, HubConnectionFactory>();

// TODO: Add missing client services as they are implemented
// builder.Services.AddScoped<IPlayerService, PlayerClientService>();
// builder.Services.AddScoped<IGameStateService, GameStateClientService>();
// builder.Services.AddScoped<IAIAgentService, AIAgentClientService>();
// builder.Services.AddScoped<IEventService, EventClientService>();

// Add logging configuration
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
    
    if (builder.Environment.IsProduction())
    {
        logging.SetMinimumLevel(LogLevel.Warning);
    }
    else
    {
        logging.SetMinimumLevel(LogLevel.Information);
    }
});

// Add memory caching for performance
builder.Services.AddMemoryCache();

// Add performance optimization configuration
builder.Services.Configure<PerformanceConfig>(builder.Configuration.GetSection(PerformanceConfig.SectionName));

// Add Application Insights for Web application monitoring
var applicationInsightsConnectionString = builder.Configuration.GetValue<string>("ApplicationInsights:ConnectionString");
if (!string.IsNullOrEmpty(applicationInsightsConnectionString))
{
    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        options.ConnectionString = applicationInsightsConnectionString;
        options.EnableAdaptiveSampling = true;
        options.EnableQuickPulseMetricStream = true;
    });
}

// For development, we use in-memory cache
// Production deployments will configure Redis separately via environment variables
if (builder.Environment.IsProduction())
{
    var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");
    if (!string.IsNullOrEmpty(redisConnectionString))
    {
        try
        {
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "WorldLeadersGame.Web";
            });
            
            // Use logger factory instead of BuildServiceProvider during configuration
            using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole());
            var productionLogger = loggerFactory.CreateLogger<Program>();
            productionLogger.LogInformation("📊 Redis distributed cache configured for production");
        }
        catch (Exception ex)
        {
            // Use logger factory instead of BuildServiceProvider during configuration
            using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole());
            var errorLogger = loggerFactory.CreateLogger<Program>();
            errorLogger.LogError(ex, "❌ Redis configuration failed in production");
            throw; // Fail fast in production if Redis is expected but not available
        }
    }
}
else
{
    // Use logger factory instead of BuildServiceProvider during configuration
    using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole());
    var devLogger = loggerFactory.CreateLogger<Program>();
    devLogger.LogInformation("🧠 Using in-memory cache for session storage (development mode)");
}

// Add service defaults (Aspire) for production monitoring
builder.AddServiceDefaults();

var app = builder.Build();

// Log the API configuration for debugging
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("🌍 World Leaders Game Web Application Starting");
logger.LogInformation("📡 API Base URL: {ApiBaseUrl}", apiBaseUrl);
logger.LogInformation("🔧 Environment: {Environment}", app.Environment.EnvironmentName);
logger.LogInformation("🛡️ Child Safety Mode: {ChildSafetyMode}", builder.Configuration["GameSettings:EnableChildSafetyMode"]);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    // Only redirect to HTTPS in production
    app.UseHttpsRedirection();
}
else
{
    app.UseDeveloperExceptionPage();
}

// Add security headers middleware
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    
    if (app.Environment.IsProduction())
    {
        var csp = app.Configuration["Security:ContentSecurityPolicy"] ?? 
            "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; connect-src 'self' wss: https:;";
        context.Response.Headers["Content-Security-Policy"] = csp;
    }
    
    await next();
});

app.UseStaticFiles();
// Temporarily disable session middleware to resolve Redis dependency issue
// app.UseSession(); // Enable session middleware
app.UseAntiforgery();

// Health endpoints are automatically mapped by MapDefaultEndpoints() below
// This includes /health, /alive, and /ready endpoints for Azure deployment slot swaps

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map default service endpoints (Aspire) - Enabled for production health monitoring
app.MapDefaultEndpoints();

app.Run();

/// <summary>
/// Helper method to determine the API base URL with fallback logic
/// Context: Production-ready configuration for educational game API
/// </summary>
/// <param name="configuration">Application configuration</param>
/// <param name="environment">Host environment</param>
/// <returns>API base URL for HTTP client configuration</returns>
static string GetApiBaseUrl(IConfiguration configuration, IWebHostEnvironment environment)
{
    return configuration["ApiSettings:BaseUrl"] ?? 
           Environment.GetEnvironmentVariable("API_BASE_URL") ?? 
           (environment.IsProduction() ? "https://api.worldleadersgame.co.uk" : "https://localhost:7155");
}
