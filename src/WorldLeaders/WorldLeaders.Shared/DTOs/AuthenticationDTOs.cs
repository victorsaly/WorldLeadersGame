using System.ComponentModel.DataAnnotations;
using WorldLeaders.Shared.Enums;

namespace WorldLeaders.Shared.DTOs;

/// <summary>
/// Context: Educational game authentication for 12-year-old players
/// Educational Objective: Secure login flow with child safety validation
/// Safety Requirements: Age verification, parental consent, COPPA/GDPR compliance
/// </summary>

/// <summary>
/// User registration request with child safety considerations
/// </summary>
public record RegisterUserRequest
{
    /// <summary>
    /// Username for the account (must be appropriate for children)
    /// </summary>
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public required string Username { get; init; }

    /// <summary>
    /// Email address (for teacher/admin accounts or parental contact)
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public required string Email { get; init; }

    /// <summary>
    /// Secure password (minimum requirements enforced)
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    public required string Password { get; init; }

    /// <summary>
    /// Password confirmation
    /// </summary>
    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public required string ConfirmPassword { get; init; }

    /// <summary>
    /// Display name for the game (child-friendly identifier)
    /// </summary>
    [Required]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Display name must be between 2 and 50 characters")]
    public required string DisplayName { get; init; }

    /// <summary>
    /// Date of birth for COPPA compliance
    /// </summary>
    [Required]
    public required DateTime DateOfBirth { get; init; }

    /// <summary>
    /// User role in the educational context
    /// </summary>
    public UserRole Role { get; init; } = UserRole.Student;

    /// <summary>
    /// Parental email (required for child accounts under 13)
    /// </summary>
    [EmailAddress]
    public string? ParentalEmail { get; init; }

    /// <summary>
    /// School name (for teacher accounts)
    /// </summary>
    [StringLength(200)]
    public string? SchoolName { get; init; }

    /// <summary>
    /// GDPR consent for data processing
    /// </summary>
    public bool HasGdprConsent { get; init; }

    /// <summary>
    /// Parental consent (required for children under 13)
    /// </summary>
    public bool HasParentalConsent { get; init; }
}

/// <summary>
/// User login request
/// </summary>
public record LoginRequest
{
    /// <summary>
    /// Username or email address
    /// </summary>
    [Required]
    public required string UsernameOrEmail { get; init; }

    /// <summary>
    /// Password
    /// </summary>
    [Required]
    public required string Password { get; init; }

    /// <summary>
    /// Remember login for extended session (not recommended for child accounts)
    /// </summary>
    public bool RememberMe { get; init; } = false;
}

/// <summary>
/// Successful authentication response
/// </summary>
public record AuthenticationResponse
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public required string AccessToken { get; init; }

    /// <summary>
    /// Token expiration time
    /// </summary>
    public required DateTime ExpiresAt { get; init; }

    /// <summary>
    /// User information
    /// </summary>
    public required UserInfoDto User { get; init; }

    /// <summary>
    /// Whether child safety features are enabled
    /// </summary>
    public required bool ChildSafetyEnabled { get; init; }

    /// <summary>
    /// Session timeout in minutes (shorter for children)
    /// </summary>
    public required int SessionTimeoutMinutes { get; init; }
}

/// <summary>
/// User information DTO (safe for client-side use)
/// </summary>
public record UserInfoDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Username
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    /// Display name
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Email address
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// User role
    /// </summary>
    public required UserRole Role { get; init; }

    /// <summary>
    /// Whether this is a child account
    /// </summary>
    public required bool IsChild { get; init; }

    /// <summary>
    /// User's age
    /// </summary>
    public required int Age { get; init; }

    /// <summary>
    /// Whether account is active
    /// </summary>
    public required bool IsActive { get; init; }

    /// <summary>
    /// Player ID if user has a game profile
    /// </summary>
    public Guid? PlayerId { get; init; }

    /// <summary>
    /// Last login timestamp
    /// </summary>
    public required DateTime LastLoginAt { get; init; }
}

/// <summary>
/// Password change request
/// </summary>
public record ChangePasswordRequest
{
    /// <summary>
    /// Current password
    /// </summary>
    [Required]
    public required string CurrentPassword { get; init; }

    /// <summary>
    /// New password
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    public required string NewPassword { get; init; }

    /// <summary>
    /// New password confirmation
    /// </summary>
    [Required]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public required string ConfirmNewPassword { get; init; }
}

/// <summary>
/// Child safety validation request
/// </summary>
public record ChildSafetyValidationRequest
{
    /// <summary>
    /// User ID to validate
    /// </summary>
    public required Guid UserId { get; init; }

    /// <summary>
    /// Content or action to validate
    /// </summary>
    [Required]
    public required string Content { get; init; }

    /// <summary>
    /// Type of validation required
    /// </summary>
    public required string ValidationType { get; init; }
}

/// <summary>
/// Child safety validation response
/// </summary>
public record ChildSafetyValidationResponse
{
    /// <summary>
    /// Whether the content/action is safe
    /// </summary>
    public required bool IsApproved { get; init; }

    /// <summary>
    /// Reason for approval/rejection
    /// </summary>
    public required string Reason { get; init; }

    /// <summary>
    /// Confidence score (0-1)
    /// </summary>
    public required double ConfidenceScore { get; init; }

    /// <summary>
    /// Any safety warnings
    /// </summary>
    public List<string> Warnings { get; init; } = new();
}

/// <summary>
/// User cost tracking summary
/// </summary>
public record UserCostSummaryDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public required Guid UserId { get; init; }

    /// <summary>
    /// Date for tracking
    /// </summary>
    public required DateTime Date { get; init; }

    /// <summary>
    /// Total cost for the day in GBP
    /// </summary>
    public required decimal TotalCost { get; init; }

    /// <summary>
    /// Whether daily limit is exceeded
    /// </summary>
    public required bool IsOverDailyLimit { get; init; }

    /// <summary>
    /// Remaining budget for the day
    /// </summary>
    public required decimal RemainingBudget { get; init; }

    /// <summary>
    /// Service usage breakdown
    /// </summary>
    public required Dictionary<string, decimal> ServiceCosts { get; init; }
}

/// <summary>
/// Session information for monitoring
/// </summary>
public record SessionInfoDto
{
    /// <summary>
    /// Session ID
    /// </summary>
    public required Guid SessionId { get; init; }

    /// <summary>
    /// When session started
    /// </summary>
    public required DateTime StartedAt { get; init; }

    /// <summary>
    /// When session expires
    /// </summary>
    public required DateTime ExpiresAt { get; init; }

    /// <summary>
    /// Last activity
    /// </summary>
    public required DateTime LastActivityAt { get; init; }

    /// <summary>
    /// Remaining time in session
    /// </summary>
    public required TimeSpan RemainingTime { get; init; }

    /// <summary>
    /// Whether session is about to expire (warning threshold)
    /// </summary>
    public required bool IsNearExpiration { get; init; }
}