#!/bin/bash

# 🔐 GitHub Secrets Setup for Azure Deployment
# This script helps you configure the required secrets for GitHub Actions deployment

echo "🔐 GitHub Secrets Setup for World Leaders Game Azure Deployment"
echo "================================================================"
echo ""

# Check if GitHub CLI is available
if ! command -v gh &> /dev/null; then
    echo "❌ GitHub CLI is not installed. Please install it first:"
    echo "   brew install gh"
    echo "   Or visit: https://cli.github.com/"
    exit 1
fi

# Check if logged into GitHub CLI
if ! gh auth status &> /dev/null; then
    echo "🔐 Please log into GitHub CLI..."
    gh auth login
fi

echo "📋 Required GitHub Secrets for Azure Deployment"
echo "==============================================="
echo ""

# Function to set a GitHub secret
set_github_secret() {
    local secret_name=$1
    local secret_description=$2
    local current_value=$3
    
    echo "🔑 Setting up: $secret_name"
    echo "   Description: $secret_description"
    
    if [ -n "$current_value" ]; then
        echo "   Current value: ${current_value:0:20}..."
        read -p "   Use current value? (y/n): " use_current
        if [ "$use_current" = "y" ]; then
            gh secret set "$secret_name" --body "$current_value"
            echo "   ✅ Secret set successfully"
            return
        fi
    fi
    
    read -s -p "   Enter value for $secret_name: " secret_value
    echo ""
    
    if [ -n "$secret_value" ]; then
        gh secret set "$secret_name" --body "$secret_value"
        echo "   ✅ Secret set successfully"
    else
        echo "   ⚠️  Skipped (empty value)"
    fi
    echo ""
}

echo "1️⃣ Azure Service Principal Credentials"
echo "======================================"
echo ""
echo "First, create a service principal for GitHub Actions:"
echo "az ad sp create-for-rbac --name 'github-worldleaders-deploy' --role contributor --scopes /subscriptions/$(az account show --query id -o tsv) --sdk-auth"
echo ""
read -p "Press Enter after creating the service principal..."

set_github_secret "AZURE_CREDENTIALS" "Azure service principal JSON for authentication" ""
set_github_secret "AZURE_SUBSCRIPTION_ID" "Azure subscription ID" "69f49397-9070-41d6-8463-f95cd26ca5c8"

echo "2️⃣ Azure AI Service Secrets"
echo "==========================="
echo ""

# Get current values from local config if available
if [ -f "azure-ai-config.local.json" ]; then
    OPENAI_ENDPOINT=$(jq -r '.AzureOpenAI.Endpoint' azure-ai-config.local.json 2>/dev/null)
    OPENAI_KEY=$(jq -r '.AzureOpenAI.ApiKey' azure-ai-config.local.json 2>/dev/null)
    CONTENT_ENDPOINT=$(jq -r '.ContentModerator.Endpoint' azure-ai-config.local.json 2>/dev/null)
    CONTENT_KEY=$(jq -r '.ContentModerator.ApiKey' azure-ai-config.local.json 2>/dev/null)
    SPEECH_KEY=$(jq -r '.SpeechServices.ApiKey' azure-ai-config.local.json 2>/dev/null)
fi

set_github_secret "AZURE_OPENAI_ENDPOINT" "Azure OpenAI service endpoint" "$OPENAI_ENDPOINT"
set_github_secret "AZURE_OPENAI_API_KEY" "Azure OpenAI API key" "$OPENAI_KEY"
set_github_secret "CONTENT_MODERATOR_ENDPOINT" "Content Moderator endpoint" "$CONTENT_ENDPOINT"
set_github_secret "CONTENT_MODERATOR_API_KEY" "Content Moderator API key" "$CONTENT_KEY"
set_github_secret "SPEECH_SERVICES_API_KEY" "Speech Services API key" "$SPEECH_KEY"

echo "3️⃣ JWT Authentication Secrets"
echo "=============================="
echo ""

# Generate a secure JWT secret if not provided
JWT_SECRET=$(openssl rand -base64 64 | tr -d '\n')
set_github_secret "JWT_SECRET_KEY" "JWT signing secret (64 characters minimum)" "$JWT_SECRET"

echo "4️⃣ Azure AD B2C Secrets (Optional)"
echo "=================================="
echo ""
echo "If you've set up Azure AD B2C, configure these secrets:"
echo "If not, you can skip these and use local JWT authentication."
echo ""

set_github_secret "AZURE_B2C_TENANT_ID" "Azure AD B2C tenant ID" ""
set_github_secret "AZURE_B2C_CLIENT_ID" "Azure AD B2C client ID" ""
set_github_secret "AZURE_B2C_CLIENT_SECRET" "Azure AD B2C client secret" ""

echo "5️⃣ Custom Domain & Static Web Apps (Optional)"
echo "=============================================="
echo ""

set_github_secret "CUSTOM_DOMAIN_NAME" "Custom domain for the game (optional)" ""
set_github_secret "AZURE_STATIC_WEB_APPS_TOKEN" "Azure Static Web Apps deployment token" ""

echo "✅ GitHub Secrets Configuration Complete!"
echo "========================================"
echo ""
echo "📋 Secrets configured for:"
echo "   🔐 Azure authentication and deployment"
echo "   🧠 Azure AI services (OpenAI, Content Moderation, Speech)"
echo "   🔑 JWT authentication security"
echo "   🎯 Azure AD B2C integration (if configured)"
echo "   🌐 Custom domain and documentation deployment"
echo ""
echo "🚀 Next Steps:"
echo "1. Commit and push your changes to trigger the deployment workflow"
echo "2. Monitor the deployment at: https://github.com/$(gh repo view --json owner,name -q '.owner.login + \"/\" + .name')/actions"
echo "3. Your educational game will be deployed to Azure App Service"
echo ""
echo "🎮 Production URLs (after deployment):"
echo "   🌐 Web App: https://worldleaders-web-prod.azurewebsites.net"
echo "   🔗 API: https://worldleaders-api-prod.azurewebsites.net"
echo "   📚 Documentation: https://worldleaders-docs-prod.azurestaticapps.net"
echo ""
echo "🎓 Your educational game for 12-year-olds will be live!"
