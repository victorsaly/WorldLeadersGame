using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LiteDB;
using WorldLeaders.Infrastructure.Configuration;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.Infrastructure.Services;

/// <summary>
/// LiteDB-based audit logger implementation for compliance and security events
/// Context: Educational game security monitoring for 12-year-old players
/// Safety Requirements: Persistent, queryable audit logging for child safety compliance
/// </summary>
public class LiteDbAuditLogger : IAuditLogger, IDisposable
{
    private readonly ILogger<LiteDbAuditLogger> _logger;
    private readonly LiteDatabase _database;
    private readonly ILiteCollection<AuditEventDocument> _auditEvents;
    private bool _disposed = false;

    public LiteDbAuditLogger(
        IOptions<LiteDbOptions> liteDbOptions, 
        ILogger<LiteDbAuditLogger> logger)
    {
        _logger = logger;
        var connectionString = liteDbOptions.Value.ConnectionString ?? 
            Path.Combine(AppContext.BaseDirectory, "Data", "audit-events.db");
        
        // Ensure directory exists
        var directory = Path.GetDirectoryName(connectionString);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _database = new LiteDatabase(connectionString);
        _auditEvents = _database.GetCollection<AuditEventDocument>("audit_events");

        // Create indexes for better query performance
        _auditEvents.EnsureIndex(x => x.EventType);
        _auditEvents.EnsureIndex(x => x.UserId);
        _auditEvents.EnsureIndex(x => x.Timestamp);
        _auditEvents.EnsureIndex(x => x.IsChildSafetyEvent);

        _logger.LogInformation("LiteDB audit logger initialized with database: {ConnectionString}", connectionString);
    }

    /// <summary>
    /// Log security or compliance event
    /// </summary>
    public async Task LogEventAsync(string eventType, string message, object? data = null, Guid? userId = null)
    {
        try
        {
            var auditEvent = new AuditEventDocument
            {
                Id = ObjectId.NewObjectId(),
                EventId = Guid.NewGuid(),
                EventType = eventType,
                Message = message,
                UserId = userId,
                Data = data != null ? BsonMapper.Global.ToDocument(data) : null,
                Timestamp = DateTime.UtcNow,
                Severity = "Info",
                Source = "WorldLeadersGame",
                IsChildSafetyEvent = eventType.StartsWith("ChildSafety_", StringComparison.OrdinalIgnoreCase)
            };

            _auditEvents.Insert(auditEvent);
            
            _logger.LogInformation("Audit event logged: {EventType} - {Message}", eventType, message);
            
            // Simulate async operation for interface compatibility
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log audit event: {EventType}", eventType);
        }
    }

    /// <summary>
    /// Log child safety event with enhanced metadata
    /// </summary>
    public async Task LogChildSafetyEventAsync(string eventType, Guid childUserId, string message, object? data = null)
    {
        var enhancedData = new
        {
            ChildUserId = childUserId,
            IsChildSafetyEvent = true,
            ComplianceContext = "COPPA_GDPR_UK_Educational",
            Data = data
        };

        await LogEventAsync($"ChildSafety_{eventType}", message, enhancedData, childUserId);
    }

    /// <summary>
    /// Log compliance violation
    /// </summary>
    public async Task LogComplianceViolationAsync(string violationType, string description, Guid? userId = null)
    {
        var violationData = new
        {
            ViolationType = violationType,
            DetectedAt = DateTime.UtcNow,
            Severity = "High",
            RequiresAction = true
        };

        var auditEvent = new AuditEventDocument
        {
            Id = ObjectId.NewObjectId(),
            EventId = Guid.NewGuid(),
            EventType = "ComplianceViolation",
            Message = description,
            UserId = userId,
            Data = BsonMapper.Global.ToDocument(violationData),
            Timestamp = DateTime.UtcNow,
            Severity = "High",
            Source = "WorldLeadersGame",
            IsChildSafetyEvent = false
        };

        _auditEvents.Insert(auditEvent);
        
        _logger.LogWarning("Compliance violation logged: {ViolationType} - {Description}", violationType, description);
        
        await Task.CompletedTask;
    }

    /// <summary>
    /// Get audit events for reporting
    /// </summary>
    public async Task<List<AuditEvent>> GetAuditEventsAsync(DateTime fromDate, DateTime toDate, Guid? userId = null)
    {
        try
        {
            var query = _auditEvents.Query()
                .Where(x => x.Timestamp >= fromDate && x.Timestamp <= toDate);

            if (userId.HasValue)
            {
                query = query.Where(x => x.UserId == userId.Value);
            }

            var documents = query
                .OrderByDescending(x => x.Timestamp)
                .ToList();

            var auditEvents = documents.Select(doc => new AuditEvent
            {
                EventId = doc.EventId,
                Timestamp = doc.Timestamp,
                EventType = doc.EventType,
                Message = doc.Message,
                UserId = doc.UserId,
                Data = doc.Data?.RawValue,
                Severity = doc.Severity,
                Source = doc.Source
            }).ToList();

            _logger.LogInformation("Retrieved {Count} audit events for date range {FromDate} to {ToDate}", 
                auditEvents.Count, fromDate, toDate);

            return await Task.FromResult(auditEvents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve audit events");
            return new List<AuditEvent>();
        }
    }

    /// <summary>
    /// Get child safety events for specific child
    /// </summary>
    public async Task<List<AuditEvent>> GetChildSafetyEventsAsync(Guid childUserId, DateTime fromDate, DateTime toDate)
    {
        try
        {
            var documents = _auditEvents.Query()
                .Where(x => x.UserId == childUserId && 
                           x.IsChildSafetyEvent == true &&
                           x.Timestamp >= fromDate && 
                           x.Timestamp <= toDate)
                .OrderByDescending(x => x.Timestamp)
                .ToList();

            var auditEvents = documents.Select(doc => new AuditEvent
            {
                EventId = doc.EventId,
                Timestamp = doc.Timestamp,
                EventType = doc.EventType,
                Message = doc.Message,
                UserId = doc.UserId,
                Data = doc.Data?.RawValue,
                Severity = doc.Severity,
                Source = doc.Source
            }).ToList();

            _logger.LogInformation("Retrieved {Count} child safety events for user {ChildUserId}", 
                auditEvents.Count, childUserId);

            return await Task.FromResult(auditEvents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve child safety events for user {ChildUserId}", childUserId);
            return new List<AuditEvent>();
        }
    }

    /// <summary>
    /// Cleanup old audit events (for GDPR compliance - data retention policies)
    /// </summary>
    public async Task CleanupOldEventsAsync(TimeSpan retentionPeriod)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.Subtract(retentionPeriod);
            var deletedCount = _auditEvents.DeleteMany(x => x.Timestamp < cutoffDate);
            
            _logger.LogInformation("Cleaned up {DeletedCount} old audit events older than {CutoffDate}", 
                deletedCount, cutoffDate);
                
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup old audit events");
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _database?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// LiteDB document model for audit events
/// </summary>
public class AuditEventDocument
{
    public ObjectId Id { get; set; } = ObjectId.NewObjectId();
    public Guid EventId { get; set; }
    public DateTime Timestamp { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public BsonDocument? Data { get; set; }
    public string Severity { get; set; } = "Info";
    public string Source { get; set; } = "WorldLeadersGame";
    public bool IsChildSafetyEvent { get; set; }
}
