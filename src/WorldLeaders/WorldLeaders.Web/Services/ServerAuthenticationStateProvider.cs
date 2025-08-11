using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using WorldLeaders.Shared.DTOs;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;

namespace WorldLeaders.Web.Services;

/// <summary>
/// Context: Educational game server-side authentication for 12-year-old players
/// Educational Objective: Secure server-side authentication state management
/// Safety Requirements: Child safety validation, secure session management
/// Real-World Connection: Professional authentication patterns for educational platforms
/// </summary>
public class ServerAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ServerAuthenticationStateProvider> _logger;
    
    private const string AUTH_TOKEN_KEY = "worldleaders.auth.token";
    private const string AUTH_USER_KEY = "worldleaders.auth.user";
    
    public ServerAuthenticationStateProvider(
        IHttpContextAccessor httpContextAccessor,
        ILogger<ServerAuthenticationStateProvider> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Get the current authentication state from server-side session
    /// Educational Context: Safe authentication for child users
    /// </summary>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            _logger.LogInformation("🔐 Getting authentication state from server session");
            
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Session == null)
            {
                _logger.LogInformation("� No HTTP context or session available");
                return CreateAnonymousState();
            }

            // Get authentication data from server session
            var tokenJson = httpContext.Session.GetString(AUTH_TOKEN_KEY);
            var userJson = httpContext.Session.GetString(AUTH_USER_KEY);
            
            if (string.IsNullOrEmpty(tokenJson) || string.IsNullOrEmpty(userJson))
            {
                _logger.LogInformation("🔓 No authentication data found in session");
                return CreateAnonymousState();
            }

            try
            {
                var user = JsonSerializer.Deserialize<UserInfoDto>(userJson);
                if (user == null)
                {
                    _logger.LogInformation("🔓 Failed to deserialize user info");
                    return CreateAnonymousState();
                }

                // Validate token is not expired
                if (IsTokenExpired(tokenJson))
                {
                    _logger.LogInformation("🔓 Authentication token has expired");
                    await ClearAuthenticationDataAsync();
                    return CreateAnonymousState();
                }

                // Create authenticated state with claims
                var claims = CreateClaimsFromUser(user, tokenJson);
                var identity = new ClaimsIdentity(claims, "session");
                var principal = new ClaimsPrincipal(identity);
                
                _logger.LogInformation("✅ User authenticated: {Username} (Role: {Role})", 
                    user.Username, user.Role);
                
                return new AuthenticationState(principal);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "❌ Error deserializing authentication data");
                await ClearAuthenticationDataAsync();
                return CreateAnonymousState();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting authentication state");
            return CreateAnonymousState();
        }
    }

    /// <summary>
    /// Mark user as authenticated and store their data in server session
    /// Educational Context: Safe login process for child users
    /// </summary>
    public Task MarkUserAsAuthenticatedAsync(AuthenticationResponse authResponse)
    {
        try
        {
            _logger.LogInformation("🔐 Storing authentication data in server session for user: {Username}", 
                authResponse.User.Username);

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Session == null)
            {
                throw new InvalidOperationException("HTTP context or session not available");
            }

            // Store authentication data in server session
            var userJson = JsonSerializer.Serialize(authResponse.User);
            httpContext.Session.SetString(AUTH_TOKEN_KEY, authResponse.AccessToken);
            httpContext.Session.SetString(AUTH_USER_KEY, userJson);

            // Create authenticated state
            var claims = CreateClaimsFromUser(authResponse.User, authResponse.AccessToken);
            var identity = new ClaimsIdentity(claims, "session");
            var principal = new ClaimsPrincipal(identity);
            
            // Notify authentication state has changed
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
            
            _logger.LogInformation("✅ User marked as authenticated: {Username}", authResponse.User.Username);
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error storing authentication data");
            throw;
        }
    }

    /// <summary>
    /// Mark user as logged out and clear all authentication data
    /// Educational Context: Safe logout process maintaining child privacy
    /// </summary>
    public async Task MarkUserAsLoggedOutAsync()
    {
        try
        {
            _logger.LogInformation("🔓 Clearing authentication data from server session");
            
            await ClearAuthenticationDataAsync();
            
            // Notify authentication state has changed to anonymous
            NotifyAuthenticationStateChanged(Task.FromResult(CreateAnonymousState()));
            
            _logger.LogInformation("✅ User marked as logged out");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error clearing authentication data");
            throw;
        }
    }

    /// <summary>
    /// Get current user information from server session
    /// Educational Context: Safe user data access for child accounts
    /// </summary>
    public Task<UserInfoDto?> GetCurrentUserAsync()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Session == null)
                return Task.FromResult<UserInfoDto?>(null);
                
            var userJson = httpContext.Session.GetString(AUTH_USER_KEY);
            if (string.IsNullOrEmpty(userJson))
                return Task.FromResult<UserInfoDto?>(null);

            return Task.FromResult(JsonSerializer.Deserialize<UserInfoDto>(userJson));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting current user from session");
            return Task.FromResult<UserInfoDto?>(null);
        }
    }

    /// <summary>
    /// Get current authentication token from server session
    /// Educational Context: Secure token access for API calls
    /// </summary>
    public Task<string?> GetCurrentTokenAsync()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Session == null)
                return Task.FromResult<string?>(null);
                
            var token = httpContext.Session.GetString(AUTH_TOKEN_KEY);
            if (string.IsNullOrEmpty(token))
                return Task.FromResult<string?>(null);

            return Task.FromResult(IsTokenExpired(token) ? null : token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting current token from session");
            return Task.FromResult<string?>(null);
        }
    }

    /// <summary>
    /// Check if user is currently authenticated
    /// Educational Context: Safe authentication status check
    /// </summary>
    public async Task<bool> IsAuthenticatedAsync()
    {
        var state = await GetAuthenticationStateAsync();
        return state.User.Identity?.IsAuthenticated == true;
    }

    /// <summary>
    /// Refresh authentication state after component render
    /// Educational Context: Update authentication state when session data changes
    /// </summary>
    public async Task RefreshAuthenticationStateAsync()
    {
        try
        {
            var currentState = await GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(currentState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error refreshing authentication state");
        }
    }

    /// <summary>
    /// Create claims from user information for authorization
    /// Educational Context: Safe claims creation for child user authorization
    /// </summary>
    private List<Claim> CreateClaimsFromUser(UserInfoDto user, string token)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("DisplayName", user.DisplayName),
            new("IsActive", user.IsActive.ToString()),
            new("IsChild", user.IsChild.ToString()),
            new("Age", user.Age.ToString()),
            new("jwt", token)
        };

        // Add player ID if available
        if (user.PlayerId.HasValue)
        {
            claims.Add(new Claim("PlayerId", user.PlayerId.Value.ToString()));
        }

        return claims;
    }

    /// <summary>
    /// Create anonymous authentication state
    /// Educational Context: Safe anonymous state for non-authenticated users
    /// </summary>
    private AuthenticationState CreateAnonymousState()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        return new AuthenticationState(anonymous);
    }

    /// <summary>
    /// Clear all authentication data from server session
    /// Educational Context: Complete data cleanup for child privacy
    /// </summary>
    private Task ClearAuthenticationDataAsync()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Session != null)
            {
                httpContext.Session.Remove(AUTH_TOKEN_KEY);
                httpContext.Session.Remove(AUTH_USER_KEY);
            }
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error clearing authentication data from session");
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Check if JWT token is expired
    /// Educational Context: Safe token validation for child security
    /// </summary>
    private bool IsTokenExpired(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return DateTime.UtcNow >= jwtToken.ValidTo;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Error validating token expiration, treating as expired");
            return true; // Treat invalid tokens as expired for safety
        }
    }
}
