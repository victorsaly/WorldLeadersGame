#!/bin/bash

# 🌐 CloudFlare DNS Automation Script for worldleadersgame.co.uk
# Automatically configure DNS records using CloudFlare API

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${BLUE}🌐 CloudFlare DNS Setup for World Leaders Game${NC}"
echo -e "${BLUE}===============================================${NC}"
echo ""

# Configuration
DOMAIN="worldleadersgame.co.uk"
RESOURCE_GROUP="worldleaders-prod-rg"
WEB_APP_NAME="worldleaders-web-prod"
API_APP_NAME="worldleaders-api-prod"

# Get Azure targets
echo -e "${YELLOW}📋 Getting Azure App Service URLs...${NC}"
WEB_APP_URL=$(az webapp show --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME --query "defaultHostName" -o tsv)
API_APP_URL=$(az webapp show --resource-group $RESOURCE_GROUP --name $API_APP_NAME --query "defaultHostName" -o tsv)

echo -e "${GREEN}✅ Azure targets:${NC}"
echo "   Web App: $WEB_APP_URL"
echo "   API App: $API_APP_URL"
echo ""

# CloudFlare API Configuration
echo -e "${YELLOW}🔧 CloudFlare API Setup${NC}"
echo "To use this script automatically, you need:"
echo "1. CloudFlare API Token with Zone:Edit permissions"
echo "2. Zone ID for your domain"
echo ""

read -p "Do you have CloudFlare API credentials? (y/N): " has_api

if [[ $has_api =~ ^[Yy]$ ]]; then
    echo ""
    read -p "Enter your CloudFlare API Token: " CF_API_TOKEN
    read -p "Enter your Zone ID: " CF_ZONE_ID
    
    echo ""
    echo -e "${BLUE}🚀 Configuring DNS records via CloudFlare API...${NC}"
    
    # Function to create DNS record
    create_dns_record() {
        local record_type=$1
        local record_name=$2
        local record_content=$3
        local proxied=$4
        
        echo "Creating $record_type record: $record_name → $record_content"
        
        curl -s -X POST "https://api.cloudflare.com/client/v4/zones/$CF_ZONE_ID/dns_records" \
            -H "Authorization: Bearer $CF_API_TOKEN" \
            -H "Content-Type: application/json" \
            --data "{
                \"type\": \"$record_type\",
                \"name\": \"$record_name\",
                \"content\": \"$record_content\",
                \"ttl\": 1,
                \"proxied\": $proxied
            }" | jq -r '.success'
    }
    
    # Create DNS records
    echo ""
    echo "1. Creating root domain CNAME..."
    SUCCESS1=$(create_dns_record "CNAME" "$DOMAIN" "$WEB_APP_URL" "true")
    
    echo "2. Creating API subdomain CNAME..."
    SUCCESS2=$(create_dns_record "CNAME" "api.$DOMAIN" "$API_APP_URL" "true")
    
    echo "3. Creating www subdomain CNAME..."
    SUCCESS3=$(create_dns_record "CNAME" "www.$DOMAIN" "$WEB_APP_URL" "true")
    
    echo ""
    if [[ "$SUCCESS1" == "true" && "$SUCCESS2" == "true" && "$SUCCESS3" == "true" ]]; then
        echo -e "${GREEN}✅ DNS records created successfully!${NC}"
    else
        echo -e "${RED}❌ Some DNS records failed to create${NC}"
        echo "Check your API token permissions and zone ID"
    fi
    
else
    echo ""
    echo -e "${YELLOW}📋 Manual CloudFlare DNS Configuration${NC}"
    echo "Please add these records in your CloudFlare dashboard:"
    echo ""
    
    cat << EOF
╔══════════════════════════════════════════════════════════════════╗
║                     CloudFlare DNS Records                      ║
╚══════════════════════════════════════════════════════════════════╝

Record 1 - Main Domain:
   Type: CNAME
   Name: @
   Target: $WEB_APP_URL
   Proxy: ☁️ Proxied (Orange Cloud ON)
   TTL: Auto

Record 2 - API Subdomain:
   Type: CNAME
   Name: api
   Target: $API_APP_URL
   Proxy: ☁️ Proxied (Orange Cloud ON)
   TTL: Auto

Record 3 - WWW Subdomain:
   Type: CNAME
   Name: www
   Target: $WEB_APP_URL
   Proxy: ☁️ Proxied (Orange Cloud ON)
   TTL: Auto

Optional - Documentation:
   Type: CNAME
   Name: docs
   Target: victorsaly.github.io
   Proxy: ☁️ Proxied (Orange Cloud ON)
   TTL: Auto

EOF
fi

echo ""
echo -e "${BLUE}🔒 CloudFlare Security Settings${NC}"
echo "Recommended CloudFlare settings:"
echo ""
echo "SSL/TLS:"
echo "  • Encryption mode: Full (strict)"
echo "  • Always Use HTTPS: On"
echo "  • Minimum TLS Version: 1.2"
echo ""
echo "Security:"
echo "  • Security Level: Medium"
echo "  • Browser Integrity Check: On"
echo "  • Challenge Passage: 30 minutes"
echo ""
echo "Speed:"
echo "  • Auto Minify: HTML, CSS, JS"
echo "  • Brotli: On"
echo "  • Early Hints: On"
echo ""

echo -e "${YELLOW}⏰ Waiting for DNS propagation...${NC}"
echo "DNS changes typically take 5-30 minutes to propagate"
echo ""

