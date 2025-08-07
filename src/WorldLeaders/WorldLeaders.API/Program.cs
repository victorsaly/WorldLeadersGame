using WorldLeaders.API.Hubs;
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
            "https://localhost:7155",          // API default port
            "http://localhost:5203",           // API actual port  
            "https://localhost:7060", 
            "http://localhost:5122",           // Blazor Web app
            "http://localhost:4000",           // Jekyll docs site  
            "https://localhost:4000",          // Jekyll docs site (HTTPS)
            "https://docs.worldleadersgame.co.uk", // Production docs
            "https://worldleadersgame.co.uk"   // Production game
        )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for SignalR
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

// Add service defaults (Aspire)
builder.AddServiceDefaults();

var app = builder.Build();

// Initialize database with educational content
await app.Services.EnsureDatabaseCreatedAsync();

// Configure the HTTP request pipeline

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

// Map SignalR hubs
app.MapHub<GameHub>("/gamehub");

// Map default service endpoints (Aspire)
app.MapDefaultEndpoints();

app.Run();
