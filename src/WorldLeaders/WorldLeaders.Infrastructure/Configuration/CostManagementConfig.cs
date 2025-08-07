namespace WorldLeaders.Infrastructure.Configuration;

/// <summary>
/// Context: Educational game cost management for 12-year-old players
/// Educational Objective: Enhanced Azure cost tracking configuration for UK South region
/// Safety Requirements: Budget controls, educational compliance, GDPR compliance
/// </summary>

/// <summary>
/// Enhanced budget configuration with UK educational focus
/// </summary>
public class BudgetConfig
{
    /// <summary>
    /// UK Educational default budget configuration
    /// </summary>
    public static BudgetConfig UKEducationalDefaults => new()
    {
        DailyLimitGBP = 0.08m,
        AlertThresholdPercentage = 0.80m, // 80% = £0.064
        EmergencyThrottlingEnabled = true,
        Region = "UK South",
        Currency = "GBP",
        EducationalCompliance = new EducationalComplianceConfig(),
        AlertConfiguration = new AlertConfig()
    };

    /// <summary>
    /// Daily budget limit in GBP
    /// </summary>
    public decimal DailyLimitGBP { get; set; } = 0.08m;

    /// <summary>
    /// Alert threshold as percentage (0.8 = 80%)
    /// </summary>
    public decimal AlertThresholdPercentage { get; set; } = 0.80m;

    /// <summary>
    /// Whether emergency throttling is enabled
    /// </summary>
    public bool EmergencyThrottlingEnabled { get; set; } = true;

    /// <summary>
    /// Azure region for cost tracking
    /// </summary>
    public string Region { get; set; } = "UK South";

    /// <summary>
    /// Currency code
    /// </summary>
    public string Currency { get; set; } = "GBP";

    /// <summary>
    /// Educational compliance configuration
    /// </summary>
    public EducationalComplianceConfig EducationalCompliance { get; set; } = new();

    /// <summary>
    /// Alert configuration
    /// </summary>
    public AlertConfig AlertConfiguration { get; set; } = new();

    /// <summary>
    /// Calculate alert threshold in GBP
    /// </summary>
    public decimal AlertThresholdGBP => DailyLimitGBP * AlertThresholdPercentage;
}

/// <summary>
/// Educational compliance configuration for UK deployment
/// </summary>
public class EducationalComplianceConfig
{
    /// <summary>
    /// Target educational efficiency score (85+ points/£)
    /// </summary>
    public decimal TargetEducationalEfficiency { get; set; } = 85m;

    /// <summary>
    /// Whether parental notifications are required
    /// </summary>
    public bool RequireParentNotifications { get; set; } = true;

    /// <summary>
    /// Whether school notifications are required
    /// </summary>
    public bool RequireSchoolNotifications { get; set; } = true;

    /// <summary>
    /// GDPR compliance settings
    /// </summary>
    public bool EnableGdprCompliance { get; set; } = true;

    /// <summary>
    /// Data retention period in days
    /// </summary>
    public int DataRetentionDays { get; set; } = 365;

    /// <summary>
    /// Educational value tracking
    /// </summary>
    public bool TrackEducationalOutcomes { get; set; } = true;

    /// <summary>
    /// Educational tags for Azure resources
    /// </summary>
    public Dictionary<string, string> ResourceTags { get; set; } = new()
    {
        ["Environment"] = "Educational",
        ["AgeGroup"] = "12-year-olds",
        ["Compliance"] = "GDPR-COPPA",
        ["Region"] = "UK-South"
    };
}

/// <summary>
/// Alert configuration for budget monitoring
/// </summary>
public class AlertConfig
{
    /// <summary>
    /// Whether real-time alerts are enabled
    /// </summary>
    public bool EnableRealTimeAlerts { get; set; } = true;

    /// <summary>
    /// Email addresses for budget alerts
    /// </summary>
    public List<string> NotificationEmails { get; set; } = new();

    /// <summary>
    /// Webhook URL for alert notifications
    /// </summary>
    public string? WebhookUrl { get; set; }

    /// <summary>
    /// Alert frequency limits (prevent spam)
    /// </summary>
    public TimeSpan MinimumAlertInterval { get; set; } = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Alert message templates
    /// </summary>
    public AlertMessageTemplates MessageTemplates { get; set; } = new();
}

/// <summary>
/// Alert message templates for different scenarios
/// </summary>
public class AlertMessageTemplates
{
    /// <summary>
    /// Warning alert (80% threshold)
    /// </summary>
    public string WarningTemplate { get; set; } = 
        "Educational Budget Warning: Student has reached {percentage}% of daily limit (£{current}/£{limit}). Learning session will continue with monitoring.";

    /// <summary>
    /// Limit exceeded alert
    /// </summary>
    public string LimitExceededTemplate { get; set; } = 
        "Educational Budget Limit Exceeded: Student has exceeded daily limit of £{limit}. Emergency throttling may be activated to protect educational resources.";

    /// <summary>
    /// Emergency throttling alert
    /// </summary>
    public string EmergencyThrottlingTemplate { get; set; } = 
        "Emergency Throttling Activated: Educational session for student has been throttled due to budget protection. Contact administrator if this impacts learning objectives.";

    /// <summary>
    /// Educational efficiency alert
    /// </summary>
    public string EducationalEfficiencyTemplate { get; set; } = 
        "Educational Efficiency Alert: Learning score of {score} points/£ is below target of {target} points/£. Consider optimizing learning activities.";
}

/// <summary>
/// Azure Cost Management service configuration
/// </summary>
public class AzureCostManagementConfig
{
    public const string SectionName = "AzureCostManagement";

    /// <summary>
    /// Whether Azure Cost Management integration is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Azure subscription ID
    /// </summary>
    public string SubscriptionId { get; set; } = string.Empty;

    /// <summary>
    /// Resource group name for educational resources
    /// </summary>
    public string ResourceGroupName { get; set; } = "worldleaders-educational-rg";

    /// <summary>
    /// Azure region for cost queries
    /// </summary>
    public string Region { get; set; } = "UK South";

    /// <summary>
    /// How often to sync with Azure Cost Management
    /// </summary>
    public TimeSpan SyncInterval { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// Service principal credentials for Azure Cost Management API
    /// </summary>
    public AzureCredentialsConfig Credentials { get; set; } = new();

    /// <summary>
    /// Cost query configuration
    /// </summary>
    public CostQueryConfig QueryConfig { get; set; } = new();
}

/// <summary>
/// Azure credentials configuration
/// </summary>
public class AzureCredentialsConfig
{
    /// <summary>
    /// Tenant ID for Azure AD
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Client ID for service principal
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Client secret for service principal
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;
}

/// <summary>
/// Cost query configuration
/// </summary>
public class CostQueryConfig
{
    /// <summary>
    /// Services to track costs for
    /// </summary>
    public List<string> TrackedServices { get; set; } = new()
    {
        "Microsoft.CognitiveServices",
        "Microsoft.Web",
        "Microsoft.Storage",
        "Microsoft.KeyVault",
        "Microsoft.Insights"
    };

    /// <summary>
    /// Cost aggregation granularity
    /// </summary>
    public string Granularity { get; set; } = "Daily"; // Daily, Monthly, Yearly

    /// <summary>
    /// Educational resource filters
    /// </summary>
    public Dictionary<string, string> ResourceFilters { get; set; } = new()
    {
        ["Environment"] = "Educational",
        ["Application"] = "WorldLeadersGame"
    };
}