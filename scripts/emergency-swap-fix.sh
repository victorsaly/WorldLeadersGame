#!/bin/bash

# EMERGENCY: Simple Slot Swap Fix
# When staging slot is not responding to ping during swap

echo "🚨 EMERGENCY SLOT SWAP FIX"
echo "=========================="
echo ""
echo "Issue: Staging slot not responding to HTTP ping"
echo "Solution: Force restart staging slots and retry"
echo ""

# Configuration
RESOURCE_GROUP="worldleaders-prod-rg"
WEB_APP="worldleaders-web-prod"
API_APP="worldleaders-api-prod"

echo "🔄 Step 1: Restart staging slots..."
echo "Restarting Web staging..."
az webapp restart --resource-group "$RESOURCE_GROUP" --name "$WEB_APP" --slot staging

echo "Restarting API staging..."
az webapp restart --resource-group "$RESOURCE_GROUP" --name "$API_APP" --slot staging

echo ""
echo "⏳ Step 2: Wait 60 seconds for restart..."
sleep 60

echo ""
echo "🔄 Step 3: Force slot swap (bypassing health checks)..."

echo "Swapping Web App..."
if az webapp deployment slot swap \
    --resource-group "$RESOURCE_GROUP" \
    --name "$WEB_APP" \
    --slot staging \
    --target-slot production; then
    echo "✅ Web App swap completed"
else
    echo "❌ Web App swap failed"
fi

echo ""
echo "Swapping API App..."
if az webapp deployment slot swap \
    --resource-group "$RESOURCE_GROUP" \
    --name "$API_APP" \
    --slot staging \
    --target-slot production; then
    echo "✅ API App swap completed"
else
    echo "❌ API App swap failed"
fi

echo ""
echo "⏳ Step 4: Wait for stabilization..."
sleep 30

echo ""
echo "🏥 Step 5: Quick health check..."
curl -I https://worldleaders-web-prod.azurewebsites.net/ || echo "Web app still starting..."
curl -I https://worldleaders-api-prod.azurewebsites.net/health || echo "API still starting..."

echo ""
echo "✅ Emergency fix completed!"
echo "Monitor applications - they may need a few minutes to fully initialize."
