---
layout: page
title: "UK Azure Deployment Guide - Personal Project"
date: 2025-08-04
category: "deployment-guide"
tags: ["azure", "uk", "personal-project", "deployment"]
author: "AI-Generated with Human Oversight"
educational_objective: "Secure UK-based Azure deployment for personal educational project"
---

# UK Azure Deployment Guide - Personal Project

**Target**: Deploy World Leaders Game AI agents to UK Azure regions for personal educational project  
**Region**: UK South (uksouth) for data residency and performance  
**Account**: Personal Azure subscription (not company)

---

## 🇬🇧 UK-Specific Configuration

### **Why UK Region?**

- **Data Residency**: Keep educational data within UK jurisdiction
- **Performance**: Lower latency for UK users
- **Compliance**: Easier GDPR compliance with UK data residency
- **Cost**: Competitive pricing for UK-based services

### **Regional Setup**

- **Primary Region**: UK South (uksouth) - London area
- **Alternative**: UK West (ukwest) - Cardiff area
- **Resource Naming**: UK-specific naming convention with `-uk` suffix

---

## 🔧 Pre-Deployment Checklist

### **Step 1: Account Verification**

```bash
# Run the pre-flight checklist to verify setup
./azure-preflight.sh
```

This script will:

- ✅ Verify Azure CLI installation
- ✅ Check authentication status
- ✅ Help select personal vs company account
- ✅ Confirm UK region availability
- ✅ Estimate costs for personal project
- ✅ Verify OpenAI access requirements

### **Step 2: Tenant Selection**

The script will help you choose between:

- **Company Tenant**: Your work Azure account
- **Personal Tenant**: Your personal Azure subscription ✅ **Choose This**

**Important**: Ensure you're using your personal Azure subscription to avoid:

- Company billing for personal projects
- Corporate policy restrictions
- Data governance complications

---

## 🚀 Deployment Process

### **Quick Start**

```bash
# 1. Run pre-flight checks (interactive)
./azure-preflight.sh

# 2. Or run setup directly (interactive)
./azure-setup.sh
```

### **What Gets Created**

| Resource              | UK Name                      | Purpose                     | UK Location |
| --------------------- | ---------------------------- | --------------------------- | ----------- |
| **Resource Group**    | `rg-worldleaders-personal`   | Container for all resources | UK South    |
| **Azure OpenAI**      | `worldleaders-openai-uk`     | GPT-4 for educational AI    | UK South    |
| **Content Moderator** | `worldleaders-contentmod-uk` | Child safety validation     | UK South    |
| **Speech Services**   | `worldleaders-speech-uk`     | Language learning           | UK South    |
| **Key Vault**         | `worldleaders-kv-uk`         | Secure credential storage   | UK South    |

---

## 💰 UK Cost Estimates (Personal Project)

### **Monthly Costs in GBP**

| Service               | Light Usage | Moderate Usage | Educational Discount  |
| --------------------- | ----------- | -------------- | --------------------- |
| **Azure OpenAI**      | £30-40      | £60-80         | Possible 20% discount |
| **Content Moderator** | £15-20      | £25-35         | Standard rates        |
| **Speech Services**   | £10-15      | £20-30         | Standard rates        |
| **Key Vault**         | £2-3        | £3-5           | Standard rates        |
| **Total**             | **£57-78**  | **£108-150**   | **£46-120**           |

### **Cost Optimization Tips**

- **Free Tiers**: Use where available for initial testing
- **Budget Alerts**: Set up alerts at £50, £100, £150
- **Usage Monitoring**: Regular review of token consumption
- **Development Mode**: Use local fallbacks during development

---

## 🛡️ UK Data Protection & Security

### **GDPR Compliance**

- **Data Residency**: All data processed within UK Azure regions
- **Child Data Protection**: COPPA-compliant child safety measures
- **Audit Trail**: Complete logging for compliance auditing
- **Data Minimization**: Only necessary data collected and processed

### **Security Configuration**

```json
{
  "DataResidency": "UK",
  "Encryption": "AES-256",
  "KeyManagement": "Azure Key Vault",
  "AccessControl": "RBAC",
  "ChildSafety": "Multi-layer validation",
  "AuditLogging": "Complete interaction logging"
}
```

---

## 🔧 UK-Specific Configuration

### **Updated appsettings.json**

```json
{
  "SpeechServices": {
    "Region": "uksouth",
    "Endpoint": "",
    "ApiKey": "",
    "SupportedLanguages": [
      "en-GB",
      "en-US",
      "es-ES",
      "fr-FR",
      "de-DE",
      "zh-CN",
      "ja-JP"
    ]
  }
}
```

**Note**: `en-GB` added as primary language for UK English pronunciation.

### **Resource Naming Convention**

- **Pattern**: `{service}-{project}-{region}`
- **Examples**:
  - `worldleaders-openai-uk`
  - `worldleaders-contentmod-uk`
  - `worldleaders-speech-uk`

---

