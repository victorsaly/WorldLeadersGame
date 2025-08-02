var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database for the educational game
var postgres = builder.AddPostgres("postgres")
    .WithEnvironment("POSTGRES_DB", "worldleaders");

var database = postgres.AddDatabase("worldleadersdb");

// Add the Game API service
var apiService = builder.AddProject("worldleaders-api", "../WorldLeaders.API/WorldLeaders.API.csproj")
    .WithReference(database);

// Add the Blazor Web application  
var webApp = builder.AddProject("worldleaders-web", "../WorldLeaders.Web/WorldLeaders.Web.csproj")
    .WithReference(apiService);

builder.Build().Run();
