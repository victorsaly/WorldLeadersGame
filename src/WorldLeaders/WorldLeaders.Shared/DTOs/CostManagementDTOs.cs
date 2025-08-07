using System.ComponentModel.DataAnnotations;

namespace WorldLeaders.Shared.DTOs;

/// <summary>
/// Context: Educational game cost management for 12-year-old players
/// Educational Objective: Enhanced Azure cost tracking with UK South focus (£0.08/user/day)
/// Safety Requirements: Real-time attribution, budget controls, educational compliance
/// </summary>

/// <summary>
/// Enhanced real-time cost tracking with .NET 8 record types for built-in comparison
/// </summary>
public record RealTimeCostData(
    Guid UserId,
    DateTime Timestamp,
    string ServiceType,
    decimal CostGBP,
    string Region,
    Dictionary<string, object> Metadata) : IComparable<RealTimeCostData>
{
    /// <summary>
    /// Educational efficiency score per pound spent (target: 85+ points/£)
    /// </summary>
    public decimal EducationalEfficiencyScore { get; init; } = 0m;
    
    /// <summary>
    /// Built-in comparison for .NET 8 performance optimization
    /// </summary>
    public int CompareTo(RealTimeCostData? other)
    {
        if (other is null) return 1;
        var timestampComparison = Timestamp.CompareTo(other.Timestamp);
        return timestampComparison != 0 ? timestampComparison : CostGBP.CompareTo(other.CostGBP);
    }
}

/// <summary>
/// Budget alert configuration for UK educational deployment
/// </summary>
public record BudgetAlertConfig(
    decimal DailyLimitGBP,
    decimal AlertThresholdPercentage, // 80% = 0.8
    bool EmergencyThrottlingEnabled,
    string NotificationEmail,
    string UKRegion = "UK South")
{
    /// <summary>
    /// Calculate alert threshold in GBP (80% of £0.08 = £0.064)
    /// </summary>
    public decimal AlertThresholdGBP => DailyLimitGBP * AlertThresholdPercentage;
    
    /// <summary>
    /// Educational compliance settings
    /// </summary>
    public bool RequireEducationalApproval { get; init; } = true;
    public bool EnableParentNotifications { get; init; } = true;
}

/// <summary>
/// Cost forecasting data with basic ML patterns
/// </summary>
public record CostForecastData(
    Guid UserId,
    DateTime ForecastDate,
    decimal PredictedCostGBP,
    decimal ConfidenceScore,
    List<string> TrendFactors)
{
    /// <summary>
    /// Educational usage pattern classification
    /// </summary>
    public string UsagePattern { get; init; } = "Standard"; // Standard, Heavy, Light, Peak, Off-Peak
    
    /// <summary>
    /// Predicted educational efficiency for the day
    /// </summary>
    public decimal PredictedEducationalScore { get; init; } = 85m;
}

/// <summary>
/// Enhanced cost summary with educational metrics
/// </summary>
public record EnhancedCostSummaryDto(
    Guid UserId,
    DateTime Date,
    decimal TotalCostGBP,
    bool IsOverDailyLimit,
    decimal RemainingBudgetGBP)
{
    /// <summary>
    /// Service breakdown with granular attribution
    /// </summary>
    public required Dictionary<string, CostServiceBreakdown> ServiceBreakdown { get; init; }
    
    /// <summary>
    /// Educational efficiency metrics
    /// </summary>
    public decimal AverageEducationalScore { get; init; } = 85m;
    public int LearningObjectivesAchieved { get; init; } = 0;
    public TimeSpan ActiveLearningTime { get; init; } = TimeSpan.Zero;
    
    /// <summary>
    /// Predictive analytics
    /// </summary>
    public CostForecastData? TomorrowForecast { get; init; }
    
    /// <summary>
    /// UK educational compliance
    /// </summary>
    public bool IsEducationallyCompliant { get; init; } = true;
    public List<string> ComplianceNotes { get; init; } = new();
}

/// <summary>
/// Granular service cost breakdown
/// </summary>
public record CostServiceBreakdown(
    string ServiceName,
    decimal CostGBP,
    int CallCount,
    decimal AverageCostPerCall)
{
    /// <summary>
    /// Educational value metrics per service
    /// </summary>
    public decimal EducationalValueScore { get; init; } = 85m;
    public List<string> LearningOutcomes { get; init; } = new();
    
    /// <summary>
    /// Performance metrics
    /// </summary>
    public TimeSpan AverageResponseTime { get; init; } = TimeSpan.Zero;
    public decimal SuccessRate { get; init; } = 1.0m; // 100%
}

/// <summary>
/// Budget alert notification
/// </summary>
public record BudgetAlertNotification(
    Guid UserId,
    DateTime AlertTime,
    string AlertType, // "Warning", "Limit", "Emergency"
    decimal CurrentCostGBP,
    decimal LimitGBP,
    string Message)
{
    /// <summary>
    /// Educational context for the alert
    /// </summary>
    public string EducationalContext { get; init; } = "Standard learning session";
    public List<string> SuggestedActions { get; init; } = new();
    
    /// <summary>
    /// Parent/school notification details
    /// </summary>
    public bool RequiresParentNotification { get; init; } = false;
    public bool RequiresSchoolNotification { get; init; } = false;
}

/// <summary>
/// Azure Cost Management integration request
/// </summary>
public record AzureCostQueryRequest(
    string SubscriptionId,
    DateTime StartDate,
    DateTime EndDate,
    string ResourceGroupName,
    List<string> ServiceNames)
{
    /// <summary>
    /// UK South region filter
    /// </summary>
    public string Region { get; init; } = "UK South";
    
    /// <summary>
    /// Educational resource tags
    /// </summary>
    public Dictionary<string, string> EducationalTags { get; init; } = new()
    {
        ["Environment"] = "Educational",
        ["AgeGroup"] = "12-year-olds",
        ["Compliance"] = "GDPR-COPPA"
    };
}

/// <summary>
/// Azure Cost Management response
/// </summary>
public record AzureCostQueryResponse(
    decimal TotalCostGBP,
    List<CostServiceBreakdown> ServiceBreakdown,
    DateTime QueryDate,
    bool IsComplete)
{
    /// <summary>
    /// Regional cost breakdown
    /// </summary>
    public Dictionary<string, decimal> RegionalCosts { get; init; } = new();
    
    /// <summary>
    /// Educational efficiency calculated from Azure data
    /// </summary>
    public decimal CalculatedEducationalEfficiency { get; init; } = 85m;
}

/// <summary>
/// School/parent cost transparency report
/// </summary>
public record EducationalCostTransparencyReport(
    string SchoolName,
    DateTime ReportDate,
    List<Guid> StudentUserIds,
    decimal TotalEducationalSpendGBP)
{
    /// <summary>
    /// Educational value delivered
    /// </summary>
    public int TotalLearningObjectivesAchieved { get; init; } = 0;
    public TimeSpan TotalEducationalTime { get; init; } = TimeSpan.Zero;
    public decimal AverageEducationalScore { get; init; } = 85m;
    
    /// <summary>
    /// Cost efficiency metrics
    /// </summary>
    public decimal CostPerLearningObjective { get; init; } = 0m;
    public decimal CostPerEducationalHour { get; init; } = 0m;
    
    /// <summary>
    /// GDPR-compliant summary (no individual student data)
    /// </summary>
    public List<string> EducationalOutcomes { get; init; } = new();
    public string PrivacyCompliance { get; init; } = "GDPR Compliant - Aggregated Data Only";
}