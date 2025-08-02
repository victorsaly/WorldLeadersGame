using WorldLeaders.Web.Components;
using Microsoft.AspNetCore.SignalR.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HttpClient for API communication
builder.Services.AddHttpClient("GameAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5203/"); // API URL
});

// Add SignalR client for real-time updates
builder.Services.AddSingleton<HubConnection>(provider =>
{
    return new HubConnectionBuilder()
        .WithUrl("http://localhost:5203/gamehub") // SignalR hub URL
        .Build();
});

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
