using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.API.Controllers;

/// <summary>
/// Context: Educational game authentication for 12-year-old players
/// Educational Objective: Secure API endpoints for child-safe authentication
/// Safety Requirements: JWT token management, child safety validation, UK South compliance
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController(
    IAuthenticationService authenticationService,
    IUserManagerService userManagerService,
    IPerUserCostTracker costTracker,
    ILogger<AuthController> logger) : ControllerBase
{
    /// <summary>
    /// Register a new user with comprehensive child safety validation
    /// </summary>
    /// <param name="request">User registration request</param>
    /// <returns>Authentication response with JWT token</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationResponse), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(typeof(string), 500)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        try
        {
            logger.LogInformation("Registration attempt for username: {Username}", request.Username);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                
                return BadRequest($"Validation failed: {string.Join(", ", errors)}");
            }

            var response = await authenticationService.RegisterAsync(request);
            
            logger.LogInformation("User registered successfully: {Username}", request.Username);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Registration failed for {Username}: {Error}", request.Username, ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during registration for username: {Username}", request.Username);
            return StatusCode(500, "An error occurred during registration. Please try again.");
        }
    }

    /// <summary>
    /// Authenticate user and create secure session
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication response with JWT token</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationResponse), 200)]
    [ProducesResponseType(typeof(string), 401)]
    [ProducesResponseType(typeof(string), 500)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            logger.LogInformation("Login attempt for user: {UsernameOrEmail}", request.UsernameOrEmail);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                
                return BadRequest($"Validation failed: {string.Join(", ", errors)}");
            }

            var response = await authenticationService.LoginAsync(request);
            
            logger.LogInformation("User logged in successfully: {UsernameOrEmail}", request.UsernameOrEmail);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning("Login failed for {UsernameOrEmail}: {Error}", request.UsernameOrEmail, ex.Message);
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during login for user: {UsernameOrEmail}", request.UsernameOrEmail);
            return StatusCode(500, "An error occurred during login. Please try again.");
        }
    }

    /// <summary>
    /// Create a guest session for exploring the system without registration
    /// </summary>
    /// <param name="request">Guest access request</param>
    /// <returns>Guest authentication response with limited access</returns>
    [HttpPost("guest")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(GuestAuthenticationResponse), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(typeof(string), 500)]
    public async Task<IActionResult> CreateGuestSession([FromBody] GuestAccessRequest request)
    {
        try
        {
            logger.LogInformation("Guest access request for display name: {DisplayName}, age: {Age}", 
                request.DisplayName ?? "Anonymous", request.Age);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                
                return BadRequest($"Validation failed: {string.Join(", ", errors)}");
            }

            var response = await authenticationService.CreateGuestSessionAsync(request);
            
            logger.LogInformation("Guest session created successfully for: {DisplayName}, duration: {Duration}min", 
                response.Guest.DisplayName, request.SessionDurationMinutes);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Guest session creation failed: {Error}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during guest session creation");
            return StatusCode(500, "An error occurred creating guest session. Please try again.");
        }
    }

    /// <summary>
    /// Logout user and invalidate session
    /// </summary>
    /// <returns>Success status</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(string), 401)]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user session");
            }

            var sessionTokenClaim = User.FindFirst("jti")?.Value;
            if (string.IsNullOrEmpty(sessionTokenClaim))
            {
                return Unauthorized("Invalid session token");
            }

            var success = await authenticationService.LogoutAsync(userId, sessionTokenClaim);
            
            if (success)
            {
                logger.LogInformation("User logged out successfully: {UserId}", userId);
                return Ok(new { message = "Logged out successfully", timestamp = DateTime.UtcNow });
            }
            else
            {
                logger.LogWarning("Logout failed for user: {UserId}", userId);
                return BadRequest("Logout failed");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during logout");
            return StatusCode(500, "An error occurred during logout. Please try again.");
        }
    }

    /// <summary>
    /// Refresh authentication token
    /// </summary>
    /// <returns>New authentication response</returns>
    [HttpPost("refresh")]
    [Authorize]
    [ProducesResponseType(typeof(AuthenticationResponse), 200)]
    [ProducesResponseType(typeof(string), 401)]
    [ProducesResponseType(typeof(string), 500)]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Invalid authorization header");
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var response = await authenticationService.RefreshTokenAsync(token);
            
            logger.LogInformation("Token refreshed successfully for user");
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning("Token refresh failed: {Error}", ex.Message);
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, "An error occurred during token refresh. Please try again.");
        }
    }

    /// <summary>
    /// Change user password
    /// </summary>
    /// <param name="request">Password change request</param>
    /// <returns>Success status</returns>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(typeof(string), 401)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user session");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                
                return BadRequest($"Validation failed: {string.Join(", ", errors)}");
            }

            var success = await authenticationService.ChangePasswordAsync(userId, request);
            
            if (success)
            {
                logger.LogInformation("Password changed successfully for user: {UserId}", userId);
                return Ok(new { message = "Password changed successfully", timestamp = DateTime.UtcNow });
            }
            else
            {
                logger.LogWarning("Password change failed for user: {UserId}", userId);
                return BadRequest("Password change failed. Please check your current password.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during password change");
            return StatusCode(500, "An error occurred during password change. Please try again.");
        }
    }

    /// <summary>
    /// Get current user session information
    /// </summary>
    /// <returns>Session information</returns>
    [HttpGet("session")]
    [Authorize]
    [ProducesResponseType(typeof(SessionInfoDto), 200)]
    [ProducesResponseType(typeof(string), 401)]
    public async Task<IActionResult> GetSessionInfo()
    {
        try
        {
            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user session");
            }

            var sessionTokenClaim = User.FindFirst("jti")?.Value;
            if (string.IsNullOrEmpty(sessionTokenClaim))
            {
                return Unauthorized("Invalid session token");
            }

            var sessionInfo = await authenticationService.GetSessionInfoAsync(userId, sessionTokenClaim);
            
            if (sessionInfo == null)
            {
                return Unauthorized("Session not found");
            }

            return Ok(sessionInfo);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting session info");
            return StatusCode(500, "An error occurred getting session information.");
        }
    }

    /// <summary>
    /// Get current user's daily cost summary
    /// </summary>
    /// <returns>Cost summary</returns>
    [HttpGet("cost-summary")]
    [Authorize]
    [ProducesResponseType(typeof(UserCostSummaryDto), 200)]
    [ProducesResponseType(typeof(string), 401)]
    public async Task<IActionResult> GetCostSummary()
    {
        try
        {
            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user session");
            }

            var costSummary = await costTracker.GetDailyCostSummaryAsync(userId);
            return Ok(costSummary);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting cost summary");
            return StatusCode(500, "An error occurred getting cost summary.");
        }
    }

    /// <summary>
    /// Get current user profile (for authenticated users)
    /// </summary>
    /// <returns>User profile information</returns>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfoDto), 200)]
    [ProducesResponseType(typeof(string), 401)]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user session");
            }

            var user = await userManagerService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var userInfo = new UserInfoDto
            {
                Id = user.Id,
                Username = user.UserName!,
                DisplayName = user.DisplayName,
                Email = user.Email!,
                Role = user.Role,
                IsChild = user.IsChild,
                Age = user.CalculateAge(),
                IsActive = user.IsActive,
                PlayerId = user.PlayerId,
                LastLoginAt = user.LastLoginAt
            };

            return Ok(userInfo);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user profile");
            return StatusCode(500, "An error occurred getting user profile.");
        }
    }
}