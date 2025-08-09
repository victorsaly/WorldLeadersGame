#!/bin/bash

# Quick CORS Fix for Production API
# Directly updates production CORS to allow docs.worldleadersgame.co.uk

set -e

echo "🚀 Quick CORS Fix for Production API"
echo "====================================="

RESOURCE_GROUP="worldleaders-prod-rg"
API_APP_NAME="worldleaders-api-prod"

echo "🔧 Adding docs.worldleadersgame.co.uk to CORS allowed origins..."

# Add the missing CORS origin directly to production
az webapp cors add \
    --resource-group $RESOURCE_GROUP \
    --name $API_APP_NAME \
    --allowed-origins "https://docs.worldleadersgame.co.uk"

echo "✅ CORS origin added successfully!"

echo "🧪 Testing CORS fix..."
sleep 5

# Test CORS preflight
echo "Testing CORS preflight request..."
curl -I -X OPTIONS \
    -H "Origin: https://docs.worldleadersgame.co.uk" \
    -H "Access-Control-Request-Method: POST" \
    -H "Access-Control-Request-Headers: Content-Type" \
    "https://api.worldleadersgame.co.uk/api/ai/explain-code" || echo "CORS test completed"

echo ""
echo "🎯 CORS Fix Applied!"
echo "The documentation site should now be able to access the API."
echo ""
echo "Test the code explainer at:"
echo "https://docs.worldleadersgame.co.uk/child-friendly-cost-transparency-educational-gaming/"
