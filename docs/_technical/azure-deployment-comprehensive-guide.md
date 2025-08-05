---
layout: page
title: "Azure Deployment Comprehensive Guide"
date: 2025-08-05
category: "technical-guide"
tags: ["azure", "deployment", "infrastructure", "educational-platform"]
author: "AI-Generated with Human Oversight"
---

# Azure Deployment Files - World Leaders Game

This directory contains all the necessary files to deploy the World Leaders Game to Azure with custom domain routing.

## 🎯 Educational Context

**Target Audience**: 12-year-old learners worldwide  
**Learning Objectives**: Geography, Economics, Language Learning  
**Safety Priority**: Child-safe AI content and interactions

## 📁 Deployment Files

### Core Deployment

- **`deploy-azure.sh`** - Main deployment script for Azure infrastructure and applications
- **`azure-deploy.bicep`** - Azure Resource Manager template for infrastructure
- **`azure-appgateway.bicep`** - Application Gateway configuration for path-based routing
- **`setup-custom-domain.sh`** - Custom domain configuration with multiple routing options

### CI/CD Pipeline

- **`.github/workflows/azure-deploy.yml`** - GitHub Actions workflow for automated deployment

### Documentation

- **`docs/_technical/azure-deployment-guide.md`** - Comprehensive deployment guide

## 🚀 Quick Start

### 1. Prerequisites

```bash
# Install Azure CLI
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# Install .NET 8 SDK
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0

# Login to Azure
az login
```

### 2. Deploy Infrastructure & Applications

```bash
# Make scripts executable
chmod +x deploy-azure.sh setup-custom-domain.sh

# Deploy to Azure
./deploy-azure.sh
```

### 3. Configure Custom Domain (Optional)

```bash
# Setup custom domain with path routing
./setup-custom-domain.sh your-domain.com
```

## 🏗️ Architecture

### Routing Structure

```
your-domain.com/          → Blazor Web App (Main Game)
your-domain.com/api/      → ASP.NET Core API (Game Backend)
your-domain.com/docs/     → Static Web App (Documentation)
```

### Azure Resources Created

- **Resource Group**: `worldleaders-prod-rg`
- **App Service Plan**: B1 Basic tier (cost-efficient)
- **Web App**: `worldleaders-web-prod` (Blazor Server)
- **API App**: `worldleaders-api-prod` (ASP.NET Core API)
- **Static Web App**: `worldleaders-docs-prod` (Jekyll docs)
- **Application Gateway**: Path-based routing (optional)
- **Application Insights**: Monitoring and analytics
- **Storage Account**: Game assets and backups

## 🔧 Configuration Options

### Simple CNAME Setup

- **Main Game**: `your-domain.com` → CNAME → `worldleaders-web-prod.azurewebsites.net`
- **API**: `api.your-domain.com` → CNAME → `worldleaders-api-prod.azurewebsites.net`
- **Docs**: `docs.your-domain.com` → CNAME → Static Web App URL

### Application Gateway (Recommended)

- **All Traffic**: `your-domain.com` → A Record → Application Gateway IP
- **Path Routing**: Gateway handles `/api/*` and `/docs/*` routing internally

## 📊 Monitoring

### Application Insights Metrics

- **Performance**: Page load times, API response times
- **Educational Metrics**: Learning objective completion rates
- **Safety Metrics**: Content moderation effectiveness
- **User Analytics**: Engagement and retention metrics

### Health Check Endpoints

- **Web App**: `https://your-domain.com/`
- **API**: `https://your-domain.com/api/health`
- **Documentation**: `https://your-domain.com/docs/`

## 💰 Cost Estimates (UK South Region)

### Monthly Costs

- **App Service Plan B1**: ~£16/month
- **Application Gateway**: ~£45/month (if used)
- **Static Web App**: Free tier
- **Application Insights**: Pay-as-you-go (~£5-15/month)
- **Storage Account**: ~£2/month

**Total**: £20-80/month depending on configuration

### Cost Optimization

- Use Basic tier for development/testing
- Scale down during low usage periods
- Consider Reserved Instances for production

## 🛡️ Security & Child Protection

### Implemented Security

- **HTTPS Only**: All traffic encrypted
- **Content Security Policy**: XSS protection
- **CORS Configuration**: Restricted cross-origin requests
- **Input Validation**: All user inputs sanitized
- **Child Safety**: AI content filtering and moderation

### Compliance

- **COPPA**: Children's privacy protection
- **GDPR**: European data protection
- **WCAG 2.1 AA**: Accessibility standards

## 🔄 Automated Deployment

### GitHub Actions

The workflow automatically:

1. Builds and tests the .NET applications
2. Validates educational content safety
3. Deploys to Azure App Services
4. Deploys documentation to Static Web Apps
5. Runs health checks
6. Provides deployment summary

### Triggering Deployment

```bash
# Automatic on push to main branch
git push origin main

# Manual trigger
gh workflow run azure-deploy.yml
```

## 🧪 Testing

### Local Testing

```bash
# Start local development
cd src/WorldLeaders
dotnet run --project WorldLeaders.AppHost
```

### Production Testing

```bash
# Health checks
curl https://your-domain.com/
curl https://your-domain.com/api/health
curl https://your-domain.com/docs/

# Educational features
curl https://your-domain.com/api/game/territories
curl https://your-domain.com/api/game/player/status
```

## 🔧 Troubleshooting

### Common Issues

#### Deployment Failures

```bash
# Check logs
az webapp log tail --resource-group worldleaders-prod-rg --name worldleaders-web-prod

# Resource status
az resource list --resource-group worldleaders-prod-rg --output table
```

#### DNS Issues

```bash
# Check DNS propagation
nslookup your-domain.com
dig your-domain.com

# Verify SSL certificates
az webapp config ssl list --resource-group worldleaders-prod-rg
```

#### Performance Issues

```bash
# Application Insights queries
az monitor app-insights query \
  --resource-group worldleaders-prod-rg \
  --app worldleaders-prod-insights \
  --analytics-query "requests | where timestamp > ago(1h) | summarize count() by bin(timestamp, 5m)"
```

## 📚 Educational Value

### Learning Objectives Supported

- **Geography**: Country recognition, location awareness
- **Economics**: GDP concepts, resource management
- **Language**: Pronunciation practice, cultural awareness
- **Critical Thinking**: Strategic planning, decision-making

### Child-Safe Features

- **Content Moderation**: AI responses filtered for appropriateness
- **Positive Messaging**: Encouraging, supportive interactions
- **Educational Focus**: All content serves learning objectives
- **Privacy Protection**: No personal information collection

## 🎊 Success Criteria

### Technical Success

- ✅ All services respond to health checks
- ✅ Custom domain routing functional
- ✅ SSL certificates active
- ✅ Monitoring and alerts configured

### Educational Success

- 🎯 Learning objectives measurable and achieved
- 🛡️ 100% appropriate content delivery
- ♿ WCAG 2.1 AA accessibility compliance
- 📊 Positive engagement metrics

---

**🌍 Ready to Educate the World!**

This deployment system creates a robust, scalable, and child-safe educational platform that helps 12-year-old learners explore geography, economics, and languages through engaging AI-assisted gameplay.

**Support**: For deployment questions or issues, see the troubleshooting section or create an issue in the repository.
