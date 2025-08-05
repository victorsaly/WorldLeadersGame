#!/bin/bash

# 🌐 Configure worldleadersgame.co.uk Domain
# Link your custom domain to the deployed Azure services

echo "🌐 Configuring worldleadersgame.co.uk domain"
echo "============================================="
echo ""

# Configuration
DOMAIN="worldleadersgame.co.uk"
RESOURCE_GROUP="worldleaders-prod-rg"
WEB_APP_NAME="worldleaders-web-prod"
API_APP_NAME="worldleaders-api-prod"

# Get current Azure App Service URLs
echo "📋 Getting current Azure URLs..."
WEB_APP_URL=$(az webapp show --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME --query "defaultHostName" -o tsv)
API_APP_URL=$(az webapp show --resource-group $RESOURCE_GROUP --name $API_APP_NAME --query "defaultHostName" -o tsv)

echo "✅ Current deployment URLs:"
echo "   Web App: https://$WEB_APP_URL"
echo "   API App: https://$API_APP_URL"
echo ""

echo "🔧 STEP 1: DNS Configuration Required"
echo "======================================"
echo "You need to add these DNS records to your domain provider:"
echo ""
echo "🌐 Main domain record:"
echo "   Type: CNAME"
echo "   Name: @ (root domain)"
echo "   Value: $WEB_APP_URL"
echo "   TTL: 3600"
echo ""
echo "🔧 API subdomain record:"
echo "   Type: CNAME"
echo "   Name: api"
echo "   Value: $API_APP_URL"
echo "   TTL: 3600"
echo ""

# Alternative A records if CNAME not supported for root domain
echo "⚠️  If your DNS provider doesn't support CNAME for root domain, use A records:"
INBOUND_IP=$(az webapp show --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME --query "inboundIpAddress" -o tsv)
echo "   Type: A"
echo "   Name: @"
echo "   Value: $INBOUND_IP"
echo "   TTL: 3600"
echo ""

read -p "Have you configured the DNS records? (y/N): " dns_configured

if [[ $dns_configured =~ ^[Yy]$ ]]; then
    echo ""
    echo "🔧 STEP 2: Adding Custom Domains to Azure"
    echo "=========================================="
    
    echo "Adding $DOMAIN to web app..."
    if az webapp config hostname add \
        --resource-group $RESOURCE_GROUP \
        --webapp-name $WEB_APP_NAME \
        --hostname $DOMAIN 2>/dev/null; then
        echo "✅ Main domain added successfully"
    else
        echo "❌ Failed to add main domain (DNS may not be propagated yet)"
    fi
    
    echo ""
    echo "Adding api.$DOMAIN to API app..."
    if az webapp config hostname add \
        --resource-group $RESOURCE_GROUP \
        --webapp-name $API_APP_NAME \
        --hostname "api.$DOMAIN" 2>/dev/null; then
        echo "✅ API subdomain added successfully"
    else
        echo "❌ Failed to add API subdomain (DNS may not be propagated yet)"
    fi
    
    echo ""
    echo "🔒 STEP 3: Enabling SSL Certificates"
    echo "===================================="
    
    echo "Enabling managed SSL for $DOMAIN..."
    if az webapp config ssl bind \
        --resource-group $RESOURCE_GROUP \
        --name $WEB_APP_NAME \
        --certificate-type Managed \
        --hostname $DOMAIN 2>/dev/null; then
        echo "✅ SSL enabled for main domain"
    else
        echo "❌ SSL setup failed (may need to wait for domain verification)"
    fi
    
    echo "Enabling managed SSL for api.$DOMAIN..."
    if az webapp config ssl bind \
        --resource-group $RESOURCE_GROUP \
        --name $API_APP_NAME \
        --certificate-type Managed \
        --hostname "api.$DOMAIN" 2>/dev/null; then
        echo "✅ SSL enabled for API subdomain"
    else
        echo "❌ SSL setup failed for API (may need to wait for domain verification)"
    fi
    
    echo ""
    echo "🎉 Domain Configuration Complete!"
    echo "================================="
    echo ""
    echo "🌐 Your World Leaders Game will be available at:"
    echo "   Main game: https://$DOMAIN"
    echo "   API: https://api.$DOMAIN"
    echo "   Health check: https://api.$DOMAIN/health"
    echo ""
    echo "⏰ Note: SSL certificates may take 15-30 minutes to activate"
    echo "💡 DNS propagation can take up to 48 hours globally"
    
else
    echo ""
    echo "📋 Please configure your DNS records first:"
    echo ""
    echo "1. Log into your domain provider (where you bought worldleadersgame.co.uk)"
    echo "2. Find DNS management / DNS settings"
    echo "3. Add these records:"
    echo ""
    echo "   CNAME: @ → $WEB_APP_URL"
    echo "   CNAME: api → $API_APP_URL"
    echo ""
    echo "4. Save changes and wait for propagation (5-60 minutes)"
    echo "5. Run this script again"
    echo ""
    echo "🔍 Test DNS propagation with:"
    echo "   dig $DOMAIN"
    echo "   dig api.$DOMAIN"
fi

echo ""
echo "🧪 Testing Current Deployment:"
echo "==============================="
echo "Azure URLs (always work):"
echo "   🌐 Web: https://$WEB_APP_URL"
echo "   🔧 API: https://$API_APP_URL"
echo ""

# Test if the sites are responding
echo "Testing web app..."
if curl -s -o /dev/null -w "%{http_code}" "https://$WEB_APP_URL" | grep -q "200\|302\|301"; then
    echo "✅ Web app is responding"
else
    echo "❌ Web app may have issues"
fi

echo "Testing API app..."
if curl -s -o /dev/null -w "%{http_code}" "https://$API_APP_URL" | grep -q "200\|302\|301"; then
    echo "✅ API app is responding"
else
    echo "❌ API app may have issues"
fi

echo ""
echo "✅ Setup script completed!"
