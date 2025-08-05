#!/bin/bash

# 🔍 Quick Azure Domain Test Script
# Test TXT records and try domain addition step by step

echo "🔍 Testing Azure Domain Setup"
echo "============================="
echo ""

DOMAIN="worldleadersgame.co.uk"
RESOURCE_GROUP="worldleaders-prod-rg"
WEB_APP_NAME="worldleaders-web-prod"
API_APP_NAME="worldleaders-api-prod"
VERIFICATION_TOKEN="D618B57BA7A1EE5C3F47B6CB7388E279FC0FCEE8D2FBD941C677BDE2AA7D6141"

echo "🔍 Step 1: Checking TXT records"
echo "==============================="

echo "Checking asuid.$DOMAIN..."
TXT1=$(dig +short TXT asuid.$DOMAIN @8.8.8.8 | tr -d '"')
if [[ "$TXT1" == *"$VERIFICATION_TOKEN"* ]]; then
    echo "✅ Main domain TXT record found"
else
    echo "❌ Main domain TXT record NOT found"
    echo "   Expected: $VERIFICATION_TOKEN"
    echo "   Found: $TXT1"
fi

echo ""
echo "Checking asuid.api.$DOMAIN..."
TXT2=$(dig +short TXT asuid.api.$DOMAIN @8.8.8.8 | tr -d '"')
if [[ "$TXT2" == *"$VERIFICATION_TOKEN"* ]]; then
    echo "✅ API domain TXT record found"
else
    echo "❌ API domain TXT record NOT found"
    echo "   Expected: $VERIFICATION_TOKEN"
    echo "   Found: $TXT2"
fi

echo ""
read -p "Do the TXT records look correct? (y/N): " continue_setup

if [[ $continue_setup =~ ^[Yy]$ ]]; then
    echo ""
    echo "🔧 Step 2: Adding custom domains"
    echo "================================="
    
    echo "Adding $DOMAIN to web app..."
    az webapp config hostname add \
        --resource-group $RESOURCE_GROUP \
        --webapp-name $WEB_APP_NAME \
        --hostname $DOMAIN
    
    echo ""
    echo "Adding api.$DOMAIN to API app..."
    az webapp config hostname add \
        --resource-group $RESOURCE_GROUP \
        --webapp-name $API_APP_NAME \
        --hostname "api.$DOMAIN"
    
    echo ""
    echo "🔒 Step 3: Creating SSL certificates"
    echo "===================================="
    
    echo "Creating SSL for $DOMAIN..."
    az webapp config ssl create \
        --resource-group $RESOURCE_GROUP \
        --name $WEB_APP_NAME \
        --hostname $DOMAIN
    
    echo ""
    echo "Creating SSL for api.$DOMAIN..."
    az webapp config ssl create \
        --resource-group $RESOURCE_GROUP \
        --name $API_APP_NAME \
        --hostname "api.$DOMAIN"
    
    echo ""
    echo "🎉 Setup completed!"
    echo "=================="
    echo ""
    echo "Your sites should be available at:"
    echo "  🌐 Main Game: https://$DOMAIN"
    echo "  🔧 API: https://api.$DOMAIN"
    echo "  📚 Documentation: https://docs.$DOMAIN"
    echo ""
    echo "⏰ SSL certificates may take 15-30 minutes to activate"
    
else
    echo ""
    echo "📋 Please add these TXT records in CloudFlare first:"
    echo ""
    echo "Record 1:"
    echo "   Type: TXT"
    echo "   Name: asuid"
    echo "   Content: $VERIFICATION_TOKEN"
    echo "   Proxy: OFF"
    echo ""
    echo "Record 2:"
    echo "   Type: TXT"
    echo "   Name: asuid.api"
    echo "   Content: $VERIFICATION_TOKEN"
    echo "   Proxy: OFF"
    echo ""
    echo "Wait 5-15 minutes, then run this script again."
fi
