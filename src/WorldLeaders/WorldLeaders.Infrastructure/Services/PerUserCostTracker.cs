using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WorldLeaders.Infrastructure.Configuration;
using WorldLeaders.Infrastructure.Data;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Models;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.Infrastructure.Services;

/// <summary>
/// Context: Educational game cost tracking for 12-year-old players
/// Educational Objective: Per-user cost monitoring for Azure services (£0.08/user/day target)
/// Safety Requirements: Cost limit enforcement, granular attribution, UK South region compliance
/// </summary>
public class PerUserCostTracker(
    IOptions<CostTrackingOptions> costTrackingOptions,
    WorldLeadersDbContext dbContext,
    ILogger<PerUserCostTracker> logger) : IPerUserCostTracker
{
    private readonly CostTrackingOptions _options = costTrackingOptions.Value;

    /// <summary>
    /// Track Azure service usage for a user with granular cost attribution
    /// </summary>
    public async Task<UserCostSummaryDto> TrackUsageAsync(Guid userId, string serviceType, decimal estimatedCost)
    {
        logger.LogInformation("Tracking usage for user {UserId}: {ServiceType} - £{Cost}", 
            userId, serviceType, estimatedCost);

        try
        {
            if (!_options.Enabled)
            {
                logger.LogInformation("Cost tracking disabled - returning zero cost summary");
                return await GetZeroCostSummaryAsync(userId);
            }

            var today = DateTime.UtcNow.Date;
            
            // Get or create cost tracking record for today
            var costTracking = await dbContext.UserCostTracking
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Date == today);

            if (costTracking == null)
            {
                costTracking = new UserCostTracking
                {
                    UserId = userId,
                    Date = today,
                    AiServiceCalls = 0,
                    AiServiceCost = 0,
                    SpeechServiceCalls = 0,
                    SpeechServiceCost = 0,
                    ContentModerationCalls = 0,
                    ContentModerationCost = 0
                };
                dbContext.UserCostTracking.Add(costTracking);
            }

            // Update costs based on service type
            switch (serviceType.ToLowerInvariant())
            {
                case "openai":
                case "azureopenai":
                case "ai":
                    costTracking.AiServiceCalls++;
                    costTracking.AiServiceCost += estimatedCost;
                    break;

                case "speech":
                case "speechservices":
                case "azurespeech":
                    costTracking.SpeechServiceCalls++;
                    costTracking.SpeechServiceCost += estimatedCost;
                    break;

                case "contentmoderator":
                case "moderation":
                case "safety":
                    costTracking.ContentModerationCalls++;
                    costTracking.ContentModerationCost += estimatedCost;
                    break;

                default:
                    logger.LogWarning("Unknown service type for cost tracking: {ServiceType}", serviceType);
                    // Default to AI service cost for unknown services
                    costTracking.AiServiceCalls++;
                    costTracking.AiServiceCost += estimatedCost;
                    break;
            }

            await dbContext.SaveChangesAsync();

            var totalCost = costTracking.AiServiceCost + costTracking.SpeechServiceCost + costTracking.ContentModerationCost;
            var isOverLimit = totalCost > _options.DailyCostLimitGBP;

            if (isOverLimit && _options.BlockOnLimitExceeded)
            {
                logger.LogWarning("User {UserId} exceeded daily cost limit: £{TotalCost} > £{Limit}", 
                    userId, totalCost, _options.DailyCostLimitGBP);
            }

            return new UserCostSummaryDto
            {
                UserId = userId,
                Date = today,
                TotalCost = totalCost,
                IsOverDailyLimit = isOverLimit,
                RemainingBudget = Math.Max(0, _options.DailyCostLimitGBP - totalCost),
                ServiceCosts = new Dictionary<string, decimal>
                {
                    ["AI Services"] = costTracking.AiServiceCost,
                    ["Speech Services"] = costTracking.SpeechServiceCost,
                    ["Content Moderation"] = costTracking.ContentModerationCost
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error tracking usage for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Get daily cost summary for a user
    /// </summary>
    public async Task<UserCostSummaryDto> GetDailyCostSummaryAsync(Guid userId, DateTime? date = null)
    {
        try
        {
            var targetDate = (date ?? DateTime.UtcNow).Date;
            
            if (!_options.Enabled)
            {
                return await GetZeroCostSummaryAsync(userId, targetDate);
            }

            var costTracking = await dbContext.UserCostTracking
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Date == targetDate);

            if (costTracking == null)
            {
                return await GetZeroCostSummaryAsync(userId, targetDate);
            }

            var totalCost = costTracking.AiServiceCost + costTracking.SpeechServiceCost + costTracking.ContentModerationCost;
            var isOverLimit = totalCost > _options.DailyCostLimitGBP;

            return new UserCostSummaryDto
            {
                UserId = userId,
                Date = targetDate,
                TotalCost = totalCost,
                IsOverDailyLimit = isOverLimit,
                RemainingBudget = Math.Max(0, _options.DailyCostLimitGBP - totalCost),
                ServiceCosts = new Dictionary<string, decimal>
                {
                    ["AI Services"] = costTracking.AiServiceCost,
                    ["Speech Services"] = costTracking.SpeechServiceCost,
                    ["Content Moderation"] = costTracking.ContentModerationCost
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting daily cost summary for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Check if user has exceeded daily cost limit
    /// </summary>
    public async Task<bool> IsOverDailyLimitAsync(Guid userId)
    {
        try
        {
            if (!_options.Enabled || !_options.BlockOnLimitExceeded)
            {
                return false;
            }

            var summary = await GetDailyCostSummaryAsync(userId);
            return summary.IsOverDailyLimit;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking daily limit for user {UserId}", userId);
            return false; // Don't block on errors
        }
    }

    /// <summary>
    /// Get cost summaries for multiple users (admin view)
    /// </summary>
    public async Task<List<UserCostSummaryDto>> GetCostSummariesAsync(List<Guid> userIds, DateTime startDate, DateTime endDate)
    {
        try
        {
            if (!_options.Enabled)
            {
                return userIds.Select(id => GetZeroCostSummaryAsync(id, startDate).Result).ToList();
            }

            var costTrackings = await dbContext.UserCostTracking
                .Where(c => userIds.Contains(c.UserId) && 
                           c.Date >= startDate.Date && 
                           c.Date <= endDate.Date)
                .ToListAsync();

            var summaries = new List<UserCostSummaryDto>();

            foreach (var userId in userIds)
            {
                var userCosts = costTrackings.Where(c => c.UserId == userId).ToList();
                
                if (!userCosts.Any())
                {
                    summaries.Add(await GetZeroCostSummaryAsync(userId, startDate));
                    continue;
                }

                var totalAiCost = userCosts.Sum(c => c.AiServiceCost);
                var totalSpeechCost = userCosts.Sum(c => c.SpeechServiceCost);
                var totalModerationCost = userCosts.Sum(c => c.ContentModerationCost);
                var totalCost = totalAiCost + totalSpeechCost + totalModerationCost;

                // For multi-day summaries, check if any single day exceeded the limit
                var maxDailyCost = userCosts.Max(c => c.AiServiceCost + c.SpeechServiceCost + c.ContentModerationCost);
                var isOverLimit = maxDailyCost > _options.DailyCostLimitGBP;

                summaries.Add(new UserCostSummaryDto
                {
                    UserId = userId,
                    Date = startDate,
                    TotalCost = totalCost,
                    IsOverDailyLimit = isOverLimit,
                    RemainingBudget = Math.Max(0, _options.DailyCostLimitGBP - maxDailyCost),
                    ServiceCosts = new Dictionary<string, decimal>
                    {
                        ["AI Services"] = totalAiCost,
                        ["Speech Services"] = totalSpeechCost,
                        ["Content Moderation"] = totalModerationCost
                    }
                });
            }

            logger.LogInformation("Retrieved cost summaries for {UserCount} users from {StartDate} to {EndDate}", 
                userIds.Count, startDate, endDate);

            return summaries;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting cost summaries for users");
            throw;
        }
    }

    /// <summary>
    /// Reset daily costs (typically run at midnight UTC)
    /// </summary>
    public async Task<int> ResetDailyCostsAsync()
    {
        try
        {
            if (!_options.Enabled)
            {
                logger.LogInformation("Cost tracking disabled - no reset needed");
                return 0;
            }

            var yesterday = DateTime.UtcNow.Date.AddDays(-1);
            
            // Archive costs older than retention period
            var retentionDays = 90; // 90 days retention
            var retentionCutoff = DateTime.UtcNow.Date.AddDays(-retentionDays);
            
            var oldCosts = await dbContext.UserCostTracking
                .Where(c => c.Date < retentionCutoff)
                .ToListAsync();

            if (oldCosts.Any())
            {
                dbContext.UserCostTracking.RemoveRange(oldCosts);
                logger.LogInformation("Archived {Count} old cost tracking records", oldCosts.Count);
            }

            await dbContext.SaveChangesAsync();

            // Count active users from yesterday for reporting
            var activeUsers = await dbContext.UserCostTracking
                .Where(c => c.Date == yesterday)
                .CountAsync();

            logger.LogInformation("Daily cost reset completed. {ActiveUsers} users had activity yesterday", activeUsers);

            return activeUsers;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during daily cost reset");
            throw;
        }
    }

    /// <summary>
    /// Helper method to create zero-cost summary
    /// </summary>
    private async Task<UserCostSummaryDto> GetZeroCostSummaryAsync(Guid userId, DateTime? date = null)
    {
        var targetDate = (date ?? DateTime.UtcNow).Date;
        
        return await Task.FromResult(new UserCostSummaryDto
        {
            UserId = userId,
            Date = targetDate,
            TotalCost = 0,
            IsOverDailyLimit = false,
            RemainingBudget = _options.DailyCostLimitGBP,
            ServiceCosts = new Dictionary<string, decimal>
            {
                ["AI Services"] = 0,
                ["Speech Services"] = 0,
                ["Content Moderation"] = 0
            }
        });
    }
}