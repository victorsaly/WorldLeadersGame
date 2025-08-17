using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WorldLeaders.Infrastructure.Configuration;
using WorldLeaders.Infrastructure.Data;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Models;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.Infrastructure.Services;

/// <summary>
/// Context: Educational game child safety validation for 12-year-old players
/// Educational Objective: COPPA/GDPR compliant safety validation and audit trail
/// Safety Requirements: Content moderation, age verification, parental consent management
/// </summary>
public class ChildSafetyValidator(
    IOptions<ChildSafetyOptions> childSafetyOptions,
    WorldLeadersDbContext dbContext,
    IContentModerationService contentModerationService,
    ILogger<ChildSafetyValidator> logger) : IChildSafetyValidator
{
    private readonly ChildSafetyOptions _options = childSafetyOptions.Value;

    /// <summary>
    /// Validate user registration for child safety compliance
    /// </summary>
    public async Task<ChildSafetyValidationResponse> ValidateRegistrationAsync(RegisterUserRequest request)
    {
        logger.LogInformation("Validating registration for user: {Username}", request.Username);

        var warnings = new List<string>();
        var age = CalculateAge(request.DateOfBirth);

        try
        {
            // Check age requirements
            if (age < 0)
            {
                return new ChildSafetyValidationResponse
                {
                    IsApproved = false,
                    Reason = "Invalid date of birth - future date not allowed",
                    ConfidenceScore = 1.0,
                    Warnings = warnings
                };
            }

            if (age > 100)
            {
                warnings.Add("Unusually high age detected - please verify date of birth");
            }

            // Child-specific validation (under 13)
            if (age < _options.ChildAgeThreshold)
            {
                // COPPA compliance checks
                if (_options.RequireParentalConsent && !request.HasParentalConsent)
                {
                    return new ChildSafetyValidationResponse
                    {
                        IsApproved = false,
                        Reason = "Parental consent required for users under 13 (COPPA compliance)",
                        ConfidenceScore = 1.0,
                        Warnings = warnings
                    };
                }

                if (string.IsNullOrEmpty(request.ParentalEmail))
                {
                    return new ChildSafetyValidationResponse
                    {
                        IsApproved = false,
                        Reason = "Parental email required for users under 13",
                        ConfidenceScore = 1.0,
                        Warnings = warnings
                    };
                }

                warnings.Add("Child account detected - enhanced safety features will be enabled");
            }

            // GDPR compliance check
            if (_options.EnforceGdprCompliance && !request.HasGdprConsent)
            {
                return new ChildSafetyValidationResponse
                {
                    IsApproved = false,
                    Reason = "GDPR consent required for data processing",
                    ConfidenceScore = 1.0,
                    Warnings = warnings
                };
            }

            // Validate display name for appropriateness
            var nameValidation = await ValidateContentAsync(new ChildSafetyValidationRequest
            {
                UserId = Guid.Empty, // New user, no ID yet
                Content = request.DisplayName,
                ValidationType = "DisplayName"
            });

            if (!nameValidation.IsApproved)
            {
                return new ChildSafetyValidationResponse
                {
                    IsApproved = false,
                    Reason = $"Display name inappropriate: {nameValidation.Reason}",
                    ConfidenceScore = nameValidation.ConfidenceScore,
                    Warnings = warnings.Concat(nameValidation.Warnings).ToList()
                };
            }

            // Validate username for appropriateness
            var usernameValidation = await ValidateContentAsync(new ChildSafetyValidationRequest
            {
                UserId = Guid.Empty, // New user, no ID yet
                Content = request.Username,
                ValidationType = "Username"
            });

            if (!usernameValidation.IsApproved)
            {
                return new ChildSafetyValidationResponse
                {
                    IsApproved = false,
                    Reason = $"Username inappropriate: {usernameValidation.Reason}",
                    ConfidenceScore = usernameValidation.ConfidenceScore,
                    Warnings = warnings.Concat(usernameValidation.Warnings).ToList()
                };
            }

            logger.LogInformation("Registration validation passed for user: {Username}", request.Username);

            return new ChildSafetyValidationResponse
            {
                IsApproved = true,
                Reason = "Registration meets all child safety requirements",
                ConfidenceScore = 0.95,
                Warnings = warnings
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during registration validation for user: {Username}", request.Username);
            
            return new ChildSafetyValidationResponse
            {
                IsApproved = false,
                Reason = "Validation service error - please try again",
                ConfidenceScore = 0.0,
                Warnings = warnings
            };
        }
    }

    /// <summary>
    /// Validate content for child appropriateness
    /// </summary>
    public async Task<ChildSafetyValidationResponse> ValidateContentAsync(ChildSafetyValidationRequest request)
    {
        try
        {
            // Use content moderation service for primary validation
            var moderationResult = await contentModerationService.ValidateContentAsync(request.Content, request.ValidationType);

            // Additional custom validation for educational context
            var customValidation = ValidateEducationalContent(request.Content, request.ValidationType);

            // Combine results
            var isApproved = moderationResult.IsApproved && customValidation.IsApproved;
            var reason = isApproved 
                ? "Content approved for educational use" 
                : $"{moderationResult.Reason}; {customValidation.Reason}".Trim(';', ' ');

            var warnings = new List<string>();
            if (moderationResult.Concerns?.Any() == true)
                warnings.AddRange(moderationResult.Concerns);
            if (customValidation.Warnings?.Any() == true)
                warnings.AddRange(customValidation.Warnings);

            var confidenceScore = Math.Min(moderationResult.ConfidenceScore, customValidation.ConfidenceScore);

            // Log validation result
            if (_options.LogAllEvents)
            {
                await LogSafetyEventAsync(new ChildSafetyAudit
                {
                    UserId = request.UserId,
                    EventType = SafetyEventType.ContentFlagged,
                    Description = $"Content validation: {request.ValidationType} - {(isApproved ? "Approved" : "Rejected")}",
                    Severity = isApproved ? SafetyEventSeverity.Info : SafetyEventSeverity.Medium,
                    ActionTaken = !isApproved,
                    ActionDescription = isApproved ? null : $"Content blocked: {reason}"
                });
            }

            return new ChildSafetyValidationResponse
            {
                IsApproved = isApproved,
                Reason = reason,
                ConfidenceScore = confidenceScore,
                Warnings = warnings
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating content for user: {UserId}", request.UserId);
            
            return new ChildSafetyValidationResponse
            {
                IsApproved = false,
                Reason = "Content validation service error",
                ConfidenceScore = 0.0,
                Warnings = new List<string> { "Validation service temporarily unavailable" }
            };
        }
    }

    /// <summary>
    /// Check if user requires parental consent based on age
    /// </summary>
    public bool RequiresParentalConsent(DateTime dateOfBirth)
    {
        if (!_options.RequireParentalConsent)
            return false;

        var age = CalculateAge(dateOfBirth);
        return age < _options.ChildAgeThreshold;
    }

    /// <summary>
    /// Validate parental consent token (simplified implementation)
    /// </summary>
    public async Task<bool> ValidateParentalConsentAsync(string consentToken)
    {
        try
        {
            // In a real implementation, this would validate a cryptographically signed token
            // For now, we'll do basic validation
            
            if (string.IsNullOrEmpty(consentToken) || consentToken.Length < 32)
            {
                return false;
            }

            // Check if token exists in database and is still valid
            // TODO: In production, implement a proper ParentalConsent entity with tokens
            // For now, validate token format and check user consent status
            
            // Validate token format (should be a proper JWT or signed token)
            if (string.IsNullOrEmpty(consentToken) || consentToken.Length < 32)
            {
                logger.LogWarning("Invalid parental consent token format");
                return false;
            }

            // In a production system, you would:
            // var consentRecord = await dbContext.ParentalConsents
            //     .FirstOrDefaultAsync(pc => pc.Token == consentToken && pc.ExpiresAt > DateTime.UtcNow);
            // return consentRecord != null;
            
            // For development/educational purposes, validate token has proper structure
            // Real implementation would verify cryptographic signature and expiration
            try
            {
                // Basic token structure validation (placeholder for proper JWT/token validation)
                var parts = consentToken.Split('.');
                if (parts.Length >= 2) // JWT-like structure
                {
                    // Simulate database lookup delay
                    await Task.Delay(50);
                    
                    // In development, accept well-formed tokens
                    // TODO: Replace with actual database lookup and cryptographic validation
                    return true;
                }
                
                logger.LogWarning("Parental consent token structure invalid");
                return false;
            }
            catch (Exception tokenEx)
            {
                logger.LogError(tokenEx, "Error parsing parental consent token");
                return false;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating parental consent token");
            return false;
        }
    }

    /// <summary>
    /// Generate parental consent request token
    /// </summary>
    public async Task<string> GenerateParentalConsentRequestAsync(Guid childUserId, string parentalEmail)
    {
        try
        {
            // In a real implementation, this would generate a cryptographically secure token
            // and send an email to the parent with a consent link
            
            var token = $"{childUserId}_{parentalEmail}_{DateTime.UtcNow.Ticks}";
            var base64Token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));
            
            logger.LogInformation("Generated parental consent token for child: {ChildUserId}", childUserId);
            
            // In production, send email here
            await Task.Delay(100); // Simulate async email operation
            
            return base64Token;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating parental consent token for child: {ChildUserId}", childUserId);
            throw;
        }
    }

    /// <summary>
    /// Log safety event for audit trail
    /// </summary>
    public async Task<bool> LogSafetyEventAsync(ChildSafetyAudit audit)
    {
        try
        {
            dbContext.ChildSafetyAudits.Add(audit);
            await dbContext.SaveChangesAsync();
            
            logger.LogInformation("Safety event logged: {EventType} for user {UserId}", audit.EventType, audit.UserId);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error logging safety event: {EventType} for user {UserId}", audit.EventType, audit.UserId);
            return false;
        }
    }

    /// <summary>
    /// Calculate age from date of birth
    /// </summary>
    private static int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }

    /// <summary>
    /// Custom validation for educational content
    /// </summary>
    private ChildSafetyValidationResponse ValidateEducationalContent(string content, string validationType)
    {
        var warnings = new List<string>();
        
        // Basic educational content validation
        if (string.IsNullOrWhiteSpace(content))
        {
            return new ChildSafetyValidationResponse
            {
                IsApproved = false,
                Reason = "Content cannot be empty",
                ConfidenceScore = 1.0,
                Warnings = warnings
            };
        }

        // Check for length limits based on type
        var maxLength = validationType.ToLowerInvariant() switch
        {
            "username" => 50,
            "displayname" => 50,
            "message" => 1000,
            "gamecontent" => 500,
            _ => 1000
        };

        if (content.Length > maxLength)
        {
            return new ChildSafetyValidationResponse
            {
                IsApproved = false,
                Reason = $"Content exceeds maximum length of {maxLength} characters",
                ConfidenceScore = 1.0,
                Warnings = warnings
            };
        }

        // Enhanced inappropriate content detection with pattern matching
        var inappropriateWords = new[]
        {
            "password", "secret", "private", "personal", "address", "phone",
            "email", "contact", "meet", "alone", "gift", "money"
        };

        var lowerContent = content.ToLowerInvariant();
        var foundInappropriateWord = inappropriateWords.Any(pattern => lowerContent.Contains(pattern));

        // Enhanced pattern detection for personal information
        var foundInappropriatePattern = false;
        var violationReason = "";

        // Detect address patterns (street addresses)
        if (System.Text.RegularExpressions.Regex.IsMatch(lowerContent, @"\d+\s+([\w\s]+\s+)?(street|st|avenue|ave|road|rd|drive|dr|lane|ln|way|blvd|boulevard)"))
        {
            foundInappropriatePattern = true;
            violationReason = "street address detected";
        }

        // Detect phone number patterns
        if (System.Text.RegularExpressions.Regex.IsMatch(lowerContent, @"(\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}|\d{10})"))
        {
            foundInappropriatePattern = true;
            violationReason = "phone number detected";
        }

        // Detect email patterns (simple check for @ symbol in email context)
        if (System.Text.RegularExpressions.Regex.IsMatch(lowerContent, @"\b[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}\b"))
        {
            foundInappropriatePattern = true;
            violationReason = "email address detected";
        }

        var foundInappropriate = foundInappropriateWord || foundInappropriatePattern;

        if (foundInappropriate)
        {
            if (!string.IsNullOrEmpty(violationReason))
            {
                warnings.Add($"Content may contain personal information - {violationReason}");
            }
            else
            {
                warnings.Add("Content may contain personal information - please review");
            }
        }

        // Educational appropriateness check
        if (validationType.ToLowerInvariant() == "gamecontent")
        {
            var educationalKeywords = new[] { "country", "geography", "language", "leader", "economic", "learn" };
            var hasEducationalContent = educationalKeywords.Any(keyword => lowerContent.Contains(keyword));
            
            if (!hasEducationalContent)
            {
                warnings.Add("Content may not be educational - consider adding learning elements");
            }
        }

        return new ChildSafetyValidationResponse
        {
            IsApproved = !foundInappropriate,
            Reason = foundInappropriate ? "Content may contain inappropriate information" : "Content meets educational standards",
            ConfidenceScore = foundInappropriate ? 0.9 : 0.8, // High confidence when clearly inappropriate
            Warnings = warnings
        };
    }

    /// <summary>
    /// Validate content for child safety and educational appropriateness (interface compatibility)
    /// </summary>
    public async Task<ContentSafetyResult> ValidateContentAsync(string content, string contentType)
    {
        try
        {
            var validation = await ValidateContentAsync(new ChildSafetyValidationRequest 
            { 
                UserId = Guid.Empty, // Anonymous validation
                Content = content, 
                ValidationType = contentType 
            });

            return new ContentSafetyResult(
                validation.IsApproved,
                validation.Reason,
                validation.ConfidenceScore,
                validation.Warnings
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating content safety");
            return new ContentSafetyResult(false, "Validation error", 0.0, new List<string> { "Validation failed" });
        }
    }

    /// <summary>
    /// Quick safety check for real-time validation (interface compatibility)
    /// </summary>
    public async Task<bool> IsContentSafeForChildrenAsync(string content)
    {
        try
        {
            var result = await ValidateContentAsync(content, "general");
            return result.IsApproved;
        }
        catch
        {
            return false; // Fail safe
        }
    }

    /// <summary>
    /// Validate that user input doesn't contain personal information (interface compatibility)
    /// </summary>
    public async Task<bool> ValidateUserInputSafetyAsync(string input)
    {
        try
        {
            var result = await ValidateContentAsync(input, "userinput");
            return result.IsApproved;
        }
        catch
        {
            return false; // Fail safe
        }
    }

    /// <summary>
    /// Check if content is age-appropriate for 12-year-olds (interface compatibility)
    /// </summary>
    public async Task<bool> IsAgeAppropriateAsync(string content)
    {
        try
        {
            var result = await ValidateContentAsync(content, "agechecks");
            return result.IsApproved;
        }
        catch
        {
            return false; // Fail safe
        }
    }
}