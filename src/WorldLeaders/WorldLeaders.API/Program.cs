using WorldLeaders.API.Hubs;
using WorldLeaders.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Entity Framework with PostgreSQL
builder.Services.AddDbContext<WorldLeadersDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "World Leaders Game API", 
        Version = "v1",
        Description = "Educational game API for 12-year-old players to learn about world leadership, geography, and languages"
    });
});

// Add service defaults (Aspire)
builder.AddServiceDefaults();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "World Leaders Game API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
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
