using WorldLeaders.Web.Components;
using WorldLeaders.Web.Services;
using WorldLeaders.Infrastructure.Extensions;
using Microsoft.AspNetCore.SignalR.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Infrastructure services (EF Core + Game Services)
builder.Services.AddInfrastructure(builder.Configuration);

// Add HttpClient for API communication
builder.Services.AddHttpClient("GameAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7155/"); // API HTTPS URL
    // Allow self-signed certificates for development
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

// Add SignalR client factory for real-time updates
builder.Services.AddSingleton<IHubConnectionFactory, HubConnectionFactory>();

// Add service defaults (Aspire)
builder.AddServiceDefaults();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map default service endpoints (Aspire)
app.MapDefaultEndpoints();

app.Run();
