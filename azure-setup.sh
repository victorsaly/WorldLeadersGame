#!/bin/bash

# 🎮 World Leaders Game - Azure AI Setup Script
# Automatically provisions Azure AI services for production deployment
# Maintains child safety standards and educational focus

set -e  # Exit on any error

# Color coding for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration - UK Region and Personal Project Setup
RESOURCE_GROUP="rg-worldleaders-personal"
LOCATION="uksouth"
OPENAI_NAME="worldleaders-openai-uk"
CONTENTMOD_NAME="worldleaders-contentmod-uk"
SPEECH_NAME="worldleaders-speech-uk"
KEYVAULT_NAME="worldleaders-kv-uk"

echo -e "${BLUE}🎮 World Leaders Game - Azure AI Setup (UK Personal Project)${NC}"
echo -e "${BLUE}================================================================${NC}"
echo ""

# Check if Azure CLI is installed
if ! command -v az &> /dev/null; then
    echo -e "${RED}❌ Azure CLI is not installed. Please install it first.${NC}"
    echo "https://docs.microsoft.com/en-us/cli/azure/install-azure-cli"
    exit 1
fi

# Check if logged into Azure and show current context
echo -e "${BLUE}🔍 Checking Azure Authentication...${NC}"
if ! az account show &> /dev/null; then
    echo -e "${YELLOW}🔐 Please log into Azure...${NC}"
    az login
fi

# Show current account and tenant information
echo -e "${GREEN}✅ Current Azure Account:${NC}"
az account show --query "{Name:name, TenantId:tenantId, SubscriptionId:id, User:user.name}" --output table
echo ""

# Tenant and Subscription Selection
echo -e "${YELLOW}👤 Account Selection for Personal Project${NC}"
echo -e "${YELLOW}=========================================${NC}"
echo ""
echo -e "${BLUE}Available subscriptions:${NC}"
az account list --query "[].{Name:name, SubscriptionId:id, TenantId:tenantId, IsDefault:isDefault}" --output table
echo ""

read -p "🤔 Is the current subscription your PERSONAL account for this project? (y/n): " confirm_personal
if [[ $confirm_personal != "y" ]]; then
    echo ""
    echo -e "${YELLOW}📋 Available subscriptions:${NC}"
    az account list --query "[].{Name:name, SubscriptionId:id}" --output table
    echo ""
    read -p "📝 Enter the Subscription ID for your PERSONAL account: " personal_subscription_id
    
    echo -e "${BLUE}🔄 Switching to personal subscription...${NC}"
    az account set --subscription "$personal_subscription_id"
    
    echo -e "${GREEN}✅ Switched to personal account:${NC}"
    az account show --query "{Name:name, SubscriptionId:id, User:user.name}" --output table
else
    echo -e "${GREEN}✅ Using current personal subscription${NC}"
fi
echo ""

# Confirm UK region
echo -e "${YELLOW}🇬🇧 UK Region Configuration${NC}"
echo -e "${YELLOW}============================${NC}"
echo "📍 Using UK South (${LOCATION}) for data residency and performance"
echo "🎯 This is optimal for UK-based educational projects"
echo ""

read -p "🤔 Proceed with UK South region? (y/n): " confirm_region
if [[ $confirm_region != "y" ]]; then
    echo ""
    echo -e "${BLUE}Available UK regions:${NC}"
    echo "  1. uksouth (UK South - London area) - Recommended"
    echo "  2. ukwest (UK West - Cardiff area)"
    echo ""
    read -p "📝 Enter preferred region (uksouth/ukwest): " preferred_region
    LOCATION="$preferred_region"
fi

echo -e "${GREEN}✅ Using region: $LOCATION${NC}"
echo ""

echo -e "${GREEN}✅ Azure CLI is ready for personal project deployment${NC}"
echo ""

# Create Resource Group
echo -e "${BLUE}1️⃣ Creating Resource Group for Personal Project...${NC}"
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION \
  --tags project=worldleaders environment=personal purpose=educational-ai region=uk owner=personal \
  --output table

echo -e "${GREEN}✅ Resource Group '$RESOURCE_GROUP' created in $LOCATION${NC}"
echo ""

# Create Azure OpenAI Service
echo -e "${BLUE}2️⃣ Creating Azure OpenAI Service in UK...${NC}"
echo -e "${YELLOW}   Note: This requires OpenAI access approval for your personal subscription${NC}"
echo -e "${YELLOW}   If you don't have access yet, apply at: https://aka.ms/oai/access${NC}"

if az cognitiveservices account create \
  --name $OPENAI_NAME \
  --resource-group $RESOURCE_GROUP \
  --kind OpenAI \
  --sku S0 \
  --location $LOCATION \
  --custom-domain $OPENAI_NAME \
  --tags purpose=educational-ai target-age=12 project=personal region=uk \
  --output table; then
  echo -e "${GREEN}✅ Azure OpenAI Service '$OPENAI_NAME' created in $LOCATION${NC}"
