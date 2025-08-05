---
layout: page
title: "Azure Deployment Guide - World Leaders Game"
date: 2025-08-04
category: "deployment-guide"
tags: ["azure", "deployment", "educational-game", "production"]
author: "AI-Generated with Human Oversight"
educational_objective: "Deploy educational game to Azure with custom domain routing"
---

# Azure Deployment Guide - World Leaders Game

**🎯 Objective**: Deploy the educational World Leaders Game to Azure with professional custom domain routing  
**🌍 Architecture**: Web App + API + Documentation with path-based routing  
**👥 Target**: 12-year-old learners worldwide

---

## 🏗️ Architecture Overview

### Deployment Structure

```
your-domain.com/          → Blazor Web App (Main Game)
your-domain.com/api/      → ASP.NET Core API (Game Logic)
your-domain.com/docs/     → Static Web App (Documentation)
```

### Azure Resources

- **App Service Plan**: B1 Basic tier for cost efficiency
- **Web App**: Blazor Server application for interactive gameplay
- **API App**: REST API with SignalR for real-time updates
- **Static Web App**: Jekyll documentation site
- **Application Gateway**: Path-based routing and SSL termination
- **Application Insights**: Monitoring and analytics

---

## 🚀 Quick Deployment

### Prerequisites

- Azure CLI installed and configured
- .NET 8 SDK installed
- Custom domain name (optional but recommended)

### 1. Deploy Infrastructure

```bash
# Clone the repository
git clone https://github.com/victorsaly/WorldLeadersGame.git
cd WorldLeadersGame

# Run the deployment script
./deploy-azure.sh
```

### 2. Configure Custom Domain (Optional)

```bash
# Setup custom domain with path routing
./setup-custom-domain.sh your-domain.com
```

---

## 📋 Detailed Deployment Steps

### Step 1: Initial Azure Setup

1. **Login to Azure**

   ```bash
   az login
   ```

2. **Set Subscription** (if needed)

   ```bash
   az account list
   az account set --subscription "your-subscription-id"
   ```

3. **Deploy Base Infrastructure**

   ```bash
   ./deploy-azure.sh
   ```

   This script will:

   - ✅ Create resource group in UK South region
   - ✅ Deploy App Service Plan (B1 Basic)
   - ✅ Create Web App for Blazor game
   - ✅ Create API App for game backend
   - ✅ Setup Static Web App for documentation
   - ✅ Configure Application Insights monitoring
   - ✅ Build and deploy .NET applications

### Step 2: Custom Domain Configuration

#### Option A: Simple CNAME Setup

```bash
# DNS Configuration
your-domain.com        → CNAME → worldleaders-web-prod.azurewebsites.net
api.your-domain.com    → CNAME → worldleaders-api-prod.azurewebsites.net
docs.your-domain.com   → CNAME → [static-web-app-url]
```

#### Option B: Application Gateway (Recommended)

```bash
# Deploy Application Gateway for path routing
./setup-custom-domain.sh your-domain.com

# DNS Configuration
your-domain.com → A Record → [Application Gateway IP]
```

### Step 3: SSL Certificate Setup

For production deployments:

```bash
# Enable managed certificates
az webapp config ssl bind \
  --resource-group worldleaders-prod-rg \
  --name worldleaders-web-prod \
  --certificate-thumbprint [certificate-thumbprint] \
  --ssl-type SNI
```

---

## 🔧 Configuration Details

### Environment Variables

#### Web App Configuration

```bash
ASPNETCORE_ENVIRONMENT=Production
ApiSettings__BaseUrl=https://your-domain.com/api
APPLICATIONINSIGHTS_CONNECTION_STRING=[auto-configured]
```

#### API App Configuration

```bash
ASPNETCORE_ENVIRONMENT=Production
AllowedOrigins__Web=https://your-domain.com
APPLICATIONINSIGHTS_CONNECTION_STRING=[auto-configured]
```

### CORS Configuration

The API is configured to accept requests from:

- Main domain (web app)
- Development localhost (for testing)

### Database Configuration

Currently using in-memory database for simplicity. For production, consider:

- Azure SQL Database
- Azure Database for PostgreSQL
- Azure Cosmos DB

---

## 📊 Monitoring & Analytics

### Application Insights Integration

- **Performance Monitoring**: Track page load times and API response times
- **Error Tracking**: Monitor exceptions and failed requests
- **User Analytics**: Track educational engagement metrics
- **Custom Events**: Monitor educational objectives achievement

### Key Metrics to Monitor

