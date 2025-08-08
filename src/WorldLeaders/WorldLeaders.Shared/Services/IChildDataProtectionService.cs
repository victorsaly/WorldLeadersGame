using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Models;

namespace WorldLeaders.Shared.Services;

/// <summary>
/// Context: Educational game child data protection for 12-year-old players
/// Educational Objective: COPPA/GDPR compliant data protection with UK South residency
/// Safety Requirements: End-to-end encryption, automated compliance monitoring, child safety
/// </summary>
public interface IChildDataProtectionService
{
    /// <summary>
    /// Encrypt sensitive child data using Azure Key Vault UK South
    /// </summary>
    Task<string> EncryptChildDataAsync(string data, Guid childUserId);

    /// <summary>
    /// Decrypt child data with audit trail
    /// </summary>
    Task<string> DecryptChildDataAsync(string encryptedData, Guid childUserId);

    /// <summary>
    /// Validate data processing compliance (COPPA/GDPR)
    /// </summary>
    Task<ComplianceValidationResult> ValidateDataProcessingAsync(DataProcessingRequest request);

    /// <summary>
    /// Generate automated Data Protection Impact Assessment (DPIA)
    /// </summary>
    Task<DPIAReport> GenerateDataProtectionImpactAssessmentAsync(Guid childUserId);

    /// <summary>
    /// Monitor and report compliance status
    /// </summary>
    Task<ComplianceMonitoringReport> GetComplianceReportAsync(DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Request parental consent with transparency controls
    /// </summary>
    Task<ParentalConsentRequest> RequestParentalConsentAsync(Guid childUserId, string parentalEmail);

    /// <summary>
    /// Validate and process parental consent response
    /// </summary>
    Task<bool> ProcessParentalConsentResponseAsync(string consentToken, bool isConsented);

    /// <summary>
    /// Get parent dashboard with child activity transparency
    /// </summary>
    Task<ParentalDashboardDto> GetParentalDashboardAsync(Guid childUserId, string parentalEmail);

    /// <summary>
    /// Log security event with UK educational context
    /// </summary>
    Task<bool> LogSecurityEventAsync(SecurityAuditEvent auditEvent);

    /// <summary>
    /// Perform automated security health check
    /// </summary>
    Task<SecurityHealthCheckResult> PerformSecurityHealthCheckAsync();

    /// <summary>
    /// Generate safeguarding report for UK educational requirements
    /// </summary>
    Task<SafeguardingReport> GenerateSafeguardingReportAsync(DateTime fromDate, DateTime toDate);
}