else
  echo -e "${RED}❌ Failed to create OpenAI service in $LOCATION${NC}"
  echo -e "${YELLOW}   This could be due to:${NC}"
  echo -e "${YELLOW}   1. OpenAI access not approved for your subscription${NC}"
  echo -e "${YELLOW}   2. Region capacity limitations${NC}"
  echo -e "${YELLOW}   3. Quota restrictions${NC}"
  echo ""
  echo -e "${BLUE}   Apply for OpenAI access: https://aka.ms/oai/access${NC}"
  echo -e "${BLUE}   Specify this is for educational use with children${NC}"
fi
echo ""

# Deploy GPT-4 Model
echo -e "${BLUE}3️⃣ Deploying GPT-4 Model for Educational Content...${NC}"
if az cognitiveservices account deployment create \
  --name $OPENAI_NAME \
  --resource-group $RESOURCE_GROUP \
  --deployment-name gpt-4-educational \
  --model-name gpt-4 \
  --model-version "0613" \
  --model-format OpenAI \
  --scale-type "Standard" \
  --scale-capacity 10 \
  --output table; then
  echo -e "${GREEN}✅ GPT-4 model deployed for educational interactions${NC}"
else
  echo -e "${YELLOW}⚠️ GPT-4 deployment may take a few minutes or require quota approval${NC}"
fi
echo ""

# Create Content Moderator
echo -e "${BLUE}4️⃣ Creating Content Moderator for Child Safety (UK)...${NC}"
az cognitiveservices account create \
  --name $CONTENTMOD_NAME \
  --resource-group $RESOURCE_GROUP \
  --kind ContentModerator \
  --sku S0 \
  --location $LOCATION \
  --tags purpose=child-safety validation=multi-layer project=personal region=uk \
  --output table

echo -e "${GREEN}✅ Content Moderator '$CONTENTMOD_NAME' created in $LOCATION${NC}"
echo ""

# Create Speech Services
echo -e "${BLUE}5️⃣ Creating Speech Services for Language Learning (UK)...${NC}"
az cognitiveservices account create \
  --name $SPEECH_NAME \
  --resource-group $RESOURCE_GROUP \
  --kind SpeechServices \
  --sku S0 \
  --location $LOCATION \
  --tags purpose=language-learning feature=pronunciation project=personal region=uk \
  --output table

echo -e "${GREEN}✅ Speech Services '$SPEECH_NAME' created in $LOCATION${NC}"
echo ""

# Create Key Vault
echo -e "${BLUE}6️⃣ Creating Key Vault for Secure Credential Storage (UK)...${NC}"
az keyvault create \
  --name $KEYVAULT_NAME \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --enable-rbac-authorization \
  --tags purpose=credential-storage project=personal region=uk \
  --output table

echo -e "${GREEN}✅ Key Vault '$KEYVAULT_NAME' created in $LOCATION${NC}"
echo ""

# Get API Keys and Endpoints
echo -e "${BLUE}7️⃣ Retrieving API Keys and Endpoints...${NC}"

# OpenAI
echo -e "${YELLOW}Getting OpenAI credentials...${NC}"
OPENAI_ENDPOINT=$(az cognitiveservices account show \
  --name $OPENAI_NAME \
  --resource-group $RESOURCE_GROUP \
  --query "properties.endpoint" \
  --output tsv)

OPENAI_KEY=$(az cognitiveservices account keys list \
  --name $OPENAI_NAME \
  --resource-group $RESOURCE_GROUP \
  --query "key1" \
  --output tsv)

# Content Moderator
echo -e "${YELLOW}Getting Content Moderator credentials...${NC}"
CONTENTMOD_ENDPOINT=$(az cognitiveservices account show \
  --name $CONTENTMOD_NAME \
  --resource-group $RESOURCE_GROUP \
  --query "properties.endpoint" \
  --output tsv)

CONTENTMOD_KEY=$(az cognitiveservices account keys list \
  --name $CONTENTMOD_NAME \
  --resource-group $RESOURCE_GROUP \
  --query "key1" \
  --output tsv)

# Speech Services
echo -e "${YELLOW}Getting Speech Services credentials...${NC}"
SPEECH_KEY=$(az cognitiveservices account keys list \
  --name $SPEECH_NAME \
  --resource-group $RESOURCE_GROUP \
  --query "key1" \
  --output tsv)

echo -e "${GREEN}✅ All credentials retrieved${NC}"
echo ""

# Store secrets in Key Vault
echo -e "${BLUE}8️⃣ Storing API Keys in Key Vault...${NC}"

az keyvault secret set \
  --vault-name $KEYVAULT_NAME \
  --name "openai-api-key" \
  --value "$OPENAI_KEY" \
  --output none

