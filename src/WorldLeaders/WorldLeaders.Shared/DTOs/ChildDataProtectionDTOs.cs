using WorldLeaders.Shared.Enums;

namespace WorldLeaders.Shared.DTOs;

/// <summary>
/// Data protection compliance validation result
/// Context: UK educational compliance for child data protection
/// </summary>
public record ComplianceValidationResult
{
    public bool IsCompliant { get; init; }
    public string ComplianceLevel { get; init; } = string.Empty;
    public List<string> Violations { get; init; } = new();
    public List<string> Recommendations { get; init; } = new();
    public double ComplianceScore { get; init; }
    public DateTime AssessmentDate { get; init; } = DateTime.UtcNow;
    public string Region { get; init; } = "UK South";
}

/// <summary>
/// Data processing request for compliance validation
/// </summary>
public record DataProcessingRequest
{
    public Guid ChildUserId { get; init; }
    public string DataType { get; init; } = string.Empty;
    public string ProcessingPurpose { get; init; } = string.Empty;
    public bool HasParentalConsent { get; init; }
    public bool HasGdprConsent { get; init; }
    public string LegalBasis { get; init; } = string.Empty;
    public int DataRetentionDays { get; init; }
    public bool IsEducationalProcessing { get; init; } = true;
}

/// <summary>
/// Data Protection Impact Assessment report
/// </summary>
public record DPIAReport
{
    public Guid ChildUserId { get; init; }
    public DateTime AssessmentDate { get; init; } = DateTime.UtcNow;
    public string RiskLevel { get; init; } = string.Empty;
    public List<string> IdentifiedRisks { get; init; } = new();
    public List<string> MitigationMeasures { get; init; } = new();
    public bool RequiresParentalNotification { get; init; }
    public bool RequiresAuthorityNotification { get; init; }
    public string AssessmentSummary { get; init; } = string.Empty;
    public ComplianceValidationResult ComplianceStatus { get; init; } = new();
}

/// <summary>
/// Compliance monitoring report
/// </summary>
public record ComplianceMonitoringReport
{
    public DateTime ReportStartDate { get; init; }
    public DateTime ReportEndDate { get; init; }
    public int TotalChildAccounts { get; init; }
    public int AccountsWithParentalConsent { get; init; }
    public int AccountsWithGdprConsent { get; init; }
    public double OverallComplianceScore { get; init; }
    public List<ComplianceViolation> Violations { get; init; } = new();
    public List<SecurityIncident> SecurityIncidents { get; init; } = new();
    public string Region { get; init; } = "UK South";
    public bool IsUkEducationalCompliant { get; init; }
}

/// <summary>
/// Compliance violation details
/// </summary>
public record ComplianceViolation
{
    public Guid ViolationId { get; init; } = Guid.NewGuid();
    public DateTime DetectedAt { get; init; } = DateTime.UtcNow;
    public string ViolationType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty;
    public Guid? AffectedUserId { get; init; }
    public bool IsResolved { get; init; }
    public DateTime? ResolvedAt { get; init; }
    public string ResolutionAction { get; init; } = string.Empty;
}

/// <summary>
/// Security incident record
/// </summary>
public record SecurityIncident
{
    public Guid IncidentId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public string IncidentType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty;
    public Guid? AffectedUserId { get; init; }
    public bool RequiresParentalNotification { get; init; }
    public bool RequiresAuthorityNotification { get; init; }
    public string ResponseAction { get; init; } = string.Empty;
    public bool IsResolved { get; init; }
}

/// <summary>
/// Parental consent request
/// </summary>
public record ParentalConsentRequest
{
    public Guid RequestId { get; init; } = Guid.NewGuid();
    public Guid ChildUserId { get; init; }
    public string ChildDisplayName { get; init; } = string.Empty;
    public string ParentalEmail { get; init; } = string.Empty;
    public DateTime RequestedAt { get; init; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; init; }
    public string ConsentToken { get; init; } = string.Empty;
    public string ConsentUrl { get; init; } = string.Empty;
    public List<string> DataProcessingPurposes { get; init; } = new();
    public bool IsUkEducationalContext { get; init; } = true;
}

/// <summary>
/// Parental dashboard information
/// </summary>
public record ParentalDashboardDto
{
    public Guid ChildUserId { get; init; }
    public string ChildDisplayName { get; init; } = string.Empty;
    public int ChildAge { get; init; }
    public DateTime LastActivityAt { get; init; }
    public TimeSpan TotalPlayTime { get; init; }
    public List<LearningActivity> RecentActivities { get; init; } = new();
    public List<SafetyAlert> SafetyAlerts { get; init; } = new();
    public ConsentStatus ConsentStatus { get; init; } = new();
    public Dictionary<string, object> EducationalProgress { get; init; } = new();
    public decimal DailyCostSpend { get; init; }
    public bool IsWithinSafetyLimits { get; init; } = true;
}

