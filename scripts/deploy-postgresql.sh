#!/bin/bash

# Deploy PostgreSQL Flexible Server for World Leaders Educational Game
# This provides a long-term, scalable database solution

set -e

echo "🐘 Deploying PostgreSQL Database for World Leaders Game"

# Configuration
RESOURCE_GROUP="worldleaders-prod-rg"
LOCATION="uksouth"
DEPLOYMENT_NAME="worldleaders-postgres-$(date +%Y%m%d-%H%M%S)"
TEMPLATE_FILE="infrastructure/azure-postgresql.bicep"

# Generate secure credentials
DB_ADMIN_USER="worldleaders_admin"
DB_ADMIN_PASSWORD=$(openssl rand -base64 32 | tr -d "=+/" | cut -c1-25)

echo "📋 Configuration:"
echo "  Resource Group: $RESOURCE_GROUP"
echo "  Location: $LOCATION"
echo "  Admin User: $DB_ADMIN_USER"
echo "  Admin Password: [Generated securely]"

# Check if resource group exists
if ! az group show --name $RESOURCE_GROUP &> /dev/null; then
    echo "❌ Resource group $RESOURCE_GROUP does not exist"
    echo "Creating resource group..."
    az group create --name $RESOURCE_GROUP --location $LOCATION
fi

# Deploy PostgreSQL server
echo "🚀 Deploying PostgreSQL Flexible Server..."
DEPLOYMENT_OUTPUT=$(az deployment group create \
    --resource-group $RESOURCE_GROUP \
    --name $DEPLOYMENT_NAME \
    --template-file $TEMPLATE_FILE \
    --parameters \
        environment=prod \
        location=$LOCATION \
        administratorLogin=$DB_ADMIN_USER \
        administratorPassword=$DB_ADMIN_PASSWORD \
    --query '[properties.outputs.postgresServerFQDN.value, properties.outputs.databaseName.value, properties.outputs.estimatedMonthlyCost.value]' \
    --output tsv)

# Parse output
SERVER_FQDN=$(echo "$DEPLOYMENT_OUTPUT" | sed -n '1p')
DATABASE_NAME=$(echo "$DEPLOYMENT_OUTPUT" | sed -n '2p')
ESTIMATED_COST=$(echo "$DEPLOYMENT_OUTPUT" | sed -n '3p')

# Build connection string
CONNECTION_STRING="Host=$SERVER_FQDN;Database=$DATABASE_NAME;Username=$DB_ADMIN_USER;Password=$DB_ADMIN_PASSWORD;SSL Mode=Require;Trust Server Certificate=true"

echo "✅ PostgreSQL deployment completed successfully!"
echo ""
echo "📊 Database Information:"
echo "  Server FQDN: $SERVER_FQDN"
echo "  Database Name: $DATABASE_NAME"
echo "  Estimated Cost: $ESTIMATED_COST"
echo ""

# Update App Service with connection string
echo "🔧 Updating App Service configuration..."

# Set connection string for both production and staging slots
az webapp config connection-string set \
    --name "worldleaders-api-prod" \
    --resource-group $RESOURCE_GROUP \
    --connection-string-type "PostgreSQL" \
    --settings DefaultConnection="$CONNECTION_STRING"

az webapp config connection-string set \
    --name "worldleaders-api-prod" \
    --resource-group $RESOURCE_GROUP \
    --slot "staging" \
    --connection-string-type "PostgreSQL" \
    --settings DefaultConnection="$CONNECTION_STRING"

# Update database provider setting
az webapp config appsettings set \
    --name "worldleaders-api-prod" \
    --resource-group $RESOURCE_GROUP \
    --settings "Database__Provider=PostgreSQL"

az webapp config appsettings set \
    --name "worldleaders-api-prod" \
    --resource-group $RESOURCE_GROUP \
    --slot "staging" \
    --settings "Database__Provider=PostgreSQL"

echo "✅ App Service configuration updated!"
echo ""

# Store credentials securely
echo "🔐 Storing credentials securely..."
echo "Database Admin Username: $DB_ADMIN_USER" > .postgres-credentials
echo "Database Admin Password: $DB_ADMIN_PASSWORD" >> .postgres-credentials
echo "Connection String: $CONNECTION_STRING" >> .postgres-credentials
echo "Credentials saved to .postgres-credentials (add to .gitignore!)"

echo ""
echo "🎯 Next Steps:"
echo "1. Add .postgres-credentials to .gitignore"
echo "2. Store credentials in your password manager"
echo "3. Run migration: dotnet ef database update"
echo "4. Test the application deployment"
echo ""
echo "💰 Cost Information:"
echo "  $ESTIMATED_COST"
echo "  Monitor costs at: https://portal.azure.com/#view/Microsoft_Azure_CostManagement/Menu/~/overview"
