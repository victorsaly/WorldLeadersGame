---
layout: page
title: "Issue 5.1: API Security & JWT Authentication Implementation"
date: 2025-08-06
issue_number: "5.1"
week: 5
priority: "critical"
status: "planned"
estimated_hours: 12
ai_autonomy_target: "85%"
production_focus:
  [
    "API security",
    "JWT authentication",
    "Rate limiting",
    "Request throttling",
  ]
azure_services:
  ["Azure AD B2C", "Azure API Management", "Application Insights"]
security_requirements:
  [
    "Client-to-client authentication",
    "API rate limiting",
    "Per-user cost tracking",
    "Child data protection",
  ]
dependencies: ["Week 4 production deployment", "Azure infrastructure"]
related_milestones: ["milestone-05-production-security-authentication"]
azure_region: "UK South"
dotnet_version: "8.0"
documentation_updates:
  - "README.md - Authentication setup guide"
  - "docs/issues.md - Issue completion status"
  - "docs/journey/week-05-production-security.md - Implementation journey"
  - "docs/_posts/YYYY-MM-DD-jwt-authentication-educational-platforms.md - LinkedIn/Medium article"
---

# Issue 5.1: API Security & JWT Authentication Implementation 🔐

**Week 5 Focus**: Production-grade API security with JWT authentication and intelligent rate limiting  
**Security Mission**: Protect educational game APIs while ensuring seamless child-friendly user experience  
**Cost Management**: Implement throttling to prevent Azure AI credit overuse while maintaining performance

---

## 🎯 Issue Objectives

### Primary Security Goals

- [ ] **JWT Authentication System**: Secure client-to-client communication between Frontend and Backend
- [ ] **Azure AD B2C Integration**: Child-safe authentication with parental controls and COPPA compliance
- [ ] **API Rate Limiting**: Intelligent throttling to prevent overuse of Azure AI credits
- [ ] **Request Monitoring**: Real-time tracking of API usage and cost optimization
- [ ] **Security Headers**: Implement CORS, CSRF protection, and security headers
- [ ] **API Gateway**: Azure API Management for centralized security and monitoring

### Cost Management & Performance

- [ ] **AI Service Throttling**: Intelligent rate limiting for Azure OpenAI and Speech Services
- [ ] **Credit Monitoring**: Real-time tracking of Azure AI service usage and costs
- [ ] **Caching Strategy**: Aggressive caching to reduce API calls and improve performance
- [ ] **Performance Optimization**: API response time under 500ms for child engagement
- [ ] **Load Balancing**: Prepare for 1000+ daily users with efficient resource allocation

---

## 🔧 Implementation Phases

### Phase 1: Azure AD B2C Setup (3 hours)

#### 1.1 Azure AD B2C Tenant Configuration
```bash
# Azure CLI commands for AD B2C setup in UK South region
az ad b2c tenant create \
  --tenant-name "worldleadersgame" \
  --domain-name "worldleadersgame.onmicrosoft.com" \
  --country-code "GB" \
  --data-residency "UnitedKingdom" \
  --location "UK South"
```

#### 1.2 Application Registration with .NET 8 Configuration
```csharp
// .NET 8 optimized configuration with modern patterns
public class AzureAdB2CConfig
{
    public required string TenantId { get; init; }
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
    public required string Authority { get; init; }
    public required string RedirectUri { get; init; }
    public string Scope { get; init; } = "openid profile email";
    public string Region { get; init; } = "UK South";
}

// API application registration with .NET 8 records
public record ApiAuthConfig(
    string ApiClientId,
    string ApiClientSecret,
    string[] AllowedScopes,
    bool RequireHttpsMetadata = true,
    string Region = "UK South"
)
{
    public static ApiAuthConfig Default => new(
        ApiClientId: string.Empty,
        ApiClientSecret: string.Empty,
        AllowedScopes: ["api.read", "api.write"]
    );
}
```

#### 1.3 Child-Safe User Flows
```json
{
  "userFlows": {
    "signUpSignIn": {
      "name": "B2C_1_child_safe_signup_signin",
      "ageVerification": true,
      "parentalConsent": true,
      "dataMinimization": true,
      "privacyCompliance": "COPPA"
    },
    "profileEdit": {
      "name": "B2C_1_profile_edit",
      "restrictedFields": ["email", "age", "real_name"],
      "usernameOnly": true
    }
  }
}
```

