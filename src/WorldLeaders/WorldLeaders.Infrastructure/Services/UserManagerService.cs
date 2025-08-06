using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WorldLeaders.Infrastructure.Data;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Models;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.Infrastructure.Services;

/// <summary>
/// Context: Educational game user management for 12-year-old players
/// Educational Objective: Secure user administration with child safety compliance
/// Safety Requirements: COPPA/GDPR compliance, audit trail, parental oversight
/// </summary>
public class UserManagerService(
    UserManager<ApplicationUser> userManager,
    WorldLeadersDbContext dbContext,
    IChildSafetyValidator childSafetyValidator,
    ILogger<UserManagerService> logger) : IUserManagerService
{
    /// <summary>
    /// Get user by ID with safety checks
    /// </summary>
    public async Task<ApplicationUser?> GetUserByIdAsync(Guid userId)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            
            if (user != null && !user.IsActive)
            {
                logger.LogWarning("Attempted to access inactive user: {UserId}", userId);
                return null;
            }

            return user;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Get user by username or email
    /// </summary>
    public async Task<ApplicationUser?> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
    {
        try
        {
            var user = await userManager.FindByNameAsync(usernameOrEmail) 
                ?? await userManager.FindByEmailAsync(usernameOrEmail);

            if (user != null && !user.IsActive)
            {
                logger.LogWarning("Attempted to access inactive user: {UsernameOrEmail}", usernameOrEmail);
                return null;
            }

            return user;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user by username/email: {UsernameOrEmail}", usernameOrEmail);
            throw;
        }
    }

    /// <summary>
    /// Update user profile with validation
    /// </summary>
    public async Task<bool> UpdateUserAsync(Guid userId, Dictionary<string, object> updates)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                logger.LogWarning("User not found for update: {UserId}", userId);
                return false;
            }

            var hasChanges = false;

            foreach (var update in updates)
            {
                switch (update.Key.ToLowerInvariant())
                {
                    case "displayname":
                        if (update.Value is string displayName && !string.IsNullOrEmpty(displayName))
                        {
                            // Validate display name for child safety
                            var validation = await childSafetyValidator.ValidateContentAsync(new()
                            {
                                UserId = userId,
                                Content = displayName,
                                ValidationType = "DisplayName"
                            });

                            if (!validation.IsApproved)
                            {
                                logger.LogWarning("Display name update rejected for user {UserId}: {Reason}", 
                                    userId, validation.Reason);
                                return false;
                            }

                            user.DisplayName = displayName;
                            hasChanges = true;
                        }
                        break;

                    case "parentalemail":
                        if (update.Value is string parentalEmail)
                        {
                            user.ParentalEmail = parentalEmail;
                            hasChanges = true;
                        }
                        break;

                    case "schoolname":
                        if (update.Value is string schoolName)
                        {
                            user.SchoolName = schoolName;
                            hasChanges = true;
                        }
                        break;

                    case "hasparentalconsent":
                        if (update.Value is bool parentalConsent)
                        {
                            user.HasParentalConsent = parentalConsent;
                            
                            // Log safety event for parental consent change
                            await childSafetyValidator.LogSafetyEventAsync(new ChildSafetyAudit
                            {
                                UserId = userId,
                                EventType = SafetyEventType.ParentalConsent,
                                Description = $"Parental consent updated to: {parentalConsent}",
                                Severity = SafetyEventSeverity.Info,
                                ActionTaken = true,
                                ActionDescription = "Parental consent status updated"
                            });
                            
                            hasChanges = true;
                        }
                        break;

                    case "hasgdprconsent":
                        if (update.Value is bool gdprConsent)
                        {
                            user.HasGdprConsent = gdprConsent;
                            user.GdprConsentDate = gdprConsent ? DateTime.UtcNow : null;
                            
                            // Log safety event for GDPR consent change
                            await childSafetyValidator.LogSafetyEventAsync(new ChildSafetyAudit
                            {
                                UserId = userId,
                                EventType = SafetyEventType.GdprRequest,
                                Description = $"GDPR consent updated to: {gdprConsent}",
                                Severity = SafetyEventSeverity.Info,
                                ActionTaken = true,
                                ActionDescription = "GDPR consent status updated"
                            });
                            
                            hasChanges = true;
                        }
                        break;

                    default:
                        logger.LogWarning("Unknown update field: {Field} for user {UserId}", update.Key, userId);
                        break;
                }
            }

            if (hasChanges)
            {
                user.UpdatedAt = DateTime.UtcNow;
                var result = await userManager.UpdateAsync(user);
                
                if (result.Succeeded)
                {
                    logger.LogInformation("User updated successfully: {UserId}", userId);
                    return true;
                }
                else
                {
                    logger.LogError("User update failed: {UserId}. Errors: {Errors}", 
                        userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return false;
                }
            }

            return true; // No changes needed
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user: {UserId}", userId);
            return false;
        }
    }

    /// <summary>
    /// Deactivate user account (soft delete with audit trail)
    /// </summary>
    public async Task<bool> DeactivateUserAsync(Guid userId, string reason)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                logger.LogWarning("User not found for deactivation: {UserId}", userId);
                return false;
            }

            if (!user.IsActive)
            {
                logger.LogWarning("User already inactive: {UserId}", userId);
                return true;
            }

            // Soft delete - mark as inactive instead of deleting
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            // Terminate all active sessions
            await TerminateAllSessionsAsync(userId);

            var result = await userManager.UpdateAsync(user);
            
            if (result.Succeeded)
            {
                // Log safety event for account deactivation
                await childSafetyValidator.LogSafetyEventAsync(new ChildSafetyAudit
                {
                    UserId = userId,
                    EventType = SafetyEventType.PolicyViolation,
                    Description = $"Account deactivated. Reason: {reason}",
                    Severity = SafetyEventSeverity.High,
                    ActionTaken = true,
                    ActionDescription = "User account deactivated"
                });

                logger.LogInformation("User deactivated: {UserId}. Reason: {Reason}", userId, reason);
                return true;
            }
            else
            {
                logger.LogError("User deactivation failed: {UserId}. Errors: {Errors}", 
                    userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deactivating user: {UserId}", userId);
            return false;
        }
    }

    /// <summary>
    /// Get all active sessions for a user
    /// </summary>
    public async Task<List<UserSession>> GetActiveSessionsAsync(Guid userId)
    {
        try
        {
            var sessions = await dbContext.UserSessions
                .Where(s => s.UserId == userId && s.IsActive && s.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(s => s.LastActivityAt)
                .ToListAsync();

            logger.LogInformation("Retrieved {SessionCount} active sessions for user: {UserId}", 
                sessions.Count, userId);

            return sessions;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting active sessions for user: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Terminate all sessions for a user
    /// </summary>
    public async Task<int> TerminateAllSessionsAsync(Guid userId)
    {
        try
        {
            var activeSessions = await dbContext.UserSessions
                .Where(s => s.UserId == userId && s.IsActive)
                .ToListAsync();

            foreach (var session in activeSessions)
            {
                session.IsActive = false;
            }

            await dbContext.SaveChangesAsync();

            // Log safety event for session termination
            if (activeSessions.Any())
            {
                await childSafetyValidator.LogSafetyEventAsync(new ChildSafetyAudit
                {
                    UserId = userId,
                    EventType = SafetyEventType.SessionTimeout,
                    Description = $"All sessions terminated. Count: {activeSessions.Count}",
                    Severity = SafetyEventSeverity.Info,
                    ActionTaken = true,
                    ActionDescription = "All user sessions terminated"
                });
            }

            logger.LogInformation("Terminated {SessionCount} sessions for user: {UserId}", 
                activeSessions.Count, userId);

            return activeSessions.Count;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error terminating sessions for user: {UserId}", userId);
            throw;
        }
    }
}