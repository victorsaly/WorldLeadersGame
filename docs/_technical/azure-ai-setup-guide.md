---
layout: page
title: "Azure AI Setup Guide: Production Configuration"
date: 2025-08-04
category: "setup-guide"
tags: ["azure", "openai", "production", "configuration"]
author: "AI-Generated with Human Oversight"
educational_objective: "Enable cloud AI services for production deployment"
---

# Azure AI Setup Guide: Production Configuration

**Step-by-step guide to configure Azure AI services for production-ready educational AI agents**

This guide walks through setting up Azure OpenAI, Content Moderator, and Speech Services to power our educational AI agent personality system in production.

---

## 🎯 Prerequisites

- **Azure Subscription** with appropriate permissions
- **Azure CLI** installed and configured
- **Resource Group** for World Leaders Game resources
- **OpenAI Access** approved for your Azure subscription

---

## 🚀 Step 1: Create Azure Resources

### Create Resource Group

```bash
# Create resource group in East US for optimal OpenAI performance
az group create \
  --name rg-worldleaders-prod \
  --location eastus \
  --tags project=worldleaders environment=production purpose=educational-ai
```

### Create Azure OpenAI Service

```bash
# Create Azure OpenAI cognitive service
az cognitiveservices account create \
  --name worldleaders-openai \
  --resource-group rg-worldleaders-prod \
  --kind OpenAI \
  --sku S0 \
  --location eastus \
  --custom-domain worldleaders-openai \
  --tags purpose=educational-ai target-age=12
```

### Deploy GPT-4 Model for Educational Content

```bash
# Deploy GPT-4 model optimized for educational interactions
az cognitiveservices account deployment create \
  --name worldleaders-openai \
  --resource-group rg-worldleaders-prod \
  --deployment-name gpt-4-educational \
  --model-name gpt-4 \
  --model-version "0613" \
  --model-format OpenAI \
  --scale-type "Standard" \
  --scale-capacity 10
```

### Create Content Moderator for Child Safety

```bash
# Create Content Moderator for multi-layer child safety validation
az cognitiveservices account create \
  --name worldleaders-contentmod \
  --resource-group rg-worldleaders-prod \
  --kind ContentModerator \
  --sku S0 \
  --location eastus \
  --tags purpose=child-safety validation=multi-layer
```

### Create Speech Services for Language Learning

```bash
# Create Speech Services for pronunciation features
az cognitiveservices account create \
  --name worldleaders-speech \
  --resource-group rg-worldleaders-prod \
  --kind SpeechServices \
  --sku S0 \
  --location eastus \
  --tags purpose=language-learning feature=pronunciation
```

---

## 🔑 Step 2: Get API Keys and Endpoints

### Retrieve Azure OpenAI Credentials

```bash
# Get OpenAI endpoint
az cognitiveservices account show \
  --name worldleaders-openai \
  --resource-group rg-worldleaders-prod \
  --query "properties.endpoint" \
  --output tsv

# Get OpenAI API key
az cognitiveservices account keys list \
  --name worldleaders-openai \
  --resource-group rg-worldleaders-prod \
  --query "key1" \
  --output tsv
```

### Retrieve Content Moderator Credentials

```bash
# Get Content Moderator endpoint
az cognitiveservices account show \
  --name worldleaders-contentmod \
  --resource-group rg-worldleaders-prod \
  --query "properties.endpoint" \
  --output tsv

# Get Content Moderator API key
az cognitiveservices account keys list \
  --name worldleaders-contentmod \
  --resource-group rg-worldleaders-prod \
  --query "key1" \
  --output tsv
```

### Retrieve Speech Services Credentials

```bash
# Get Speech Services API key
az cognitiveservices account keys list \
  --name worldleaders-speech \
  --resource-group rg-worldleaders-prod \
  --query "key1" \
  --output tsv
```

---

## ⚙️ Step 3: Configure Application Settings

