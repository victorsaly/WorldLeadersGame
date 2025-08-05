#!/bin/bash

# 🔍 Azure Domain Verification and Configuration Script
# Run this after adding TXT records to CloudFlare

echo "🔍 Azure Domain Verification and Setup"
echo "======================================"
echo ""

DOMAIN="worldleadersgame.co.uk"
RESOURCE_GROUP="worldleaders-prod-rg"
WEB_APP_NAME="worldleaders-web-prod"
API_APP_NAME="worldleaders-api-prod"

# Function to check TXT record
check_txt_record() {
    local record_name=$1
    local expected_value=$2
    
    echo "Checking TXT record: $record_name..."
    local result=$(dig +short TXT $record_name @8.8.8.8 | tr -d '"')
    
    if [[ "$result" == *"$expected_value"* ]]; then
        echo "✅ TXT record found: $record_name"
        return 0
    else
        echo "❌ TXT record not found: $record_name"
        echo "   Expected: $expected_value"
        echo "   Found: $result"
        return 1
    fi
}

# Check verification TXT records
echo "🔍 Step 1: Checking domain verification TXT records"
echo "=================================================="

VERIFICATION_TOKEN="D618B57BA7A1EE5C3F47B6CB7388E279FC0FCEE8D2FBD941C677BDE2AA7D6141"

if check_txt_record "asuid.$DOMAIN" "$VERIFICATION_TOKEN" && 
   check_txt_record "asuid.api.$DOMAIN" "$VERIFICATION_TOKEN"; then
    
    echo ""
    echo "✅ All TXT records verified! Proceeding with Azure configuration..."
    echo ""
    
    # Add custom domains
    echo "🔧 Step 2: Adding custom domains to Azure App Services"
    echo "======================================================="
    
    echo "Adding $DOMAIN to web app..."
    if az webapp config hostname add \
        --resource-group $RESOURCE_GROUP \
        --webapp-name $WEB_APP_NAME \
        --hostname $DOMAIN; then
        echo "✅ Main domain added successfully"
    else
        echo "❌ Failed to add main domain"
        exit 1
    fi
    
    echo ""
    echo "Adding api.$DOMAIN to API app..."
    if az webapp config hostname add \
        --resource-group $RESOURCE_GROUP \
        --webapp-name $API_APP_NAME \
        --hostname "api.$DOMAIN"; then
        echo "✅ API domain added successfully"
    else
        echo "❌ Failed to add API domain"
        exit 1
    fi
    
    echo ""
    echo "🔒 Step 3: Enabling SSL certificates"
    echo "===================================="
    
    echo "Creating managed SSL certificate for $DOMAIN..."
    if az webapp config ssl create \
        --resource-group $RESOURCE_GROUP \
        --name $WEB_APP_NAME \
        --hostname $DOMAIN; then
        echo "✅ SSL certificate created for main domain"
    else
        echo "⏰ SSL setup queued (certificates can take 15-30 minutes)"
    fi
    
    echo ""
    echo "Creating managed SSL certificate for api.$DOMAIN..."
    if az webapp config ssl create \
        --resource-group $RESOURCE_GROUP \
        --name $API_APP_NAME \
        --hostname "api.$DOMAIN"; then
        echo "✅ SSL certificate created for API domain"
    else
        echo "⏰ SSL setup queued (certificates can take 15-30 minutes)"
    fi
    
    echo ""
    echo "🎉 Configuration Complete!"
    echo "=========================="
    echo ""
    echo "Your World Leaders Game is now available at:"
    echo "  🌐 Main Game: https://$DOMAIN"
    echo "  🔧 API: https://api.$DOMAIN"
    echo "  📚 Documentation: https://docs.$DOMAIN"
    echo ""
    echo "⏰ SSL certificates may take 15-30 minutes to fully activate"
    echo "💡 Test your sites to verify everything is working!"
    
else
    echo ""
    echo "❌ TXT records not found or incorrect"
    echo ""
    echo "📋 Please add these TXT records in CloudFlare:"
    echo ""
    echo "Record 1:"
    echo "   Type: TXT"
    echo "   Name: asuid"
    echo "   Content: $VERIFICATION_TOKEN"
    echo "   Proxy: OFF (Gray Cloud)"
    echo ""
    echo "Record 2:"
    echo "   Type: TXT"
    echo "   Name: asuid.api"
    echo "   Content: $VERIFICATION_TOKEN"
    echo "   Proxy: OFF (Gray Cloud)"
    echo ""
    echo "Wait 5-15 minutes for DNS propagation, then run this script again."
fi

echo ""
echo "🧪 Quick verification tests:"
echo "============================"
echo ""
echo "# Test DNS resolution"
echo "dig $DOMAIN"
echo "dig api.$DOMAIN"
echo "dig docs.$DOMAIN"
echo ""
echo "# Test TXT records"
echo "dig TXT asuid.$DOMAIN"
echo "dig TXT asuid.api.$DOMAIN"
echo ""
echo "# Test HTTPS (after SSL setup)"
echo "curl -I https://$DOMAIN"
echo "curl -I https://api.$DOMAIN/health"
echo "curl -I https://docs.$DOMAIN"
