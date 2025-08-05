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

// Add Infrastructure services (EF Core + Game Services)
builder.Services.AddInfrastructure(builder.Configuration);

// Add SignalR for real-time updates
builder.Services.AddSignalR();

// Add CORS for educational game frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("EducationalGamePolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7060", "http://localhost:5122") // Blazor app URLs
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
        Description = "Educational game API for 12-year-old players to learn about world leadership, geography, and languages",
        Contact = new()
        {
            Name = "World Leaders Game",
            Url = new Uri("https://github.com/victorsaly/WorldLeadersGame")
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

// Map controllers
app.MapControllers();

// Map SignalR hubs
app.MapHub<GameHub>("/gamehub");

// Map default service endpoints (Aspire)
app.MapDefaultEndpoints();

app.Run();