### Phase 2: JWT Authentication Implementation (4 hours)

#### 2.1 Backend Authentication Service (.NET 8 Optimized)
```csharp
// Context: Production-ready authentication for educational game API with .NET 8 features
// Security Objective: Protect child data while enabling seamless gameplay
// Performance Target: Token validation under 50ms using .NET 8 optimizations
public class JwtAuthenticationService(
    IConfiguration configuration,
    IMemoryCache cache,
    ILogger<JwtAuthenticationService> logger) : IJwtAuthenticationService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IMemoryCache _cache = cache;
    private readonly ILogger<JwtAuthenticationService> _logger = logger;

    public async Task<AuthenticationResult> ValidateTokenAsync(string token)
    {
        try
        {
            // Fast token validation with caching
            var cachedResult = await _cache.GetAsync($"token_validation_{token.GetHashCode()}");
            if (cachedResult != null)
            {
                return JsonSerializer.Deserialize<AuthenticationResult>(cachedResult);
            }

            // Validate JWT token against Azure AD B2C
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetTokenValidationParameters();
            
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            
            var result = new AuthenticationResult
            {
                IsValid = true,
                UserId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Username = principal.FindFirst("preferred_username")?.Value,
                Age = int.Parse(principal.FindFirst("age")?.Value ?? "12"),
                Scopes = principal.FindAll("scp").Select(c => c.Value).ToList()
            };

            // Cache successful validation for 5 minutes
            await _cache.SetAsync($"token_validation_{token.GetHashCode()}", 
                JsonSerializer.SerializeToUtf8Bytes(result),
                TimeSpan.FromMinutes(5));

            return result;
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning("Token validation failed: {Error}", ex.Message);
            return AuthenticationResult.Invalid();
        }
    }

    private TokenValidationParameters GetTokenValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _configuration["AzureAdB2C:Authority"],
            ValidateAudience = true,
            ValidAudience = _configuration["AzureAdB2C:ClientId"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),
            ValidateIssuerSigningKey = true,
            IssuerSigningKeyResolver = GetSigningKeys
        };
    }
}
```

#### 2.2 Frontend Authentication Integration
```typescript
// Context: Child-friendly authentication for educational game frontend
// Safety Objective: Seamless authentication without exposing sensitive data
// UX Objective: Authentication flow suitable for 12-year-old users
export class AuthenticationService {
    private msalInstance: PublicClientApplication;
    private isAuthenticated: boolean = false;

    constructor() {
        this.msalInstance = new PublicClientApplication({
            auth: {
                clientId: process.env.AZURE_B2C_CLIENT_ID!,
                authority: process.env.AZURE_B2C_AUTHORITY!,
                redirectUri: window.location.origin,
            },
            cache: {
                cacheLocation: 'localStorage', // Safe for educational app
                storeAuthStateInCookie: false,
            },
        });
    }

    async loginChild(): Promise<AuthResult> {
        try {
            const loginRequest = {
                scopes: ['openid', 'profile', 'api.worldleadersgame.read'],
                prompt: 'select_account',
            };

            const response = await this.msalInstance.loginPopup(loginRequest);
            this.isAuthenticated = true;

            // Store minimal data for gameplay
            localStorage.setItem('player_token', response.accessToken);
            localStorage.setItem('player_username', response.account?.username || 'Player');

            return {
                success: true,
                username: response.account?.username || 'Player',
                token: response.accessToken,
                expiresAt: new Date(response.expiresOn!),
            };
        } catch (error) {
            console.error('Child-safe login failed:', error);
            return { success: false, error: 'Login failed. Please try again!' };
        }
    }

    async getApiToken(): Promise<string> {
        if (!this.isAuthenticated) {
            throw new Error('User not authenticated');
        }

        const silentRequest = {
            scopes: ['api.worldleadersgame.read', 'api.worldleadersgame.write'],
            account: this.msalInstance.getActiveAccount(),
        };

        const response = await this.msalInstance.acquireTokenSilent(silentRequest);
        return response.accessToken;
    }
}
```

### Phase 3: Rate Limiting & Throttling (3 hours)

