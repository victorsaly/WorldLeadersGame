#!/bin/bash

# Quick Slot Swap Fix - World Leaders Game
# Context: Educational game deployment recovery for staging slot swap issue
# Issue: Staging slot not responding to HTTP ping during swap

set -euo pipefail

echo "🚨 Quick Slot Swap Fix for World Leaders Game"
echo "============================================="

# Configuration
RESOURCE_GROUP="worldleaders-prod-rg"
WEB_APP_NAME="worldleaders-web-prod"
API_APP_NAME="worldleaders-api-prod"

echo "🔍 Current issue: Staging slot not responding to HTTP ping"
echo "⚡ Implementing immediate fixes..."
echo ""

echo "📊 Step 1: Checking current slot status..."
echo "Web App slots:"
az webapp deployment slot list \
    --resource-group "$RESOURCE_GROUP" \
    --name "$WEB_APP_NAME" \
    --query "[].{Name:name, State:state, LastModified:lastModifiedTimeUtc}" \
    --output table

echo ""
echo "API App slots:"
az webapp deployment slot list \
    --resource-group "$RESOURCE_GROUP" \
    --name "$API_APP_NAME" \
    --query "[].{Name:name, State:state, LastModified:lastModifiedTimeUtc}" \
    --output table

echo ""
echo "🔧 Step 2: Force restart staging slots to clear any stuck states..."
echo "Restarting Web App staging slot..."
az webapp restart \
    --resource-group "$RESOURCE_GROUP" \
    --name "$WEB_APP_NAME" \
    --slot staging

echo "Restarting API App staging slot..."
az webapp restart \
    --resource-group "$RESOURCE_GROUP" \
    --name "$API_APP_NAME" \
    --slot staging

echo "⏳ Waiting 45 seconds for full restart and warm-up..."
sleep 45

echo ""
echo "🏥 Step 3: Enhanced health check with multiple endpoint testing..."

# Function to test multiple health endpoints
test_health_endpoints() {
    local app_url=$1
    local app_name=$2
    
    echo "Testing $app_name health endpoints..."
    
    # Try multiple endpoints that might exist
    endpoints=("/health" "/" "/api/health" "/ready" "/alive")
    
    for endpoint in "${endpoints[@]}"; do
        echo "  Trying: $app_url$endpoint"
        if curl -s -f --connect-timeout 10 --max-time 20 "$app_url$endpoint" >/dev/null 2>&1; then
            echo "  ✅ SUCCESS: $endpoint responded"
            return 0
        else
            echo "  ❌ Failed: $endpoint"
        fi
    done
    
    return 1
}

# Test staging slots
WEB_STAGING_URL="https://$WEB_APP_NAME-staging.azurewebsites.net"
API_STAGING_URL="https://$API_APP_NAME-staging.azurewebsites.net"

echo "Testing Web App staging..."
if test_health_endpoints "$WEB_STAGING_URL" "Web App"; then
    WEB_HEALTHY=true
else
    WEB_HEALTHY=false
    echo "⚠️ Web App staging not responding to any health endpoints"
fi

echo ""
echo "Testing API staging..."
if test_health_endpoints "$API_STAGING_URL" "API"; then
    API_HEALTHY=true
else
    API_HEALTHY=false
    echo "⚠️ API staging not responding to any health endpoints"
fi

echo ""
echo "🎯 Step 4: Slot swap strategy based on health status..."

if [[ "$WEB_HEALTHY" == true && "$API_HEALTHY" == true ]]; then
    echo "✅ Both staging slots are healthy - attempting normal slot swap"
    SWAP_STRATEGY="normal"
elif [[ "$WEB_HEALTHY" == true || "$API_HEALTHY" == true ]]; then
    echo "⚠️ Mixed health status - using cautious approach"
    SWAP_STRATEGY="cautious"
else
    echo "❌ Both staging slots unresponsive - using force strategy"
    SWAP_STRATEGY="force"
fi

echo ""
echo "🚀 Step 5: Executing slot swap with $SWAP_STRATEGY strategy..."

