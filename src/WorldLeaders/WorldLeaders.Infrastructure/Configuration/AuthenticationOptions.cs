namespace WorldLeaders.Infrastructure.Configuration;

/// <summary>
/// Configuration options for JWT authentication
/// Context: Educational game authentication for 12-year-old players
/// Safety Requirements: Secure token configuration with child-appropriate timeouts
/// </summary>
public record JwtOptions
{
    public const string SectionName = "Jwt";

    /// <summary>
    /// Secret key for JWT signing (must be at least 32 characters)
    /// </summary>
    public required string SecretKey { get; init; }

    /// <summary>
    /// JWT issuer (your application identifier)
    /// </summary>
    public required string Issuer { get; init; }

    /// <summary>
    /// JWT audience (typically your application URL)
    /// </summary>
    public required string Audience { get; init; }

    /// <summary>
    /// Token expiration time in minutes (shorter for child accounts)
    /// </summary>
    public int ExpirationMinutes { get; init; } = 60;

    /// <summary>
    /// Child session timeout in minutes (stricter for safety)
    /// </summary>
    public int ChildSessionTimeoutMinutes { get; init; } = 30;

    /// <summary>
    /// Whether to validate issuer
    /// </summary>
    public bool ValidateIssuer { get; init; } = true;

    /// <summary>
    /// Whether to validate audience
    /// </summary>
    public bool ValidateAudience { get; init; } = true;

    /// <summary>
    /// Whether to validate token lifetime
    /// </summary>
    public bool ValidateLifetime { get; init; } = true;

    /// <summary>
    /// Clock skew tolerance in minutes
    /// </summary>
    public int ClockSkewMinutes { get; init; } = 5;
}

/// <summary>
/// Azure AD B2C configuration for UK South region
/// </summary>
public record AzureAdB2COptions
{
    public const string SectionName = "AzureAdB2C";

    /// <summary>
    /// Azure AD B2C tenant ID
    /// </summary>
    public required string TenantId { get; init; }

    /// <summary>
    /// B2C application (client) ID
    /// </summary>
    public required string ClientId { get; init; }

    /// <summary>
    /// B2C application secret
    /// </summary>
    public required string ClientSecret { get; init; }

    /// <summary>
    /// B2C instance URL (e.g., https://yourtenant.b2clogin.com)
    /// </summary>
    public required string Instance { get; init; }

    /// <summary>
    /// Primary domain for B2C tenant
    /// </summary>
    public required string Domain { get; init; }

    /// <summary>
    /// Sign-up/sign-in policy name
    /// </summary>
    public required string SignUpSignInPolicyId { get; init; }

    /// <summary>
    /// Password reset policy name
    /// </summary>
    public required string ResetPasswordPolicyId { get; init; }

    /// <summary>
    /// Profile edit policy name
    /// </summary>
    public required string EditProfilePolicyId { get; init; }

    /// <summary>
    /// Azure region (must be UK South for educational data residency)
    /// </summary>
    public string Region { get; init; } = "UK South";

    /// <summary>
    /// Whether B2C integration is enabled
    /// </summary>
    public bool Enabled { get; init; } = false;

    /// <summary>
    /// API scopes for B2C
    /// </summary>
    public List<string> ApiScopes { get; init; } = new();

    /// <summary>
    /// Default configuration for UK educational deployment
    /// </summary>
    public static AzureAdB2COptions UKEducationalDefaults => new()
    {
        TenantId = "your-tenant-id",
        ClientId = "your-client-id", 
        ClientSecret = "your-client-secret",
        Instance = "https://yourtenant.b2clogin.com",
        Domain = "yourtenant.onmicrosoft.com",
        SignUpSignInPolicyId = "B2C_1_susi_educational",
        ResetPasswordPolicyId = "B2C_1_pwd_reset_educational", 
        EditProfilePolicyId = "B2C_1_profile_edit_educational",
        Region = "UK South",
        Enabled = false,
        ApiScopes = new List<string> { "https://yourtenant.onmicrosoft.com/api/access" }
    };
}

/// <summary>
/// Child safety and compliance configuration
/// </summary>
public record ChildSafetyOptions
{
    public const string SectionName = "ChildSafety";

    /// <summary>
    /// Whether child safety features are enabled
    /// </summary>
    public bool Enabled { get; init; } = true;

    /// <summary>
    /// Age threshold for child accounts (COPPA compliance)
    /// </summary>
    public int ChildAgeThreshold { get; init; } = 13;

    /// <summary>
    /// Whether parental consent is required for child accounts
    /// </summary>
    public bool RequireParentalConsent { get; init; } = true;

    /// <summary>
    /// Session timeout for child accounts (minutes)
    /// </summary>
    public int ChildSessionTimeoutMinutes { get; init; } = 30;