#### 3.1 API Rate Limiting Middleware
```csharp
// Context: Cost-effective rate limiting for educational game API
// Objective: Prevent Azure AI credit overuse while maintaining user experience
// Target: Support 1000+ daily users within budget constraints
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimitService _rateLimitService;
    private readonly ILogger<RateLimitingMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);
        var endpoint = context.Request.Path.Value;

        // Different rate limits for different endpoints
        var rateLimitConfig = GetRateLimitConfig(endpoint);
        
        var rateLimitResult = await _rateLimitService.CheckRateLimitAsync(
            clientId, endpoint, rateLimitConfig);

        if (!rateLimitResult.IsAllowed)
        {
            context.Response.StatusCode = 429; // Too Many Requests
            context.Response.Headers.Add("Retry-After", 
                rateLimitResult.RetryAfterSeconds.ToString());
            
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                error = "Rate limit exceeded",
                message = "Please wait before making more requests",
                retryAfterSeconds = rateLimitResult.RetryAfterSeconds,
                isChildFriendly = true
            }));
            
            _logger.LogWarning("Rate limit exceeded for client {ClientId} on endpoint {Endpoint}", 
                clientId, endpoint);
            return;
        }

        // Add rate limit headers for transparency
        context.Response.Headers.Add("X-RateLimit-Limit", rateLimitConfig.RequestsPerMinute.ToString());
        context.Response.Headers.Add("X-RateLimit-Remaining", rateLimitResult.RemainingRequests.ToString());
        context.Response.Headers.Add("X-RateLimit-Reset", rateLimitResult.ResetTime.ToString());

        await _next(context);
    }

    private RateLimitConfig GetRateLimitConfig(string endpoint)
    {
        return endpoint switch
        {
            var e when e.Contains("/ai/agent") => new RateLimitConfig
            {
                RequestsPerMinute = 10,  // AI agents - higher cost
                RequestsPerHour = 100,
                BurstLimit = 3
            },
            var e when e.Contains("/speech") => new RateLimitConfig
            {
                RequestsPerMinute = 5,   // Speech recognition - highest cost
                RequestsPerHour = 50,
                BurstLimit = 2
            },
            var e when e.Contains("/game") => new RateLimitConfig
            {
                RequestsPerMinute = 30,  // Game mechanics - low cost
                RequestsPerHour = 1000,
                BurstLimit = 10
            },
            _ => new RateLimitConfig
            {
                RequestsPerMinute = 20,  // Default rate limit
                RequestsPerHour = 500,
                BurstLimit = 5
            }
        };
    }
}
```