/// <summary>
/// Learning activity record for parents
/// </summary>
public record LearningActivity
{
    public DateTime ActivityAt { get; init; }
    public string ActivityType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int EducationalValue { get; init; }
    public bool WasSupervised { get; init; }
    public string LearningObjectives { get; init; } = string.Empty;
}

/// <summary>
/// Safety alert for parents
/// </summary>
public record SafetyAlert
{
    public DateTime AlertAt { get; init; }
    public string AlertType { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty;
    public bool RequiresAction { get; init; }
    public string ActionRequired { get; init; } = string.Empty;
}

/// <summary>
/// Parental consent status
/// </summary>
public record ConsentStatus
{
    public bool HasParentalConsent { get; init; }
    public DateTime? ConsentGrantedAt { get; init; }
    public DateTime? ConsentExpiresAt { get; init; }
    public bool HasGdprConsent { get; init; }
    public DateTime? GdprConsentGrantedAt { get; init; }
    public List<string> ConsentedDataTypes { get; init; } = new();
    public bool RequiresRenewal { get; init; }
}

/// <summary>
/// Security audit event
/// </summary>
public record SecurityAuditEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime EventAt { get; init; } = DateTime.UtcNow;
    public Guid? UserId { get; init; }
    public string EventType { get; init; } = string.Empty;
    public string EventCategory { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty;
    public Dictionary<string, object> EventData { get; init; } = new();
    public string IpAddress { get; init; } = string.Empty;
    public string UserAgent { get; init; } = string.Empty;
    public string Region { get; init; } = "UK South";
    public bool RequiresFollowUp { get; init; }
}

/// <summary>
/// Security health check result
/// </summary>
public record SecurityHealthCheckResult
{
    public DateTime CheckedAt { get; init; } = DateTime.UtcNow;
    public bool IsHealthy { get; init; }
    public double OverallScore { get; init; }
    public List<SecurityCheckItem> Checks { get; init; } = new();
    public List<string> Recommendations { get; init; } = new();
    public string Region { get; init; } = "UK South";
    public bool IsUkEducationalCompliant { get; init; }
}

/// <summary>
/// Individual security check item
/// </summary>
public record SecurityCheckItem
{
    public string CheckName { get; init; } = string.Empty;
    public bool Passed { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty;
    public string Recommendation { get; init; } = string.Empty;
    public DateTime CheckedAt { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Safeguarding report for UK educational requirements
/// </summary>
public record SafeguardingReport
{
    public DateTime ReportStartDate { get; init; }
    public DateTime ReportEndDate { get; init; }
    public int TotalChildAccounts { get; init; }
    public int ActiveChildAccounts { get; init; }
    public List<SafeguardingIncident> Incidents { get; init; } = new();
    public List<SafeguardingMetric> SafeguardingMetrics { get; init; } = new();
    public ComplianceValidationResult DfEComplianceStatus { get; init; } = new();
    public bool RequiresAuthorityNotification { get; init; }
    public string ReportSummary { get; init; } = string.Empty;
}

/// <summary>
/// Safeguarding incident
/// </summary>
public record SafeguardingIncident
{
    public Guid IncidentId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public Guid ChildUserId { get; init; }
    public string IncidentType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string RiskLevel { get; init; } = string.Empty;
    public string ActionTaken { get; init; } = string.Empty;
    public bool ParentsNotified { get; init; }
    public bool AuthoritiesNotified { get; init; }
    public bool IsResolved { get; init; }
}

/// <summary>
/// Safeguarding metric
/// </summary>
public record SafeguardingMetric
{
    public string MetricName { get; init; } = string.Empty;
    public double Value { get; init; }
    public string Unit { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public double Threshold { get; init; }
    public bool IsWithinThreshold { get; init; }
    public string Description { get; init; } = string.Empty;
}

/// <summary>
/// Child privacy configuration for UK standards
/// </summary>
public record ChildPrivacyConfig
{
    public bool EnableEndToEndEncryption { get; init; } = true;
    public string EncryptionAlgorithm { get; init; } = "AES-256-GCM";
    public string KeyVaultRegion { get; init; } = "UK South";
    public int DataRetentionDays { get; init; } = 365;
    public bool RequireParentalConsent { get; init; } = true;
    public bool RequireGdprConsent { get; init; } = true;
    public int MaxSessionDurationMinutes { get; init; } = 30;
    public bool EnableAutomaticDataDeletion { get; init; } = true;
    public bool EnableComplianceMonitoring { get; init; } = true;
    public bool EnableSafeguardingAlerts { get; init; } = true;

    /// <summary>
    /// Default UK standards configuration
    /// </summary>
    public static ChildPrivacyConfig UKStandards => new()
    {
        EnableEndToEndEncryption = true,
        EncryptionAlgorithm = "AES-256-GCM",
        KeyVaultRegion = "UK South",
        DataRetentionDays = 365,
        RequireParentalConsent = true,
        RequireGdprConsent = true,
        MaxSessionDurationMinutes = 30,
        EnableAutomaticDataDeletion = true,
        EnableComplianceMonitoring = true,
        EnableSafeguardingAlerts = true
    };
}