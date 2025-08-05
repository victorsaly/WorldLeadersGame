#!/bin/bash

# Setup GitHub Azure Secrets for World Leaders Game
# This script helps create and configure Azure Service Principal for GitHub Actions

set -e

echo "🔧 Setting up Azure Service Principal for GitHub Actions"
echo "🎮 World Leaders Educational Game Deployment"
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
RESOURCE_GROUP="worldleaders-prod-rg"
SP_NAME="worldleaders-github-actions"

echo -e "${BLUE}📋 Configuration:${NC}"
echo "  Resource Group: $RESOURCE_GROUP"
echo "  Service Principal: $SP_NAME"
echo ""

# Check if Azure CLI is installed
if ! command -v az &> /dev/null; then
    echo -e "${RED}❌ Azure CLI is not installed.${NC}"
    echo "Please install Azure CLI: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli"
    exit 1
fi

# Check if logged in to Azure
if ! az account show &> /dev/null; then
    echo -e "${YELLOW}🔐 Please log in to Azure...${NC}"
    az login
fi

# Get current subscription
SUBSCRIPTION_ID=$(az account show --query id --output tsv)
echo -e "${GREEN}✅ Using subscription: $SUBSCRIPTION_ID${NC}"

# Check if resource group exists
if ! az group show --name $RESOURCE_GROUP &> /dev/null; then
    echo -e "${RED}❌ Resource group '$RESOURCE_GROUP' not found.${NC}"
    echo "Please create the resource group first or update the script with the correct name."
    exit 1
fi

echo -e "${GREEN}✅ Resource group '$RESOURCE_GROUP' found${NC}"

# Create Service Principal
echo -e "${BLUE}🔧 Creating Service Principal...${NC}"

SCOPE="/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP"

# Create the service principal and capture the output
SP_OUTPUT=$(az ad sp create-for-rbac \
    --name "$SP_NAME" \
    --role contributor \
    --scopes "$SCOPE" \
    --json-auth)

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Service Principal created successfully!${NC}"
else
    echo -e "${RED}❌ Failed to create Service Principal${NC}"
    exit 1
fi

# Extract values from the JSON output
CLIENT_ID=$(echo $SP_OUTPUT | jq -r '.clientId')
TENANT_ID=$(echo $SP_OUTPUT | jq -r '.tenantId')
SUBSCRIPTION_ID_FROM_SP=$(echo $SP_OUTPUT | jq -r '.subscriptionId')

# Create federated credential for GitHub Actions OIDC
echo -e "${BLUE}🔐 Creating federated credential for GitHub OIDC...${NC}"
REPO_OWNER="victorsaly"
REPO_NAME="WorldLeadersGame"

# Create federated credential for main branch
az ad app federated-credential create \
    --id $CLIENT_ID \
    --parameters "{\"name\":\"github-main\",\"issuer\":\"https://token.actions.githubusercontent.com\",\"subject\":\"repo:${REPO_OWNER}/${REPO_NAME}:ref:refs/heads/main\",\"audiences\":[\"api://AzureADTokenExchange\"]}"

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Federated credential created for main branch!${NC}"
else
    echo -e "${YELLOW}⚠️  Federated credential creation failed (may already exist)${NC}"
fi

echo ""
echo -e "${YELLOW}🔑 GitHub Repository Secrets to Add:${NC}"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo -e "${BLUE}Secret Name:${NC} AZURE_CLIENT_ID"
echo -e "${GREEN}Value:${NC} $CLIENT_ID"
echo ""
echo -e "${BLUE}Secret Name:${NC} AZURE_TENANT_ID"
echo -e "${GREEN}Value:${NC} $TENANT_ID"
echo ""
echo -e "${BLUE}Secret Name:${NC} AZURE_SUBSCRIPTION_ID"
echo -e "${GREEN}Value:${NC} $SUBSCRIPTION_ID_FROM_SP"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo -e "${YELLOW}📝 How to add these secrets to GitHub:${NC}"
echo "1. Go to your GitHub repository: https://github.com/victorsaly/WorldLeadersGame"
echo "2. Navigate to Settings → Secrets and variables → Actions"
echo "3. Click 'New repository secret' for each secret above"
echo "4. Copy and paste the exact values shown above"
echo ""
echo -e "${BLUE}🔐 OIDC Authentication Setup:${NC}"
echo "✅ Federated credential created for secure, password-less authentication"
echo "✅ No client secret required - GitHub Actions will use OIDC tokens"
echo "✅ Enhanced security with temporary, scoped credentials"
echo ""
echo -e "${GREEN}🎉 Setup complete!${NC}"
echo "Your Azure Service Principal is ready for GitHub Actions deployment."
