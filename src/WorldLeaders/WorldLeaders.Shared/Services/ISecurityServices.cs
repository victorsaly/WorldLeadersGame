namespace WorldLeaders.Shared.Services;

/// <summary>
/// Azure Key Vault client for UK South region
/// Context: Educational game child data protection
/// Safety Requirements: UK data residency, encryption key management
/// </summary>
public interface IKeyVaultClient
{
    /// <summary>
    /// Encrypt data using Azure Key Vault in UK South region
    /// </summary>
    Task<string> EncryptAsync(string data, string keyName);

    /// <summary>
    /// Decrypt data using Azure Key Vault
    /// </summary>
    Task<string> DecryptAsync(string encryptedData, string keyName);

    /// <summary>
    /// Create or rotate encryption key
    /// </summary>
    Task<string> CreateKeyAsync(string keyName, string keyType = "RSA");

    /// <summary>
    /// Validate key vault connectivity and permissions
    /// </summary>
    Task<bool> ValidateConnectionAsync();

    /// <summary>
    /// Get key vault region for compliance verification
    /// </summary>
    string GetRegion();
}

/// <summary>
/// Audit logger for compliance and security events
/// Context: Educational game security monitoring
/// Safety Requirements: Child activity logging, compliance audit trail
/// </summary>
public interface IAuditLogger
{
    /// <summary>
    /// Log security or compliance event
    /// </summary>
    Task LogEventAsync(string eventType, string message, object? data = null, Guid? userId = null);

    /// <summary>
    /// Log child safety event with enhanced metadata
    /// </summary>
    Task LogChildSafetyEventAsync(string eventType, Guid childUserId, string message, object? data = null);

    /// <summary>
    /// Log compliance violation
    /// </summary>
    Task LogComplianceViolationAsync(string violationType, string description, Guid? userId = null);

    /// <summary>
    /// Get audit events for reporting
    /// </summary>
    Task<List<AuditEvent>> GetAuditEventsAsync(DateTime fromDate, DateTime toDate, Guid? userId = null);
}

/// <summary>
/// Compliance validator for UK educational requirements
/// Context: Educational game COPPA/GDPR compliance
/// Safety Requirements: Automated compliance checking, UK DfE standards
/// </summary>
public interface IComplianceValidator
{
    /// <summary>
    /// Validate COPPA compliance for child account
    /// </summary>
    Task<bool> ValidateCoppaComplianceAsync(Guid childUserId);

    /// <summary>
    /// Validate GDPR compliance for data processing
    /// </summary>
    Task<bool> ValidateGdprComplianceAsync(string dataType, string processingPurpose);

    /// <summary>
    /// Validate UK DfE educational standards compliance
    /// </summary>
    Task<bool> ValidateUkEducationalComplianceAsync(Guid userId);

    /// <summary>
    /// Perform automated compliance audit
    /// </summary>
    Task<ComplianceAuditResult> PerformComplianceAuditAsync();

    /// <summary>
    /// Get compliance status for specific user
    /// </summary>
    Task<UserComplianceStatus> GetUserComplianceStatusAsync(Guid userId);
}

/// <summary>
/// Audit event record
/// </summary>
public record AuditEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string EventType { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public Guid? UserId { get; init; }
    public object? Data { get; init; }
    public string Severity { get; init; } = "Info";
    public string Source { get; init; } = "WorldLeadersGame";
}

/// <summary>
/// Compliance audit result
/// </summary>
public record ComplianceAuditResult
{
    public DateTime AuditDate { get; init; } = DateTime.UtcNow;
    public bool IsCompliant { get; init; }
    public double ComplianceScore { get; init; }
    public List<ComplianceIssue> Issues { get; init; } = new();
    public List<string> Recommendations { get; init; } = new();
    public Dictionary<string, bool> ComplianceChecks { get; init; } = new();
}

/// <summary>
/// Compliance issue
/// </summary>
public record ComplianceIssue
{
    public string Type { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty;
    public string Recommendation { get; init; } = string.Empty;
    public Guid? AffectedUserId { get; init; }
}

/// <summary>
/// User compliance status
/// </summary>
public record UserComplianceStatus
{
    public Guid UserId { get; init; }
    public bool IsCoppaCompliant { get; init; }
    public bool IsGdprCompliant { get; init; }
    public bool IsUkEducationalCompliant { get; init; }
    public DateTime LastChecked { get; init; } = DateTime.UtcNow;
    public List<string> MissingConsents { get; init; } = new();
    public DateTime? ConsentExpiryDate { get; init; }
}