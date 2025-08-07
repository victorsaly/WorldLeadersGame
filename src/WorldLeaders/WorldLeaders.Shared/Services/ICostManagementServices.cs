using WorldLeaders.Shared.DTOs;

namespace WorldLeaders.Shared.Services;

/// <summary>
/// Context: Educational game Azure cost management for 12-year-old players
/// Educational Objective: Real-time Azure cost tracking with UK South regional pricing
/// Safety Requirements: Budget protection, educational compliance, GDPR compliance
/// </summary>
public interface IAzureCostManagementClient
{
    /// <summary>
    /// Query Azure Cost Management API for real-time cost data
    /// </summary>
    /// <param name="request">Cost query request with educational filters</param>
    /// <returns>Cost data with UK South regional pricing</returns>
    Task<AzureCostQueryResponse> QueryCostsAsync(AzureCostQueryRequest request);

    /// <summary>
    /// Get real-time cost data for specific educational resources
    /// </summary>
    /// <param name="subscriptionId">Azure subscription ID</param>
    /// <param name="resourceGroupName">Educational resource group</param>
    /// <param name="startDate">Query start date</param>
    /// <param name="endDate">Query end date</param>
    /// <returns>Enhanced cost summary with educational metrics</returns>
    Task<EnhancedCostSummaryDto> GetEducationalResourceCostsAsync(
        string subscriptionId, 
        string resourceGroupName, 
        DateTime startDate, 
        DateTime endDate);

    /// <summary>
    /// Get UK South regional pricing information
    /// </summary>
    /// <param name="serviceNames">Azure services to get pricing for</param>
    /// <returns>Regional pricing data in GBP</returns>
    Task<Dictionary<string, decimal>> GetUKSouthPricingAsync(List<string> serviceNames);

    /// <summary>
    /// Calculate educational efficiency score from Azure cost data
    /// </summary>
    /// <param name="costs">Cost data</param>
    /// <param name="educationalMetrics">Learning outcomes achieved</param>
    /// <returns>Educational efficiency score (target: 85+ points/£)</returns>
    Task<decimal> CalculateEducationalEfficiencyAsync(
        decimal costs, 
        Dictionary<string, object> educationalMetrics);

    /// <summary>
    /// Create budget alert in Azure Cost Management
    /// </summary>
    /// <param name="budgetName">Budget name</param>
    /// <param name="limitGBP">Budget limit in GBP</param>
    /// <param name="alertEmails">Email addresses for alerts</param>
    /// <returns>Success status</returns>
    Task<bool> CreateBudgetAlertAsync(string budgetName, decimal limitGBP, List<string> alertEmails);

    /// <summary>
    /// Get cost forecasts using Azure ML and historical data
    /// </summary>
    /// <param name="userId">User to forecast for</param>
    /// <param name="forecastDays">Number of days to forecast</param>
    /// <returns>Cost forecast data with confidence scores</returns>
    Task<List<CostForecastData>> GetCostForecastsAsync(Guid userId, int forecastDays = 7);
}

/// <summary>
/// Enhanced real-time cost tracker with Azure integration
/// </summary>
public interface IRealTimeCostTracker : IPerUserCostTracker
{
    /// <summary>
    /// Track real-time cost with educational efficiency scoring
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="serviceType">Azure service type</param>
    /// <param name="estimatedCost">Estimated cost in GBP</param>
    /// <param name="educationalMetrics">Learning metrics achieved</param>
    /// <returns>Enhanced cost summary with efficiency scoring</returns>
    Task<EnhancedCostSummaryDto> TrackRealTimeCostAsync(
        Guid userId, 
        string serviceType, 
        decimal estimatedCost,
        Dictionary<string, object>? educationalMetrics = null);

    /// <summary>
    /// Check if user should be throttled due to budget limits
    /// </summary>
    /// <param name="userId">User to check</param>
    /// <returns>Throttling recommendation with educational context</returns>
    Task<BudgetThrottlingRecommendation> ShouldThrottleUserAsync(Guid userId);

    /// <summary>
    /// Get real-time cost data for monitoring dashboard
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Real-time cost tracking data</returns>
    Task<RealTimeCostData> GetRealTimeCostDataAsync(Guid userId);

    /// <summary>
    /// Trigger budget alert if thresholds are exceeded
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="currentCost">Current cost</param>
    /// <returns>Alert notification if triggered</returns>
    Task<BudgetAlertNotification?> CheckAndTriggerBudgetAlertAsync(Guid userId, decimal currentCost);

    /// <summary>
    /// Calculate educational efficiency for user's spending
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="timeframe">Timeframe to calculate for</param>
    /// <returns>Educational efficiency score</returns>
    Task<decimal> CalculateEducationalEfficiencyScoreAsync(Guid userId, TimeSpan timeframe);
}

/// <summary>
/// Budget throttling recommendation with educational context
/// </summary>
public record BudgetThrottlingRecommendation(
    bool ShouldThrottle,
    string Reason,
    decimal CurrentCostGBP,
    decimal LimitGBP,
    TimeSpan RecommendedDelay)
{
    /// <summary>
    /// Educational impact assessment
    /// </summary>
    public string EducationalImpact { get; init; } = "Minimal";
    
    /// <summary>
    /// Alternative learning activities suggested
    /// </summary>
    public List<string> AlternativeLearningActivities { get; init; } = new();
    
    /// <summary>
    /// Whether parents/school should be notified
    /// </summary>
    public bool RequiresNotification { get; init; } = false;
}