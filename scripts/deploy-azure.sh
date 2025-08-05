#!/bin/bash

# World Leaders Game - Azure Deployment Script
# Context: Educational game deployment for 12-year-old learners
# Purpose: Deploy game to Azure with custom domain routing

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
PROJECT_NAME="worldleaders"
ENVIRONMENT="prod"
LOCATION="uksouth"
RESOURCE_GROUP="${PROJECT_NAME}-${ENVIRONMENT}-rg"

# Optional: Set your custom domain here
CUSTOM_DOMAIN="" # e.g., "worldleadersgame.com"

echo -e "${BLUE}🎮 World Leaders Game - Azure Deployment${NC}"
echo -e "${BLUE}======================================${NC}"
echo ""

# Check if Azure CLI is installed and logged in
echo -e "${YELLOW}🔍 Checking Azure CLI...${NC}"
if ! command -v az &> /dev/null; then
    echo -e "${RED}❌ Azure CLI not found. Please install it first.${NC}"
    echo "Visit: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli"
    exit 1
fi

# Check if logged in
if ! az account show &> /dev/null; then
    echo -e "${YELLOW}🔐 Please log in to Azure...${NC}"
    az login
fi

# Show current subscription
SUBSCRIPTION=$(az account show --query name -o tsv)
echo -e "${GREEN}✅ Logged in to: ${SUBSCRIPTION}${NC}"
echo ""

# Ask for custom domain
if [ -z "$CUSTOM_DOMAIN" ]; then
    echo -e "${YELLOW}🌐 Custom Domain Configuration${NC}"
    echo "Do you have a custom domain for this deployment? (e.g., worldleadersgame.co.uk)"
    read -p "Enter domain name (or press Enter to skip): " CUSTOM_DOMAIN
    echo ""
fi

# Create resource group if it doesn't exist
echo -e "${YELLOW}📦 Creating resource group...${NC}"
az group create \
    --name $RESOURCE_GROUP \
    --location $LOCATION \
    --tags project="World Leaders Game" environment=$ENVIRONMENT purpose="Educational game for 12-year-olds"

# Deploy infrastructure using Bicep
echo -e "${YELLOW}🏗️ Deploying Azure infrastructure...${NC}"
DEPLOYMENT_OUTPUT=$(az deployment group create \
    --resource-group $RESOURCE_GROUP \
    --template-file infrastructure/azure-deploy.bicep \
    --parameters projectName=$PROJECT_NAME environment=$ENVIRONMENT location=$LOCATION customDomainName="$CUSTOM_DOMAIN" \
    --query 'properties.outputs' \
    --output json)

echo -e "${GREEN}✅ Infrastructure deployed successfully!${NC}"
echo ""

# Extract output values
WEB_APP_URL=$(echo $DEPLOYMENT_OUTPUT | jq -r '.webAppUrl.value')
API_APP_URL=$(echo $DEPLOYMENT_OUTPUT | jq -r '.apiAppUrl.value')
STATIC_SITE_URL=$(echo $DEPLOYMENT_OUTPUT | jq -r '.staticSiteUrl.value')
STORAGE_ACCOUNT=$(echo $DEPLOYMENT_OUTPUT | jq -r '.storageAccountName.value')

echo -e "${BLUE}🌐 Deployment URLs:${NC}"
echo -e "   Web App (Game): ${GREEN}$WEB_APP_URL${NC}"
echo -e "   API:           ${GREEN}$API_APP_URL${NC}"
echo -e "   Documentation: ${GREEN}$STATIC_SITE_URL${NC}"
echo ""

# Build and deploy the .NET applications
echo -e "${YELLOW}🔨 Building .NET applications...${NC}"
cd src/WorldLeaders

# Build the solution
dotnet restore
dotnet build --configuration Release

# Publish Web App
echo -e "${YELLOW}📦 Publishing Web App...${NC}"
dotnet publish WorldLeaders.Web/WorldLeaders.Web.csproj \
    --configuration Release \
    --output ../../publish/web \
    --self-contained false

# Publish API
echo -e "${YELLOW}📦 Publishing API...${NC}"
dotnet publish WorldLeaders.API/WorldLeaders.API.csproj \
    --configuration Release \
    --output ../../publish/api \
    --self-contained false

