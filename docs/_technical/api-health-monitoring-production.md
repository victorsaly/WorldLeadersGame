---
layout: page
title: "API Health Monitoring & Production Documentation"
date: 2025-08-05
category: "technical-guide"
tags: ["health-checks", "swagger", "production", "monitoring"]
educational_objective: "Professional API monitoring and documentation for educational game services"
---

# API Health Monitoring & Production Documentation

## 🎯 Educational Objective

Implement professional-grade API monitoring and documentation that supports the educational mission while maintaining production-ready standards for child safety and game reliability.

## 🏥 Health Endpoints

### Production Health Endpoints

Our health endpoints are now available in **all environments** for proper monitoring:

#### Primary Health Check

- **URL**: `/health`
- **Purpose**: Comprehensive health check of all registered services
- **Use Case**: Container orchestration, load balancer health checks
- **Response**: `200 OK` if healthy, `503 Service Unavailable` if unhealthy

#### Liveness Check

- **URL**: `/alive`
- **Purpose**: Basic liveness check (only checks tagged with "live")
- **Use Case**: Kubernetes liveness probes, basic service monitoring
- **Response**: `200 OK` if alive, `503 Service Unavailable` if dead

### Why Health Endpoints in Production?

#### Container Orchestration

```yaml
# Kubernetes example
livenessProbe:
  httpGet:
    path: /alive
    port: 8080
  initialDelaySeconds: 30
  periodSeconds: 10

readinessProbe:
  httpGet:
    path: /health
    port: 8080
  initialDelaySeconds: 5
  periodSeconds: 5
```

#### Load Balancer Integration

- **Azure Application Gateway**: Uses health endpoints for backend pool monitoring
- **Azure Load Balancer**: Health probes ensure traffic only goes to healthy instances
- **Cloudflare**: Can monitor health endpoints for failover scenarios

#### Educational Game Reliability

- **Child Safety**: Ensures AI services are responding before allowing child interactions
- **Learning Continuity**: Prevents game interruptions during educational activities
- **Parent Confidence**: Reliable service monitoring builds trust in educational platform

## 📚 Swagger Documentation

### Development vs Production Configuration

#### Development Environment

```csharp
// Full Swagger UI with testing capabilities
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "World Leaders Game API v1");
    c.RoutePrefix = string.Empty; // Available at root URL
    c.DisplayRequestDuration();
    c.EnableTryItOutByDefault();
});
```

#### Production Environment

```csharp
// Documentation-only Swagger UI
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "World Leaders Game API v1");
    c.RoutePrefix = "docs"; // Available at /docs
    c.DocumentTitle = "World Leaders Game API Documentation";
    c.DefaultModelExpandDepth(1);
    c.DefaultModelsExpandDepth(1);
    c.SupportedSubmitMethods(); // Disables "Try it out" functionality
});
```

### Educational Transparency Benefits

#### For Educators

- **API Understanding**: Teachers can see exactly what educational data is collected
- **Integration Planning**: Schools can understand how to integrate with the game
- **Safety Verification**: Parents can review what AI endpoints are available

#### For Developers

- **Educational Context**: Clear documentation of learning objectives for each endpoint
- **Child Safety Compliance**: Visible safety measures and content filtering
- **Integration Examples**: Real-world usage patterns for educational technology

## 🔒 Production Security Considerations

### Health Endpoint Security

```csharp
// Health checks don't expose sensitive information
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
    .AddDbContextCheck<GameDbContext>("database")
    .AddCheck<AzureAIServiceHealthCheck>("azure-ai")
    .AddCheck<ChildSafetyServiceHealthCheck>("child-safety");
```

#### What Health Endpoints DON'T Expose:

- Database connection strings
- API keys or secrets
- User data or child information
- Detailed error messages
- Internal service configurations

#### What They DO Provide:

- Service availability status
- Basic dependency health (database, external services)
- Response time indicators
- Educational service readiness

