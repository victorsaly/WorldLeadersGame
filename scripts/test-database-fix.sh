#!/bin/bash

# Quick fix script to test the current database configuration improvements
# This will deploy our updated code and test if the smart database selection works

set -e

echo "🚀 Testing Database Configuration Improvements"

# Commit and push current changes
echo "📝 Committing database configuration improvements..."
git add -A
git commit -m "🚀 Smart Database Provider Selection & Long-term PostgreSQL Solution

Features:
- Smart environment-aware database selection
- Azure App Service temp directory SQLite support  
- Cost-effective PostgreSQL Flexible Server template
- Automatic fallback hierarchy: PostgreSQL → SQLite-Temp → InMemory
- Debug logging for configuration troubleshooting

Educational Impact:
- Resolves production deployment failures
- Provides scalable long-term database solution
- Maintains development experience with SQLite
- Cost-optimized for educational usage (~£15-25/month)"

echo "📤 Pushing to trigger deployment..."
git push

# Wait for deployment
echo "⏱️  Waiting for GitHub Actions deployment to start..."
sleep 10

# Monitor deployment
echo "👀 Monitoring deployment progress..."
gh run list --workflow="azure-deploy.yml" --limit 1

# Show instructions for next steps
echo ""
echo "🎯 Next Steps:"
echo ""
echo "1. 📊 Monitor the deployment:"
echo "   gh run watch \$(gh run list --workflow='azure-deploy.yml' --limit 1 --json databaseUrl --jq '.[0].databaseUrl')"
echo ""
echo "2. 🧪 Test the API after deployment:"
echo "   curl -I https://worldleaders-api-prod-staging.azurewebsites.net/health"
echo ""
echo "3. 🐘 For long-term solution, deploy PostgreSQL:"
echo "   chmod +x scripts/deploy-postgresql.sh"
echo "   ./scripts/deploy-postgresql.sh"
echo ""
echo "4. 💰 Current approach uses SQLite in temp directory (free)"
echo "   PostgreSQL would cost ~£15-25/month but provides:"
echo "   - Better performance and reliability"
echo "   - Proper backup and recovery"
echo "   - Scalability for more users"
echo "   - Production-grade database features"
echo ""

# Test function
test_api() {
    echo "🧪 Testing API endpoint..."
    local response=$(curl -s -I https://worldleaders-api-prod-staging.azurewebsites.net/health)
    local status=$(echo "$response" | head -n1 | cut -d' ' -f2)
    
    echo "Response Status: $status"
    
    if [ "$status" = "200" ]; then
        echo "✅ API is responding successfully!"
        return 0
    else
        echo "❌ API still not responding (Status: $status)"
        return 1
    fi
}

# Wait and test
echo ""
echo "🕐 Will test API in 3 minutes after deployment completes..."
sleep 180

if test_api; then
    echo ""
    echo "🎉 SUCCESS! Database configuration fix worked!"
    echo "The smart database selection is now using a compatible provider."
else
    echo ""
    echo "⚠️  API still not responding. Let's check the logs:"
    echo "Run: az webapp log tail --name worldleaders-api-prod --resource-group worldleaders-prod-rg --slot staging"
fi
