# Enhanced Slot Swap Strategy - World Leaders Game
# Add this to your deployment workflow to prevent staging slot ping failures

echo "🔧 Enhanced staging slot preparation..."

# Step 1: Extended staging warm-up with multiple health endpoint attempts
echo "🔥 Enhanced staging warm-up process..."
for i in {1..3}; do
    echo "Warm-up attempt $i/3..."
    
    # Try multiple endpoints to wake up the staging slots
    curl -s --connect-timeout 5 --max-time 10 "https://$WEB_APP_NAME-staging.azurewebsites.net/" || true
    curl -s --connect-timeout 5 --max-time 10 "https://$WEB_APP_NAME-staging.azurewebsites.net/health" || true
    curl -s --connect-timeout 5 --max-time 10 "https://$API_APP_NAME-staging.azurewebsites.net/" || true
    curl -s --connect-timeout 5 --max-time 10 "https://$API_APP_NAME-staging.azurewebsites.net/health" || true
    
    sleep 20
done

# Step 2: Verify Kudu/SCM is responsive before attempting swap
echo "🔍 Verifying Kudu/SCM readiness..."
curl -s --connect-timeout 10 --max-time 30 "https://$WEB_APP_NAME-staging.scm.azurewebsites.net/" || echo "Kudu warming up..."
curl -s --connect-timeout 10 --max-time 30 "https://$API_APP_NAME-staging.scm.azurewebsites.net/" || echo "Kudu warming up..."

# Step 3: Force staging slot restart if health checks fail
echo "🏥 Pre-swap health verification with auto-restart..."
WEB_HEALTH=$(curl -s -o /dev/null -w "%{http_code}" --connect-timeout 10 --max-time 30 "https://$WEB_APP_NAME-staging.azurewebsites.net/health" || echo "000")
API_HEALTH=$(curl -s -o /dev/null -w "%{http_code}" --connect-timeout 10 --max-time 30 "https://$API_APP_NAME-staging.azurewebsites.net/health" || echo "000")

if [[ "$WEB_HEALTH" != "200" ]]; then
    echo "⚠️ Web staging not responding - forcing restart..."
    az webapp restart --resource-group "$RESOURCE_GROUP" --name "$WEB_APP_NAME" --slot staging
    sleep 30
fi

if [[ "$API_HEALTH" != "200" ]]; then
    echo "⚠️ API staging not responding - forcing restart..."
    az webapp restart --resource-group "$RESOURCE_GROUP" --name "$API_APP_NAME" --slot staging
    sleep 30
fi

# Step 4: Enhanced slot swap with fallback strategies
echo "🔄 Enhanced slot swap with fallback strategies..."

# Function for smart slot swap
perform_smart_swap() {
    local app_name=$1
    local app_type=$2
    
    echo "🔄 Swapping $app_type..."
    
    # Strategy 1: Normal swap
    if az webapp deployment slot swap \
        --resource-group "$RESOURCE_GROUP" \
        --name "$app_name" \
        --slot staging \
        --target-slot production \
        --timeout 300; then
        echo "✅ $app_type normal swap successful"
        return 0
    fi
    
    echo "⚠️ Normal swap failed, trying restart + retry..."
    
    # Strategy 2: Restart staging and retry
    az webapp restart --resource-group "$RESOURCE_GROUP" --name "$app_name" --slot staging
    sleep 45
    
    if az webapp deployment slot swap \
        --resource-group "$RESOURCE_GROUP" \
        --name "$app_name" \
        --slot staging \
        --target-slot production \
        --timeout 300; then
        echo "✅ $app_type swap successful after restart"
        return 0
    fi
    
    echo "❌ $app_type swap failed after retry"
    return 1
}

# Execute smart swaps
perform_smart_swap "$WEB_APP_NAME" "Web App"
perform_smart_swap "$API_APP_NAME" "API App"

echo "✅ Enhanced slot swap process completed"
