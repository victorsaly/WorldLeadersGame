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

# Get inbound IP for A record alternative
INBOUND_IP=$(az webapp show --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME --query "inboundIpAddress" -o tsv)
echo "⚠️  If your DNS provider doesn't support CNAME for root domain, use A record:"
echo "   Type: A"
echo "   Name: @"
echo "   Value: $INBOUND_IP"
echo "   TTL: 3600"
echo ""

echo "🧪 Testing Current Deployment:"
echo "==============================="
echo ""

# Test if the sites are responding
echo "Testing web app at $WEB_APP_URL..."
HTTP_CODE=$(curl -s -o /dev/null -w "%{http_code}" "https://$WEB_APP_URL" || echo "000")
if [[ $HTTP_CODE =~ ^(200|301|302)$ ]]; then
    echo "✅ Web app is responding (HTTP $HTTP_CODE)"
else
    echo "❌ Web app may have issues (HTTP $HTTP_CODE)"
fi

echo "Testing API app at $API_APP_URL..."
HTTP_CODE=$(curl -s -o /dev/null -w "%{http_code}" "https://$API_APP_URL" || echo "000")
if [[ $HTTP_CODE =~ ^(200|301|302)$ ]]; then
    echo "✅ API app is responding (HTTP $HTTP_CODE)"
else
    echo "❌ API app may have issues (HTTP $HTTP_CODE)"
fi

echo ""
echo "🎉 Deployment Summary:"
echo "======================"
echo "✅ Web App deployed: https://$WEB_APP_URL"
echo "✅ API App deployed: https://$API_APP_URL"
echo ""
echo "📋 Next Steps for Custom Domain:"
echo "1. Go to your domain provider (where you bought worldleadersgame.co.uk)"
echo "2. Add the DNS records shown above"
echo "3. Wait for DNS propagation (5-60 minutes)"
echo "4. Then run: az webapp config hostname add --resource-group $RESOURCE_GROUP --webapp-name $WEB_APP_NAME --hostname $DOMAIN"
echo "5. And: az webapp config hostname add --resource-group $RESOURCE_GROUP --webapp-name $API_APP_NAME --hostname api.$DOMAIN"
echo ""
echo "🔒 For SSL, run after adding hostnames:"
echo "az webapp config ssl bind --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME --certificate-type Managed --hostname $DOMAIN"
echo "az webapp config ssl bind --resource-group $RESOURCE_GROUP --name $API_APP_NAME --certificate-type Managed --hostname api.$DOMAIN"
