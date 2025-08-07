using WorldLeaders.Web.Services;

namespace WorldLeaders.Web.Handlers;

/// <summary>
/// Context: Educational game JWT authentication for 12-year-old players
/// Educational Objective: Automatically attach JWT tokens to API requests for secure communication
/// Safety Requirements: Token validation, child safety compliance, secure request handling
/// </summary>
public class JwtAuthenticationHandler : DelegatingHandler
{
    private readonly IServiceProvider _serviceProvider;

    public JwtAuthenticationHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Skip authentication for login/register endpoints
        var requestPath = request.RequestUri?.AbsolutePath ?? "";
        if (requestPath.Contains("/auth/login") || requestPath.Contains("/auth/register"))
        {
            return await base.SendAsync(request, cancellationToken);
        }

        // Create scope to resolve scoped services
        using var scope = _serviceProvider.CreateScope();
        var authService = scope.ServiceProvider.GetService<IAuthenticationClientService>();

        if (authService != null)
        {
            try
            {
                var token = await authService.GetTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch (Exception)
            {
                // Silent fail - request will proceed without token if token retrieval fails
                // This allows anonymous endpoints to work while protecting authenticated ones
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