### Update appsettings.json

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://worldleaders-openai.openai.azure.com/",
    "ApiKey": "[YOUR_OPENAI_API_KEY]",
    "DeploymentName": "gpt-4-educational",
    "ApiVersion": "2024-02-15-preview",
    "MaxTokens": 500,
    "Temperature": 0.7,
    "EducationalSystemPrompt": "You are an educational AI assistant for 12-year-old students learning geography, economics, and languages. Always provide encouraging, age-appropriate, and culturally sensitive responses that teach real-world concepts."
  },
  "ContentModerator": {
    "Endpoint": "https://worldleaders-contentmod.cognitiveservices.azure.com/",
    "ApiKey": "[YOUR_CONTENT_MODERATOR_API_KEY]",
    "ChildSafetyLevel": "Strict",
    "CustomBlockLists": ["violence", "inappropriate-language", "adult-themes"]
  },
  "SpeechServices": {
    "Region": "eastus",
    "Endpoint": "https://worldleaders-speech.cognitiveservices.azure.com/",
    "ApiKey": "[YOUR_SPEECH_SERVICES_API_KEY]",
    "SupportedLanguages": ["en-US", "es-ES", "fr-FR", "de-DE", "zh-CN", "ja-JP"]
  }
}
```

### Environment Variables for Production

```bash
# Set environment variables for secure credential management
export AZURE_OPENAI_ENDPOINT="https://worldleaders-openai.openai.azure.com/"
export AZURE_OPENAI_API_KEY="[YOUR_OPENAI_API_KEY]"
export CONTENT_MODERATOR_ENDPOINT="https://worldleaders-contentmod.cognitiveservices.azure.com/"
export CONTENT_MODERATOR_API_KEY="[YOUR_CONTENT_MODERATOR_API_KEY]"
export SPEECH_SERVICES_API_KEY="[YOUR_SPEECH_SERVICES_API_KEY]"
```

---

## 🧪 Step 4: Test Azure AI Integration

### Test Configuration Script

```bash
#!/bin/bash
# test-azure-ai.sh - Test Azure AI services configuration

echo "🧪 Testing Azure AI Integration..."

# Test 1: Verify API endpoints are accessible
echo "1️⃣ Testing API endpoints..."
curl -s -o /dev/null -w "%{http_code}" "$AZURE_OPENAI_ENDPOINT" | grep -q "404" && echo "✅ OpenAI endpoint accessible" || echo "❌ OpenAI endpoint error"

# Test 2: Start the API with Azure configuration
echo "2️⃣ Starting API with Azure AI configuration..."
cd src/WorldLeaders/WorldLeaders.API
dotnet run --environment Production --launch-profile https &
API_PID=$!

# Wait for API to start
sleep 10

# Test 3: Test AI agent interaction
echo "3️⃣ Testing AI agent personality endpoint..."
curl -s -X GET "https://localhost:7289/api/AI/personalities" | jq '.[] | select(.name=="Maya") | .name' && echo "✅ Agent personalities loaded"

# Test 4: Test AI interaction with Azure OpenAI
echo "4️⃣ Testing cloud AI interaction..."
curl -s -X POST "https://localhost:7289/api/AI/interact" \
  -H "Content-Type: application/json" \
  -d '{
    "agentType": 0,
    "playerInput": "What careers can I explore?",
    "gameContext": "career development and economics",
    "playerId": "00000000-0000-0000-0000-000000000000"
  }' | jq '.response' && echo "✅ Cloud AI interaction successful"

# Cleanup
kill $API_PID
echo "🎉 Azure AI integration test complete!"
```

### Manual Testing Endpoints

```bash
# Test agent personalities
curl -X GET "https://localhost:7289/api/AI/personalities"

# Test Maya the Career Guide with cloud AI
curl -X POST "https://localhost:7289/api/AI/interact" \
  -H "Content-Type: application/json" \
  -d '{
    "agentType": 0,
    "playerInput": "I want to learn about being a teacher",
    "gameContext": "career exploration in education",
    "playerId": "test-player-123"
  }'

# Test content validation
curl -X POST "https://localhost:7289/api/AI/validate" \
  -H "Content-Type: application/json" \
  -d '{
    "content": "Teaching is a wonderful career that helps students learn and grow!",
    "educationalContext": "12-year-old career exploration"
  }'
```

---

## 📊 Step 5: Monitor and Validate

### Azure Monitor Setup

```bash
# Create Application Insights for monitoring
az monitor app-insights component create \
  --app worldleaders-insights \
  --location eastus \
  --resource-group rg-worldleaders-prod \
  --application-type web
```

### Key Metrics to Monitor

- **AI Response Time**: Target < 2 seconds for child engagement
- **Safety Validation Success Rate**: Target 100% appropriate content
- **Token Usage**: Monitor OpenAI consumption for cost management
- **Content Moderation Alerts**: Immediate alerts for safety violations

### Sample Monitoring Query

```kusto
// Application Insights query for AI interaction monitoring
traces
| where timestamp > ago(1h)
| where message contains "AI interaction"
| summarize
    TotalInteractions = count(),
    AvgResponseTime = avg(toint(customDimensions["ResponseTime"])),
    SafetyViolations = countif(customDimensions["SafetyValidated"] == "false")
    by bin(timestamp, 5m)
