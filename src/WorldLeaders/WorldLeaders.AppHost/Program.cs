var builder = DistributedApplication.CreateBuilder(args);

// Educational game infrastructure for 1000+ concurrent users
// Includes Redis for distributed caching and performance optimization

// Add Redis for multi-layer caching strategy
var redis = builder.AddRedis("cache", port: 6379)
    .WithDataVolume(); // Persist cache data for better performance

// Add the Game API service with Redis caching
var apiService = builder.AddProject("worldleaders-api", "../WorldLeaders.API/WorldLeaders.API.csproj")
    .WithReference(redis);

// Add the Blazor Web application with Redis caching support
var webApp = builder.AddProject("worldleaders-web", "../WorldLeaders.Web/WorldLeaders.Web.csproj")
    .WithReference(apiService)
    .WithReference(redis);

builder.Build().Run();