### Swagger Security in Production

#### API Key Authentication (Future Enhancement)

```csharp
// Ready for API key implementation
c.AddSecurityDefinition("ApiKey", new()
{
    Type = SecuritySchemeType.ApiKey,
    In = ParameterLocation.Header,
    Name = "X-API-Key",
    Description = "API Key for accessing educational game endpoints (production only)"
});
```

#### Content Protection

- **No Sensitive Data**: Swagger schemas exclude sensitive child information
- **Educational Focus**: Documentation emphasizes learning objectives
- **Read-Only Production**: "Try it out" disabled in production
- **Limited Access**: Available at `/docs` instead of root URL

## 🎮 Educational Game Monitoring

### Child-Safe Monitoring Metrics

```csharp
public class EducationalGameHealthChecks
{
    // Verify AI content moderation is working
    public async Task<HealthCheckResult> CheckChildSafetyAsync()
    {
        var isAIFilterWorking = await _contentModerator.TestFilterAsync();
        var areFallbacksReady = await _fallbackService.ValidateAsync();

        return isAIFilterWorking && areFallbacksReady
            ? HealthCheckResult.Healthy("Child safety systems operational")
            : HealthCheckResult.Unhealthy("Child safety systems need attention");
    }

    // Verify educational content is available
    public async Task<HealthCheckResult> CheckEducationalContentAsync()
    {
        var territoryData = await _territoryService.GetHealthCheckAsync();
        var languageData = await _languageService.GetHealthCheckAsync();

        return territoryData && languageData
            ? HealthCheckResult.Healthy("Educational content systems operational")
            : HealthCheckResult.Unhealthy("Educational content needs refresh");
    }
}
```

## 🌐 Monitoring URLs

### Development

- **Health Check**: `https://localhost:7155/health`
- **Liveness**: `https://localhost:7155/alive`
- **Swagger UI**: `https://localhost:7155/` (root)
- **API Docs**: `https://localhost:7155/swagger/v1/swagger.json`

### Production

- **Health Check**: `https://worldleadersgame.co.uk/api/health`
- **Liveness**: `https://worldleadersgame.co.uk/api/alive`
- **Swagger UI**: `https://worldleadersgame.co.uk/api/docs`
- **API Docs**: `https://worldleadersgame.co.uk/api/swagger/v1/swagger.json`

## 📊 Integration Examples

### Azure Application Insights

```csharp
// Health check integration with Azure monitoring
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddHealthChecks()
    .AddApplicationInsightsPublisher();
```

### Custom Educational Metrics

```csharp
// Track educational game-specific health metrics
public class EducationalGameMetrics
{
    public static readonly Counter<int> ChildInteractions =
        Meter.CreateCounter<int>("educational_game.child_interactions");

    public static readonly Histogram<double> LearningSessionDuration =
        Meter.CreateHistogram<double>("educational_game.learning_session_duration");

    public static readonly Counter<int> SafetyInterventions =
        Meter.CreateCounter<int>("educational_game.safety_interventions");
}
```

## 🎯 Next Steps

### Immediate Implementation

- [x] Enable health endpoints in production
- [x] Configure Swagger for production with security considerations
- [x] Document educational monitoring approach

### Future Enhancements

- [ ] Implement custom educational health checks
- [ ] Add Azure Application Insights integration
- [ ] Create educational metrics dashboard
- [ ] Implement API key authentication for production
- [ ] Add detailed educational service monitoring

## 🔗 Related Documentation

- [Azure Deployment Guide](./azure-deployment-comprehensive-guide.md)
- [.NET Aspire Integration](./dotnet-aspire-integration.md)
- [AI Safety & Child Protection](../.github/copilot-instructions/ai-safety-and-child-protection.md)

---

**Educational Value**: These monitoring capabilities ensure the educational game maintains high reliability and transparency, supporting both child safety and educational effectiveness while meeting professional production standards.
