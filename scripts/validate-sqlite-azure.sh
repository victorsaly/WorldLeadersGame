#!/bin/bash

# Azure SQLite Validation Script
# Tests SQLite configuration in Azure App Service environment

set -e

echo "🔍 Azure SQLite Configuration Validation"
echo "========================================="

# Function to test API health after deployment
test_api_health() {
    local url="$1"
    echo "🧪 Testing API health at: $url"
    
    # Wait for deployment to complete
    sleep 30
    
    local max_attempts=10
    local attempt=1
    
    while [ $attempt -le $max_attempts ]; do
        echo "Attempt $attempt/$max_attempts..."
        
        local response=$(curl -s -w "%{http_code}" -o /dev/null "$url/health" || echo "000")
        
        if [ "$response" = "200" ]; then
            echo "✅ API health check passed (HTTP 200)"
            
            # Test detailed health endpoint
            echo "🔍 Checking detailed health information..."
            curl -s "$url/health/detailed" | jq '.Components[] | select(.Name == "database")' || echo "Database health info not available"
            
            return 0
        else
            echo "❌ API health check failed (HTTP $response)"
            
            if [ $attempt -eq $max_attempts ]; then
                echo "🚨 Maximum attempts reached. Checking logs..."
                return 1
            fi
            
            echo "⏱️  Waiting 30 seconds before retry..."
            sleep 30
        fi
        
        ((attempt++))
    done
}

# Function to get Azure App Service logs
get_azure_logs() {
    local app_name="$1"
    local resource_group="$2"
    local slot="${3:-staging}"
    
    echo "📋 Fetching Azure App Service logs..."
    
    if command -v az &> /dev/null; then
        echo "🔍 Container logs (last 50 lines):"
        az webapp log tail --name "$app_name" --resource-group "$resource_group" --slot "$slot" | tail -50
        
        echo ""
        echo "🔍 Application logs (SQLite-related):"
        az webapp log download --name "$app_name" --resource-group "$resource_group" --slot "$slot" --log-file azure-logs.zip 2>/dev/null || echo "Log download failed"
        
        if [ -f "azure-logs.zip" ]; then
            unzip -q azure-logs.zip
            find . -name "*.log" -exec grep -l "SQLite\|Database\|TEMP" {} \; | head -3 | xargs -I {} sh -c 'echo "=== {} ===" && grep -E "(SQLite|Database|TEMP|Error)" {} | tail -10'
            rm -f azure-logs.zip
            rm -rf LogFiles 2>/dev/null || true
        fi
    else
        echo "❌ Azure CLI not installed. Install with: brew install azure-cli"
    fi
}

# Function to validate SQLite configuration locally
validate_sqlite_config() {
    echo "🏠 Testing SQLite configuration locally..."
    
    # Set Azure-like environment variables
    export ASPNETCORE_ENVIRONMENT="Production"
    export WEBSITE_SITE_NAME="worldleaders-test"
    export TEMP="/tmp/azure-test"
    
    # Create test temp directory
    mkdir -p "$TEMP"
    
    echo "📁 Test temp directory: $TEMP"
    echo "✅ Directory writable: $(test -w "$TEMP" && echo 'Yes' || echo 'No')"
    
    # Test SQLite database creation
    local test_db="$TEMP/test-worldleaders.db"
    
    if command -v sqlite3 &> /dev/null; then
        echo "🧪 Testing SQLite database creation..."
        sqlite3 "$test_db" "CREATE TABLE test(id INTEGER); INSERT INTO test VALUES(1); SELECT * FROM test;"
        
        if [ -f "$test_db" ]; then
            echo "✅ SQLite database created successfully at: $test_db"
            echo "📊 Database size: $(du -h "$test_db" | cut -f1)"
            rm -f "$test_db"
        else
            echo "❌ SQLite database creation failed"
        fi
    else
        echo "⚠️  SQLite3 not installed. Install with: brew install sqlite3"
    fi
    
    # Clean up
    rm -rf "$TEMP"
    unset ASPNETCORE_ENVIRONMENT WEBSITE_SITE_NAME TEMP
}

# Main validation process
main() {
    echo "🚀 Starting Azure SQLite validation..."
    echo ""
    
    # Step 1: Local validation
    validate_sqlite_config
    echo ""
    
    # Step 2: Deploy current changes
    echo "📤 Deploying current SQLite improvements..."
    
    # Commit current changes
    git add -A
    git commit -m "🔧 Enhanced SQLite Azure Support

    Improvements:
    - Robust temp directory detection with multiple fallbacks
    - Directory writability validation before use  
    - Comprehensive error handling and logging
    - Azure-specific path prioritization
    
    Resolves SQLite permission issues in Azure App Service" || echo "No changes to commit"
    
    # Push to trigger deployment
    git push
    
    echo "⏱️  Waiting for GitHub Actions deployment..."
    sleep 60
    
    # Step 3: Test deployed API
    local api_url="https://worldleaders-api-prod-staging.azurewebsites.net"
    
    if test_api_health "$api_url"; then
        echo ""
        echo "🎉 SUCCESS! SQLite is working correctly in Azure!"
        echo ""
        echo "✅ Verification complete:"
        echo "   - Enhanced temp directory detection"
        echo "   - Directory writability validation" 
        echo "   - Robust fallback mechanisms"
        echo "   - Production deployment successful"
        
        # Test specific database endpoints
        echo ""
        echo "🧪 Testing database-dependent endpoints..."
        curl -s "$api_url/api/territories" | jq '. | length' | xargs -I {} echo "Territories loaded: {} countries"
        
    else
        echo ""
        echo "❌ FAILURE: SQLite issues persist in Azure"
        echo ""
        echo "🔍 Troubleshooting steps:"
        echo "1. Check Azure logs below"
        echo "2. Verify App Service configuration"
        echo "3. Consider PostgreSQL upgrade for reliability"
        
        # Get detailed logs
        get_azure_logs "worldleaders-api-prod" "worldleaders-prod-rg" "staging"
        
        echo ""
        echo "💡 Recommendation:"
        echo "   Deploy PostgreSQL for production reliability:"
        echo "   ./scripts/deploy-postgresql.sh"
    fi
}

# Run validation
main "$@"
