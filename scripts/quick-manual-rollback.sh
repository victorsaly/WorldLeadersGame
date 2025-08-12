#!/bin/bash

# Quick Manual Rollback - World Leaders Game
# Context: Immediate fix for the current rollback failure
# Purpose: Manually perform the correct rollback operation

set -euo pipefail

echo "🚨 QUICK MANUAL ROLLBACK - World Leaders Game"
echo "============================================"
echo "🔧 Fixing the current deployment issue immediately"
echo ""

# Azure configuration
RESOURCE_GROUP="worldleaders-prod-rg"
WEB_APP_NAME="worldleaders-web-prod"
API_APP_NAME="worldleaders-api-prod"

echo "📋 Performing immediate rollback with correct Azure logic..."
echo "   FROM: staging slots (containing previous production code)"
echo "   TO: production (main slots)"
echo ""

# Check Azure CLI login
if ! az account show > /dev/null 2>&1; then
    echo "❌ Not logged into Azure CLI"
    echo "Please run: az login"
    exit 1
fi

echo "✅ Azure CLI authenticated"
echo ""

echo "🔄 Step 1: Rolling back Web App..."
echo "Swapping from staging back to production..."

if az webapp deployment slot swap \
    --resource-group "$RESOURCE_GROUP" \
    --name "$WEB_APP_NAME" \
    --slot staging \
    --target-slot production; then
    echo "✅ Web App rollback successful"
    WEB_ROLLBACK_SUCCESS=true
else
    echo "❌ Web App rollback failed"
    WEB_ROLLBACK_SUCCESS=false
fi

echo ""
echo "🔄 Step 2: Rolling back API..."
echo "Swapping from staging back to production..."

if az webapp deployment slot swap \
    --resource-group "$RESOURCE_GROUP" \
    --name "$API_APP_NAME" \
    --slot staging \
    --target-slot production; then
    echo "✅ API rollback successful"
    API_ROLLBACK_SUCCESS=true
else
    echo "❌ API rollback failed"
    API_ROLLBACK_SUCCESS=false
fi

echo ""
echo "⏳ Step 3: Waiting for rollback to stabilize..."
sleep 30

echo ""
echo "🏥 Step 4: Testing rollback health..."

WEB_URL="https://$WEB_APP_NAME.azurewebsites.net"
API_URL="https://$API_APP_NAME.azurewebsites.net"

echo "Testing Web App: $WEB_URL"
if curl -f -s --max-time 10 "$WEB_URL" > /dev/null 2>&1; then
    echo "✅ Web App is responding"
    WEB_HEALTHY=true
else
    echo "❌ Web App not responding"
    WEB_HEALTHY=false
fi

echo "Testing API: $API_URL"  
if curl -f -s --max-time 10 "$API_URL" > /dev/null 2>&1; then
    echo "✅ API is responding"
    API_HEALTHY=true
else
    echo "❌ API not responding"
    API_HEALTHY=false
fi

echo ""
echo "📊 ROLLBACK SUMMARY"
echo "=================="
echo "Web App Rollback: $([ "$WEB_ROLLBACK_SUCCESS" == true ] && echo "✅ SUCCESS" || echo "❌ FAILED")"
echo "API Rollback: $([ "$API_ROLLBACK_SUCCESS" == true ] && echo "✅ SUCCESS" || echo "❌ FAILED")"
echo "Web App Health: $([ "$WEB_HEALTHY" == true ] && echo "✅ HEALTHY" || echo "❌ UNHEALTHY")"
echo "API Health: $([ "$API_HEALTHY" == true ] && echo "✅ HEALTHY" || echo "❌ UNHEALTHY")"

echo ""
if [[ "$WEB_ROLLBACK_SUCCESS" == true && "$API_ROLLBACK_SUCCESS" == true ]]; then
    echo "🎉 MANUAL ROLLBACK COMPLETED SUCCESSFULLY!"
    echo "🎮 Educational game should be restored to working state"
    echo ""
    echo "🌐 Game URL: $WEB_URL"
    echo "🔧 API URL: $API_URL"
    echo ""
    echo "✅ The GitHub Actions workflow has been fixed for future deployments"
else
    echo "❌ ROLLBACK ISSUES DETECTED"
    echo "Please check Azure portal for more details"
fi

echo ""
echo "🔗 Azure Portal Links:"
echo "Web App: https://portal.azure.com/#@/resource/subscriptions/$(az account show --query id -o tsv)/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.Web/sites/$WEB_APP_NAME"
echo "API App: https://portal.azure.com/#@/resource/subscriptions/$(az account show --query id -o tsv)/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.Web/sites/$API_APP_NAME"
