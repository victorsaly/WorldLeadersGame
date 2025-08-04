---
layout: page
title: "Azure AI Security Guide: Safe Credential Management"
date: 2025-08-04
category: "security-guide"
tags: ["azure", "security", "credentials", "api-keys"]
author: "AI-Generated with Human Oversight"
educational_objective: "Secure Azure AI credentials for educational project development"
---

# Azure AI Security Guide: Safe Credential Management

**Complete guide to securely configure Azure AI services while protecting sensitive API keys from public repositories**

This guide explains how to safely manage Azure AI credentials for the World Leaders Game, ensuring API keys remain secure while maintaining full development functionality.

---

## 🔒 Security Architecture

### Files That Are Safe to Commit

- ✅ `azure-ai-config.json` - Template with placeholder values
- ✅ `.env.production.template` - Template for environment variables
- ✅ `appsettings.json` - Contains placeholder values only
- ✅ All other source code files

### Files That Contain Secrets (Excluded from Git)

- ❌ `.env.production` - Real API keys (in .gitignore)
- ❌ `azure-ai-config.local.json` - Real configuration (in .gitignore)
- ❌ `appsettings.local.json` - Real app settings (in .gitignore)

---

## 🚀 Development Setup Instructions

### Step 1: Copy Template Files

```bash
# Copy environment template
cp .env.production.template .env.production

# Copy config template
cp azure-ai-config.json azure-ai-config.local.json

# Copy app settings to local
cp src/WorldLeaders/WorldLeaders.API/appsettings.json src/WorldLeaders/WorldLeaders.API/appsettings.local.json
```

### Step 2: Update with Real Credentials

Edit the copied files with your actual Azure credentials:

- **`.env.production`** - Add your Azure API keys
- **`azure-ai-config.local.json`** - Add your service endpoints
- **`appsettings.local.json`** - Add your complete configuration

### Step 3: Test Configuration

```bash
# Test all Azure AI services
./test-azure-ai.sh

# Start application with secure config
cd src/WorldLeaders/WorldLeaders.API
dotnet run --launch-profile https
```

---

## 🌐 Production Deployment

### Azure Key Vault Integration

For production environments, use Azure Key Vault:

```bash
# Store secrets in Key Vault
az keyvault secret set --vault-name worldleaders-kv-uk --name "openai-api-key" --value "your-key"
az keyvault secret set --vault-name worldleaders-kv-uk --name "content-moderator-key" --value "your-key"
az keyvault secret set --vault-name worldleaders-kv-uk --name "speech-services-key" --value "your-key"
```

### Environment Variables

For cloud hosting platforms, set these environment variables:

```bash
AZURE_OPENAI_ENDPOINT=https://your-service.openai.azure.com/
AZURE_OPENAI_API_KEY=your-openai-key
AZURE_OPENAI_DEPLOYMENT_NAME=gpt-4-educational
CONTENT_MODERATOR_ENDPOINT=https://your-region.api.cognitive.microsoft.com/
CONTENT_MODERATOR_API_KEY=your-content-moderator-key
SPEECH_SERVICES_API_KEY=your-speech-key
SPEECH_SERVICES_REGION=your-region
```

---

## 🧪 Testing Azure AI Services

### Local Development Testing

```bash
# Test all services with local credentials
./test-azure-ai.sh

# Start application with local configuration
cd src/WorldLeaders/WorldLeaders.API
dotnet run --launch-profile https
```

### API Endpoints for Verification

- **AI Personalities**: `GET https://localhost:7289/api/AI/personalities`
- **AI Interaction**: `POST https://localhost:7289/api/AI/interact`
- **Content Validation**: `POST https://localhost:7289/api/AI/validate`

### Sample Test Commands

```bash
# Test OpenAI connection
curl -X GET "https://localhost:7289/api/AI/personalities" \
  -H "Accept: application/json"

# Test AI agent interaction
curl -X POST "https://localhost:7289/api/AI/interact" \
  -H "Content-Type: application/json" \
  -d '{
    "agentType": 0,
    "playerInput": "What jobs help me learn about geography?",
    "gameContext": "career exploration",
    "playerId": "test-player-id"
  }'
```

---

## 🛡️ Security Best Practices

### ✅ Recommended Actions