# Function to check DNS propagation
check_dns() {
    local domain=$1
    local expected=$2
    
    echo "Checking $domain..."
    local result=$(dig +short $domain @8.8.8.8 | head -1)
    if [[ "$result" == *"$expected"* ]] || [[ "$result" != "" ]]; then
        echo -e "${GREEN}✅ $domain is resolving${NC}"
        return 0
    else
        echo -e "${RED}❌ $domain not yet resolving${NC}"
        return 1
    fi
}

# Wait and check DNS propagation
attempt=1
max_attempts=10

while [ $attempt -le $max_attempts ]; do
    echo ""
    echo "Attempt $attempt of $max_attempts..."
    
    if check_dns "$DOMAIN" "$WEB_APP_URL" && check_dns "api.$DOMAIN" "$API_APP_URL"; then
        echo ""
        echo -e "${GREEN}🎉 DNS propagation successful!${NC}"
        break
    fi
    
    if [ $attempt -eq $max_attempts ]; then
        echo ""
        echo -e "${YELLOW}⏰ DNS still propagating. This can take up to 48 hours globally.${NC}"
        echo "You can continue with Azure configuration once DNS resolves."
        break
    fi
    
    echo "Waiting 30 seconds before next check..."
    sleep 30
    ((attempt++))
done

echo ""
echo -e "${BLUE}🔧 Azure Custom Domain Setup${NC}"
echo "Once DNS is propagated, run these commands:"
echo ""
echo "# Add main domain to web app"
echo "az webapp config hostname add \\"
echo "    --resource-group $RESOURCE_GROUP \\"
echo "    --webapp-name $WEB_APP_NAME \\"
echo "    --hostname $DOMAIN"
echo ""
echo "# Add API subdomain"
echo "az webapp config hostname add \\"
echo "    --resource-group $RESOURCE_GROUP \\"
echo "    --webapp-name $API_APP_NAME \\"
echo "    --hostname api.$DOMAIN"
echo ""
echo "# Enable SSL certificates"
echo "az webapp config ssl bind \\"
echo "    --resource-group $RESOURCE_GROUP \\"
echo "    --name $WEB_APP_NAME \\"
echo "    --certificate-type Managed \\"
echo "    --hostname $DOMAIN"
echo ""
echo "az webapp config ssl bind \\"
echo "    --resource-group $RESOURCE_GROUP \\"
echo "    --name $API_APP_NAME \\"
echo "    --certificate-type Managed \\"
echo "    --hostname api.$DOMAIN"
echo ""

read -p "Would you like to run Azure configuration now? (y/N): " run_azure

if [[ $run_azure =~ ^[Yy]$ ]]; then
    echo ""
    echo -e "${BLUE}🚀 Configuring Azure App Services...${NC}"
    
    echo "Adding $DOMAIN to web app..."
    if az webapp config hostname add \
        --resource-group $RESOURCE_GROUP \
        --webapp-name $WEB_APP_NAME \
        --hostname $DOMAIN 2>/dev/null; then
        echo -e "${GREEN}✅ Main domain added${NC}"
    else
        echo -e "${RED}❌ Failed to add main domain (DNS may not be fully propagated)${NC}"
    fi
    
    echo "Adding api.$DOMAIN to API app..."
    if az webapp config hostname add \
        --resource-group $RESOURCE_GROUP \
        --webapp-name $API_APP_NAME \
        --hostname "api.$DOMAIN" 2>/dev/null; then
        echo -e "${GREEN}✅ API domain added${NC}"
    else
        echo -e "${RED}❌ Failed to add API domain (DNS may not be fully propagated)${NC}"
    fi
    
    echo ""
    echo -e "${BLUE}🔒 Enabling SSL certificates...${NC}"
    
    echo "Enabling SSL for $DOMAIN..."
    if az webapp config ssl bind \
        --resource-group $RESOURCE_GROUP \
        --name $WEB_APP_NAME \
        --certificate-type Managed \
        --hostname $DOMAIN 2>/dev/null; then
        echo -e "${GREEN}✅ SSL enabled for main domain${NC}"
    else
        echo -e "${YELLOW}⏰ SSL setup queued (may take 15-30 minutes)${NC}"
    fi
    
    echo "Enabling SSL for api.$DOMAIN..."
    if az webapp config ssl bind \
        --resource-group $RESOURCE_GROUP \
        --name $API_APP_NAME \
        --certificate-type Managed \
        --hostname "api.$DOMAIN" 2>/dev/null; then
        echo -e "${GREEN}✅ SSL enabled for API domain${NC}"
    else
        echo -e "${YELLOW}⏰ SSL setup queued (may take 15-30 minutes)${NC}"
    fi
fi

echo ""
echo -e "${GREEN}🎉 Setup Complete!${NC}"
echo "================================"
echo ""
echo "Your World Leaders Game will be available at:"
echo -e "${GREEN}🌐 Main Game: https://$DOMAIN${NC}"
echo -e "${GREEN}🔧 API: https://api.$DOMAIN${NC}"
echo -e "${GREEN}📊 Health Check: https://api.$DOMAIN/health${NC}"
echo ""
echo -e "${YELLOW}📋 Next Steps:${NC}"
echo "1. Wait 15-30 minutes for SSL certificates to activate"
echo "2. Test your site at the custom domain"
echo "3. Configure CloudFlare settings as recommended above"
echo "4. Set up monitoring and analytics"
echo ""
echo -e "${BLUE}💡 Useful Commands:${NC}"
echo "# Test DNS resolution"
echo "dig $DOMAIN"
echo "dig api.$DOMAIN"
echo ""
echo "# Test HTTPS certificates"
echo "curl -I https://$DOMAIN"
echo "curl -I https://api.$DOMAIN"
echo ""
echo "✅ CloudFlare DNS setup completed!"
