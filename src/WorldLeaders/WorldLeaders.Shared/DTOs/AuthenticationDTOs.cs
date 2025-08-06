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
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public required string Username { get; set; }    /// <summary>
                                                     /// Email address (for teacher/admin accounts or parental contact)
                                                     /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(254, ErrorMessage = "Email address is too long")]
    public required string Email { get; set; }    /// <summary>
                                                  /// Secure password (minimum requirements enforced)
                                                  /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    public required string Password { get; set; }    /// <summary>
                                                     /// Password confirmation
                                                     /// </summary>
    [Required(ErrorMessage = "Password confirmation is required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public required string ConfirmPassword { get; set; }    /// <summary>
                                                            /// Display name for the game (child-friendly identifier)
                                                            /// </summary>
    [Required(ErrorMessage = "Display name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Display name must be between 2 and 50 characters")]
    public required string DisplayName { get; set; }    /// Child safety: Date of birth for age verification and COPPA compliance
    [Required(ErrorMessage = "Date of birth is required")]
    public required DateTime DateOfBirth { get; set; }

    /// Computed age for convenience (calculated from DateOfBirth)
    public int Age
    {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
        set
        {
            // Set DateOfBirth based on age for form binding convenience
            DateOfBirth = DateTime.Today.AddYears(-value);
        }
    }

    /// User role (defaulted to Student for educational context)
    public UserRole Role { get; set; } = UserRole.Student;

    /// Optional: Parental email for additional safety measures
    public string? ParentalEmail { get; set; }

    /// Optional: School name for educational context
    public string? SchoolName { get; set; }

    /// GDPR compliance consent
    public bool HasGdprConsent { get; set; }

    /// Child safety: Parental consent for users under 18
    public bool HasParentalConsent { get; set; }
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
    public required string UsernameOrEmail { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    [Required]
    public required string Password { get; set; }

    /// <summary>
    /// Remember login for extended session (not recommended for child accounts)
    /// </summary>
    public bool RememberMe { get; init; } = false;
}

/// <summary>
/// Guest access request for exploring the system without registration
/// </summary>
public record GuestAccessRequest
{
    /// <summary>
    /// Optional display name for the guest session (child-friendly identifier)
    /// </summary>
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Display name must be between 2 and 50 characters")]
    public string? DisplayName { get; set; }

    /// <summary>
    /// Age for appropriate content filtering (optional for exploration)
    /// </summary>
    [Range(5, 18, ErrorMessage = "Age must be between 5 and 18 for educational game access")]
    public int? Age { get; set; }

    /// <summary>
    /// Whether parental permission has been indicated (not required for guest mode)
    /// </summary>
    public bool HasParentalAwareness { get; set; } = false;

    /// <summary>
    /// Session duration preference in minutes (limited for guest access)
    /// </summary>
    [Range(5, 30, ErrorMessage = "Guest session duration must be between 5 and 30 minutes")]
    public int SessionDurationMinutes { get; set; } = 15;
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
/// Guest access authentication response for temporary exploration
/// </summary>
public record GuestAuthenticationResponse
{
    /// <summary>
    /// Temporary JWT access token for guest session
    /// </summary>
    public required string AccessToken { get; init; }

    /// <summary>
    /// Token expiration time (limited for guest access)
    /// </summary>
    public required DateTime ExpiresAt { get; init; }

    /// <summary>
    /// Guest session information
    /// </summary>
    public required GuestUserInfoDto Guest { get; init; }

    /// <summary>
    /// Child safety features are always enabled for guest access
    /// </summary>
    public bool ChildSafetyEnabled { get; init; } = true;

    /// <summary>
    /// Session timeout in minutes (limited for guest sessions)
    /// </summary>
    public required int SessionTimeoutMinutes { get; init; }

    /// <summary>
    /// Available features for guest users (limited subset)
    /// </summary>
    public List<string> AvailableFeatures { get; init; } = new();

    /// <summary>
    /// Encouragement message to register for full access
    /// </summary>
    public string? RegistrationEncouragement { get; init; }
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
/// Guest user information DTO for temporary exploration sessions
/// </summary>
public record GuestUserInfoDto
{
    /// <summary>
    /// Temporary guest session ID
    /// </summary>
    public required Guid GuestId { get; init; }

    /// <summary>
    /// Display name for guest session (if provided)
    /// </summary>
    public string DisplayName { get; init; } = "Guest Explorer";

    /// <summary>
    /// Guest session type (always guest)
    /// </summary>
    public UserRole Role { get; init; } = UserRole.Guest;

    /// <summary>
    /// Age for content filtering (if provided)
    /// </summary>
    public int? Age { get; init; }

    /// <summary>
    /// Whether this is a child guest (child safety always enabled)
    /// </summary>
    public bool IsChild { get; init; } = true;

    /// <summary>
    /// Guest session start time
    /// </summary>
    public required DateTime SessionStartedAt { get; init; }

    /// <summary>
    /// When guest session expires
    /// </summary>
    public required DateTime SessionExpiresAt { get; init; }

    /// <summary>
    /// Whether parental awareness was indicated
    /// </summary>
    public required bool HasParentalAwareness { get; init; }
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