using WorldLeaders.Web.Components;
using WorldLeaders.Web.Services;
using WorldLeaders.Web.Handlers;
using WorldLeaders.Shared.Services;
using Microsoft.AspNetCore.SignalR.Client;

var builder = WebApplication.CreateBuilder(args);

// Add configuration sources
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure security headers and options
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
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

// Add service defaults (Aspire) - includes health checks
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
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

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
app.UseAntiforgery();

// Map health checks
app.MapHealthChecks("/health");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map default service endpoints (Aspire)
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
