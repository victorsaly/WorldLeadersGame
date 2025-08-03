var builder = DistributedApplication.CreateBuilder(args);

// Educational game now uses in-memory database for simplified development
// No external database dependencies required for this educational application

// Add the Game API service with in-memory database
var apiService = builder.AddProject("worldleaders-api", "../WorldLeaders.API/WorldLeaders.API.csproj");

// Add the Blazor Web application  
var webApp = builder.AddProject("worldleaders-web", "../WorldLeaders.Web/WorldLeaders.Web.csproj")
    .WithReference(apiService);

builder.Build().Run();
