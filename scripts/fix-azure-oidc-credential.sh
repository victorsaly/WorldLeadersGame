#!/bin/bash

# Fix Azure OIDC Federated Credential for GitHub Actions
# This script updates the federated credential to use the correct subject claim
# for the production environment

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🔧 Fixing Azure OIDC Federated Credential...${NC}"
echo ""

# Repository information
REPO_OWNER="victorsaly"
REPO_NAME="WorldLeadersGame"

# Check if user is logged into Azure
if ! az account show &> /dev/null; then
    echo -e "${RED}❌ Not logged into Azure CLI${NC}"
    echo "Please run: az login"
    exit 1
fi

echo -e "${BLUE}🔍 Finding existing service principal...${NC}"

# Find the service principal by name
SP_NAME="worldleaders-github-actions"
CLIENT_ID=$(az ad sp list --display-name "$SP_NAME" --query "[0].appId" -o tsv)

if [ -z "$CLIENT_ID" ] || [ "$CLIENT_ID" = "null" ]; then
    echo -e "${RED}❌ Service principal '$SP_NAME' not found${NC}"
    echo "Please run the full setup script first: ./scripts/setup-github-azure-secrets.sh"
    exit 1
fi

echo -e "${GREEN}✅ Found service principal: $CLIENT_ID${NC}"
echo ""

# List existing federated credentials
echo -e "${BLUE}🔍 Checking existing federated credentials...${NC}"
EXISTING_CREDS=$(az ad app federated-credential list --id $CLIENT_ID --query "[].name" -o tsv)

if [ -n "$EXISTING_CREDS" ]; then
    echo "Existing credentials:"
    echo "$EXISTING_CREDS" | while read -r cred; do
        echo "  - $cred"
    done
    echo ""
fi

# Delete old credential if it exists
OLD_CRED_NAME="github-main"
if echo "$EXISTING_CREDS" | grep -q "$OLD_CRED_NAME"; then
    echo -e "${YELLOW}🗑️  Removing old federated credential: $OLD_CRED_NAME${NC}"
    az ad app federated-credential delete --id $CLIENT_ID --federated-credential-id $OLD_CRED_NAME --yes
fi

# Create new credential for production environment
echo -e "${BLUE}🔐 Creating new federated credential for production environment...${NC}"
az ad app federated-credential create \
    --id $CLIENT_ID \
    --parameters "{
        \"name\": \"github-production\",
        \"issuer\": \"https://token.actions.githubusercontent.com\",
        \"subject\": \"repo:${REPO_OWNER}/${REPO_NAME}:environment:production\",
        \"description\": \"GitHub Actions OIDC for World Leaders Game production environment\",
        \"audiences\": [\"api://AzureADTokenExchange\"]
    }"

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Federated credential created successfully!${NC}"
else
    echo -e "${RED}❌ Failed to create federated credential${NC}"
    exit 1
fi

echo ""
echo -e "${GREEN}🎉 OIDC Credential Fix Complete!${NC}"
echo ""
echo -e "${YELLOW}📋 Summary:${NC}"
echo "• Service Principal: $SP_NAME"
echo "• Client ID: $CLIENT_ID"
echo "• Subject: repo:${REPO_OWNER}/${REPO_NAME}:environment:production"
echo "• Issuer: https://token.actions.githubusercontent.com"
echo ""
echo -e "${BLUE}🚀 Your GitHub Actions workflow should now authenticate successfully!${NC}"
echo ""

# Verify the credential was created
echo -e "${BLUE}🔍 Verifying federated credential...${NC}"
PROD_CRED=$(az ad app federated-credential list --id $CLIENT_ID --query "[?name=='github-production'].subject" -o tsv)

if [ "$PROD_CRED" = "repo:${REPO_OWNER}/${REPO_NAME}:environment:production" ]; then
    echo -e "${GREEN}✅ Verification successful! Credential is properly configured.${NC}"
else
    echo -e "${RED}❌ Verification failed. Credential may not be configured correctly.${NC}"
fi
