#!/bin/bash

# Fix Zero-Downtime Deployment Issue
# Context: Educational game platform deployment recovery
# Issue: Staging slot not responding to health checks

set -euo pipefail

echo "🔧 Fixing zero-downtime deployment issue..."

# Configuration
RESOURCE_GROUP="worldleaders-prod-rg"
WEB_APP_NAME="worldleaders-web-prod"
API_APP_NAME="worldleaders-api-prod"

echo "🔍 Diagnosing deployment slots..."

# Check current slot status
echo "📊 Web App Slots Status:"
az webapp deployment slot list --resource-group "$RESOURCE_GROUP" --name "$WEB_APP_NAME" --query "[].{Name:name, State:state, LastModified:lastModifiedTimeUtc}" --output table

echo ""
echo "📊 API App Slots Status:"
az webapp deployment slot list --resource-group "$RESOURCE_GROUP" --name "$API_APP_NAME" --query "[].{Name:name, State:state, LastModified:lastModifiedTimeUtc}" --output table

echo ""
echo "🏥 Testing current health endpoints..."

# Test production slots
echo "Testing Production Web Health:"
curl -I -s --connect-timeout 5 --max-time 10 "https://worldleaders-web-prod.azurewebsites.net/health" || echo "❌ Production web health not responding"

echo "Testing Production API Health:"
curl -I -s --connect-timeout 5 --max-time 10 "https://worldleaders-api-prod.azurewebsites.net/health" || echo "❌ Production API health not responding"

# Test staging slots
echo "Testing Staging Web Health:"
curl -I -s --connect-timeout 5 --max-time 10 "https://worldleaders-web-prod-staging.azurewebsites.net/health" || echo "❌ Staging web health not responding"

echo "Testing Staging API Health:"
curl -I -s --connect-timeout 5 --max-time 10 "https://worldleaders-api-prod-staging.azurewebsites.net/health" || echo "❌ Staging API health not responding"

echo ""
echo "🔧 Fix Options:"
echo ""

# Option 1: Manual slot swap with health check bypass
echo "Option 1: Force slot swap (bypass health checks)"
echo "  Command: az webapp deployment slot swap --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME --slot staging --target-slot production"
echo ""

# Option 2: Deploy health fix to staging
echo "Option 2: Deploy health check fix to staging"
echo "  1. Deploy updated Web app to staging slot"
echo "  2. Verify health endpoints work"
echo "  3. Retry normal slot swap"
echo ""

# Option 3: Quick staging restart
echo "Option 3: Restart staging slot and retry"
echo "  Command: az webapp restart --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME --slot staging"
echo ""

# Offer immediate fix
echo "🚀 Immediate Fix Recommendation:"
echo "Since this is an educational platform serving children, we should use the safest approach."
echo ""

read -r -p "🤔 Choose fix option (1=Force swap, 2=Deploy fix, 3=Restart staging, q=quit): " choice

case $choice in
    1)
        echo "⚠️  Performing force slot swap (bypassing health checks)..."
        echo "🔄 Swapping Web App..."
        if az webapp deployment slot swap --resource-group "$RESOURCE_GROUP" --name "$WEB_APP_NAME" --slot staging --target-slot production; then
            echo "✅ Web app slot swap completed"
        else
            echo "❌ Web app slot swap failed"
        fi
        
        echo "🔄 Swapping API App..."
        if az webapp deployment slot swap --resource-group "$RESOURCE_GROUP" --name "$API_APP_NAME" --slot staging --target-slot production; then
            echo "✅ API app slot swap completed"
        else
            echo "❌ API app slot swap failed"
        fi
        ;;
    2)
        echo "📚 To deploy the health check fix:"
        echo "1. Commit and push the updated Web/Program.cs file"
        echo "2. Trigger GitHub Actions deployment"
        echo "3. Wait for staging deployment to complete"
        echo "4. Retry the slot swap"
        echo ""
        echo "The fix has already been applied to the code - just need to deploy it."
        ;;
    3)
        echo "🔄 Restarting staging slots..."
        echo "Restarting Web App staging slot..."
        az webapp restart --resource-group "$RESOURCE_GROUP" --name "$WEB_APP_NAME" --slot staging
        
        echo "Restarting API App staging slot..."
        az webapp restart --resource-group "$RESOURCE_GROUP" --name "$API_APP_NAME" --slot staging
        
        echo "⏱️  Waiting 30 seconds for apps to restart..."
        sleep 30
        
        echo "🧪 Testing health endpoints after restart..."
        curl -I -s --connect-timeout 5 --max-time 10 "https://worldleaders-web-prod-staging.azurewebsites.net/health" || echo "❌ Still not responding"
        ;;
    q)
        echo "👋 Exiting without changes"
        exit 0
        ;;
    *)
        echo "❌ Invalid option"
        exit 1
        ;;
esac

echo ""
echo "🎯 Next Steps After Fix:"
echo "1. Test all health endpoints are responding"
echo "2. Monitor application performance"
echo "3. Verify educational platform is serving students correctly"
echo "4. Update deployment documentation with health check requirements"
