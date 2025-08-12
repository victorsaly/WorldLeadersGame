#!/bin/bash

# Emergency Rollback Fix - World Leaders Game
# Context: Fix automated rollback issues in GitHub Actions workflow
# Issue: Incorrect slot naming assumption causing rollback failures
# Solution: Correct Azure slot swap logic for production/staging structure

set -euo pipefail

echo "🚨 EMERGENCY ROLLBACK FIX - World Leaders Game"
echo "=============================================="
echo "📋 Fixing automated rollback for educational platform"
echo ""

# Configuration matching Azure deployment
RESOURCE_GROUP="worldleaders-prod-rg"
WEB_APP_NAME="worldleaders-web-prod"
API_APP_NAME="worldleaders-api-prod"

echo "🔍 Understanding Azure App Service Slot Structure..."
echo "📊 Correct structure:"
echo "   - Production: $WEB_APP_NAME (main slot, not a sub-slot)"
echo "   - Staging: $WEB_APP_NAME/slots/staging"
echo "   - Production: $API_APP_NAME (main slot, not a sub-slot)"
echo "   - Staging: $API_APP_NAME/slots/staging"
echo ""

echo "❌ INCORRECT assumption (causing failure):"
echo "   - Looking for: $WEB_APP_NAME/slots/production (DOES NOT EXIST)"
echo ""
echo "✅ CORRECT rollback logic:"
echo "   - Swap FROM: staging slot"
echo "   - Swap TO: main production site (default slot)"
echo ""

echo "🏥 Step 1: Checking current deployment status..."

echo "📊 Web App deployment slots:"
if az webapp deployment slot list \
    --resource-group "$RESOURCE_GROUP" \
    --name "$WEB_APP_NAME" \
    --query "[].{Name:name, State:state, DefaultHostName:defaultHostName}" \
    --output table 2>/dev/null; then
    echo "✅ Web App slots listed successfully"
else
    echo "❌ Failed to list Web App slots - checking if app exists..."
    if az webapp show --resource-group "$RESOURCE_GROUP" --name "$WEB_APP_NAME" > /dev/null 2>&1; then
        echo "✅ Web App exists but may not have slots configured"
    else
        echo "❌ Web App does not exist in resource group"
        exit 1
    fi
fi

echo ""
echo "📊 API App deployment slots:"
if az webapp deployment slot list \
    --resource-group "$RESOURCE_GROUP" \
    --name "$API_APP_NAME" \
    --query "[].{Name:name, State:state, DefaultHostName:defaultHostName}" \
    --output table 2>/dev/null; then
    echo "✅ API App slots listed successfully"
else
    echo "❌ Failed to list API App slots - checking if app exists..."
    if az webapp show --resource-group "$RESOURCE_GROUP" --name "$API_APP_NAME" > /dev/null 2>&1; then
        echo "✅ API App exists but may not have slots configured"
    else
        echo "❌ API App does not exist in resource group"
        exit 1
    fi
fi

echo ""
echo "🔄 Step 2: Performing CORRECT rollback logic..."
echo "💡 Rollback means: swap staging content back to production"
echo "    This moves the previous production code from staging back to production"

# Function to perform correct rollback
perform_correct_rollback() {
    local app_name=$1
    local app_type=$2
    
    echo "🔄 Rolling back $app_type ($app_name)..."
    
    # Check if staging slot exists
    if ! az webapp deployment slot list \
        --resource-group "$RESOURCE_GROUP" \
        --name "$app_name" \
        --query "[?name=='staging']" \
        --output tsv | grep -q staging; then
        echo "⚠️ No staging slot found for $app_type"
        echo "   This means rollback is not possible with current slot configuration"
        echo "   Manual intervention required"
        return 1
    fi
    
    echo "✅ Staging slot confirmed for $app_type"
    
    # Perform the CORRECT rollback swap
    # This swaps staging (old production) back to production (current)
    echo "🔄 Swapping staging back to production for $app_type..."
    if az webapp deployment slot swap \
        --resource-group "$RESOURCE_GROUP" \
        --name "$app_name" \
        --slot staging \
        --target-slot production; then
        echo "✅ $app_type rollback successful"
        return 0
    else
        echo "❌ $app_type rollback failed"
        echo "🔍 Attempting diagnostic information..."
        
        # Get detailed status
        echo "📊 $app_type status:"
        az webapp show \
            --resource-group "$RESOURCE_GROUP" \
            --name "$app_name" \
            --query "{state:state, availabilityState:availabilityState, hostNames:hostNames}" \
            --output table
        
        # Try restart and retry
        echo "🔧 Attempting restart of staging slot..."
        if az webapp restart --resource-group "$RESOURCE_GROUP" --name "$app_name" --slot staging; then
            echo "✅ Staging slot restarted"
            
            echo "⏳ Waiting 30 seconds for restart..."
            sleep 30
            
            echo "🔄 Retrying rollback swap..."
            if az webapp deployment slot swap \
                --resource-group "$RESOURCE_GROUP" \
                --name "$app_name" \
                --slot staging \
                --target-slot production; then
                echo "✅ $app_type rollback successful after restart"
                return 0
            else
                echo "❌ $app_type rollback failed even after restart"
                return 1
            fi
        else
            echo "❌ Failed to restart staging slot"
            return 1
        fi
    fi
}