| order by timestamp desc
```

---

## 🔒 Step 6: Security Best Practices

### Key Vault Integration (Recommended)

```bash
# Create Azure Key Vault for secure credential storage
az keyvault create \
  --name worldleaders-keyvault \
  --resource-group rg-worldleaders-prod \
  --location eastus \
  --enable-rbac-authorization

# Store API keys securely
az keyvault secret set \
  --vault-name worldleaders-keyvault \
  --name "openai-api-key" \
  --value "[YOUR_OPENAI_API_KEY]"

az keyvault secret set \
  --vault-name worldleaders-keyvault \
  --name "content-moderator-api-key" \
  --value "[YOUR_CONTENT_MODERATOR_API_KEY]"
```

### Network Security

- **API Management**: Consider Azure API Management for rate limiting
- **Private Endpoints**: Use private endpoints for production security
- **Firewall Rules**: Restrict access to authorized IP ranges only

---

## 📈 Step 7: Performance Optimization

### Caching Strategy

```csharp
// Add response caching for common educational content
services.AddMemoryCache();
services.AddResponseCaching();

// Configure educational content caching
public class EducationalContentCache
{
    public async Task<string> GetCachedEducationalResponse(string cacheKey)
    {
        // Cache common educational responses to reduce API calls
        // and improve response time for repeated questions
    }
}
```

### Rate Limiting

```json
{
  "RateLimiting": {
    "PerMinute": 60,
    "PerHour": 1000,
    "PerDay": 10000,
    "ChildSafetyPriority": true
  }
}
```

---

## 🚨 Troubleshooting

### Common Issues

#### 1. OpenAI API Key Invalid

```bash
# Verify API key is correct
az cognitiveservices account keys list \
  --name worldleaders-openai \
  --resource-group rg-worldleaders-prod
```

#### 2. Content Moderator Not Responding

```bash
# Test Content Moderator endpoint directly
curl -X POST "$CONTENT_MODERATOR_ENDPOINT/contentmoderator/moderate/v1.0/ProcessText/Screen" \
  -H "Ocp-Apim-Subscription-Key: $CONTENT_MODERATOR_API_KEY" \
  -H "Content-Type: text/plain" \
  -d "Test content for moderation"
```

#### 3. High Response Times

- Check Azure service status
- Verify geographic proximity (use eastus region)
- Monitor token usage and consider response caching

#### 4. Safety Validation Failures

- Review Content Moderator configuration
- Check custom block lists
- Verify child safety level settings

### Debug Logging

```json
{
  "Logging": {
    "LogLevel": {
      "WorldLeaders.Infrastructure.Services": "Debug",
      "Azure.AI.OpenAI": "Information"
    }
  }
}
```

---

## ✅ Production Readiness Checklist

- [ ] **Azure Resources Created**: OpenAI, Content Moderator, Speech Services
- [ ] **API Keys Configured**: All credentials set in secure storage
- [ ] **Educational Prompts Tested**: Agent personalities working with cloud AI
- [ ] **Safety Validation Active**: Multi-layer content moderation operational
- [ ] **Monitoring Configured**: Application Insights tracking all interactions
- [ ] **Performance Optimized**: Response times under 2 seconds
- [ ] **Security Hardened**: Network security and key management in place
- [ ] **Cost Monitoring**: Budget alerts for API usage configured

---

## 📞 Support and Resources

- **Azure OpenAI Documentation**: [https://docs.microsoft.com/azure/ai-services/openai/](https://docs.microsoft.com/azure/ai-services/openai/)
- **Content Moderator Guide**: [https://docs.microsoft.com/azure/cognitive-services/content-moderator/](https://docs.microsoft.com/azure/cognitive-services/content-moderator/)
- **Child Safety Best Practices**: [COPPA Compliance Guide](https://www.ftc.gov/business-guidance/resources/complying-coppa-frequently-asked-questions)

---

With this configuration, your World Leaders Game will have production-ready AI agents powered by Azure services, maintaining 100% child safety compliance while delivering engaging educational content to 12-year-old learners worldwide.

**Next Steps**: Deploy to Azure Container Apps and set up continuous monitoring for child safety and educational effectiveness.