- Keep real API keys in local files (git-ignored)
- Use `appsettings.local.json` for development secrets
- Test with local credentials before committing code
- Use Azure Key Vault for production deployments
- Set up budget alerts in Azure Portal
- Regularly rotate API keys
- Monitor usage logs for unauthorized access

### ❌ Security Violations

- Never commit real API keys to git repositories
- Never share credentials in chat, email, or documentation
- Never use production keys in development environments
- Never push `.env.production` or `*.local.json` files
- Never hardcode API keys in source code
- Never store credentials in browser local storage

---

## 💰 Cost Management

### Monitoring and Budgets

- **Usage Dashboard**: [Azure Portal Billing](https://portal.azure.com/billing)
- **Budget Alerts**: Set ~£50/month for development usage
- **Pay-as-you-go**: Only charged for actual API calls
- **Free Tier Benefits**: 5,000 content checks + 5 hours speech/month

### Cost Optimization Tips

```bash
# Set up budget alerts
az consumption budget create \
  --resource-group rg-worldleaders-personal \
  --budget-name "educational-ai-budget" \
  --amount 50 \
  --time-grain Monthly

# Monitor daily usage
az monitor metrics list \
  --resource /subscriptions/your-subscription/resourceGroups/rg-worldleaders-personal/providers/Microsoft.CognitiveServices/accounts/worldleaders-openai-uk \
  --metric TotalCalls
```

---

## 🚨 Security Incident Response

### If Credentials Are Compromised

1. **Immediate Actions**:

   - Regenerate all affected API keys in Azure Portal
   - Update local configuration files with new keys
   - Test application with new credentials
   - Review Azure usage logs for unauthorized access

2. **Preventive Measures**:
   - Enable Azure Security Center monitoring
   - Set up anomaly detection alerts
   - Implement IP address restrictions where possible
   - Review access logs regularly

### Key Rotation Schedule

```bash
# Monthly key rotation (recommended)
az cognitiveservices account keys regenerate \
  --name worldleaders-openai-uk \
  --resource-group rg-worldleaders-personal \
  --key-name key1
```

---

## 🔧 Configuration Management

### Secure Configuration Loading

The application uses a layered configuration approach:

1. **Development**: Loads from `appsettings.local.json` if available
2. **Environment Variables**: Falls back to environment variables
3. **Default Config**: Uses placeholder values if no secrets found

### Configuration Priority

```
1. appsettings.local.json (highest priority - development)
2. Environment Variables (production)
3. appsettings.json (lowest priority - templates only)
```

---

## 📞 Support and Resources

### Azure Support

- **Technical Issues**: [Azure Portal Support](https://portal.azure.com/#blade/Microsoft_Azure_Support/HelpAndSupportBlade)
- **Billing Questions**: [Azure Billing Support](https://portal.azure.com/billing)
- **Security Concerns**: [Azure Security Center](https://portal.azure.com/#blade/Microsoft_Azure_Security/SecurityMenuBlade)

### Application Support

- **Bug Reports**: Create GitHub issue in repository
- **Feature Requests**: Discussion in GitHub Discussions
- **Security Vulnerabilities**: Contact repository maintainer privately

### Educational Resources

- **Azure AI Documentation**: [Microsoft Learn](https://learn.microsoft.com/azure/ai-services/)
- **OpenAI Best Practices**: [Azure OpenAI Service](https://learn.microsoft.com/azure/ai-services/openai/)
- **Child Safety Guidelines**: [Responsible AI](https://www.microsoft.com/ai/responsible-ai)

---

## 📋 Security Checklist

### Before Committing Code

- [ ] No real API keys in committed files
- [ ] `.env.production` added to `.gitignore`
- [ ] Local configuration files excluded from git
- [ ] Template files contain only placeholder values
- [ ] Security guide updated with current practices

### Before Production Deployment

- [ ] Azure Key Vault configured for credential storage
- [ ] Environment variables set on hosting platform
- [ ] Budget alerts configured for cost control
- [ ] IP restrictions enabled where appropriate
- [ ] Monitoring and logging enabled

### Regular Maintenance

- [ ] API keys rotated monthly
- [ ] Usage logs reviewed weekly
- [ ] Security updates applied promptly
- [ ] Access permissions reviewed quarterly
- [ ] Backup and recovery procedures tested

---

**Security is paramount when developing educational applications for children. This guide ensures Azure AI integration remains both powerful and protected.**
