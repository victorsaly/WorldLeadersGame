using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using WorldLeaders.Shared.Enums;

namespace WorldLeaders.Shared.Models;

/// <summary>
/// Context: Educational game authentication for 12-year-old players
/// Educational Objective: Secure, COPPA/GDPR compliant user management
/// Safety Requirements: Child data protection, parental oversight, UK South data residency
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>
    /// Display name for the educational game (child-friendly)
    /// </summary>
    [Required]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Display name must be between 2 and 50 characters")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Date of birth for COPPA compliance (under 13 requires parental consent)
    /// </summary>
    [Required]
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Parental email for child safety and consent management
    /// </summary>
    [EmailAddress]
    [StringLength(255)]
    public string? ParentalEmail { get; set; }

    /// <summary>
    /// Whether parental consent has been obtained (COPPA requirement)
    /// </summary>
    public bool HasParentalConsent { get; set; }

    /// <summary>
    /// Educational institution or school name for teacher accounts
    /// </summary>
    [StringLength(200)]
    public string? SchoolName { get; set; }

    /// <summary>
    /// Data processing consent for GDPR compliance
    /// </summary>
    public bool HasGdprConsent { get; set; }

    /// <summary>
    /// Date when GDPR consent was given
    /// </summary>
    public DateTime? GdprConsentDate { get; set; }

    /// <summary>
    /// User's role in the educational context
    /// </summary>
    public UserRole Role { get; set; } = UserRole.Student;

    /// <summary>
    /// Whether the account is active and can access the game
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Last login timestamp for session management
    /// </summary>
    public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Account creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Reference to the player's game profile (if they have one)
    /// </summary>
    public Guid? PlayerId { get; set; }

    /// <summary>
    /// Navigation property to the player's game profile
    /// </summary>
    public Player? Player { get; set; }

    /// <summary>
    /// Whether this is a child account (under 13)
    /// </summary>
    public bool IsChild => CalculateAge() < 13;

    /// <summary>
    /// Whether this account requires enhanced safety features
    /// </summary>
    public bool RequiresChildSafety => IsChild || Role == UserRole.Student;

    /// <summary>
    /// Calculate the user's current age
    /// </summary>
    /// <returns>Age in years</returns>
    public int CalculateAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }
}

/// <summary>
/// User session tracking for child safety and monitoring
/// </summary>
public class UserSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// User who owns this session
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Session token (JWT ID)
    /// </summary>
    [Required]
    public string SessionToken { get; set; } = string.Empty;
    
    /// <summary>
    /// When the session started
    /// </summary>
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the session expires (child sessions have shorter timeouts)
    /// </summary>
    public DateTime ExpiresAt { get; set; }
    
    /// <summary>
    /// Last activity timestamp
    /// </summary>
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Whether this session is still active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// IP address for security monitoring
    /// </summary>
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// User agent for device tracking
    /// </summary>
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// Navigation property to user
    /// </summary>
    public ApplicationUser? User { get; set; }
}

/// <summary>
/// Per-user cost tracking for Azure services (£0.08/user/day target)
/// </summary>
public class UserCostTracking
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// User being tracked
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Date for cost tracking
    /// </summary>
    public DateTime Date { get; set; } = DateTime.UtcNow.Date;
    
    /// <summary>
    /// AI service calls made by this user today
    /// </summary>
    public int AiServiceCalls { get; set; }
    
    /// <summary>
    /// Estimated cost in GBP for AI services
    /// </summary>
    public decimal AiServiceCost { get; set; }
    
    /// <summary>
    /// Speech recognition service usage
    /// </summary>
    public int SpeechServiceCalls { get; set; }
    
    /// <summary>
    /// Estimated cost in GBP for speech services
    /// </summary>
    public decimal SpeechServiceCost { get; set; }
    
    /// <summary>
    /// Content moderation API calls
    /// </summary>
    public int ContentModerationCalls { get; set; }
    
    /// <summary>
    /// Estimated cost in GBP for content moderation
    /// </summary>
    public decimal ContentModerationCost { get; set; }
    
    /// <summary>
    /// Total estimated cost for this user today
    /// </summary>
    public decimal TotalCost => AiServiceCost + SpeechServiceCost + ContentModerationCost;
    
    /// <summary>
    /// Whether daily cost limit has been exceeded
    /// </summary>
    public bool IsOverDailyLimit => TotalCost > 0.08m; // £0.08/user/day limit
    
    /// <summary>
    /// Navigation property to user
    /// </summary>
    public ApplicationUser? User { get; set; }
}

/// <summary>
/// Child safety audit log for COPPA/GDPR compliance
/// </summary>
public class ChildSafetyAudit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// User involved in the safety event
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Type of safety event
    /// </summary>
    public SafetyEventType EventType { get; set; }
    
    /// <summary>
    /// Description of the safety event
    /// </summary>
    [Required]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Severity level of the event
    /// </summary>
    public SafetyEventSeverity Severity { get; set; }
    
    /// <summary>
    /// Whether action was taken
    /// </summary>
    public bool ActionTaken { get; set; }
    
    /// <summary>
    /// Description of action taken
    /// </summary>
    public string? ActionDescription { get; set; }
    
    /// <summary>
    /// When the event occurred
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Navigation property to user
    /// </summary>
    public ApplicationUser? User { get; set; }
}