#### 3.2 Per-User Azure AI Cost Tracking (.NET 8)
```csharp
// Context: Track Azure AI costs per individual user for educational analytics
// Objective: Understand cost patterns and optimize per-learner resource usage
// Target: Detailed cost attribution for 1000+ daily users within budget
public class PerUserCostTracker(
    IMemoryCache cache,
    IConfiguration configuration,
    ILogger<PerUserCostTracker> logger) : IPerUserCostTracker
{
    private readonly IMemoryCache _cache = cache;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<PerUserCostTracker> _logger = logger;

    // Per-user daily budget allocation (distributed from total $10/day)
    private readonly decimal MaxDailyUserCost = 0.10m; // $0.10 per user per day
    private readonly decimal MaxHourlyUserCost = 0.005m; // $0.005 per user per hour

    public async Task<UserCostValidation> ValidateUserRequestAsync(string userId, string serviceType, string operation)
    {
        var userCostKey = $"user_cost_{userId}_{DateTime.UtcNow:yyyyMMdd}";
        var userHourlyCostKey = $"user_cost_hourly_{userId}_{DateTime.UtcNow:yyyyMMddHH}";

        var currentDailyCost = await GetUserDailyCostAsync(userId);
        var currentHourlyCost = await GetUserHourlyCostAsync(userId);
        var estimatedCost = EstimateOperationCostPerUser(serviceType, operation);

        // Check per-user budget limits
        if (currentDailyCost + estimatedCost > MaxDailyUserCost)
        {
            _logger.LogWarning("User {UserId} would exceed daily budget. Current: ${Current}, Estimated: ${Estimated}", 
                userId, currentDailyCost, estimatedCost);
            
            return new UserCostValidation
            {
                IsAllowed = false,
                Reason = $"Daily user budget (${MaxDailyUserCost}) would be exceeded",
                CurrentDailyCost = currentDailyCost,
                EstimatedCost = estimatedCost,
                SuggestedAction = "Try again tomorrow or use cached content"
            };
        }

        if (currentHourlyCost + estimatedCost > MaxHourlyUserCost)
        {
            return new UserCostValidation
            {
                IsAllowed = false,
                Reason = $"Hourly user budget (${MaxHourlyUserCost}) would be exceeded",
                CurrentHourlyCost = currentHourlyCost,
                EstimatedCost = estimatedCost,
                SuggestedAction = "Please wait a few minutes before making more AI requests"
            };
        }

        // Track the cost for this user
        await RecordUserCostAsync(userId, serviceType, operation, estimatedCost);

        return new UserCostValidation
        {
            IsAllowed = true,
            EstimatedCost = estimatedCost,
            RemainingDailyBudget = MaxDailyUserCost - currentDailyCost - estimatedCost,
            CostBreakdown = GetUserCostBreakdown(userId)
        };
    }

    private async Task RecordUserCostAsync(string userId, string serviceType, string operation, decimal cost)
    {
        var today = DateTime.UtcNow.Date;
        var hour = DateTime.UtcNow.Hour;

        // Daily tracking
        var dailyKey = $"user_cost_{userId}_{today:yyyyMMdd}";
        var currentDailyCost = await _cache.GetOrCreateAsync(dailyKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
            return new UserDailyCost { UserId = userId, Date = today, TotalCost = 0m, Operations = new() };
        });

        currentDailyCost!.TotalCost += cost;
        currentDailyCost.Operations.Add(new UserOperation
        {
            Timestamp = DateTime.UtcNow,
            ServiceType = serviceType,
            Operation = operation,
            Cost = cost
        });

        // Hourly tracking
        var hourlyKey = $"user_cost_hourly_{userId}_{today:yyyyMMddHH}";
        var currentHourlyCost = await _cache.GetOrCreateAsync(hourlyKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            return 0m;
        });

        await _cache.SetAsync(hourlyKey, currentHourlyCost + cost, TimeSpan.FromHours(1));

        _logger.LogInformation("Recorded cost for user {UserId}: {ServiceType}.{Operation} = ${Cost} (Daily total: ${DailyTotal})", 
            userId, serviceType, operation, cost, currentDailyCost.TotalCost);
    }

    public async Task<UserCostReport> GenerateUserCostReportAsync(string userId, DateTime? startDate = null)
    {
        var start = startDate ?? DateTime.UtcNow.AddDays(-7); // Last 7 days by default
        var report = new UserCostReport
        {
            UserId = userId,
            ReportPeriod = $"{start:yyyy-MM-dd} to {DateTime.UtcNow:yyyy-MM-dd}",
            GeneratedAt = DateTime.UtcNow
        };

        // Collect cost data for the period
        for (var date = start.Date; date <= DateTime.UtcNow.Date; date = date.AddDays(1))
        {
            var dailyKey = $"user_cost_{userId}_{date:yyyyMMdd}";
            var dailyCost = await _cache.GetAsync<UserDailyCost>(dailyKey);
            
            if (dailyCost != null)
            {
                report.DailyCosts.Add(dailyCost);
                report.TotalCost += dailyCost.TotalCost;
            }
        }

        // Calculate cost per service type
        report.CostByService = report.DailyCosts
            .SelectMany(dc => dc.Operations)
            .GroupBy(op => op.ServiceType)
            .ToDictionary(g => g.Key, g => g.Sum(op => op.Cost));

        // Educational usage insights
        report.EducationalInsights = GenerateEducationalCostInsights(report);

        return report;
    }

    private EducationalCostInsights GenerateEducationalCostInsights(UserCostReport report)
    {
        var totalOperations = report.DailyCosts.SelectMany(dc => dc.Operations).Count();
        var avgCostPerOperation = totalOperations > 0 ? report.TotalCost / totalOperations : 0;

        return new EducationalCostInsights
        {
            AverageCostPerLearningSession = avgCostPerOperation,
            MostUsedService = report.CostByService.OrderByDescending(kvp => kvp.Value).FirstOrDefault().Key ?? "None",
            EfficiencyScore = CalculateEfficiencyScore(report),
            Recommendations = GenerateCostOptimizationRecommendations(report)
        };
    }

    private decimal CalculateEfficiencyScore(UserCostReport report)
    {
        // Score based on educational value per dollar spent
        var aiInteractions = report.DailyCosts.SelectMany(dc => dc.Operations).Count(op => op.ServiceType == "OpenAI");
        var speechInteractions = report.DailyCosts.SelectMany(dc => dc.Operations).Count(op => op.ServiceType == "Speech");
        
        var educationalInteractions = aiInteractions + speechInteractions;
        var educationalCost = report.CostByService.GetValueOrDefault("OpenAI", 0) + 
                             report.CostByService.GetValueOrDefault("Speech", 0);

        return educationalCost > 0 ? educationalInteractions / educationalCost : 0;
    }
}

// Supporting data structures for .NET 8
public record UserCostValidation
{
    public required bool IsAllowed { get; init; }
    public string? Reason { get; init; }
    public decimal CurrentDailyCost { get; init; }
    public decimal CurrentHourlyCost { get; init; }
    public decimal EstimatedCost { get; init; }
    public decimal RemainingDailyBudget { get; init; }
    public string? SuggestedAction { get; init; }
    public Dictionary<string, decimal>? CostBreakdown { get; init; }
}

public record UserDailyCost
{
    public required string UserId { get; init; }
    public required DateTime Date { get; init; }
    public decimal TotalCost { get; set; }
    public List<UserOperation> Operations { get; init; } = new();
}

public record UserOperation
{
    public required DateTime Timestamp { get; init; }
    public required string ServiceType { get; init; }
    public required string Operation { get; init; }
    public required decimal Cost { get; init; }
}

public record UserCostReport
{
    public required string UserId { get; init; }
    public required string ReportPeriod { get; init; }
    public required DateTime GeneratedAt { get; init; }
    public decimal TotalCost { get; set; }
    public List<UserDailyCost> DailyCosts { get; init; } = new();
    public Dictionary<string, decimal> CostByService { get; set; } = new();
    public EducationalCostInsights? EducationalInsights { get; set; }
}

public record EducationalCostInsights
{
    public decimal AverageCostPerLearningSession { get; init; }
    public string MostUsedService { get; init; } = "";
    public decimal EfficiencyScore { get; init; }
    public List<string> Recommendations { get; init; } = new();
}
```

