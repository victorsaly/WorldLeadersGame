namespace WorldLeaders.Infrastructure.Configuration;

/// <summary>
/// Configuration options for LiteDB
/// Context: Educational game persistent data storage
/// Safety Requirements: Secure file-based database for child data protection
/// </summary>
public class LiteDbOptions
{
    public const string SectionName = "LiteDb";

    /// <summary>
    /// Connection string for LiteDB database file
    /// Default: Data/worldleaders.db in application directory
    /// </summary>
    public string ConnectionString { get; set; } = Path.Combine(AppContext.BaseDirectory, "Data", "worldleaders.db");

    /// <summary>
    /// Enable password protection for database file
    /// </summary>
    public bool UsePassword { get; set; } = false;

    /// <summary>
    /// Database password (only used if UsePassword is true)
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Connection timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 60;

    /// <summary>
    /// Maximum size of database file in MB
    /// </summary>
    public int MaxSizeMB { get; set; } = 100;

    /// <summary>
    /// Enable auto-shrink to optimize file size
    /// </summary>
    public bool AutoShrink { get; set; } = true;
}
