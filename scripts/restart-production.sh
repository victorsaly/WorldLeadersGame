#!/bin/bash

# Quick Production Restart to Apply CORS Changes
# Sometimes Azure App Service needs a restart to pick up new CORS configuration

set -e

echo "🔄 Quick Production Restart for CORS Fix"
echo "========================================"

RESOURCE_GROUP="worldleaders-prod-rg"
API_APP_NAME="worldleaders-api-prod"

echo "🔄 Restarting production API to apply CORS changes..."

az webapp restart \
    --resource-group $RESOURCE_GROUP \
    --name $API_APP_NAME

echo "⏱️  Waiting for restart to complete..."
sleep 30

echo "🧪 Testing CORS after restart..."

# Test CORS preflight again
echo "Testing CORS preflight request..."
CORS_RESULT=$(curl -s -I -X OPTIONS \
    -H "Origin: https://docs.worldleadersgame.co.uk" \
    -H "Access-Control-Request-Method: POST" \
    -H "Access-Control-Request-Headers: Content-Type" \
    "https://api.worldleadersgame.co.uk/api/ai/explain-code" | head -1)

echo "CORS Response: $CORS_RESULT"

if [[ "$CORS_RESULT" == *"200"* ]]; then
    echo "✅ CORS is now working!"
else
    echo "⚠️ CORS still needs the application code update"
    echo "The staging slot has the updated code, but production needs the deployment to complete"
fi

echo ""
echo "🎯 Alternative Solution:"
echo "Use the staging API directly for now:"
echo "Change your docs to use: https://worldleaders-api-prod-staging.azurewebsites.net"
echo ""
echo "🚀 Or trigger a new deployment to get the updated CORS code:"
echo "git commit --allow-empty -m 'Trigger deployment for CORS fix' && git push"