#### 3.3 Azure AI Service Cost Management (.NET 8 Enhanced)
```csharp
// Context: Enhanced Azure AI credit monitoring with per-user attribution
// Objective: Maximize educational value while controlling costs per learner
// Target: Serve 1000+ users efficiently within total budget constraints
public class AzureAICostManager(
    IConfiguration configuration,
    IMemoryCache cache,
    IPerUserCostTracker userCostTracker,
    ILogger<AzureAICostManager> logger) : IAzureAICostManager
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IMemoryCache _cache = cache;
    private readonly IPerUserCostTracker _userCostTracker = userCostTracker;
    private readonly ILogger<AzureAICostManager> _logger = logger;

    // Daily budget limits (adjustable based on Victor's budget)
    private readonly decimal DailyBudgetLimit = 10.00m; // $10 per day
    private readonly decimal HourlyBudgetLimit = 0.50m;  // $0.50 per hour

    public async Task<CostCheckResult> ValidateRequestCostAsync(string serviceType, string operation)
    {
        var currentHourlyCost = await GetCurrentHourlyCostAsync();
        var currentDailyCost = await GetCurrentDailyCostAsync();
        
        var estimatedCost = EstimateOperationCost(serviceType, operation);

        // Check if this request would exceed budgets
        if (currentHourlyCost + estimatedCost > HourlyBudgetLimit)
        {
            _logger.LogWarning("Hourly budget limit would be exceeded. Current: ${Current}, Estimated: ${Estimated}", 
                currentHourlyCost, estimatedCost);
            
            return new CostCheckResult
            {
                IsAllowed = false,
                Reason = "Hourly budget limit reached",
                RetryAfterMinutes = GetMinutesUntilNextHour(),
                AlternativeAction = GetLowerCostAlternative(serviceType, operation)
            };
        }

        if (currentDailyCost + estimatedCost > DailyBudgetLimit)
        {
            _logger.LogWarning("Daily budget limit would be exceeded. Current: ${Current}, Estimated: ${Estimated}", 
                currentDailyCost, estimatedCost);
            
            return new CostCheckResult
            {
                IsAllowed = false,
                Reason = "Daily budget limit reached",
                RetryAfterMinutes = GetMinutesUntilMidnight(),
                AlternativeAction = GetCachedResponse(serviceType, operation)
            };
        }

        return new CostCheckResult { IsAllowed = true, EstimatedCost = estimatedCost };
    }

    private decimal EstimateOperationCost(string serviceType, string operation)
    {
        return serviceType switch
        {
            "OpenAI" => operation switch
            {
                "ChatCompletion" => 0.002m,  // ~$0.002 per request
                "TextGeneration" => 0.001m,
                _ => 0.001m
            },
            "Speech" => operation switch
            {
                "SpeechToText" => 0.004m,    // ~$0.004 per minute
                "PronunciationAssessment" => 0.006m,
                _ => 0.003m
            },
            "Cognitive" => 0.001m,
            _ => 0.0005m
        };
    }

    private string GetLowerCostAlternative(string serviceType, string operation)
    {
        return serviceType switch
        {
            "OpenAI" => "Use pre-generated educational content from cache",
            "Speech" => "Use simplified pronunciation feedback",
            "Cognitive" => "Use basic content filtering",
            _ => "Use cached response if available"
        };
    }
}
```

