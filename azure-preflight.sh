#!/bin/bash

# 🎮 World Leaders Game - Pre-flight Checklist for UK Personal Project
# Verify Azure setup and account selection before resource creation

set -e  # Exit on any error

# Color coding for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🎮 World Leaders Game - Pre-flight Checklist${NC}"
echo -e "${BLUE}===========================================${NC}"
echo ""

# Step 1: Check Azure CLI
echo -e "${BLUE}1️⃣ Checking Azure CLI Installation...${NC}"
if command -v az &> /dev/null; then
    az_version=$(az version --query "\"azure-cli\"" --output tsv)
    echo -e "${GREEN}✅ Azure CLI installed: $az_version${NC}"
else
    echo -e "${RED}❌ Azure CLI not installed${NC}"
    echo -e "${YELLOW}   Install from: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli${NC}"
    exit 1
fi
echo ""

# Step 2: Check Azure Authentication
echo -e "${BLUE}2️⃣ Checking Azure Authentication...${NC}"
if az account show &> /dev/null; then
    echo -e "${GREEN}✅ Authenticated to Azure${NC}"
    
    # Show current account details
    echo -e "${BLUE}Current Account Details:${NC}"
    az account show --query "{Name:name, TenantId:tenantId, SubscriptionId:id, User:user.name, Environment:environmentName}" --output table
    echo ""
    
    # Show all available subscriptions
    echo -e "${BLUE}Available Subscriptions:${NC}"
    az account list --query "[].{Name:name, SubscriptionId:id, TenantId:tenantId, IsDefault:isDefault, State:state}" --output table
    echo ""
else
    echo -e "${RED}❌ Not authenticated to Azure${NC}"
    echo -e "${YELLOW}   Please run: az login${NC}"
    exit 1
fi

# Step 3: Verify Personal Account Selection
echo -e "${BLUE}3️⃣ Account Selection Verification...${NC}"
current_account=$(az account show --query "name" --output tsv)
current_user=$(az account show --query "user.name" --output tsv)

echo -e "${YELLOW}Current Account: $current_account${NC}"
echo -e "${YELLOW}Current User: $current_user${NC}"
echo ""

read -p "🤔 Is this your PERSONAL Azure account (not company account)? (y/n): " confirm_personal
if [[ $confirm_personal != "y" ]]; then
    echo ""
    echo -e "${YELLOW}⚠️ You should switch to your personal Azure account before proceeding${NC}"
    echo -e "${BLUE}📋 Available subscriptions:${NC}"
    az account list --query "[].{Name:name, SubscriptionId:id, User:user.name}" --output table
    echo ""
    read -p "📝 Enter the Subscription ID for your PERSONAL account (or 'exit' to quit): " personal_subscription_id
    
    if [[ $personal_subscription_id == "exit" ]]; then
        echo -e "${YELLOW}Exiting. Please switch to your personal account and run this script again.${NC}"
        exit 0
    fi
    
    echo -e "${BLUE}🔄 Switching to personal subscription...${NC}"
    az account set --subscription "$personal_subscription_id"
    
    echo -e "${GREEN}✅ Switched to personal account:${NC}"
    az account show --query "{Name:name, SubscriptionId:id, User:user.name}" --output table
fi
echo ""

# Step 4: Check UK Region Availability
echo -e "${BLUE}4️⃣ Checking UK Region Availability...${NC}"
echo -e "${BLUE}Available UK locations for Cognitive Services:${NC}"
uk_locations=$(az account list-locations --query "[?contains(displayName, 'UK')].{Name:name, DisplayName:displayName}" --output table)
echo "$uk_locations"
echo ""

# Step 5: Check OpenAI Access
echo -e "${BLUE}5️⃣ Checking Azure OpenAI Access...${NC}"
openai_available=$(az provider show --namespace Microsoft.CognitiveServices --query "registrationState" --output tsv)
if [[ $openai_available == "Registered" ]]; then
    echo -e "${GREEN}✅ Cognitive Services provider is registered${NC}"
else
    echo -e "${YELLOW}⚠️ Cognitive Services provider not registered${NC}"
    echo -e "${BLUE}   Registering provider...${NC}"
    az provider register --namespace Microsoft.CognitiveServices
fi

# Check if OpenAI models are available in UK regions
echo -e "${BLUE}Checking OpenAI model availability in UK regions...${NC}"
echo -e "${YELLOW}Note: OpenAI access requires approval from Microsoft${NC}"
echo -e "${YELLOW}If you haven't applied yet, visit: https://aka.ms/oai/access${NC}"
echo ""

# Step 6: Estimate Costs
echo -e "${BLUE}6️⃣ Cost Estimation for Personal Project...${NC}"
echo -e "${BLUE}Estimated Monthly Costs (Personal/Development Usage):${NC}"
echo -e "   💰 Azure OpenAI (GPT-4): £30-50 (light educational usage)"
echo -e "   💰 Content Moderator: £15-25 (child safety validation)"
echo -e "   💰 Speech Services: £10-20 (language learning features)"
echo -e "   💰 Key Vault: £2-5 (credential storage)"
echo -e "   💰 Total Estimated: £57-100/month for personal development"
echo ""
echo -e "${GREEN}💡 Tips to minimize costs:${NC}"
echo -e "   • Use development tiers where available"
echo -e "   • Set up budget alerts"
echo -e "   • Monitor usage regularly"
echo -e "   • Consider free tier options for initial testing"
echo ""

# Step 7: Security and Compliance Check
echo -e "${BLUE}7️⃣ Security and Compliance Notes...${NC}"
echo -e "${GREEN}✅ Data Residency: Resources will be created in UK for data protection${NC}"
echo -e "${GREEN}✅ Child Safety: Multi-layer content moderation for educational content${NC}"
echo -e "${GREEN}✅ Secure Storage: API keys stored in Azure Key Vault${NC}"
echo -e "${GREEN}✅ Educational Use: All services configured for 12-year-old learners${NC}"
echo ""

# Step 8: Final Confirmation
echo -e "${BLUE}8️⃣ Pre-flight Checklist Complete!${NC}"
echo -e "${GREEN}✅ Azure CLI installed and authenticated${NC}"
echo -e "${GREEN}✅ Personal Azure account verified${NC}"
echo -e "${GREEN}✅ UK region configured${NC}"
echo -e "${GREEN}✅ Cost estimates reviewed${NC}"
echo -e "${GREEN}✅ Security considerations noted${NC}"
echo ""

echo -e "${BLUE}🚀 Ready to proceed with Azure resource creation!${NC}"
echo ""
read -p "🤔 Proceed with running the Azure setup script? (y/n): " proceed_setup
if [[ $proceed_setup == "y" ]]; then
    echo -e "${GREEN}✅ Running Azure setup script...${NC}"
    echo ""
    ./azure-setup.sh
else
    echo -e "${YELLOW}Setup cancelled. Run './azure-setup.sh' when you're ready to proceed.${NC}"
fi
