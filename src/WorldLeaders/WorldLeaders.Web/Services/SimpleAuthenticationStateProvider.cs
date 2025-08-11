using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using WorldLeaders.Shared.DTOs;

namespace WorldLeaders.Web.Services;

/// <summary>
/// Simplified authentication state provider for development (no session dependencies)
/// Educational Objective: Basic authentication state management for 12-year-old players
/// Safety Requirements: Child safety validation without external dependencies
/// Real-World Connection: Simplified authentication patterns for educational platforms
/// </summary>
public class SimpleAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILogger<SimpleAuthenticationStateProvider> _logger;
    
    // In-memory storage for development (this will be lost on app restart)
    private static readonly Dictionary<string, (UserInfoDto User, string Token, DateTime Expiry)> _inMemoryUsers = new();
    private static string? _currentUserId = null;
    
    public SimpleAuthenticationStateProvider(ILogger<SimpleAuthenticationStateProvider> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get the current authentication state without sessions
    /// Educational Context: Safe authentication for child users
    /// </summary>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            _logger.LogInformation("🔐 Getting authentication state (simple in-memory mode)");
            
            if (string.IsNullOrEmpty(_currentUserId) || !_inMemoryUsers.ContainsKey(_currentUserId))
            {
                _logger.LogInformation("🔓 No authentication data found in memory");
                return CreateAnonymousState();
            }

            var userData = _inMemoryUsers[_currentUserId];
            
            // Check if token has expired
            if (DateTime.UtcNow >= userData.Expiry)
            {
                _logger.LogInformation("🔓 Authentication token has expired");
                await ClearAuthenticationDataAsync();
                return CreateAnonymousState();
            }

            // Create authenticated state with claims
            var claims = CreateClaimsFromUser(userData.User, userData.Token);
            var identity = new ClaimsIdentity(claims, "memory");
            var principal = new ClaimsPrincipal(identity);
            
            _logger.LogInformation("✅ User authenticated: {Username} (Role: {Role})", 
                userData.User.Username, userData.User.Role);
            
            return new AuthenticationState(principal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting authentication state");
            return CreateAnonymousState();
        }
    }

    /// <summary>
    /// Mark user as authenticated and store their data in memory
    /// Educational Context: Safe login process for child users
    /// </summary>
    public Task MarkUserAsAuthenticatedAsync(AuthenticationResponse authResponse)
    {
        try
        {
            _logger.LogInformation("🔐 Storing authentication data in memory for user: {Username}", 
                authResponse.User.Username);

            var userId = authResponse.User.Id.ToString();
            var expiryTime = DateTime.UtcNow.AddMinutes(30); // 30 minute expiry
            
            _inMemoryUsers[userId] = (authResponse.User, authResponse.AccessToken, expiryTime);
            _currentUserId = userId;

            // Create authenticated state
            var claims = CreateClaimsFromUser(authResponse.User, authResponse.AccessToken);
            var identity = new ClaimsIdentity(claims, "memory");
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
            _logger.LogInformation("🔓 Clearing authentication data from memory");
            
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
    /// Get current user information from memory
    /// Educational Context: Safe user data access for child accounts
    /// </summary>
    public async Task<UserInfoDto?> GetCurrentUserAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(_currentUserId) || !_inMemoryUsers.ContainsKey(_currentUserId))
                return null;

            var userData = _inMemoryUsers[_currentUserId];
            
            // Check if expired
            if (DateTime.UtcNow >= userData.Expiry)
            {
                await ClearAuthenticationDataAsync();
                return null;
            }

            return userData.User;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting current user from memory");
            return null;
        }
    }

    /// <summary>
    /// Get current authentication token from memory
    /// Educational Context: Secure token access for API calls
    /// </summary>
    public async Task<string?> GetCurrentTokenAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(_currentUserId) || !_inMemoryUsers.ContainsKey(_currentUserId))
                return null;

            var userData = _inMemoryUsers[_currentUserId];
            
            // Check if expired
            if (DateTime.UtcNow >= userData.Expiry)
            {
                await ClearAuthenticationDataAsync();
                return null;
            }

            return userData.Token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting current token from memory");
            return null;
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
    /// Educational Context: Update authentication state when data changes
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
    /// Clear all authentication data from memory
    /// Educational Context: Complete data cleanup for child privacy
    /// </summary>
    private Task ClearAuthenticationDataAsync()
    {
        try
        {
            if (!string.IsNullOrEmpty(_currentUserId) && _inMemoryUsers.ContainsKey(_currentUserId))
            {
                _inMemoryUsers.Remove(_currentUserId);
            }
            _currentUserId = null;
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error clearing authentication data from memory");
            return Task.CompletedTask;
        }
    }
}