### Phase 4: Azure API Management Integration (2 hours)

#### 4.1 API Gateway Configuration
```xml
<!-- Azure API Management Policy Configuration -->
<policies>
    <inbound>
        <!-- Rate limiting per subscription -->
        <rate-limit calls="1000" renewal-period="3600" />
        
        <!-- Authentication validation -->
        <validate-jwt header-name="Authorization" failed-validation-httpcode="401">
            <openid-config url="https://worldleadersgame.b2clogin.com/.well-known/openid_configuration" />
            <audiences>
                <audience>api.worldleadersgame.co.uk</audience>
            </audiences>
            <issuers>
                <issuer>https://worldleadersgame.b2clogin.com</issuer>
            </issuers>
        </validate-jwt>
        
        <!-- Cost monitoring -->
        <set-variable name="request-cost" value="@{
            string path = context.Request.Url.Path;
            if (path.Contains(\"/ai/\")) return \"0.002\";
            if (path.Contains(\"/speech/\")) return \"0.004\";
            return \"0.0005\";
        }" />
        
        <!-- Child safety headers -->
        <set-header name="X-Content-Type-Options" exists-action="override">
            <value>nosniff</value>
        </set-header>
        <set-header name="X-Frame-Options" exists-action="override">
            <value>DENY</value>
        </set-header>
        
        <!-- CORS for child-safe domains only -->
        <cors allow-credentials="false">
            <allowed-origins>
                <origin>https://worldleadersgame.co.uk</origin>
                <origin>https://docs.worldleadersgame.co.uk</origin>
            </allowed-origins>
            <allowed-methods>
                <method>GET</method>
                <method>POST</method>
                <method>OPTIONS</method>
            </allowed-methods>
            <allowed-headers>
                <header>Authorization</header>
                <header>Content-Type</header>
                <header>X-Requested-With</header>
            </allowed-headers>
        </cors>
    </inbound>
    
    <backend>
        <forward-request timeout="30" />
    </backend>
    
    <outbound>
        <!-- Add cost tracking -->
        <set-header name="X-Request-Cost" exists-action="override">
            <value>@((string)context.Variables["request-cost"])</value>
        </set-header>
        
        <!-- Cache expensive operations -->
        <cache-store duration="300" />
    </outbound>
    
    <on-error>
        <!-- Child-friendly error responses -->
        <set-body>@{
            return new JObject(
                new JProperty("error", "Something went wrong"),
                new JProperty("message", "Please try again in a moment"),
                new JProperty("isChildFriendly", true),
                new JProperty("supportEmail", "support@worldleadersgame.co.uk")
            ).ToString();
        }</set-body>
    </on-error>
</policies>
```

---

## 🧪 Testing & Validation

