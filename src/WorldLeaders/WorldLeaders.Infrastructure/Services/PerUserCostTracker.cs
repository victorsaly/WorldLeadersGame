using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
/// Educational Objective: Enhanced per-user cost monitoring with real-time tracking (£0.08/user/day target)
/// Safety Requirements: Cost limit enforcement, granular attribution, UK South region compliance
/// </summary>
public class PerUserCostTracker(
    IOptions<CostTrackingOptions> costTrackingOptions,
    IOptions<BudgetConfig> budgetConfig,
    WorldLeadersDbContext dbContext,
    IMemoryCache memoryCache,
    IAzureCostManagementClient azureCostClient,
    ILogger<PerUserCostTracker> logger) : IRealTimeCostTracker
{
    private readonly CostTrackingOptions _options = costTrackingOptions.Value;
    private readonly BudgetConfig _budgetConfig = budgetConfig.Value;
    private readonly TimeSpan _alertCooldown = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Track real-time cost with educational efficiency scoring
    /// </summary>
    public async Task<EnhancedCostSummaryDto> TrackRealTimeCostAsync(
        Guid userId, 
        string serviceType, 
        decimal estimatedCost,
        Dictionary<string, object>? educationalMetrics = null)
    {
        logger.LogInformation("Tracking real-time cost for user {UserId}: {ServiceType} - £{Cost}", 
            userId, serviceType, estimatedCost);

        try
        {
            if (!_options.Enabled)
            {
                logger.LogInformation("Cost tracking disabled - returning zero cost summary");
                return await GetZeroEnhancedCostSummaryAsync(userId);
            }

            // Track the usage using existing method
            var basicSummary = await TrackUsageAsync(userId, serviceType, estimatedCost);

            // Calculate educational efficiency
            educationalMetrics ??= new Dictionary<string, object>();
            var educationalScore = await azureCostClient.CalculateEducationalEfficiencyAsync(
                basicSummary.TotalCost, educationalMetrics);

            // Convert to enhanced format
            var serviceBreakdown = new Dictionary<string, CostServiceBreakdown>
            {
                ["AI Services"] = new CostServiceBreakdown("AI Services", basicSummary.ServiceCosts["AI Services"], 0, 0),
                ["Speech Services"] = new CostServiceBreakdown("Speech Services", basicSummary.ServiceCosts["Speech Services"], 0, 0),
                ["Content Moderation"] = new CostServiceBreakdown("Content Moderation", basicSummary.ServiceCosts["Content Moderation"], 0, 0)
            };

            // Store real-time data in cache for dashboard
            var realTimeData = new RealTimeCostData(
                UserId: userId,
                Timestamp: DateTime.UtcNow,
                ServiceType: serviceType,
                CostGBP: estimatedCost,
                Region: "UK South",
                Metadata: educationalMetrics ?? new Dictionary<string, object>())
            {
                EducationalEfficiencyScore = educationalScore
            };

            memoryCache.Set($"realtime_cost_{userId}", realTimeData, TimeSpan.FromMinutes(60));

            // Check for budget alerts
            await CheckAndTriggerBudgetAlertAsync(userId, basicSummary.TotalCost);

            var enhancedSummary = new EnhancedCostSummaryDto(
                UserId: userId,
                Date: basicSummary.Date,
                TotalCostGBP: basicSummary.TotalCost,
                IsOverDailyLimit: basicSummary.IsOverDailyLimit,
                RemainingBudgetGBP: basicSummary.RemainingBudget)
            {
                ServiceBreakdown = serviceBreakdown,
                AverageEducationalScore = educationalScore,
                LearningObjectivesAchieved = CalculateLearningObjectives(educationalMetrics),
                ActiveLearningTime = CalculateActiveLearningTime(educationalMetrics),
                IsEducationallyCompliant = educationalScore >= _budgetConfig.EducationalCompliance.TargetEducationalEfficiency,
                ComplianceNotes = educationalScore < _budgetConfig.EducationalCompliance.TargetEducationalEfficiency 
                    ? new List<string> { $"Educational efficiency {educationalScore:F1} below target of {_budgetConfig.EducationalCompliance.TargetEducationalEfficiency}" }
                    : new List<string>()
            };

            logger.LogInformation("Real-time cost tracking completed for user {UserId}. Efficiency: {Score}", 
                userId, educationalScore);

            return enhancedSummary;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in real-time cost tracking for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Check if user should be throttled due to budget limits
    /// </summary>
    public async Task<BudgetThrottlingRecommendation> ShouldThrottleUserAsync(Guid userId)
    {
        try
        {
            if (!_options.Enabled || !_budgetConfig.EmergencyThrottlingEnabled)
            {
                return new BudgetThrottlingRecommendation(
                    ShouldThrottle: false,
                    Reason: "Throttling disabled",
                    CurrentCostGBP: 0,
                    LimitGBP: _budgetConfig.DailyLimitGBP,
                    RecommendedDelay: TimeSpan.Zero);
            }

            var summary = await GetDailyCostSummaryAsync(userId);
            
            if (!summary.IsOverDailyLimit)
            {
                return new BudgetThrottlingRecommendation(
                    ShouldThrottle: false,
                    Reason: "Within budget limits",
                    CurrentCostGBP: summary.TotalCost,
                    LimitGBP: _budgetConfig.DailyLimitGBP,
                    RecommendedDelay: TimeSpan.Zero);
            }

            // Calculate throttling recommendation
            var excessAmount = summary.TotalCost - _budgetConfig.DailyLimitGBP;
            var throttleSeconds = Math.Min(300, (int)(excessAmount * 1000)); // Max 5 minutes
            var recommendedDelay = TimeSpan.FromSeconds(throttleSeconds);

            return new BudgetThrottlingRecommendation(
                ShouldThrottle: true,
                Reason: $"Daily limit of £{_budgetConfig.DailyLimitGBP} exceeded by £{excessAmount:F4}",
                CurrentCostGBP: summary.TotalCost,
                LimitGBP: _budgetConfig.DailyLimitGBP,
                RecommendedDelay: recommendedDelay)
            {
                EducationalImpact = "Minimal - alternative learning activities available",
                AlternativeLearningActivities = new List<string>
                {
                    "Offline geography quiz",
                    "Country flag matching game",
                    "Economic concept review"
                },
                RequiresNotification = excessAmount > 0.02m // Notify if significantly over budget
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking throttling for user {UserId}", userId);
            return new BudgetThrottlingRecommendation(
                ShouldThrottle: false,
                Reason: "Error in throttling check",
                CurrentCostGBP: 0,
                LimitGBP: _budgetConfig.DailyLimitGBP,
                RecommendedDelay: TimeSpan.Zero);
        }
    }

    /// <summary>
    /// Get real-time cost data for monitoring dashboard
    /// </summary>
    public async Task<RealTimeCostData> GetRealTimeCostDataAsync(Guid userId)
    {
        try
        {
            // Try to get from cache first
            if (memoryCache.TryGetValue($"realtime_cost_{userId}", out RealTimeCostData? cachedData) && cachedData != null)
            {
                return cachedData;
            }

            // Fallback to database
            var summary = await GetDailyCostSummaryAsync(userId);
            
            return new RealTimeCostData(
                UserId: userId,
                Timestamp: DateTime.UtcNow,
                ServiceType: "Aggregated",
                CostGBP: summary.TotalCost,
                Region: "UK South",
                Metadata: new Dictionary<string, object>
                {
                    ["ServiceBreakdown"] = summary.ServiceCosts,
                    ["IsOverLimit"] = summary.IsOverDailyLimit,
                    ["RemainingBudget"] = summary.RemainingBudget
                })
            {
                EducationalEfficiencyScore = await CalculateEducationalEfficiencyScoreAsync(userId, TimeSpan.FromDays(1))
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting real-time cost data for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Trigger budget alert if thresholds are exceeded
    /// </summary>
    public async Task<BudgetAlertNotification?> CheckAndTriggerBudgetAlertAsync(Guid userId, decimal currentCost)
    {
        try
        {
            if (!_budgetConfig.AlertConfiguration.EnableRealTimeAlerts)
            {
                return null;
            }

            // Check if we're within alert cooldown period
            var alertCacheKey = $"alert_cooldown_{userId}";
            if (memoryCache.TryGetValue(alertCacheKey, out _))
            {
                return null; // Still in cooldown
            }

            var alertThreshold = _budgetConfig.AlertThresholdGBP;
            var dailyLimit = _budgetConfig.DailyLimitGBP;

            BudgetAlertNotification? alert = null;

            if (currentCost >= dailyLimit)
            {
                alert = new BudgetAlertNotification(
                    UserId: userId,
                    AlertTime: DateTime.UtcNow,
                    AlertType: "Limit",
                    CurrentCostGBP: currentCost,
                    LimitGBP: dailyLimit,
                    Message: _budgetConfig.AlertConfiguration.MessageTemplates.LimitExceededTemplate
                        .Replace("{limit}", dailyLimit.ToString())
                        .Replace("{current}", currentCost.ToString()))
                {
                    EducationalContext = "Learning session budget exceeded",
                    SuggestedActions = new List<string>
                    {
                        "Review learning efficiency",
                        "Consider offline activities",
                        "Contact administrator if needed"
                    },
                    RequiresParentNotification = _budgetConfig.EducationalCompliance.RequireParentNotifications,
                    RequiresSchoolNotification = _budgetConfig.EducationalCompliance.RequireSchoolNotifications
                };
            }
            else if (currentCost >= alertThreshold)
            {
                var percentage = (currentCost / dailyLimit * 100);
                alert = new BudgetAlertNotification(
                    UserId: userId,
                    AlertTime: DateTime.UtcNow,
                    AlertType: "Warning",
                    CurrentCostGBP: currentCost,
                    LimitGBP: dailyLimit,
                    Message: _budgetConfig.AlertConfiguration.MessageTemplates.WarningTemplate
                        .Replace("{percentage}", percentage.ToString("F0"))
                        .Replace("{current}", currentCost.ToString())
                        .Replace("{limit}", dailyLimit.ToString()))
                {
                    EducationalContext = "Learning session approaching budget limit",
                    SuggestedActions = new List<string>
                    {
                        "Monitor usage carefully",
                        "Focus on high-value learning activities"
                    }
                };
            }

            if (alert != null)
            {
                // Set cooldown to prevent alert spam
                memoryCache.Set(alertCacheKey, true, _alertCooldown);
                
                logger.LogWarning("Budget alert triggered for user {UserId}: {AlertType} - £{Current}/£{Limit}", 
                    userId, alert.AlertType, currentCost, dailyLimit);
            }

            return alert;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking budget alerts for user {UserId}", userId);
            return null;
        }
    }

    /// <summary>
    /// Calculate educational efficiency for user's spending
    /// </summary>
    public async Task<decimal> CalculateEducationalEfficiencyScoreAsync(Guid userId, TimeSpan timeframe)
    {
        try
        {
            var startDate = DateTime.UtcNow.Subtract(timeframe);
            var summaries = await GetCostSummariesAsync(new List<Guid> { userId }, startDate, DateTime.UtcNow);
            
            if (!summaries.Any())
            {
                return _budgetConfig.EducationalCompliance.TargetEducationalEfficiency;
            }

            var totalCost = summaries.Sum(s => s.TotalCost);
            
            // Get educational metrics from cache or estimate
            var educationalMetrics = new Dictionary<string, object>
            {
                ["LearningObjectivesAchieved"] = EstimateLearningObjectives(totalCost),
                ["ActiveLearningTimeMinutes"] = EstimateActiveLearningMinutes(totalCost),
                ["ChildSafetyScore"] = 95m // High safety score for educational game
            };

            return await azureCostClient.CalculateEducationalEfficiencyAsync(totalCost, educationalMetrics);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating educational efficiency for user {UserId}", userId);
            return _budgetConfig.EducationalCompliance.TargetEducationalEfficiency;
        }
    }

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

    /// <summary>
    /// Helper method to create zero-cost enhanced summary
    /// </summary>
    private async Task<EnhancedCostSummaryDto> GetZeroEnhancedCostSummaryAsync(Guid userId, DateTime? date = null)
    {
        var targetDate = (date ?? DateTime.UtcNow).Date;
        
        var serviceBreakdown = new Dictionary<string, CostServiceBreakdown>
        {
            ["AI Services"] = new CostServiceBreakdown("AI Services", 0, 0, 0),
            ["Speech Services"] = new CostServiceBreakdown("Speech Services", 0, 0, 0),
            ["Content Moderation"] = new CostServiceBreakdown("Content Moderation", 0, 0, 0)
        };

        return await Task.FromResult(new EnhancedCostSummaryDto(
            UserId: userId,
            Date: targetDate,
            TotalCostGBP: 0,
            IsOverDailyLimit: false,
            RemainingBudgetGBP: _budgetConfig.DailyLimitGBP)
        {
            ServiceBreakdown = serviceBreakdown,
            AverageEducationalScore = _budgetConfig.EducationalCompliance.TargetEducationalEfficiency,
            LearningObjectivesAchieved = 0,
            ActiveLearningTime = TimeSpan.Zero,
            IsEducationallyCompliant = true,
            ComplianceNotes = new List<string>()
        });
    }

    /// <summary>
    /// Calculate learning objectives from educational metrics
    /// </summary>
    private int CalculateLearningObjectives(Dictionary<string, object>? educationalMetrics)
    {
        if (educationalMetrics == null)
            return 0;

        if (educationalMetrics.TryGetValue("LearningObjectivesAchieved", out var objectives) 
            && objectives is int objectiveCount)
        {
            return objectiveCount;
        }

        // Estimate based on other metrics
        if (educationalMetrics.TryGetValue("ActiveLearningTimeMinutes", out var learningTime) 
            && learningTime is int minutes)
        {
            return Math.Max(0, minutes / 15); // 1 objective per 15 minutes
        }

        return 0;
    }

    /// <summary>
    /// Calculate active learning time from educational metrics
    /// </summary>
    private TimeSpan CalculateActiveLearningTime(Dictionary<string, object>? educationalMetrics)
    {
        if (educationalMetrics == null)
            return TimeSpan.Zero;

        if (educationalMetrics.TryGetValue("ActiveLearningTimeMinutes", out var learningTime) 
            && learningTime is int minutes)
        {
            return TimeSpan.FromMinutes(minutes);
        }

        // Estimate based on other metrics
        if (educationalMetrics.TryGetValue("LearningObjectivesAchieved", out var objectives) 
            && objectives is int objectiveCount)
        {
            return TimeSpan.FromMinutes(objectiveCount * 10); // 10 minutes per objective
        }

        return TimeSpan.Zero;
    }

    /// <summary>
    /// Estimate learning objectives based on cost
    /// </summary>
    private int EstimateLearningObjectives(decimal cost)
    {
        // Rough estimate: £0.02 = 1 learning objective
        return Math.Max(0, (int)(cost / 0.02m));
    }

    /// <summary>
    /// Estimate active learning minutes based on cost
    /// </summary>
    private int EstimateActiveLearningMinutes(decimal cost)
    {
        // Rough estimate: £0.01 = 5 minutes of learning
        return Math.Max(0, (int)(cost * 500));
    }
}