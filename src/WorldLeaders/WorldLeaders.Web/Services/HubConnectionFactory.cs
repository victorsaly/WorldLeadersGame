using Microsoft.AspNetCore.SignalR.Client;

namespace WorldLeaders.Web.Services;

/// <summary>
/// Factory for creating SignalR HubConnection instances
/// Educational game component - ensures safe connection management for child users
/// </summary>
public interface IHubConnectionFactory
{
    HubConnection CreateConnection();
}

/// <summary>
/// Implementation of HubConnection factory for educational game
/// Creates properly configured SignalR connections for real-time game updates
/// </summary>
public class HubConnectionFactory : IHubConnectionFactory
{
    private readonly IConfiguration _configuration;

    public HubConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public HubConnection CreateConnection()
    {
        var hubUrl = "https://localhost:7155/gamehub"; // Development URL

        return new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                // Allow self-signed certificates for development
                options.HttpMessageHandlerFactory = _ => new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
            })
            .WithAutomaticReconnect() // Auto-reconnect for better user experience
            .Build();
    }
}