## 🚦 Deployment Steps

### **1. Pre-flight Verification**

```bash
./azure-preflight.sh
```

- Interactive account selection
- UK region confirmation
- Cost estimate review
- Security checklist

### **2. Azure Resource Creation**

```bash
./azure-setup.sh
```

- Automatic UK region setup
- Personal subscription verification
- Resource creation with UK naming
- Configuration file generation

### **3. Application Configuration**

```bash
# Copy generated configuration
cp azure-ai-config.json src/WorldLeaders/WorldLeaders.API/
# Update appsettings.json with UK-specific settings
```

### **4. Testing UK Deployment**

```bash
# Start API with UK Azure configuration
cd src/WorldLeaders/WorldLeaders.API
dotnet run --launch-profile https

# Test UK-based AI agents
curl -X GET "https://localhost:7289/api/AI/personalities"
```

---

## 🌍 UK Educational Context

### **Language Support**

- **Primary**: British English (en-GB)
- **Secondary**: International English (en-US)
- **Additional**: Spanish, French, German, Mandarin, Japanese

### **Cultural Adaptation**

- **Spelling**: British English conventions
- **Currency**: Pounds (£) for economic examples
- **Geography**: UK-centric world view in educational content
- **Time Zones**: GMT/BST awareness

### **Educational Standards**

- **Curriculum**: Aligned with UK Key Stage 2/3 standards
- **Age Group**: 11-13 year olds (Year 7-8 equivalent)
- **Subjects**: Geography, Economics, Modern Languages
- **Assessment**: Compatible with UK educational frameworks

---

## 💰 Pay-As-You-Go Pricing for Educational Projects

### **Azure AI Services Pricing Model**

All Azure AI services operate on **pay-as-you-go** basis - perfect for educational projects:

#### **Azure OpenAI (GPT-4)**
- **Model**: GPT-4 (0613)
- **Input Tokens**: $0.03 per 1K tokens (~750 words)
- **Output Tokens**: $0.06 per 1K tokens (~750 words)
- **Educational Usage**: 10-50 interactions/day = £15-30/month

#### **Content Moderator**
- **Text Moderation**: $1 per 1,000 transactions
- **Educational Usage**: 100-500 validations/day = £3-15/month

#### **Speech Services**
- **Speech-to-Text**: $1 per hour of audio
- **Text-to-Speech**: $4 per 1M characters
- **Educational Usage**: 30 minutes/day = £15-25/month

### **Realistic Educational Project Costs**

#### **Small Classroom (5-10 students)**
- **Daily Usage**: 50 AI interactions, 100 safety checks, 15 min speech
- **Monthly Cost**: £25-40 total
- **Per Student**: £2.50-4.00/month

#### **Individual Learning (1-2 students)**
- **Daily Usage**: 10-20 AI interactions, 20-40 safety checks, 5 min speech
- **Monthly Cost**: £8-15 total
- **Perfect for**: Home education, tutoring, personal projects

#### **Medium School Project (20-30 students)**
- **Daily Usage**: 200 AI interactions, 400 safety checks, 60 min speech
- **Monthly Cost**: £80-120 total
- **Per Student**: £2.70-4.00/month

### **Free Tier Benefits**

- **Content Moderator**: First 5,000 transactions/month FREE
- **Speech Services**: First 5 hours speech-to-text/month FREE
- **OpenAI**: No free tier, but very affordable for educational use

### **Cost Control Features**

```bash
# Set up budget alerts in Azure
az consumption budget create \
  --resource-group rg-worldleaders-personal \
  --budget-name "educational-ai-budget" \
  --amount 50 \
  --time-grain Monthly \
  --time-period "2025-08-01" "2026-07-31"
```

---

## ⚠️ Important Considerations

### **Azure OpenAI Access**

- **Approval Required**: Microsoft approval needed for OpenAI access
- **Application Process**: Specify educational use for children
- **Alternative Regions**: If UK South unavailable, try UK West
- **Fallback Plan**: Local mock responses available if OpenAI unavailable

### **Personal vs Business Use**

- **Subscription**: Use personal Azure subscription
- **Billing**: Personal responsibility for costs
- **Support**: Community support (not business SLA)
- **Limitations**: Personal subscription quotas apply

### **Data Protection**

- **Child Privacy**: COPPA and GDPR compliance required
- **Data Retention**: Configure appropriate retention policies
- **Access Controls**: Limit access to personal use only
- **Monitoring**: Regular security and usage audits

---

## 🎯 Next Steps After Deployment

1. **Test AI Agents**: Verify all 6 personalities work with UK Azure
2. **Monitor Costs**: Set up budget alerts and usage tracking
3. **Educational Testing**: Validate content for UK educational standards
4. **Performance Tuning**: Optimize for UK user latency
5. **Compliance Review**: Ensure GDPR and child safety compliance

---

**Ready to deploy your World Leaders Game to UK Azure? Start with `./azure-preflight.sh` to verify your setup!** 🇬🇧🎮
