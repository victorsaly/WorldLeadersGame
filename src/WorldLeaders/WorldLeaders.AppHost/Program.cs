var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database for the educational game
var postgres = builder.AddPostgres("postgres")
    .WithEnvironment("POSTGRES_DB", "worldleaders");

var database = postgres.AddDatabase("worldleadersdb");

// Add the Game API service
var apiService = builder.AddProject<Projects.WorldLeaders_API>("worldleaders-api")
    .WithReference(database);

// Add the Blazor Web application
var webApp = builder.AddProject<Projects.WorldLeaders_Web>("worldleaders-web")
    .WithReference(apiService);

builder.Build().Run();