# Execute rollbacks
echo "🔄 Executing rollback for Web App..."
if perform_correct_rollback "$WEB_APP_NAME" "Web App"; then
    WEB_ROLLBACK_SUCCESS=true
else
    WEB_ROLLBACK_SUCCESS=false
fi

echo ""
echo "🔄 Executing rollback for API..."
if perform_correct_rollback "$API_APP_NAME" "API"; then
    API_ROLLBACK_SUCCESS=true
else
    API_ROLLBACK_SUCCESS=false
fi

echo ""
echo "⏳ Step 3: Post-rollback stabilization..."
sleep 20

echo ""
echo "🏥 Step 4: Post-rollback health verification..."

# Test production URLs after rollback
WEB_PROD_URL="https://$WEB_APP_NAME.azurewebsites.net"
API_PROD_URL="https://$API_APP_NAME.azurewebsites.net"

# Function to test health endpoints
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

echo "Testing production Web App health..."
if test_health_endpoints "$WEB_PROD_URL" "Production Web App"; then
    echo "✅ Production Web App is healthy after rollback"
    WEB_PROD_HEALTHY=true
else
    echo "❌ Production Web App not responding after rollback"
    WEB_PROD_HEALTHY=false
fi

echo ""
echo "Testing production API health..."
if test_health_endpoints "$API_PROD_URL" "Production API"; then
    echo "✅ Production API is healthy after rollback"
    API_PROD_HEALTHY=true
else
    echo "❌ Production API not responding after rollback"
    API_PROD_HEALTHY=false
fi

echo ""
echo "📊 EMERGENCY ROLLBACK SUMMARY"
echo "============================="
echo "Web App Rollback: $([ "$WEB_ROLLBACK_SUCCESS" == true ] && echo "✅ SUCCESS" || echo "❌ FAILED")"
echo "API Rollback: $([ "$API_ROLLBACK_SUCCESS" == true ] && echo "✅ SUCCESS" || echo "❌ FAILED")"
echo "Web Production Health: $([ "$WEB_PROD_HEALTHY" == true ] && echo "✅ HEALTHY" || echo "❌ UNHEALTHY")"
echo "API Production Health: $([ "$API_PROD_HEALTHY" == true ] && echo "✅ HEALTHY" || echo "❌ UNHEALTHY")"

echo ""
if [[ "$WEB_ROLLBACK_SUCCESS" == true && "$API_ROLLBACK_SUCCESS" == true ]]; then
    if [[ "$WEB_PROD_HEALTHY" == true && "$API_PROD_HEALTHY" == true ]]; then
        echo "🎉 EMERGENCY ROLLBACK COMPLETED SUCCESSFULLY!"
        echo "🎮 Educational game restored to previous working state!"
        echo "📚 12-year-old learners can continue their educational journey"
        echo ""
        echo "🌐 Game URL: $WEB_PROD_URL"
        echo "🔧 API URL: $API_PROD_URL"
    else
        echo "⚠️ ROLLBACK COMPLETED BUT HEALTH CHECK ISSUES"
        echo "Applications may need a few more minutes to fully initialize"
        echo "Monitor the applications - they should become healthy shortly"
    fi
else
    echo "❌ ROLLBACK ISSUES DETECTED"
    echo "Manual intervention required"
    echo ""
    echo "🔧 Next steps:"
    echo "1. Check Azure portal for detailed error messages"
    echo "2. Review application logs in Azure App Service"
    echo "3. Consider manual slot management in Azure portal"
    echo "4. Contact development team for further assistance"
fi

echo ""
echo "🔗 Azure Portal Links:"
echo "Web App: https://portal.azure.com/#@/resource/subscriptions/$(az account show --query id -o tsv)/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.Web/sites/$WEB_APP_NAME"
echo "API App: https://portal.azure.com/#@/resource/subscriptions/$(az account show --query id -o tsv)/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.Web/sites/$API_APP_NAME"

echo ""
echo "💡 ROLLBACK LOGIC FIX EXPLANATION:"
echo "=================================="
echo "❌ Previous incorrect assumption:"
echo "   - Tried to swap FROM 'production' slot TO 'staging' slot"
echo "   - But 'production' slot doesn't exist as a sub-slot"
echo ""
echo "✅ Correct rollback logic:"
echo "   - Swap FROM 'staging' slot TO 'production' (main slot)"
echo "   - This moves the previous production code back to production"
echo "   - 'production' is the default main slot, not a sub-slot"
echo ""
echo "🛠️ GitHub Actions Workflow Fix Required:"
echo "   - Update rollback logic to use correct slot names"
echo "   - Remove references to non-existent 'production' slot"
echo "   - Use 'staging' to 'production' swap direction"

echo ""
echo "🚀 Ready to update GitHub Actions workflow with correct rollback logic!"
