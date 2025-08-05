#!/bin/bash

echo "🔍 Checking DNS propagation for worldleadersgame.co.uk..."
echo ""

DOMAIN="worldleadersgame.co.uk"
RESOURCE_GROUP="worldleaders-prod-rg"
WEB_APP_NAME="worldleaders-web-prod"
API_APP_NAME="worldleaders-api-prod"

# Function to check DNS
check_dns() {
    local domain=$1
    echo "Checking $domain..."
    local result=$(dig +short $domain @8.8.8.8 | head -1)
    if [ ! -z "$result" ]; then
        echo "✅ $domain is resolving to: $result"
        return 0
    else
        echo "❌ $domain not yet resolving"
        return 1
    fi
}

# Check DNS propagation
if check_dns "$DOMAIN" && check_dns "api.$DOMAIN" && check_dns "docs.$DOMAIN"; then
    echo ""
    echo "🎉 DNS is propagated! Configuring Azure..."
    echo ""
    
    # Add main domain
    echo "Adding $DOMAIN to web app..."
    if az webapp config hostname add \
        --resource-group $RESOURCE_GROUP \
        --webapp-name $WEB_APP_NAME \
        --hostname $DOMAIN; then
        echo "✅ Main domain added successfully"
    else
        echo "❌ Failed to add main domain"
    fi
    
    # Add API domain
    echo "Adding api.$DOMAIN to API app..."
    if az webapp config hostname add \
        --resource-group $RESOURCE_GROUP \
        --webapp-name $API_APP_NAME \
        --hostname "api.$DOMAIN"; then
        echo "✅ API domain added successfully"
    else
        echo "❌ Failed to add API domain"
    fi
    
    echo ""
    echo "🔒 Enabling SSL certificates..."
    
    # Enable SSL for main domain
    echo "Enabling SSL for $DOMAIN..."
    az webapp config ssl bind \
        --resource-group $RESOURCE_GROUP \
        --name $WEB_APP_NAME \
        --certificate-type Managed \
        --hostname $DOMAIN
    
    # Enable SSL for API domain
    echo "Enabling SSL for api.$DOMAIN..."
    az webapp config ssl bind \
        --resource-group $RESOURCE_GROUP \
        --name $API_APP_NAME \
        --certificate-type Managed \
        --hostname "api.$DOMAIN"
    
    echo ""
    echo "🎉 Configuration complete!"
    echo "Your sites will be available at:"
    echo "  🌐 Main Game: https://$DOMAIN"
    echo "  🔧 API: https://api.$DOMAIN"
    echo "  📚 Documentation: https://docs.$DOMAIN"
    echo ""
    echo "⏰ SSL certificates may take 15-30 minutes to activate"
    
else
    echo ""
    echo "⏰ DNS not yet propagated. Please:"
    echo "1. Add the DNS records in CloudFlare (if not done)"
    echo "2. Wait 5-30 minutes for propagation"
    echo "3. Run this script again: ./check-and-configure.sh"
fi