cd ../..

# Create deployment packages
echo -e "${YELLOW}📦 Creating deployment packages...${NC}"
cd publish
zip -r web-deploy.zip web/
zip -r api-deploy.zip api/
cd ..

# Deploy Web App
echo -e "${YELLOW}🚀 Deploying Web App...${NC}"
WEB_APP_NAME="${PROJECT_NAME}-web-${ENVIRONMENT}"
az webapp deploy \
    --resource-group $RESOURCE_GROUP \
    --name $WEB_APP_NAME \
    --src-path publish/web-deploy.zip \
    --type zip

# Deploy API
echo -e "${YELLOW}🚀 Deploying API...${NC}"
API_APP_NAME="${PROJECT_NAME}-api-${ENVIRONMENT}"
az webapp deploy \
    --resource-group $RESOURCE_GROUP \
    --name $API_APP_NAME \
    --src-path publish/api-deploy.zip \
    --type zip

# Setup Static Web App for documentation
echo -e "${YELLOW}📚 Setting up documentation deployment...${NC}"
STATIC_SITE_NAME="${PROJECT_NAME}-docs-${ENVIRONMENT}"

# Get Static Web App deployment token
STATIC_DEPLOYMENT_TOKEN=$(az staticwebapp secrets list \
    --resource-group $RESOURCE_GROUP \
    --name $STATIC_SITE_NAME \
    --query 'properties.apiKey' \
    --output tsv)

echo -e "${GREEN}✅ Static Web App deployment token retrieved${NC}"
echo -e "${YELLOW}📝 To deploy documentation:${NC}"
echo "   1. Connect your GitHub repository to the Static Web App"
echo "   2. Configure build settings:"
echo "      - App location: /docs"
echo "      - Output location: _site"
echo "      - Build command: bundle install && bundle exec jekyll build"
echo ""

# Custom domain configuration
if [ -n "$CUSTOM_DOMAIN" ]; then
    echo -e "${BLUE}🌐 Custom Domain Configuration${NC}"
    echo -e "${BLUE}==============================${NC}"
    echo ""
    echo -e "${YELLOW}DNS Configuration Required:${NC}"
    echo ""
    echo -e "${GREEN}1. Main Domain (Game):${NC}"
    echo -e "   Add CNAME record: ${CUSTOM_DOMAIN} → ${WEB_APP_NAME}.azurewebsites.net"
    echo ""
    echo -e "${GREEN}2. API Routing:${NC}"
    echo -e "   Option A: Subdomain - Add CNAME: api.${CUSTOM_DOMAIN} → ${API_APP_NAME}.azurewebsites.net"
    echo -e "   Option B: Path routing - Configure reverse proxy for /${CUSTOM_DOMAIN}/api"
    echo ""
    echo -e "${GREEN}3. Documentation:${NC}"
    echo -e "   Configure reverse proxy for ${CUSTOM_DOMAIN}/docs → Static Web App"
    echo ""
    echo -e "${YELLOW}Application Gateway/Front Door recommended for path-based routing${NC}"
fi

# Cleanup
echo -e "${YELLOW}🧹 Cleaning up build artifacts...${NC}"
rm -rf publish/

echo ""
echo -e "${GREEN}🎉 Deployment Complete!${NC}"
echo -e "${GREEN}======================${NC}"
echo ""
echo -e "${BLUE}Next Steps:${NC}"
echo "1. 🌐 Visit your game: $WEB_APP_URL"
echo "2. 🔧 Test your API: $API_APP_URL/health"
echo "3. 📚 Configure documentation deployment"
if [ -n "$CUSTOM_DOMAIN" ]; then
    echo "4. 🌍 Configure custom domain DNS records"
    echo "5. 🔒 Add SSL certificates for custom domain"
fi
echo ""
echo -e "${YELLOW}💰 Cost Monitoring:${NC}"
echo "   Monitor costs at: https://portal.azure.com/#view/Microsoft_Azure_CostManagement/Menu/~/overview"
echo ""
echo -e "${BLUE}🎮 Happy Gaming! The World Leaders adventure awaits! 🌍${NC}"
