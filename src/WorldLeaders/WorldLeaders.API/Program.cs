using WorldLeaders.API.Hubs;
using WorldLeaders.API.HealthChecks;
using WorldLeaders.Infrastructure.Data;
using WorldLeaders.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add local configuration file if it exists
if (File.Exists("appsettings.local.json"))
{
    builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
}

// Add services to the container
builder.Services.AddControllers();

// Add .NET built-in health checks for educational platform
builder.Services.AddHealthChecks()
    .AddCheck<ChildSafetyHealthCheck>("child_safety", tags: new[] { "critical", "child_protection" })
    .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "ready", "data" });

// Add Infrastructure services (EF Core + Game Services + Authentication)
builder.Services.AddInfrastructure(builder.Configuration);

// Add SignalR for real-time updates
builder.Services.AddSignalR();

// Add CORS for educational game frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("EducationalGamePolicy", policy =>
    {
        policy.WithOrigins(
            // Development URLs
            "https://localhost:7156",          // API default port
            "http://localhost:5203",           // API actual port  
            "https://localhost:7060", 
            "http://localhost:5122",           // Blazor Web app
            "http://localhost:4000",           // Jekyll docs site  
            "https://localhost:4000",          // Jekyll docs site (HTTPS)
            
            // Production URLs
            "https://docs.worldleadersgame.co.uk",    // Production docs (GitHub Pages)
            "https://worldleadersgame.co.uk",         // Production game web app
            "https://api.worldleadersgame.co.uk",     // Production API (self-reference)
            "https://worldleadersgame.azurewebsites.net",  // Azure default domain
            "https://worldleaders-web-prod.azurewebsites.net",  // Web app Azure domain
            "https://worldleaders-api-prod.azurewebsites.net"   // API Azure domain
        )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials() // Required for SignalR
              .SetIsOriginAllowed(origin =>
              {
                  // Allow all localhost for development
                  if (origin?.Contains("localhost") == true || origin?.Contains("127.0.0.1") == true)
                      return true;
                  
                  // Allow specific production domains
                  var allowedDomains = new[] {
                      "worldleadersgame.co.uk",
                      "docs.worldleadersgame.co.uk", 
                      "api.worldleadersgame.co.uk",
                      "azurewebsites.net"
                  };
                  
                  return allowedDomains.Any(domain => origin?.EndsWith(domain) == true);
              });
    });
});

// Add Swagger for API documentation (available in all environments for educational transparency)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "World Leaders Game API",
        Version = "v1",
        Description = "Educational game API for 12-year-old players to learn about world leadership, geography, and languages with secure authentication",
        Contact = new()
        {
            Name = "World Leaders Game",
            Url = new Uri("https://github.com/victorsaly/WorldLeadersGame")
        }
    });

    // Add JWT Bearer authentication to Swagger
    c.AddSecurityDefinition("Bearer", new()
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme. Enter your token below."
    });

    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Add security definitions for production environments
    c.AddSecurityDefinition("ApiKey", new()
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "X-API-Key",
        Description = "API Key for accessing educational game endpoints (production only)"
    });
});

// Add service defaults (Aspire) - Comment out for manual execution
// builder.AddServiceDefaults();

var app = builder.Build();

// Database initialization for educational game data
try
{
    // Only try to create database if not using in-memory provider
    var databaseProvider = app.Configuration.GetValue<string>("Database:Provider") ?? "InMemory";
    
    if (databaseProvider.ToLowerInvariant() == "inmemory")
    {
        // For in-memory database, ensure the context is created and demo user is set up
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WorldLeadersDbContext>();
        await context.Database.EnsureCreatedAsync();
        
        // Create demo user for testing
        await scope.ServiceProvider.CreateDemoUserIfNeededAsync();
        
        app.Logger.LogInformation("Educational game in-memory database initialized successfully");
    }
    else
    {
        // For persistent databases, use the existing method
        await app.Services.EnsureDatabaseCreatedAsync();
        app.Logger.LogInformation("Educational game database initialized successfully");
    }
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Failed to initialize educational game database. Provider: {Provider}", 
        app.Configuration.GetValue<string>("Database:Provider"));
    
    // For production, gracefully degrade to in-memory if database creation fails
    if (app.Environment.IsProduction())
    {
        app.Logger.LogWarning("Attempting fallback to in-memory database for production deployment");
        try
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WorldLeadersDbContext>();
            await context.Database.EnsureCreatedAsync();
            app.Logger.LogInformation("Fallback to in-memory database successful");
        }
        catch (Exception fallbackEx)
        {
            app.Logger.LogCritical(fallbackEx, "Failed to initialize fallback in-memory database");
            throw; // Re-throw to prevent startup with invalid database
        }
    }
    else
    {
        throw; // Re-throw to prevent startup with invalid database in development
    }
}

// Enable Swagger in all environments (with security considerations for production)
app.UseSwagger();

if (app.Environment.IsDevelopment())
{
    // Development: Full Swagger UI with all features
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "World Leaders Game API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
    });
}
else
{
    // Production: Limited Swagger UI for educational transparency
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "World Leaders Game API v1");
        c.RoutePrefix = "docs"; // Serve Swagger UI at /docs instead of root
        c.DocumentTitle = "World Leaders Game API Documentation";
        c.DefaultModelExpandDepth(1);
        c.DefaultModelsExpandDepth(1);
        c.SupportedSubmitMethods(); // Disable all submit methods in production
    });
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("EducationalGamePolicy");

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Map .NET built-in health check endpoints for educational platform monitoring
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            Status = report.Status.ToString(),
            Service = "World Leaders Game API",
            Environment = "Educational Platform",
            Timestamp = DateTime.UtcNow,
            Message = "Educational game API health status",
            Checks = report.Entries.Select(entry => new
            {
                Name = entry.Key,
                Status = entry.Value.Status.ToString(),
                Description = entry.Value.Description,
                Data = entry.Value.Data,
                Duration = entry.Value.Duration.TotalMilliseconds
            })
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions 
        { 
            WriteIndented = true 
        }));
    }
});

// Basic health check for load balancers
app.MapHealthChecks("/alive");

// Readiness check for Kubernetes/container orchestration
app.MapHealthChecks("/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

// Detailed health check for monitoring systems
app.MapHealthChecks("/health/detailed", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            Status = report.Status.ToString(),
            Service = "World Leaders Game API",
            Environment = "Educational Platform",
            Timestamp = DateTime.UtcNow,
            TotalDuration = report.TotalDuration.TotalMilliseconds,
            Configuration = new
            {
                ChildSafetyMode = true,
                TargetAge = "12 years",
                EducationalContext = "Geography, Economics, Language Learning",
                ComplianceFrameworks = new[] { "COPPA", "GDPR", "UK Educational Standards" }
            },
            Components = report.Entries.Select(entry => new
            {
                Name = entry.Key,
                Status = entry.Value.Status.ToString(),
                Description = entry.Value.Description,
                Data = entry.Value.Data,
                Duration = entry.Value.Duration.TotalMilliseconds,
                Exception = entry.Value.Exception?.Message
            })
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions 
        { 
            WriteIndented = true 
        }));
    }
});

// Map SignalR hubs
app.MapHub<GameHub>("/gamehub");

// Map default service endpoints (Aspire) - Comment out for manual execution
// app.MapDefaultEndpoints();

app.Run();

// Make Program class accessible for testing
public partial class Program { }
