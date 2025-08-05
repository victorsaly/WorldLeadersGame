---
layout: page
title: "Complete Azure Deployment Guide"
date: 2025-08-05
category: "technical-guide"
tags: ["azure", "deployment", "production", "educational-platform"]
author: "AI-Generated with Human Oversight"
---

# 🚀 Complete Azure Deployment Guide - World Leaders Game

## 🎯 Educational Objective

Deploy a production-ready educational platform that teaches 12-year-olds about geography, economics, and languages through safe, engaging gameplay.

## 🌍 Real-World Connection

This deployment guide demonstrates how modern cloud platforms can deliver educational content globally while maintaining child safety and performance standards.

## 🔧 Technical Implementation

### Prerequisites

- Azure CLI installed and configured
- .NET 8 SDK installed
- CloudFlare account with domain management
- GitHub account for documentation hosting

### Step 1: Infrastructure Deployment

```bash
# Deploy Azure infrastructure using Bicep
az deployment group create \
  --resource-group worldleaders-prod-rg \
  --template-file infrastructure/main.bicep \
  --parameters environment=production \
  --parameters location=uksouth
```

### Step 2: Application Deployment

```bash
# Build and deploy Blazor Server application
dotnet publish src/WorldLeaders/WorldLeaders.Web \
  --configuration Release \
  --output ./publish/web

# Deploy to Azure App Service
az webapp deploy \
  --resource-group worldleaders-prod-rg \
  --name worldleaders-web-prod \
  --src-path ./publish/web

# Build and deploy API application
dotnet publish src/WorldLeaders/WorldLeaders.API \
  --configuration Release \
  --output ./publish/api

az webapp deploy \
  --resource-group worldleaders-prod-rg \
  --name worldleaders-api-prod \
  --src-path ./publish/api
```

### Step 3: DNS Configuration

```bash
# Run automated CloudFlare DNS setup
./scripts/cloudflare-auto-setup.sh

# Verify DNS propagation
./scripts/check-cloudflare-dns.sh
```

### Step 4: Custom Domain Setup

```bash
# Add TXT records for domain verification (manual CloudFlare step)
# Then run automated domain configuration
./scripts/azure-domain-setup.sh

# Test deployment
./scripts/test-azure-setup.sh
```

## 🛡️ Child Safety Measures

Every deployment step includes child protection validations:

### Content Security Policy

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy",
        "default-src 'self'; img-src 'self' data: https:; " +
        "script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'");
    await next();
});
```

### Safe Error Handling

```csharp
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        // Never expose system details to children
        var response = "Something went wrong, but we're fixing it!";
        await context.Response.WriteAsync(response);
    });
});
```

## 📊 Educational Effectiveness

### Performance Targets for Child Engagement

- **Page Load Time**: < 2 seconds (critical for 12-year-old attention)
- **API Response Time**: < 500ms for game interactions
- **AI Agent Response**: < 3 seconds for educational conversations

### Monitoring Educational Success

```csharp
public class EducationalMetricsCollector
{
    public void TrackLearningInteraction(string playerId, string concept, bool success)
    {
        // Track educational effectiveness without storing personal data
        _telemetryClient.TrackEvent("LearningInteraction", new Dictionary<string, string>
        {
            ["Concept"] = concept,
            ["Success"] = success.ToString(),
            ["Age"] = "12", // Age group only
            ["Region"] = GetGeneralRegion() // Country level only
        });
    }
}
```

## 🔄 Deployment Automation Scripts

### Available Scripts in `/scripts/` Directory

- **`azure-setup.sh`** - Initial Azure infrastructure setup
- **`deploy-azure.sh`** - Complete application deployment
- **`azure-domain-setup.sh`** - Custom domain and SSL configuration
- **`cloudflare-auto-setup.sh`** - Automated CloudFlare DNS setup
- **`test-azure-setup.sh`** - Comprehensive deployment testing
- **`check-cloudflare-dns.sh`** - DNS propagation verification

### Production Deployment Workflow

```bash
# 1. Deploy infrastructure
./scripts/azure-setup.sh

# 2. Deploy applications
./scripts/deploy-azure.sh

# 3. Configure DNS (requires manual CloudFlare TXT records)
./scripts/cloudflare-auto-setup.sh

# 4. Set up custom domains
./scripts/azure-domain-setup.sh

# 5. Validate deployment
./scripts/test-azure-setup.sh
```

## 🌐 Live Platform Architecture

### Production URLs

- **Main Game**: https://worldleadersgame.co.uk
- **Game API**: https://api.worldleadersgame.co.uk
- **Documentation**: https://docs.worldleadersgame.co.uk
- **Health Check**: https://api.worldleadersgame.co.uk/health

### Architecture Components

```
CloudFlare CDN (Global)
├── worldleadersgame.co.uk → Azure App Service (Blazor Server)
├── api.worldleadersgame.co.uk → Azure App Service (ASP.NET Core API)
└── docs.worldleadersgame.co.uk → GitHub Pages (Jekyll)

Azure Resources (UK South)
├── worldleaders-prod-rg (Resource Group)
├── worldleaders-prod-plan (App Service Plan - B1)
├── worldleaders-web-prod (Blazor Web App)
└── worldleaders-api-prod (Game API + SignalR)
```

## 🧪 Testing and Validation

### Automated Testing Suite

```bash
# Test all endpoints
curl -f https://worldleadersgame.co.uk
curl -f https://api.worldleadersgame.co.uk/health
curl -f https://docs.worldleadersgame.co.uk

# Validate SSL certificates
echo | openssl s_client -servername worldleadersgame.co.uk \
  -connect worldleadersgame.co.uk:443 2>/dev/null | \
  openssl x509 -noout -dates

# Check educational content safety
./scripts/validate-content-safety.sh
```

### Performance Validation

```csharp
[Test]
public async Task EducationalPlatform_LoadsWithinChildAttentionSpan()
{
    var stopwatch = Stopwatch.StartNew();
    var response = await _httpClient.GetAsync("https://worldleadersgame.co.uk");
    stopwatch.Stop();

    // Critical: Must load within 2 seconds for 12-year-old engagement
    Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(2000));
    Assert.That(response.IsSuccessStatusCode, Is.True);
}
```

## 📚 Cross-Module Relationships

### Educational Value

This deployment guide teaches:

- **Technology Operations** - How modern web applications reach global audiences
- **Geographic Concepts** - Azure regions and global CDN distribution
- **Problem Solving** - Step-by-step troubleshooting and validation
- **Safety Principles** - How technology protects children online

### Technical Architecture Integration

- Uses patterns from [Technical Architecture Guide](./technical-architecture)
- Implements safety measures from [AI Safety Guide](./ai-safety-and-child-protection)
- Follows documentation standards from [Documentation Standards](./documentation-standards)

---

**Remember**: Every deployment step must prioritize child safety and educational effectiveness. This platform teaches 12-year-olds about the world through safe, engaging technology.
