---
layout: page
title: "Production Deployment Configuration Guide"
date: 2025-08-06
category: "technical-guide"
tags: ["deployment", "production", "api-configuration", "security"]
author: "AI-Generated with Human Oversight"
educational_objective: "Configure secure production deployment for educational game platform"
---

# 🚀 Production Deployment Configuration Guide

**Context**: Educational game deployment for 12-year-old learners requiring secure API communication and child safety compliance.

## API URL Configuration for Production

The application is configured to automatically connect to your production API at `https://api.worldleadersgame.co.uk` when deployed.

### Configuration Priority (Highest to Lowest):

1. **Environment Variable**: `API_BASE_URL`
2. **appsettings.Production.json**: `ApiSettings:BaseUrl`
3. **Fallback for Production**: `https://api.worldleadersgame.co.uk`
4. **Development Default**: `https://localhost:7155`

## Deployment Options

### Option 1: Using appsettings.Production.json (Current Setup)
✅ **Already configured** - The production config file now points to:
```json
"ApiSettings": {
  "BaseUrl": "https://api.worldleadersgame.co.uk"
}
```

### Option 2: Using Environment Variables (Recommended for CI/CD)
Set these environment variables in your deployment pipeline:

```bash
# Required
API_BASE_URL=https://api.worldleadersgame.co.uk
ASPNETCORE_ENVIRONMENT=Production

# Optional - for API authentication
API_KEY=your-api-key-if-needed

# Database (if using PostgreSQL)
CONNECTION_STRING=your-production-database-connection-string
```

### Option 3: Azure App Service Configuration
In Azure App Service, add these Application Settings:

| Name | Value |
|------|-------|
| `ApiSettings__BaseUrl` | `https://api.worldleadersgame.co.uk` |
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `API_KEY` | `your-api-key-if-needed` |

## Educational Platform Security Requirements

### Child Safety Compliance
- **Always Enabled**: Child safety mode cannot be disabled in production
- **Content Filtering**: All AI content validated before reaching children
- **Privacy Protection**: COPPA and GDPR compliant data handling
- **Safe Fallbacks**: Educational content available when API calls fail

### Production Security Features
```csharp
// Security headers automatically applied in production
context.Response.Headers["X-Frame-Options"] = "DENY";
context.Response.Headers["X-Content-Type-Options"] = "nosniff";
context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; ...";
```

## Verification Steps

### 1. Check Logs on Startup
The application will log the API URL on startup:
```
🌍 World Leaders Game Web Application Starting
📡 API Base URL: https://api.worldleadersgame.co.uk
🔧 Environment: Production
🛡️ Child Safety Mode: True
```

### 2. Test API Connectivity
Once deployed, check the health endpoint:
```bash
curl https://your-web-app-url/health
```

### 3. Verify Educational Content Loading
The application should now load territories from your production API:
- Territory data with real GDP information
- Language learning challenges
- Cultural context for educational exploration

## Educational Game API Endpoints

### Required API Routes for Educational Features:
```
GET /api/Territory/available/{playerId}    # Available territories for exploration
GET /api/Territory/owned/{playerId}        # Player's conquered territories  
GET /api/Territory/details/{territoryId}   # Educational territory information
GET /api/Territory/cultural-context/{id}   # Cultural learning content
GET /api/Territory/language-challenges/{playerId} # Language learning opportunities
POST /api/Territory/acquire/{playerId}/{territoryId} # Territory acquisition
```

### Child Safety API Requirements:
```
GET /health                                # System health for monitoring
POST /api/content/validate                 # AI content safety validation
GET /api/fallback/{agentType}             # Safe fallback responses
```

## Environment-Specific Configuration

### Development (localhost):
```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7155",
    "TimeoutSeconds": 30,
    "EnableDetailedLogging": true
  },
  "GameSettings": {
    "EnableChildSafetyMode": true,
    "SessionTimeoutMinutes": 30
  }
}
```

### Production (worldleadersgame.co.uk):
```json
{
  "ApiSettings": {
    "BaseUrl": "https://api.worldleadersgame.co.uk",
    "TimeoutSeconds": 30,
    "EnableDetailedLogging": false
  },
  "GameSettings": {
    "EnableChildSafetyMode": true,
    "SessionTimeoutMinutes": 45,
    "MaxPlayersPerSession": 1000
  },
  "Security": {
    "EnforceHttps": true,
    "RequireSecureCookies": true,
    "HstsMaxAge": "31536000"
  }
}
```

## Troubleshooting Educational Game Deployment

### If territories are empty (educational content not loading):
1. **Check API URL**: Verify startup logs show correct production API URL
2. **Test API Health**: Ensure `https://api.worldleadersgame.co.uk/health` responds
3. **CORS Configuration**: API must allow requests from your web domain
4. **SSL/TLS Validation**: Verify API certificate is valid for educational compliance
5. **Child Safety**: Ensure content moderation endpoints are responding

### Common Educational Platform Issues:
- **CORS**: Essential for cross-domain educational content delivery
- **SSL/TLS**: Required for child data protection compliance
- **API Authentication**: May be required for educational content licensing
- **Content Moderation**: Child safety filters must be operational

## Implementation Details

### HTTP Client Configuration
```csharp
// Production-ready configuration with child safety considerations
builder.Services.AddHttpClient("GameAPI", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "WorldLeaders-Web/1.0");
    
    // Educational platform identification
    var apiKey = Environment.GetEnvironmentVariable("API_KEY");
    if (!string.IsNullOrEmpty(apiKey))
    {
        client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
    }
});
```

### Security Middleware Pipeline
```csharp
// Child-safe security headers for educational compliance
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    
    if (app.Environment.IsProduction())
    {
        // Educational platform CSP for child safety
        var csp = "default-src 'self'; script-src 'self' 'unsafe-inline'; " +
                 "style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; " +
                 "connect-src 'self' wss: https:;";
        context.Response.Headers["Content-Security-Policy"] = csp;
    }
    
    await next();
});
```

## Configuration Files Updated

### Files Modified for Production Deployment:
- ✅ `appsettings.json` - Development defaults with educational game settings
- ✅ `appsettings.Production.json` - Production configuration with security hardening
- ✅ `Program.cs` - Environment-aware configuration with child safety priorities
- ✅ `TerritoryClientService.cs` - HTTP-based API communication with safe fallbacks
- ✅ `SpeechRecognitionClientService.cs` - Educational speech services with encouraging feedback

### Educational Value Preservation:
- Real-world GDP data for economic learning
- Cultural context for geography education  
- Language pronunciation for linguistic development
- Safe AI agent personalities for mentorship
- Positive reinforcement for 12-year-old engagement

## Deployment Readiness Checklist

### Pre-Deployment:
- [ ] Production API URL configured: `https://api.worldleadersgame.co.uk`
- [ ] Child safety mode enabled and tested
- [ ] Security headers validated
- [ ] Educational content endpoints verified
- [ ] SSL/TLS certificates confirmed
- [ ] CORS policy configured for web domain

### Post-Deployment:
- [ ] Health endpoints responding
- [ ] Territory loading functional
- [ ] AI agent interactions safe and appropriate
- [ ] Language learning features operational
- [ ] Performance monitoring active
- [ ] Educational effectiveness tracking enabled

**Educational Mission**: Ensure 12-year-old learners can safely explore world geography, economics, and languages through engaging, AI-assisted gameplay with production-grade reliability and security.

---

**Related Documentation**:
- [Technical Architecture](technical-architecture)
- [AI Safety Implementation](ai-safety-implementation)  
- [Child Protection Framework](child-protection-framework)
- [Educational Game Mechanics](educational-game-mechanics)
