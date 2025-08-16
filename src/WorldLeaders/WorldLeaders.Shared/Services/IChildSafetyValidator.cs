using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Models;

namespace WorldLeaders.Shared.Services;

/// <summary>
/// Child safety validation service for comprehensive content protection
/// Educational focus: Ensure all content is safe and appropriate for 12-year-old players
/// </summary>
public interface IChildSafetyValidator
{
    /// <summary>
    /// Validate content for child safety and educational appropriateness
    /// </summary>
    /// <param name="content">The content to validate</param>
    /// <param name="contentType">Type of content being validated</param>
    /// <returns>Validation result with safety assessment</returns>
    Task<ContentSafetyResult> ValidateContentAsync(string content, string contentType);
    
    /// <summary>
    /// Quick safety check for real-time validation
    /// </summary>
    /// <param name="content">The content to check</param>
    /// <returns>True if content is safe for children</returns>
    Task<bool> IsContentSafeForChildrenAsync(string content);
    
    /// <summary>
    /// Validate that user input doesn't contain personal information
    /// </summary>
    /// <param name="input">User input to validate</param>
    /// <returns>True if input is safe (no personal info detected)</returns>
    Task<bool> ValidateUserInputSafetyAsync(string input);
    
    /// <summary>
    /// Check if content is age-appropriate for 12-year-olds
    /// </summary>
    /// <param name="content">Content to check</param>
    /// <returns>True if age-appropriate</returns>
    Task<bool> IsAgeAppropriateAsync(string content);

    /// <summary>
    /// Validate user registration for child safety compliance
    /// </summary>
    /// <param name="request">Registration request</param>
    /// <returns>Validation result</returns>
    Task<ChildSafetyValidationResponse> ValidateRegistrationAsync(RegisterUserRequest request);

    /// <summary>
    /// Validate content for child appropriateness
    /// </summary>
    /// <param name="request">Content validation request</param>
    /// <returns>Validation result</returns>
    Task<ChildSafetyValidationResponse> ValidateContentAsync(ChildSafetyValidationRequest request);

    /// <summary>
    /// Check if user requires parental consent
    /// </summary>
    /// <param name="dateOfBirth">User's date of birth</param>
    /// <returns>Whether parental consent is required</returns>
    bool RequiresParentalConsent(DateTime dateOfBirth);

    /// <summary>
    /// Validate parental consent token
    /// </summary>
    /// <param name="consentToken">Consent token from parental email</param>
    /// <returns>Whether consent is valid</returns>
    Task<bool> ValidateParentalConsentAsync(string consentToken);

    /// <summary>
    /// Generate parental consent request
    /// </summary>
    /// <param name="childUserId">Child user ID</param>
    /// <param name="parentalEmail">Parental email</param>
    /// <returns>Consent token for email verification</returns>
    Task<string> GenerateParentalConsentRequestAsync(Guid childUserId, string parentalEmail);

    /// <summary>
    /// Log safety event for audit trail
    /// </summary>
    /// <param name="audit">Safety audit entry</param>
    /// <returns>Success status</returns>
    Task<bool> LogSafetyEventAsync(ChildSafetyAudit audit);
}

/// <summary>
/// Result of child safety content validation
/// </summary>
public record ContentSafetyResult(
    bool IsApproved,
    string Reason,
    double ConfidenceScore,
    List<string> Warnings
);