- **Performance**: Page load < 2 seconds for child engagement
- **Educational Value**: Learning objective completion rates
- **Safety**: Content moderation effectiveness
- **Accessibility**: WCAG 2.1 AA compliance metrics

---

## 💰 Cost Optimization

### Current Resource Costs (UK South)

- **App Service Plan B1**: ~£16/month
- **Application Gateway**: ~£45/month
- **Static Web App**: Free tier
- **Application Insights**: Pay-as-you-go

### Cost Reduction Strategies

1. **Auto-scaling**: Scale down during low usage
2. **Reserved Instances**: 1-year commitment for discounts
3. **Dev/Test Pricing**: Available for non-production environments

---

## 🛡️ Security & Child Protection

### Implemented Security Measures

- **HTTPS Only**: All traffic encrypted
- **Content Security Policy**: Prevent XSS attacks
- **CORS Configuration**: Restrict cross-origin requests
- **Input Validation**: Sanitize all user inputs
- **Content Moderation**: AI content filtering for child safety

### Child Safety Features

- **Age-Appropriate Content**: All content validated for 12-year-olds
- **Privacy Protection**: COPPA and GDPR compliance
- **Monitoring**: Real-time content moderation
- **Fallback Responses**: Safe alternatives for AI failures

---

## 🔄 CI/CD Pipeline

### GitHub Actions Workflow

The included workflow (`/.github/workflows/azure-deploy.yml`) provides:

1. **Build & Test**: Automated testing and validation
2. **Educational Safety**: Child safety content checks
3. **Multi-Stage Deployment**: Web App, API, and Documentation
4. **Health Checks**: Post-deployment validation
5. **Rollback Support**: Automatic rollback on failure

### Triggering Deployment

```bash
# Automatic deployment on push to main
git push origin main

# Manual deployment
gh workflow run azure-deploy.yml
```

---

## 🧪 Testing the Deployment

### Health Check Endpoints

```bash
# Web App Health
curl https://your-domain.com

# API Health
curl https://your-domain.com/api/health

# Documentation
curl https://your-domain.com/docs
```

### Educational Feature Testing

1. **Game Mechanics**: Test dice rolling and career progression
2. **Territory Acquisition**: Verify GDP-based pricing
3. **AI Agents**: Test child-safe AI responses
4. **Language Learning**: Verify pronunciation features

---

## 🔧 Troubleshooting

### Common Issues

#### Deployment Failures

```bash
# Check deployment logs
az webapp log tail --resource-group worldleaders-prod-rg --name worldleaders-web-prod

# Check resource status
az resource list --resource-group worldleaders-prod-rg --output table
```

#### Custom Domain Issues

```bash
# Verify DNS propagation
nslookup your-domain.com

# Check certificate status
az webapp config ssl list --resource-group worldleaders-prod-rg
```

#### Performance Issues

```bash
# Check Application Insights
az monitor app-insights query \
  --resource-group worldleaders-prod-rg \
  --app worldleaders-prod-insights \
  --analytics-query "requests | where timestamp > ago(1h)"
```

---

## 📚 Additional Resources

### Azure Documentation

- [App Service Documentation](https://docs.microsoft.com/en-us/azure/app-service/)
- [Application Gateway Documentation](https://docs.microsoft.com/en-us/azure/application-gateway/)
- [Static Web Apps Documentation](https://docs.microsoft.com/en-us/azure/static-web-apps/)

### Educational Game Resources

- [Educational Game Development Guide](./educational-game-development-guide.md)
- [AI Safety Implementation](./ai-safety-implementation-guide.md)
- [Child Protection Framework](./child-protection-framework.md)

---

## 🎊 Success Metrics

### Deployment Success Indicators

- ✅ All services respond to health checks
- ✅ Custom domain routing works correctly
- ✅ SSL certificates are active
- ✅ Educational content loads properly
- ✅ AI agents respond appropriately
- ✅ Child safety measures are active

### Educational Effectiveness Metrics

- 📊 **Engagement**: 80%+ completion rate for learning objectives
- 🎯 **Learning Outcomes**: Measurable geography and economics knowledge gain
- 🛡️ **Safety Score**: 100% appropriate content delivery
- ♿ **Accessibility**: WCAG 2.1 AA compliance confirmed

---

**🌍 Ready to Educate the World!**

Your World Leaders Game is now deployed and ready to help 12-year-old learners explore geography, economics, and languages through engaging, AI-assisted gameplay!

**Support**: For issues or questions about this educational deployment, please check the troubleshooting section or create an issue in the GitHub repository.
