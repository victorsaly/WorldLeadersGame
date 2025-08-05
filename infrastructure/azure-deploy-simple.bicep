// Azure Bicep template for World Leaders Game deployment (Simplified)
// Context: Educational game deployment for 12-year-old learners
// Architecture: Web App + API (without Static Site for now)

param projectName string = 'worldleaders'
param environment string = 'prod'
param location string = 'uksouth'
param customDomainName string = ''

// Variables for consistent naming
var appServicePlanName = '${projectName}-${environment}-plan'
var webAppName = '${projectName}-web-${environment}'
var apiAppName = '${projectName}-api-${environment}'
var storageAccountName = '${projectName}${environment}storage'

// App Service Plan - B1 tier for cost efficiency with good performance
resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: appServicePlanName
  location: location
  kind: 'linux'
  properties: {
    reserved: true
  }
  sku: {
    name: 'B1'
    tier: 'Basic'
    size: 'B1'
    capacity: 1
  }
  tags: {
    project: 'World Leaders Game'
    environment: environment
  }
}

// Storage Account for static assets and backups
resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageAccountName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: 'Hot'
    allowBlobPublicAccess: true
    minimumTlsVersion: 'TLS1_2'
  }
  tags: {
    project: 'World Leaders Game'
    environment: environment
  }
}

// Blob container for game assets
resource gameAssetsContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-09-01' = {
  name: '${storageAccount.name}/default/game-assets'
  properties: {
    publicAccess: 'Blob'
  }
}

// Application Insights for monitoring
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${projectName}-${environment}-insights'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    IngestionMode: 'ApplicationInsights'
  }
  tags: {
    project: 'World Leaders Game'
    environment: environment
  }
}

// Web App for Blazor Server application (Main Game)
resource webApp 'Microsoft.Web/sites@2022-09-01' = {
  name: webAppName
  location: location
  kind: 'app,linux'
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      alwaysOn: true
      http20Enabled: true
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environment
        }
        {
          name: 'ApiSettings__BaseUrl'
          value: customDomainName != '' ? 'https://${customDomainName}/api' : 'https://${apiAppName}.azurewebsites.net'
        }
        {
          name: 'WEBSITE_ENABLE_SYNC_UPDATE_SITE'
          value: 'true'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
      ]
    }
  }
  tags: {
    project: 'World Leaders Game'
    environment: environment
    component: 'web-app'
  }
}

// API App Service for Game API
resource apiApp 'Microsoft.Web/sites@2022-09-01' = {
  name: apiAppName
  location: location
  kind: 'app,linux'
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      alwaysOn: true
      http20Enabled: true
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      cors: {
        allowedOrigins: [
          customDomainName != '' ? 'https://${customDomainName}' : 'https://${webAppName}.azurewebsites.net'
          'https://localhost:7154' // Development
        ]
        supportCredentials: true
      }
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environment
        }
        {
          name: 'WEBSITE_ENABLE_SYNC_UPDATE_SITE'
          value: 'true'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'AllowedOrigins__Web'
          value: customDomainName != '' ? 'https://${customDomainName}' : 'https://${webAppName}.azurewebsites.net'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
      ]
    }
  }
  tags: {
    project: 'World Leaders Game'
    environment: environment
    component: 'api'
  }
}

// Output important URLs and configuration
output webAppUrl string = 'https://${webApp.properties.defaultHostName}'
output apiAppUrl string = 'https://${apiApp.properties.defaultHostName}'
output storageAccountName string = storageAccount.name
output appInsightsInstrumentationKey string = appInsights.properties.InstrumentationKey

// Custom domain configuration instructions
output customDomainInstructions object = {
  mainDomain: {
    service: 'Web App'
    name: webAppName
    instruction: 'Add CNAME record pointing to ${webApp.properties.defaultHostName}'
  }
  apiSubdomain: {
    service: 'API App' 
    name: apiAppName
    instruction: 'Add CNAME record pointing to ${apiApp.properties.defaultHostName} or configure path-based routing'
  }
  nextSteps: {
    documentation: 'Deploy docs using GitHub Pages or add Static Web App separately'
    customDomain: 'Run setup-custom-domain.sh script for advanced routing'
  }
}
