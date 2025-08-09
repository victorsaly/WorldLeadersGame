#!/bin/bash

# World Leaders Game - Azure Custom Domain & Routing Setup
# Context: Configure custom domain with path-based routing
# Routes: domain/ → Web App, domain/api → API, domain/docs → Static Web App

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
RESOURCE_GROUP="${PROJECT_NAME}-${ENVIRONMENT}-rg"

echo -e "${BLUE}🌐 World Leaders Game - Custom Domain Setup${NC}"
echo -e "${BLUE}===========================================${NC}"
echo ""

# Get user input for domain
if [ -z "$1" ]; then
    echo -e "${YELLOW}Please provide your custom domain name${NC}"
    echo "Example: worldleadersgame.co.uk"
    read -p "Enter your domain: " CUSTOM_DOMAIN
else
    CUSTOM_DOMAIN=$1
fi

if [ -z "$CUSTOM_DOMAIN" ]; then
    echo -e "${RED}❌ Domain name is required${NC}"
    exit 1
fi

echo -e "${GREEN}🌍 Setting up routing for: ${CUSTOM_DOMAIN}${NC}"
echo ""

# Check if logged in to Azure
if ! az account show &> /dev/null; then
    echo -e "${YELLOW}🔐 Please log in to Azure...${NC}"
    az login
fi

# Get existing resource information
echo -e "${YELLOW}🔍 Getting existing Azure resources...${NC}"

WEB_APP_NAME="${PROJECT_NAME}-web-${ENVIRONMENT}"
API_APP_NAME="${PROJECT_NAME}-api-${ENVIRONMENT}"
STATIC_SITE_NAME="${PROJECT_NAME}-docs-${ENVIRONMENT}"

# Check if resources exist
if ! az webapp show --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME &> /dev/null; then
    echo -e "${RED}❌ Web App not found. Please run deployment first.${NC}"
    exit 1
fi

if ! az webapp show --resource-group $RESOURCE_GROUP --name $API_APP_NAME &> /dev/null; then
    echo -e "${RED}❌ API App not found. Please run deployment first.${NC}"
    exit 1
fi

# Get App Service default hostnames
WEB_APP_HOSTNAME=$(az webapp show --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME --query 'defaultHostName' -o tsv)
API_APP_HOSTNAME=$(az webapp show --resource-group $RESOURCE_GROUP --name $API_APP_NAME --query 'defaultHostName' -o tsv)

echo -e "${GREEN}✅ Found existing resources:${NC}"
echo -e "   Web App: ${WEB_APP_HOSTNAME}"
echo -e "   API App: ${API_APP_HOSTNAME}"
echo ""

# Offer deployment options
echo -e "${YELLOW}🚀 Choose deployment approach:${NC}"
echo "1. Simple CNAME Setup (Manual DNS configuration)"
echo "2. Application Gateway with Path Routing (Advanced)"
echo "3. Both (Recommended for production)"
echo ""
read -p "Choose option (1-3): " DEPLOYMENT_OPTION