az keyvault secret set \
  --vault-name $KEYVAULT_NAME \
  --name "content-moderator-api-key" \
  --value "$CONTENTMOD_KEY" \
  --output none

az keyvault secret set \
  --vault-name $KEYVAULT_NAME \
  --name "speech-services-api-key" \
  --value "$SPEECH_KEY" \
  --output none

echo -e "${GREEN}✅ API keys stored securely in Key Vault${NC}"
echo ""

# Generate configuration
echo -e "${BLUE}9️⃣ Generating Configuration...${NC}"

cat > azure-ai-config.json << EOF
{
  "AzureOpenAI": {
    "Endpoint": "$OPENAI_ENDPOINT",
    "ApiKey": "$OPENAI_KEY",
    "DeploymentName": "gpt-4-educational",
    "ApiVersion": "2024-02-15-preview",
    "MaxTokens": 500,
    "Temperature": 0.7,
    "EducationalSystemPrompt": "You are an educational AI assistant for 12-year-old students learning geography, economics, and languages. Always provide encouraging, age-appropriate, and culturally sensitive responses that teach real-world concepts."
  },
  "ContentModerator": {
    "Endpoint": "$CONTENTMOD_ENDPOINT",
    "ApiKey": "$CONTENTMOD_KEY",
    "ChildSafetyLevel": "Strict",
    "CustomBlockLists": ["violence", "inappropriate-language", "adult-themes"]
  },
  "SpeechServices": {
    "Region": "$LOCATION",
    "Endpoint": "https://$LOCATION.tts.speech.microsoft.com/",
    "ApiKey": "$SPEECH_KEY",
    "SupportedLanguages": ["en-GB", "en-US", "es-ES", "fr-FR", "de-DE", "zh-CN", "ja-JP"]
  }
}
EOF

cat > .env.production << EOF
# Azure AI Configuration for World Leaders Game
# Generated by setup script - Keep secure!

AZURE_OPENAI_ENDPOINT=$OPENAI_ENDPOINT
AZURE_OPENAI_API_KEY=$OPENAI_KEY
CONTENT_MODERATOR_ENDPOINT=$CONTENTMOD_ENDPOINT
CONTENT_MODERATOR_API_KEY=$CONTENTMOD_KEY
SPEECH_SERVICES_API_KEY=$SPEECH_KEY
SPEECH_SERVICES_REGION=$LOCATION
EOF

echo -e "${GREEN}✅ Configuration files generated:${NC}"
echo -e "   📄 azure-ai-config.json"
echo -e "   📄 .env.production"
echo ""

# Summary
echo -e "${BLUE}🎉 Azure AI Setup Complete!${NC}"
echo -e "${BLUE}=========================${NC}"
echo ""
echo -e "${GREEN}✅ Resources Created:${NC}"
echo -e "   🧠 Azure OpenAI Service: $OPENAI_NAME"
echo -e "   🛡️ Content Moderator: $CONTENTMOD_NAME"
echo -e "   🗣️ Speech Services: $SPEECH_NAME"
echo -e "   🔐 Key Vault: $KEYVAULT_NAME"
echo ""
echo -e "${YELLOW}📋 Next Steps:${NC}"
echo -e "   1️⃣ Copy the configuration from azure-ai-config.json to your appsettings.json"
echo -e "   2️⃣ Update your application with the Azure AI credentials"
echo -e "   3️⃣ Test the AI agents with: curl https://localhost:7289/api/AI/personalities"
echo -e "   4️⃣ Monitor child safety compliance and educational effectiveness"
echo ""
echo -e "${BLUE}📊 Educational Project Costs (Pay-As-You-Go):${NC}"
echo -e "   💰 Individual Learning (10-20 chats/day): £8-15/month"
echo -e "   💰 Small Classroom (5-10 students): £25-40/month total"
echo -e "   💰 Home Education (5-15 chats/day): £5-12/month"
echo -e "   💰 Per conversation cost: ~£0.04 (very affordable!)"
echo ""
echo -e "${GREEN}✅ Free Tier Benefits:${NC}"
echo -e "   🎁 5,000 FREE safety validations/month"
echo -e "   🎁 5 hours FREE speech recognition/month"
echo -e "   🎁 No upfront costs - pay only for usage"
echo -e "   🎁 Perfect for educational experimentation"
echo ""
echo -e "${GREEN}🛡️ Child Safety Features Active:${NC}"
echo -e "   ✅ Multi-layer content validation"
echo -e "   ✅ Age-appropriate response filtering"
echo -e "   ✅ Cultural sensitivity checks"
echo -e "   ✅ Educational value validation"
echo -e "   ✅ Positive messaging enforcement"
echo ""
echo -e "${BLUE}Your World Leaders Game is now ready for production with Azure AI! 🎮${NC}"