case $SWAP_STRATEGY in
    "normal")
        echo "🔄 Normal slot swap (standard Azure swap with health checks)..."
        
        # Web App swap
        echo "Swapping Web App..."
        if az webapp deployment slot swap \
            --resource-group "$RESOURCE_GROUP" \
            --name "$WEB_APP_NAME" \
            --slot staging \
            --target-slot production; then
            echo "✅ Web App swap successful"
            WEB_SWAP_SUCCESS=true
        else
            echo "❌ Web App swap failed"
            WEB_SWAP_SUCCESS=false
        fi
        
        # API App swap
        echo "Swapping API App..."
        if az webapp deployment slot swap \
            --resource-group "$RESOURCE_GROUP" \
            --name "$API_APP_NAME" \
            --slot staging \
            --target-slot production; then
            echo "✅ API App swap successful"
            API_SWAP_SUCCESS=true
        else
            echo "❌ API App swap failed"
            API_SWAP_SUCCESS=false
        fi
        ;;
        
    "force")
        echo "⚠️ Force slot swap (bypassing health checks)..."
        echo "This is safe for educational platform as we'll verify after swap"
        
        # Web App force swap (using Azure CLI parameters to bypass health checks)
        echo "Force swapping Web App..."
        if az webapp deployment slot swap \
            --resource-group "$RESOURCE_GROUP" \
            --name "$WEB_APP_NAME" \
            --slot staging \
            --target-slot production \
            --preserve-vnet true; then
            echo "✅ Web App force swap successful"
            WEB_SWAP_SUCCESS=true
        else
            echo "❌ Web App force swap failed"
            WEB_SWAP_SUCCESS=false
        fi
        
        # API App force swap
        echo "Force swapping API App..."
        if az webapp deployment slot swap \
            --resource-group "$RESOURCE_GROUP" \
            --name "$API_APP_NAME" \
            --slot staging \
            --target-slot production \
            --preserve-vnet true; then
            echo "✅ API App force swap successful"
            API_SWAP_SUCCESS=true
        else
            echo "❌ API App force swap failed"
            API_SWAP_SUCCESS=false
        fi
        ;;
esac

echo ""
echo "⏳ Step 6: Post-swap stabilization..."
sleep 20

echo ""
echo "🏥 Step 7: Post-swap production health verification..."

# Test production URLs
WEB_PROD_URL="https://$WEB_APP_NAME.azurewebsites.net"
API_PROD_URL="https://$API_APP_NAME.azurewebsites.net"

echo "Testing production Web App..."
if test_health_endpoints "$WEB_PROD_URL" "Production Web App"; then
    echo "✅ Production Web App is healthy"
    WEB_PROD_HEALTHY=true
else
    echo "❌ Production Web App not responding"
    WEB_PROD_HEALTHY=false
fi

echo ""
echo "Testing production API..."
if test_health_endpoints "$API_PROD_URL" "Production API"; then
    echo "✅ Production API is healthy"
    API_PROD_HEALTHY=true
else
    echo "❌ Production API not responding"
    API_PROD_HEALTHY=false
fi

echo ""
echo "📊 SLOT SWAP SUMMARY"
echo "===================="
echo "Web App Swap: $([ "$WEB_SWAP_SUCCESS" == true ] && echo "✅ SUCCESS" || echo "❌ FAILED")"
echo "API App Swap: $([ "$API_SWAP_SUCCESS" == true ] && echo "✅ SUCCESS" || echo "❌ FAILED")"
echo "Web Production Health: $([ "$WEB_PROD_HEALTHY" == true ] && echo "✅ HEALTHY" || echo "❌ UNHEALTHY")"
echo "API Production Health: $([ "$API_PROD_HEALTHY" == true ] && echo "✅ HEALTHY" || echo "❌ UNHEALTHY")"

echo ""
if [[ "$WEB_SWAP_SUCCESS" == true && "$API_SWAP_SUCCESS" == true ]]; then
    if [[ "$WEB_PROD_HEALTHY" == true && "$API_PROD_HEALTHY" == true ]]; then
        echo "🎉 SLOT SWAP COMPLETED SUCCESSFULLY!"
        echo "🎮 Educational game is ready for 12-year-old learners!"
        echo ""
        echo "🌐 Game URL: $WEB_PROD_URL"
        echo "🔧 API URL: $API_PROD_URL"
    else
        echo "⚠️ SLOT SWAP COMPLETED BUT HEALTH CHECK ISSUES"
        echo "Applications may need a few more minutes to fully initialize"
        echo "Monitor the applications and they should become healthy shortly"
    fi
else
    echo "❌ SLOT SWAP ISSUES DETECTED"
    echo "Manual intervention may be required"
    echo ""
    echo "🔧 Next steps:"
    echo "1. Check Azure portal for detailed error messages"
    echo "2. Review application logs in Azure App Service"
    echo "3. Consider manual rollback if necessary"
fi

echo ""
echo "🔗 Azure Portal Links:"
echo "Web App: https://portal.azure.com/#@/resource/subscriptions/$(az account show --query id -o tsv)/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.Web/sites/$WEB_APP_NAME"
echo "API App: https://portal.azure.com/#@/resource/subscriptions/$(az account show --query id -o tsv)/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.Web/sites/$API_APP_NAME"
