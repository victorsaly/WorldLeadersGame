using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.JSInterop;
using WorldLeaders.Shared.DTOs;
using System.IdentityModel.Tokens.Jwt;

namespace WorldLeaders.Web.Services;

/// <summary>
/// Context: Educational game authentication client for 12-year-old players
/// Educational Objective: Secure client-side authentication with child safety features
/// Safety Requirements: JWT token management, child safety validation, secure storage
/// </summary>
public interface IAuthenticationClientService
{
    Task<AuthenticationResponse> RegisterAsync(RegisterUserRequest request);
    Task<AuthenticationResponse> LoginAsync(LoginRequest request);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<string?> GetTokenAsync();
    Task<UserInfoDto?> GetCurrentUserAsync();
    Task<HttpClient> GetAuthenticatedHttpClientAsync();
    Task<bool> RefreshTokenAsync();
}

public class AuthenticationClientService : IAuthenticationClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthenticationClientService> _logger;
    private readonly ILocalStorageService _localStorage;
    
    private const string TOKEN_KEY = "worldleaders_auth_token";
    private const string USER_KEY = "worldleaders_user_info";

    public AuthenticationClientService(
        IHttpClientFactory httpClientFactory,
        ILogger<AuthenticationClientService> logger,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClientFactory.CreateClient("GameAPI");
        _logger = logger;
        _localStorage = localStorage;
    }

    public async Task<AuthenticationResponse> RegisterAsync(RegisterUserRequest request)
    {
        try
        {
            _logger.LogInformation("Registering new user: {Username}", request.Username);

            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
            
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
                if (authResponse != null)
                {
                    await StoreAuthenticationDataAsync(authResponse);
                    _logger.LogInformation("User registered successfully: {Username}", request.Username);
                    return authResponse;
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Registration failed: {errorContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration failed for user: {Username}", request.Username);
            throw;
        }
    }

    public async Task<AuthenticationResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Logging in user: {UsernameOrEmail}", request.UsernameOrEmail);

            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
                if (authResponse != null)
                {
                    await StoreAuthenticationDataAsync(authResponse);
                    _logger.LogInformation("User logged in successfully: {UsernameOrEmail}", request.UsernameOrEmail);
                    return authResponse;
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException($"Login failed: {errorContent}");
            }
            
            throw new InvalidOperationException($"Login failed: {errorContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for user: {UsernameOrEmail}", request.UsernameOrEmail);
            throw;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            _logger.LogInformation("Logging out user");
            
            await _localStorage.RemoveItemAsync(TOKEN_KEY);
            await _localStorage.RemoveItemAsync(USER_KEY);
            
            _logger.LogInformation("User logged out successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logout failed");
            throw;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return false;

        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwt.ValidTo > DateTime.UtcNow;
        }
        catch
        {
            return false;
        }
    }

    public async Task<HttpClient> GetAuthenticatedHttpClientAsync()
    {
        var token = await GetTokenAsync();
        
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        
        return _httpClient;
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>(TOKEN_KEY);
            return !string.IsNullOrEmpty(token) && !IsTokenExpired(token) ? token : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<UserInfoDto?> GetCurrentUserAsync()
    {
        try
        {
            if (!await IsAuthenticatedAsync())
                return null;

            var userInfo = await _localStorage.GetItemAsync<UserInfoDto>(USER_KEY);
            return userInfo;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync("api/auth/refresh-token", null);
            
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
                if (authResponse != null)
                {
                    await StoreAuthenticationDataAsync(authResponse);
                    return true;
                }
            }

            // If refresh fails, clear stored data
            await LogoutAsync();
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token refresh failed");
            await LogoutAsync();
            return false;
        }
    }

    private async Task StoreAuthenticationDataAsync(AuthenticationResponse response)
    {
        await _localStorage.SetItemAsync(TOKEN_KEY, response.AccessToken);
        await _localStorage.SetItemAsync(USER_KEY, response.User);
    }

    private bool IsTokenExpired(string token)
    {
        try
        {
            // Use proper JWT validation for security
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            // The ValidTo property is in UTC
            return DateTime.UtcNow >= jwtToken.ValidTo;
        }
        catch
        {
            return true; // Assume expired if parsing fails
        }
    }
}

/// <summary>
/// Local storage service interface for browser storage
/// </summary>
public interface ILocalStorageService
{
    Task<T?> GetItemAsync<T>(string key);
    Task SetItemAsync<T>(string key, T value);
    Task RemoveItemAsync(string key);
}

/// <summary>
/// Browser local storage implementation using JavaScript interop
/// Educational Context: Safe client-side storage for child authentication
/// </summary>
public class LocalStorageService : ILocalStorageService
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<T?> GetItemAsync<T>(string key)
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            return default;
        }
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
        }
        catch (Exception ex)
        {
            // Silently handle storage failures (private browsing, storage full, etc.)
            Console.WriteLine($"LocalStorage set failed: {ex.Message}");
        }
    }

    public async Task RemoveItemAsync(string key)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LocalStorage remove failed: {ex.Message}");
        }
    }
}