case $DEPLOYMENT_OPTION in
    1)
        echo -e "${BLUE}📋 Simple CNAME Setup Instructions${NC}"
        echo -e "${BLUE}===================================${NC}"
        echo ""
        echo -e "${YELLOW}DNS Configuration Required:${NC}"
        echo ""
        echo -e "${GREEN}1. Main Game (Root Domain):${NC}"
        echo -e "   Add CNAME record: ${CUSTOM_DOMAIN} → ${WEB_APP_HOSTNAME}"
        echo ""
        echo -e "${GREEN}2. API (Subdomain Approach):${NC}"
        echo -e "   Add CNAME record: api.${CUSTOM_DOMAIN} → ${API_APP_HOSTNAME}"
        echo ""
        echo -e "${GREEN}3. Documentation:${NC}"
        echo -e "   Configure Static Web App with custom domain for docs.${CUSTOM_DOMAIN}"
        echo ""
        echo -e "${YELLOW}After DNS configuration, run:${NC}"
        echo -e "   az webapp config hostname add --resource-group ${RESOURCE_GROUP} --webapp-name ${WEB_APP_NAME} --hostname ${CUSTOM_DOMAIN}"
        echo -e "   az webapp config hostname add --resource-group ${RESOURCE_GROUP} --webapp-name ${API_APP_NAME} --hostname api.${CUSTOM_DOMAIN}"
        ;;
        
    2)
        echo -e "${YELLOW}🏗️ Deploying Application Gateway...${NC}"
        
        # Deploy Application Gateway
        az deployment group create \
            --resource-group $RESOURCE_GROUP \
            --template-file azure-appgateway.bicep \
            --parameters projectName=$PROJECT_NAME \
                        environment=$ENVIRONMENT \
                        customDomainName=$CUSTOM_DOMAIN \
                        webAppBackendFqdn=$WEB_APP_HOSTNAME \
                        apiAppBackendFqdn=$API_APP_HOSTNAME \
                        docsBackendFqdn="replace-with-static-web-app-hostname"
                        
        # Get Application Gateway public IP
        APP_GW_IP=$(az network public-ip show \
            --resource-group $RESOURCE_GROUP \
            --name "${PROJECT_NAME}-${ENVIRONMENT}-pip" \
            --query 'ipAddress' -o tsv)
            
        echo -e "${GREEN}✅ Application Gateway deployed!${NC}"
        echo ""
        echo -e "${YELLOW}DNS Configuration:${NC}"
        echo -e "   Add A record: ${CUSTOM_DOMAIN} → ${APP_GW_IP}"
        echo ""
        echo -e "${GREEN}Test URLs after DNS propagation:${NC}"
        echo -e "   Game: http://${CUSTOM_DOMAIN}"
        echo -e "   API: http://${CUSTOM_DOMAIN}/api/health"
        echo -e "   Docs: http://${CUSTOM_DOMAIN}/docs"
        ;;
        
    3)
        echo -e "${YELLOW}🚀 Setting up comprehensive solution...${NC}"
        
        # Deploy Application Gateway first
        echo -e "${YELLOW}1. Deploying Application Gateway...${NC}"
        az deployment group create \
            --resource-group $RESOURCE_GROUP \
            --template-file azure-appgateway.bicep \
            --parameters projectName=$PROJECT_NAME \
                        environment=$ENVIRONMENT \
                        customDomainName=$CUSTOM_DOMAIN \
                        webAppBackendFqdn=$WEB_APP_HOSTNAME \
                        apiAppBackendFqdn=$API_APP_HOSTNAME \
                        docsBackendFqdn="replace-with-static-web-app-hostname"
        
        # Get Application Gateway public IP
        APP_GW_IP=$(az network public-ip show \
            --resource-group $RESOURCE_GROUP \
            --name "${PROJECT_NAME}-${ENVIRONMENT}-pip" \
            --query 'ipAddress' -o tsv)
        
        echo -e "${YELLOW}2. Adding custom hostnames to App Services...${NC}"
        
        # Add custom hostname to Web App
        az webapp config hostname add \
            --resource-group $RESOURCE_GROUP \
            --webapp-name $WEB_APP_NAME \
            --hostname $CUSTOM_DOMAIN || echo "Hostname may already exist"
            
        echo -e "${GREEN}✅ Comprehensive setup complete!${NC}"
        echo ""
        echo -e "${YELLOW}DNS Configuration Required:${NC}"
        echo -e "   Add A record: ${CUSTOM_DOMAIN} → ${APP_GW_IP}"
        echo ""
        echo -e "${GREEN}URLs after DNS propagation:${NC}"
        echo -e "   🎮 Game: https://${CUSTOM_DOMAIN}"
        echo -e "   🔧 API: https://${CUSTOM_DOMAIN}/api"
        echo -e "   📚 Docs: https://${CUSTOM_DOMAIN}/docs"
        echo ""
        echo -e "${YELLOW}SSL Certificate Setup:${NC}"
        echo -e "   The Application Gateway will need SSL certificates for HTTPS"
        echo -e "   Consider using Azure Certificate Manager or Let's Encrypt"
        ;;
        
    *)
        echo -e "${RED}❌ Invalid option${NC}"
        exit 1
        ;;
esac

echo ""
echo -e "${BLUE}🔒 Security Recommendations:${NC}"
echo -e "   1. Enable SSL/TLS certificates"
echo -e "   2. Configure proper CORS policies"
echo -e "   3. Set up monitoring and alerts"
echo -e "   4. Enable Application Insights"
echo ""
echo -e "${GREEN}🎊 Custom domain setup initiated!${NC}"
echo -e "${YELLOW}⏰ DNS propagation may take 24-48 hours${NC}"
echo ""

# Create a summary file
SUMMARY_FILE="domain-setup-summary.md"
cat > $SUMMARY_FILE << EOF
# World Leaders Game - Domain Setup Summary

## Configuration Applied
- **Domain**: ${CUSTOM_DOMAIN}
- **Deployment Option**: ${DEPLOYMENT_OPTION}
- **Date**: $(date)

## Resource Information
- **Web App**: ${WEB_APP_HOSTNAME}
- **API App**: ${API_APP_HOSTNAME}
- **Resource Group**: ${RESOURCE_GROUP}

## DNS Configuration Required
EOF

case $DEPLOYMENT_OPTION in
    1)
        cat >> $SUMMARY_FILE << EOF

### Simple CNAME Setup
- ${CUSTOM_DOMAIN} → ${WEB_APP_HOSTNAME}
- api.${CUSTOM_DOMAIN} → ${API_APP_HOSTNAME}
- docs.${CUSTOM_DOMAIN} → [Static Web App hostname]
EOF
        ;;
    2|3)
        cat >> $SUMMARY_FILE << EOF

### Application Gateway Setup
- ${CUSTOM_DOMAIN} → ${APP_GW_IP} (A record)

### Test URLs
- Game: http://${CUSTOM_DOMAIN}
- API: http://${CUSTOM_DOMAIN}/api/health
- Docs: http://${CUSTOM_DOMAIN}/docs
EOF
        ;;
esac

cat >> $SUMMARY_FILE << EOF

## Next Steps
1. Configure DNS records with your domain provider
2. Wait for DNS propagation (24-48 hours)
3. Test all endpoints
4. Configure SSL certificates
5. Update application configuration if needed

## Educational Game Features
- 🎯 Geography learning through territory acquisition
- 💰 Economics understanding via GDP-based pricing
- 🗣️ Language learning with pronunciation practice
- 🤖 AI-assisted learning with child-safe content

---
Generated by World Leaders Game deployment script
EOF

echo -e "${GREEN}📄 Summary saved to: ${SUMMARY_FILE}${NC}"
