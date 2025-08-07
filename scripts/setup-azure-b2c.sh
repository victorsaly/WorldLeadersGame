#!/bin/bash

# 🎓 Azure AD B2C Setup Guide for World Leaders Educational Game
# This script guides you through creating an Azure AD B2C tenant for child-safe authentication

echo "🎓 Azure AD B2C Setup for World Leaders Educational Game"
echo "========================================================"
echo ""
echo "📋 Pre-requisites Checklist:"
echo "✅ Azure subscription: VSE - Victor Saly"
echo "✅ Azure OpenAI: worldleaders-openai-uk"
echo "✅ Content Moderator: worldleaders-contentmod-uk"
echo "✅ Speech Services: worldleaders-speech-uk"
echo ""

echo "🔐 Step 1: Create Azure AD B2C Tenant"
echo "====================================="
echo ""
echo "1. Open Azure Portal: https://portal.azure.com"
echo "2. Click 'Create a resource'"
echo "3. Search for 'Azure Active Directory B2C'"
echo "4. Click 'Create'"
echo "5. Select 'Create a new Azure AD B2C Tenant'"
echo ""
echo "📝 Tenant Configuration:"
echo "   Organization name: World Leaders Educational"
echo "   Initial domain name: worldleadersedu"
echo "   Country/Region: United Kingdom"
echo "   Subscription: VSE - Victor Saly"
echo "   Resource group: rg-worldleaders-personal"
echo "   Location: UK South"
echo ""

echo "🎯 Step 2: Configure Educational Policies"
echo "========================================="
echo ""
echo "After tenant creation, you'll need to create user flows:"
echo ""
echo "1. Sign-up and sign-in policy (B2C_1_susi_educational):"
echo "   - Age verification (13+ required)"
echo "   - Parental consent collection"
echo "   - COPPA compliance fields"
echo "   - School/teacher email validation"
echo ""
echo "2. Password reset policy (B2C_1_pwd_reset_educational):"
echo "   - Enhanced security for children"
echo "   - Parental notification"
echo ""
echo "3. Profile editing policy (B2C_1_profile_edit_educational):"
echo "   - Limited profile changes"
echo "   - Parental approval for sensitive changes"
echo ""

echo "🔑 Step 3: Register Application"
echo "==============================="
echo ""
echo "1. In your B2C tenant, go to 'App registrations'"
echo "2. Click 'New registration'"
echo "3. Application details:"
echo "   Name: World Leaders Game API"
echo "   Supported account types: Accounts in this organizational directory only"
echo "   Redirect URI: https://localhost:7155/auth/callback"
echo ""
echo "4. After creation, note down:"
echo "   - Application (client) ID"
echo "   - Directory (tenant) ID"
echo ""
echo "5. Create client secret:"
echo "   - Go to 'Certificates & secrets'"
echo "   - Create new client secret"
echo "   - Note down the secret value"
echo ""

echo "📋 Step 4: Update Configuration"
echo "==============================="
echo ""
echo "Update your appsettings.json with the B2C values:"
echo ""
echo '{
  "AzureAdB2C": {
    "TenantId": "your-actual-tenant-id",
    "ClientId": "your-actual-client-id",
    "ClientSecret": "your-actual-client-secret",
    "Instance": "https://worldleadersedu.b2clogin.com",
    "Domain": "worldleadersedu.onmicrosoft.com",
    "SignUpSignInPolicyId": "B2C_1_susi_educational",
    "ResetPasswordPolicyId": "B2C_1_pwd_reset_educational",
    "EditProfilePolicyId": "B2C_1_profile_edit_educational",
    "Region": "UK South",
    "Enabled": true,
    "ApiScopes": ["https://worldleadersedu.onmicrosoft.com/api/access"]
  }
}'
echo ""

echo "🧪 Step 5: Test Authentication"
echo "=============================="
echo ""
echo "After configuration:"
echo "1. Start your application: dotnet run --project src/WorldLeaders/WorldLeaders.API"
echo "2. Test endpoints:"
echo "   - Registration: POST https://localhost:7155/api/auth/register"
echo "   - Login: POST https://localhost:7155/api/auth/login"
echo "   - Token validation: GET https://localhost:7155/api/auth/me"
echo ""

echo "🎓 Educational Compliance Features"
echo "=================================="
echo ""
echo "Your B2C setup will include:"
echo "✅ COPPA compliance (13+ age verification)"
echo "✅ Parental consent collection"
echo "✅ UK GDPR compliance"
echo "✅ Educational data residency (UK South)"
echo "✅ Child-safe authentication flows"
echo "✅ Session timeout controls (30 min for children)"
echo "✅ Cost tracking (£0.08/user/day limit)"
echo ""

echo "🔗 Useful Links"
echo "==============="
echo ""
echo "Azure Portal: https://portal.azure.com"
echo "B2C Documentation: https://docs.microsoft.com/en-us/azure/active-directory-b2c/"
echo "COPPA Compliance: https://docs.microsoft.com/en-us/azure/active-directory-b2c/age-gating"
echo ""

echo "💡 Next Steps"
echo "============="
echo ""
echo "1. Follow the manual steps above to create your B2C tenant"
echo "2. Update appsettings.json with real B2C values"
echo "3. Test the authentication system"
echo "4. Deploy to Azure App Service when ready"
echo ""

echo "🎮 Your educational game will then have:"
echo "   🔐 Enterprise-grade authentication"
echo "   👶 Child safety compliance"
echo "   🇬🇧 UK data residency"
echo "   🧠 AI-powered learning experiences"
echo "   📊 Cost tracking and controls"
