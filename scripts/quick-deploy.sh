#!/bin/bash

# 🚀 Quick Deploy - World Leaders Game to Azure Production
# One-command deployment setup for your educational game

set -e

echo "🚀 World Leaders Game - Quick Production Deployment"
echo "=================================================="
echo ""

# Color coding
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Check prerequisites
echo -e "${BLUE}🔍 Checking Prerequisites...${NC}"

# Check GitHub CLI
if ! command -v gh &> /dev/null; then
    echo -e "${RED}❌ GitHub CLI not found. Install with: brew install gh${NC}"
    exit 1
fi

# Check Azure CLI
if ! command -v az &> /dev/null; then
    echo -e "${RED}❌ Azure CLI not found. Install from: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli${NC}"
    exit 1
fi

# Check if logged into GitHub
if ! gh auth status &> /dev/null; then
    echo -e "${YELLOW}🔐 Logging into GitHub...${NC}"
    gh auth login
fi

# Check if logged into Azure
if ! az account show &> /dev/null; then
    echo -e "${YELLOW}🔐 Logging into Azure...${NC}"
    az login
fi

echo -e "${GREEN}✅ Prerequisites checked${NC}"
echo ""

# Get current Azure subscription info
SUBSCRIPTION_ID=$(az account show --query id -o tsv)
SUBSCRIPTION_NAME=$(az account show --query name -o tsv)

echo -e "${BLUE}📋 Current Azure Context:${NC}"
echo "   Subscription: $SUBSCRIPTION_NAME"
echo "   ID: $SUBSCRIPTION_ID"
echo ""

# Step 1: Create Azure Service Principal
echo -e "${BLUE}🔐 Step 1: Creating Azure Service Principal...${NC}"

SERVICE_PRINCIPAL_NAME="github-worldleaders-deploy-$(date +%s)"
echo "Creating service principal: $SERVICE_PRINCIPAL_NAME"

SP_JSON=$(az ad sp create-for-rbac \
  --name "$SERVICE_PRINCIPAL_NAME" \
  --role contributor \
  --scopes "/subscriptions/$SUBSCRIPTION_ID" \
  --sdk-auth)

echo -e "${GREEN}✅ Service principal created${NC}"
echo ""

# Step 2: Setup GitHub Secrets
echo -e "${BLUE}🔑 Step 2: Setting up GitHub Secrets...${NC}"

# Set Azure credentials
echo "$SP_JSON" | gh secret set AZURE_CREDENTIALS

# Set subscription ID
echo "$SUBSCRIPTION_ID" | gh secret set AZURE_SUBSCRIPTION_ID

# Get current AI service values from local config
if [ -f "azure-ai-config.local.json" ]; then
    echo "📝 Found local Azure AI configuration..."
    
    OPENAI_ENDPOINT=$(jq -r '.AzureOpenAI.Endpoint' azure-ai-config.local.json)
    OPENAI_KEY=$(jq -r '.AzureOpenAI.ApiKey' azure-ai-config.local.json)
    CONTENT_ENDPOINT=$(jq -r '.ContentModerator.Endpoint' azure-ai-config.local.json)
    CONTENT_KEY=$(jq -r '.ContentModerator.ApiKey' azure-ai-config.local.json)
    SPEECH_KEY=$(jq -r '.SpeechServices.ApiKey' azure-ai-config.local.json)
    
    # Set AI service secrets
    echo "$OPENAI_ENDPOINT" | gh secret set AZURE_OPENAI_ENDPOINT
    echo "$OPENAI_KEY" | gh secret set AZURE_OPENAI_API_KEY
    echo "$CONTENT_ENDPOINT" | gh secret set CONTENT_MODERATOR_ENDPOINT
    echo "$CONTENT_KEY" | gh secret set CONTENT_MODERATOR_API_KEY
    echo "$SPEECH_KEY" | gh secret set SPEECH_SERVICES_API_KEY
    
    echo -e "${GREEN}✅ Azure AI secrets configured${NC}"
else
    echo -e "${YELLOW}⚠️  No local AI config found. You'll need to set AI secrets manually.${NC}"
fi

# Generate and set JWT secret
JWT_SECRET=$(openssl rand -base64 64 | tr -d '\n')
echo "$JWT_SECRET" | gh secret set JWT_SECRET_KEY

echo -e "${GREEN}✅ GitHub secrets configured${NC}"
echo ""

# Step 3: Commit and deploy
echo -e "${BLUE}🚀 Step 3: Deploying to Production...${NC}"

# Check if there are uncommitted changes
if ! git diff-index --quiet HEAD --; then
    echo "📝 Committing current changes..."
    git add .
    git commit -m "🚀 Deploy World Leaders Educational Game to Azure Production

- Configure GitHub Actions workflow for Azure deployment
- Set up production Azure App Services
- Enable Azure AI integration (OpenAI, Content Moderator, Speech)
- Implement child-safe JWT authentication
- Configure COPPA/GDPR compliance features
- Set up cost tracking (£0.08/user/day limit)
- Deploy educational documentation site

Ready for 12-year-old learners worldwide! 🎓"
fi

# Push to trigger deployment
echo "🚀 Pushing to GitHub to trigger deployment..."
git push origin main

echo ""
echo -e "${GREEN}🎉 Deployment Initiated Successfully!${NC}"
echo "======================================"
echo ""
echo -e "${BLUE}📊 Monitor deployment progress:${NC}"
echo "   GitHub Actions: https://github.com/$(gh repo view --json owner,name -q '.owner.login + "/" + .name')/actions"
echo ""
echo -e "${BLUE}🎮 Production URLs (available after deployment):${NC}"
echo "   🌐 Educational Game: https://worldleaders-web-prod.azurewebsites.net"
echo "   🔗 Game API: https://worldleaders-api-prod.azurewebsites.net"
echo "   🩺 Health Check: https://worldleaders-api-prod.azurewebsites.net/health"
echo "   📖 API Documentation: https://worldleaders-api-prod.azurewebsites.net/swagger"
echo ""
echo -e "${BLUE}📚 Educational Features:${NC}"
echo "   ✅ COPPA/GDPR compliant for 12-year-olds"
echo "   ✅ AI-powered geography and economics learning"
echo "   ✅ Speech recognition for language learning"
echo "   ✅ Real-world country and GDP data integration"
echo "   ✅ Cost controls (£0.08/user/day limit)"
echo "   ✅ Content moderation for child safety"
echo ""
echo -e "${YELLOW}⏱️  Deployment typically takes 10-15 minutes${NC}"
echo -e "${GREEN}🎓 Your educational game will soon be live for students worldwide!${NC}"

# Optional: Open GitHub Actions in browser
read -p "Open GitHub Actions in browser to monitor deployment? (y/n): " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    gh repo view --web --branch main
fi
