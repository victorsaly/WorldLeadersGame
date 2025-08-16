using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Models;

namespace WorldLeaders.Shared.Services;

/// <summary>
/// Context: Educational game authentication for 12-year-old players
/// Educational Objective: Secure, COPPA/GDPR compliant authentication service
/// Safety Requirements: Child data protection, parental oversight, UK South data residency
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Register a new user with child safety validation
    /// </summary>
    /// <param name="request">Registration request with safety validation</param>
    /// <returns>Authentication response or validation errors</returns>
    Task<AuthenticationResponse> RegisterAsync(RegisterUserRequest request);

    /// <summary>
    /// Authenticate a user and create a secure session
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication response with session information</returns>
    Task<AuthenticationResponse> LoginAsync(LoginRequest request);

    /// <summary>
    /// Create a temporary guest session for exploring the system without registration
    /// </summary>
    /// <param name="request">Guest access request with child safety considerations</param>
    /// <returns>Guest authentication response with limited access</returns>
    Task<GuestAuthenticationResponse> CreateGuestSessionAsync(GuestAccessRequest request);

    /// <summary>
    /// Validate an existing JWT token
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>User information if token is valid</returns>
    Task<UserInfoDto?> ValidateTokenAsync(string token);

    /// <summary>
    /// Refresh an existing token (if not expired)
    /// </summary>
    /// <param name="token">Current JWT token</param>
    /// <returns>New authentication response</returns>
    Task<AuthenticationResponse> RefreshTokenAsync(string token);

    /// <summary>
    /// Logout and invalidate session
    /// </summary>
    /// <param name="userId">User to logout</param>
    /// <param name="sessionToken">Session token to invalidate</param>
    /// <returns>Success status</returns>
    Task<bool> LogoutAsync(Guid userId, string sessionToken);

    /// <summary>
    /// Change user password with validation
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Password change request</param>
    /// <returns>Success status</returns>
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);

    /// <summary>
    /// Get current session information
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="sessionToken">Session token</param>
    /// <returns>Session information</returns>
    Task<SessionInfoDto?> GetSessionInfoAsync(Guid userId, string sessionToken);

    /// <summary>
    /// Extend session if allowed (typically for non-child accounts)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="sessionToken">Session token</param>
    /// <returns>New expiration time</returns>
    Task<DateTime?> ExtendSessionAsync(Guid userId, string sessionToken);
}

/// <summary>
/// Per-user cost tracking service for Azure services
/// </summary>
public interface IPerUserCostTracker
{
    /// <summary>
    /// Track AI service usage for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="serviceType">Type of AI service used</param>
    /// <param name="estimatedCost">Estimated cost in GBP</param>
    /// <returns>Updated cost tracking</returns>
    Task<UserCostSummaryDto> TrackUsageAsync(Guid userId, string serviceType, decimal estimatedCost);

    /// <summary>
    /// Get daily cost summary for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="date">Date to check (defaults to today)</param>
    /// <returns>Cost summary</returns>
    Task<UserCostSummaryDto> GetDailyCostSummaryAsync(Guid userId, DateTime? date = null);

    /// <summary>
    /// Check if user has exceeded daily cost limit
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Whether limit is exceeded</returns>
    Task<bool> IsOverDailyLimitAsync(Guid userId);

    /// <summary>
    /// Get cost summary for multiple users (admin view)
    /// </summary>
    /// <param name="userIds">User IDs to check</param>
    /// <param name="startDate">Start date for range</param>
    /// <param name="endDate">End date for range</param>
    /// <returns>Cost summaries</returns>
    Task<List<UserCostSummaryDto>> GetCostSummariesAsync(List<Guid> userIds, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Reset daily costs (typically run at midnight)
    /// </summary>
    /// <returns>Number of users reset</returns>
    Task<int> ResetDailyCostsAsync();
}

/// <summary>
/// User manager interface for administrative functions
/// </summary>
public interface IUserManagerService
{
    /// <summary>
    /// Get user by ID with safety checks
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User information</returns>
    Task<ApplicationUser?> GetUserByIdAsync(Guid userId);

    /// <summary>
    /// Get user by username or email
    /// </summary>
    /// <param name="usernameOrEmail">Username or email</param>
    /// <returns>User information</returns>
    Task<ApplicationUser?> GetUserByUsernameOrEmailAsync(string usernameOrEmail);

    /// <summary>
    /// Update user profile with validation
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="updates">Updates to apply</param>
    /// <returns>Success status</returns>
    Task<bool> UpdateUserAsync(Guid userId, Dictionary<string, object> updates);

    /// <summary>
    /// Deactivate user account (soft delete with audit trail)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="reason">Reason for deactivation</param>
    /// <returns>Success status</returns>
    Task<bool> DeactivateUserAsync(Guid userId, string reason);

    /// <summary>
    /// Get all active sessions for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Active sessions</returns>
    Task<List<UserSession>> GetActiveSessionsAsync(Guid userId);

    /// <summary>
    /// Terminate all sessions for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Number of sessions terminated</returns>
    Task<int> TerminateAllSessionsAsync(Guid userId);
}