    /// <summary>
    /// Session timeout for adult accounts (minutes)
    /// </summary>
    public int AdultSessionTimeoutMinutes { get; init; } = 120;

    /// <summary>
    /// Warning time before session expires (minutes)
    /// </summary>
    public int SessionWarningMinutes { get; init; } = 5;

    /// <summary>
    /// Whether to log all child safety events
    /// </summary>
    public bool LogAllEvents { get; init; } = true;

    /// <summary>
    /// Content moderation strictness level
    /// </summary>
    public string ContentModerationLevel { get; init; } = "Strict";

    /// <summary>
    /// Whether GDPR compliance is enforced
    /// </summary>
    public bool EnforceGdprCompliance { get; init; } = true;

    /// <summary>
    /// Data retention period in days (GDPR requirement)
    /// </summary>
    public int DataRetentionDays { get; init; } = 365;
}

/// <summary>
/// Cost tracking configuration for Azure services
/// </summary>
public record CostTrackingOptions
{
    public const string SectionName = "CostTracking";

    /// <summary>
    /// Whether cost tracking is enabled
    /// </summary>
    public bool Enabled { get; init; } = true;

    /// <summary>
    /// Daily cost limit per user in GBP
    /// </summary>
    public decimal DailyCostLimitGBP { get; init; } = 0.08m;

    /// <summary>
    /// Currency for cost tracking
    /// </summary>
    public string Currency { get; init; } = "GBP";

    /// <summary>
    /// OpenAI API cost per 1K tokens (GPT-4)
    /// </summary>
    public decimal OpenAICostPer1KTokens { get; init; } = 0.02m;

    /// <summary>
    /// Speech services cost per hour
    /// </summary>
    public decimal SpeechServicesCostPerHour { get; init; } = 0.80m;

    /// <summary>
    /// Content moderator cost per 1K calls
    /// </summary>
    public decimal ContentModeratorCostPer1KCalls { get; init; } = 0.80m;

    /// <summary>
    /// Whether to block users who exceed daily limit
    /// </summary>
    public bool BlockOnLimitExceeded { get; init; } = true;

    /// <summary>
    /// Grace period after limit exceeded (minutes)
    /// </summary>
    public int GracePeriodMinutes { get; init; } = 15;

    /// <summary>
    /// Time zone for daily cost reset
    /// </summary>
    public string TimeZone { get; init; } = "GMT Standard Time";
}

/// <summary>
/// Authentication and authorization configuration
/// </summary>
public record AuthenticationOptions
{
    /// <summary>
    /// JWT configuration
    /// </summary>
    public required JwtOptions Jwt { get; init; }

    /// <summary>
    /// Azure AD B2C configuration
    /// </summary>
    public required AzureAdB2COptions AzureAdB2C { get; init; }

    /// <summary>
    /// Child safety configuration
    /// </summary>
    public required ChildSafetyOptions ChildSafety { get; init; }

    /// <summary>
    /// Cost tracking configuration
    /// </summary>
    public required CostTrackingOptions CostTracking { get; init; }

    /// <summary>
    /// Whether authentication is required for all endpoints
    /// </summary>
    public bool RequireAuthentication { get; init; } = true;

    /// <summary>
    /// Endpoints that don't require authentication
    /// </summary>
    public List<string> AllowAnonymousEndpoints { get; init; } = new()
    {
        "/health",
        "/swagger",
        "/api/auth/register",
        "/api/auth/login"
    };

    /// <summary>
    /// Default configuration for UK educational deployment
    /// </summary>
    public static AuthenticationOptions UKEducationalDefaults => new()
    {
        Jwt = new JwtOptions
        {
            SecretKey = "REPLACE_WITH_SECURE_KEY_FROM_CONFIG", // Must be set via configuration or environment variable
            Issuer = "WorldLeadersGame",
            Audience = "WorldLeadersGame.API",
            ExpirationMinutes = 60,
            ChildSessionTimeoutMinutes = 30
        },
        AzureAdB2C = new AzureAdB2COptions
        {
            TenantId = "SET_FROM_CONFIG",
            ClientId = "SET_FROM_CONFIG", 
            ClientSecret = "SET_FROM_CONFIG",
            Instance = "SET_FROM_CONFIG",
            Domain = "SET_FROM_CONFIG",
            SignUpSignInPolicyId = "B2C_1_susi_educational",
            ResetPasswordPolicyId = "B2C_1_pwd_reset_educational",
            EditProfilePolicyId = "B2C_1_profile_edit_educational",
            Region = "UK South",
            Enabled = false,
            ApiScopes = new List<string> { "SET_FROM_CONFIG" }
        },
        ChildSafety = new ChildSafetyOptions(),
        CostTracking = new CostTrackingOptions(),
        RequireAuthentication = true
    };
}