### Security Testing
```csharp
[TestClass]
public class ApiSecurityTests
{
    [TestMethod]
    public async Task API_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/game/territories");

        // Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task API_WithValidJWT_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var token = await GetValidJwtTokenAsync();
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/game/territories");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task API_ExceedsRateLimit_ReturnsThrottled()
    {
        // Arrange
        var client = _factory.CreateClient();
        var token = await GetValidJwtTokenAsync();
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        // Act - Make requests exceeding rate limit
        var tasks = Enumerable.Range(0, 50)
            .Select(_ => client.GetAsync("/api/ai/agent/career-guide"));
        var responses = await Task.WhenAll(tasks);

        // Assert - Some requests should be rate limited
        Assert.IsTrue(responses.Any(r => r.StatusCode == HttpStatusCode.TooManyRequests));
    }
}
```

---

## � Documentation Updates (Mandatory)

### Required Documentation Updates
- [ ] **README.md**: Add authentication setup and JWT configuration guide
- [ ] **docs/issues.md**: Update Issue 5.1 status to completed with success metrics
- [ ] **docs/journey/week-05-production-security.md**: Document implementation journey and lessons learned
- [ ] **docs/_posts/**: Create LinkedIn/Medium article about JWT authentication for educational platforms

### LinkedIn/Medium Article: "Securing Educational Platforms: JWT Authentication for Child-Safe Learning"

#### Article Outline
```markdown
# Securing Educational Platforms: JWT Authentication for Child-Safe Learning

**AI-Generated Image Prompt**: "A digital shield protecting children using educational technology, with JWT tokens floating as secure locks around a colorful learning interface, UK cybersecurity theme, professional yet child-friendly illustration"

## Introduction
Building educational platforms for children requires a delicate balance between robust security and seamless user experience. In this article, we explore implementing JWT authentication with Azure AD B2C for a child-safe educational gaming platform serving 1000+ daily learners.

## The Challenge
- Child data protection (COPPA compliance)
- Per-user cost tracking and budget management
- Scalable authentication for educational institutions
- .NET 8 performance optimizations

## Our Solution
### 1. Azure AD B2C with UK Data Residency
- Detailed configuration for UK South region
- Child-safe user flows with parental consent
- Minimal data collection principles

### 2. .NET 8 Optimizations
- Primary constructors for dependency injection
- Record types for immutable configuration
- Performance improvements in JWT validation

### 3. Per-User Cost Attribution
- Real-time tracking of Azure AI costs per learner
- Educational efficiency scoring
- Budget optimization recommendations

## Key Learnings
1. **Child Safety First**: Authentication flows must be intuitive for 12-year-olds
2. **Cost Transparency**: Per-user cost tracking enables better educational ROI
3. **Regional Compliance**: UK data residency for educational institutions
4. **Performance Matters**: Sub-50ms authentication for maintaining engagement

## Implementation Insights
[Code snippets and architecture diagrams]

## Results
- 99%+ authentication success rate
- <50ms average token validation time
- $0.10 per user daily cost target achieved
- Full COPPA compliance maintained

## Conclusion
Securing educational platforms requires more than traditional authentication. By combining Azure AD B2C, .NET 8 optimizations, and granular cost tracking, we created a solution that protects children while enabling sustainable educational technology.

---
*This article is part of our journey building World Leaders Game, an educational platform teaching geography and economics to 12-year-olds through AI-assisted gameplay.*
```

### Documentation Template Updates

#### README.md Update Section
```markdown
## 🔐 Authentication Setup

### Azure AD B2C Configuration
1. **Create B2C Tenant** in UK South region
2. **Register Applications** for client-to-client JWT flow
3. **Configure User Flows** with child-safe policies
4. **Set Per-User Cost Limits** for budget management

### Local Development
```bash
# Configure authentication secrets
dotnet user-secrets set "AzureAdB2C:ClientSecret" "your-client-secret"
dotnet user-secrets set "AzureAdB2C:TenantId" "your-tenant-id"

# Start with authentication enabled
dotnet run --project WorldLeaders.API --environment Development
```

### Production Deployment
- JWT validation with Azure AD B2C
- Per-user cost tracking enabled
- Rate limiting: 10 AI requests/minute per user
- UK data residency compliance
```

#### Journey Update Template
```markdown
## Week 5, Day 1: JWT Authentication Implementation

### 🎯 Objectives Achieved
- ✅ Azure AD B2C tenant configured in UK South
- ✅ .NET 8 primary constructors implemented
- ✅ Per-user cost tracking system operational
- ✅ Child-safe authentication flows tested

### 🔧 Technical Implementation
**Key Technologies**: Azure AD B2C, .NET 8, JWT validation
**Performance**: <50ms token validation achieved
**Cost Control**: $0.10 per user daily budget implemented

### 🧠 Lessons Learned
1. **.NET 8 Benefits**: Primary constructors reduced boilerplate by 40%
2. **Child UX**: Simple authentication flows crucial for 12-year-olds
3. **Cost Attribution**: Per-user tracking enables better resource optimization
4. **Regional Compliance**: UK data residency essential for educational institutions

### 🚧 Challenges Overcome
- **Azure B2C Setup**: Required custom policies for child-safe flows
- **Cost Tracking**: Implementing granular per-user attribution
- **Performance**: Optimizing JWT validation for high-throughput scenarios

### �📊 Success Metrics
- Authentication success rate: 99.2%
- Average response time: 47ms
- Per-user cost tracking: 100% coverage
- COPPA compliance: Fully validated

### 🔄 Next Steps
- Implement rate limiting middleware (Issue 5.2)
- Set up auto-scaling infrastructure
- Deploy cost management dashboard
```

---

## 🏗️ GitHub Milestone Integration

### Milestone 5: Production Security & Authentication
```markdown
**Milestone**: Production Security & Authentication
**Due Date**: August 13, 2025
**Description**: Complete JWT authentication system with per-user cost tracking for educational platform

**Success Criteria**:
- [ ] Azure AD B2C configured with UK data residency
- [ ] JWT authentication working with <50ms validation
- [ ] Per-user cost tracking operational
- [ ] Rate limiting preventing budget overruns
- [ ] Documentation updated (README, journey, LinkedIn article)
- [ ] All tests passing with >95% coverage

**Issues in Milestone**:
- Issue 5.1: API Security & JWT Authentication ✅
- Issue 5.2: Performance & Scalability Optimization
- Issue 5.3: Azure Cost Management & AI Service Monitoring
- Issue 5.4: Production Security Hardening & Compliance
- Issue 5.5: Infrastructure as Code & Automated Deployment Pipeline
```

### Security Metrics
- [ ] **Authentication Success Rate**: >99% successful JWT validations
- [ ] **API Response Time**: <500ms average with authentication
- [ ] **Rate Limiting Effectiveness**: 0 budget overruns
- [ ] **Security Headers**: 100% coverage on all endpoints
- [ ] **CORS Protection**: Only authorized domains allowed

### Cost Management Metrics
- [ ] **Daily Budget Compliance**: Stay within $10/day Azure AI costs
- [ ] **Request Efficiency**: >80% cache hit rate for expensive operations
- [ ] **Throttling Success**: Prevent budget overruns while maintaining UX
- [ ] **Performance Impact**: <50ms authentication overhead

### Production Readiness
- [ ] **Scalability**: Support 1000+ concurrent users
- [ ] **Reliability**: 99.9% API uptime
- [ ] **Child Safety**: 100% COPPA compliance in authentication
- [ ] **Monitoring**: Real-time cost and performance tracking

---

## 🚀 Deployment Strategy

### Development Environment
```bash
# Local development with Azure AD B2C emulator
az login
az account set --subscription "WorldLeadersGame-Dev"
dotnet user-secrets set "AzureAdB2C:ClientSecret" "dev-secret"
dotnet run --project WorldLeaders.API --environment Development
```

### Production Deployment
```bash
# Azure deployment with production configuration
az deployment group create \
  --resource-group "worldleadersgame-prod" \
  --template-file "infrastructure/azure-security.bicep" \
  --parameters \
    environment="production" \
    dailyBudgetLimit="10.00" \
    enableAdvancedSecurity="true"
```

---

## 📚 Educational Impact

### Child Safety Enhancements
- **COPPA Compliance**: Full compliance with children's online privacy laws
- **Minimal Data Collection**: Only username and age for gameplay
- **Parental Controls**: Optional parental oversight and progress sharing
- **Safe Authentication**: Child-friendly login flow with clear instructions

### Performance for Learning
- **Fast Response Times**: <500ms API responses maintain engagement
- **Reliable Access**: 99.9% uptime ensures consistent learning access
- **Cost-Effective Scaling**: Sustainable model for 1000+ daily learners
- **Progressive Enhancement**: Graceful degradation when rate limits reached

---

**Critical Success Factor**: This issue establishes the security foundation that enables sustainable, scalable educational gaming for children while protecting both user data and project costs.
