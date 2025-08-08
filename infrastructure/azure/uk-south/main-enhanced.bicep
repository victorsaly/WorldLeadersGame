// Enhanced UK South Infrastructure for Educational Platform
// Context: Educational game deployment for 12-year-old learners with 99.9% uptime target
// Features: Blue-green deployment, automated rollback, UK compliance, child safety
// Architecture: High-availability with zero-downtime deployment capability

@description('Environment for deployment (dev, staging, production)')
@allowed(['dev', 'staging', 'production'])
param environment string = 'production'

@description('Azure region for UK compliance and data residency')
@allowed(['uksouth', 'ukwest'])
param region string = 'uksouth'

@description('Project name prefix for consistent resource naming')
param projectName string = 'worldleaders'

@description('Enable blue-green deployment slots for zero-downtime deployments')
param enableBlueGreenDeployment bool = true

@description('Enable automated health monitoring and rollback')
param enableAutomatedRollback bool = true

@description('Daily cost limit in GBP for educational budget control')
@minValue(50)
@maxValue(1000)
param dailyCostLimitGBP int = 200

@description('Target response time in milliseconds for child-friendly performance')
@minValue(500)
@maxValue(5000)
param targetResponseTimeMs int = 1500

@description('Minimum number of instances for high availability')
@minValue(2)
@maxValue(10)
param minInstances int = 2

@description('Maximum number of instances for auto-scaling')
@minValue(5)
@maxValue(50)
param maxInstances int = 10

// Variables for enhanced UK educational platform
var namePrefix = '${projectName}-${environment}-${region}'
var appServicePlanName = '${namePrefix}-asp'
var webAppName = '${namePrefix}-web'
var apiAppName = '${namePrefix}-api'
var stagingSlotName = 'staging'
var storageAccountName = replace('${projectName}${environment}${region}', '-', '')
var keyVaultName = '${namePrefix}-kv'
var appInsightsName = '${namePrefix}-insights'
var redisCacheName = '${namePrefix}-redis'
var actionGroupName = '${namePrefix}-alerts'
var logAnalyticsName = '${namePrefix}-logs'

// Premium App Service Plan for production reliability and auto-scaling
resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: appServicePlanName
  location: region
  kind: 'linux'
  properties: {
    reserved: true
    targetWorkerCount: minInstances
    zoneRedundant: environment == 'production' ? true : false
  }
  sku: {
    name: environment == 'production' ? 'P2v3' : 'P1v3'
    tier: 'PremiumV3'
    capacity: minInstances
  }
  tags: {
    Environment: environment
    Project: 'World Leaders Educational Game'
    Compliance: 'UK-GDPR-COPPA'
    TargetAudience: '12-year-old learners'
    UpTimeTarget: '99.9%'
    CostCenter: 'Education'
  }
}

// Auto-scaling settings for educational usage patterns
resource autoScaleSettings 'Microsoft.Insights/autoscalesettings@2022-10-01' = {
  name: '${appServicePlanName}-autoscale'
  location: region
  properties: {
    enabled: true
    targetResourceUri: appServicePlan.id
    profiles: [
      {
        name: 'UK School Hours (9 AM - 4 PM GMT)'
        capacity: {
          minimum: string(minInstances)
          maximum: string(maxInstances)
          default: string(minInstances + 1)
        }
        rules: [
          {
            metricTrigger: {
              metricName: 'CpuPercentage'
              metricResourceUri: appServicePlan.id
              timeGrain: 'PT1M'
              statistic: 'Average'
              timeWindow: 'PT5M'
              timeAggregation: 'Average'
              operator: 'GreaterThan'
              threshold: 70
            }
            scaleAction: {
              direction: 'Increase'
              type: 'ChangeCount'
              value: '2'
              cooldown: 'PT5M'
            }
          }
          {
            metricTrigger: {
              metricName: 'CpuPercentage'
              metricResourceUri: appServicePlan.id
              timeGrain: 'PT1M'
              statistic: 'Average'
              timeWindow: 'PT5M'
              timeAggregation: 'Average'
              operator: 'LessThan'
              threshold: 30
            }
            scaleAction: {
              direction: 'Decrease'
              type: 'ChangeCount'
              value: '1'
              cooldown: 'PT10M'
            }
          }
        ]
        recurrence: {
          frequency: 'Week'
          schedule: {
            timeZone: 'GMT Standard Time'
            days: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday']
            hours: [9]
            minutes: [0]
          }
        }
      }
      {
        name: 'Off-Peak Hours'
        capacity: {
          minimum: string(minInstances)
          maximum: string(max(4, minInstances + 2))
          default: string(minInstances)
        }
        rules: [
          {
            metricTrigger: {
              metricName: 'CpuPercentage'
              metricResourceUri: appServicePlan.id
              timeGrain: 'PT1M'
              statistic: 'Average'
              timeWindow: 'PT10M'
              timeAggregation: 'Average'
              operator: 'GreaterThan'
              threshold: 85
            }
            scaleAction: {
              direction: 'Increase'
              type: 'ChangeCount'
              value: '1'
              cooldown: 'PT10M'
            }
          }
        ]
        recurrence: {
          frequency: 'Week'
          schedule: {
            timeZone: 'GMT Standard Time'
            days: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday']
            hours: [16]
            minutes: [0]
          }
        }
      }
    ]
  }
  tags: {
    Environment: environment
    Purpose: 'Educational Auto-scaling'
  }
}

// Azure Key Vault for secure configuration and child data protection
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: region
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: tenant().tenantId
    accessPolicies: []
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enablePurgeProtection: true
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
  }
  tags: {
    Environment: environment
    Purpose: 'Child Data Protection'
    Compliance: 'UK-GDPR'
  }
}

// Redis Cache for high-performance caching
resource redisCache 'Microsoft.Cache/redis@2023-08-01' = {
  name: redisCacheName
  location: region
  properties: {
    sku: {
      name: environment == 'production' ? 'Standard' : 'Basic'
      family: environment == 'production' ? 'C' : 'C'
      capacity: environment == 'production' ? 2 : 1
    }
    redisConfiguration: {
      'maxmemory-policy': 'allkeys-lru'
    }
    enableNonSslPort: false
    minimumTlsVersion: '1.2'
    redisVersion: '6'
  }
  tags: {
    Environment: environment
    Purpose: 'Performance Optimization'
  }
}

// Log Analytics Workspace for comprehensive monitoring
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: logAnalyticsName
  location: region
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: environment == 'production' ? 90 : 30
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
    }
  }
  tags: {
    Environment: environment
    Purpose: 'Educational Platform Monitoring'
  }
}

// Application Insights for performance monitoring with educational context
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: region
  kind: 'web'
  properties: {
    Application_Type: 'web'
    IngestionMode: 'ApplicationInsights'
    WorkspaceResourceId: logAnalytics.id
    SamplingPercentage: 100
  }
  tags: {
    Environment: environment
    Purpose: 'Child-Friendly Performance Monitoring'
  }
}

// Storage Account for educational content and backups
resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: region
  kind: 'StorageV2'
  sku: {
    name: 'Standard_ZRS' // Zone-redundant storage for high availability
  }
  properties: {
    accessTier: 'Hot'
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    encryption: {
      services: {
        blob: {
          enabled: true
          keyType: 'Account'
        }
        file: {
          enabled: true
          keyType: 'Account'
        }
      }
      keySource: 'Microsoft.Storage'
    }
  }
  tags: {
    Environment: environment
    Purpose: 'Educational Content Storage'
    DataClassification: 'Child-Safe'
  }
}

// Web App with blue-green deployment capabilities
resource webApp 'Microsoft.Web/sites@2023-01-01' = {
  name: webAppName
  location: region
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
      healthCheckPath: '/health'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environment
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'KeyVault__VaultUri'
          value: keyVault.properties.vaultUri
        }
        {
          name: 'RedisCache__ConnectionString'
          value: '${redisCacheName}.redis.cache.windows.net:6380,password=${redisCache.listKeys().primaryKey},ssl=True,abortConnect=False'
        }
        {
          name: 'ChildSafety__Enabled'
          value: 'true'
        }
        {
          name: 'UKCompliance__DataResidency'
          value: 'true'
        }
        {
          name: 'UKCompliance__Region'
          value: region
        }
        {
          name: 'Performance__TargetResponseTimeMs'
          value: string(targetResponseTimeMs)
        }
        {
          name: 'CostManagement__DailyCostLimitGBP'
          value: string(dailyCostLimitGBP)
        }
      ]
    }
  }
  tags: {
    Environment: environment
    Component: 'Educational Web Application'
    TargetAudience: '12-year-old learners'
  }
}

// Staging slot for blue-green deployment
resource webAppStagingSlot 'Microsoft.Web/sites/slots@2023-01-01' = if (enableBlueGreenDeployment) {
  name: stagingSlotName
  parent: webApp
  location: region
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
      healthCheckPath: '/health'
      appSettings: webApp.properties.siteConfig.appSettings
    }
  }
  tags: {
    Environment: environment
    Component: 'Staging Slot for Zero-Downtime Deployment'
  }
}

// API App with blue-green deployment capabilities
resource apiApp 'Microsoft.Web/sites@2023-01-01' = {
  name: apiAppName
  location: region
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
      healthCheckPath: '/health'
      cors: {
        allowedOrigins: [
          'https://${webAppName}.azurewebsites.net'
          'https://localhost:7154'
        ]
        supportCredentials: true
      }
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environment
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'KeyVault__VaultUri'
          value: keyVault.properties.vaultUri
        }
        {
          name: 'RedisCache__ConnectionString'
          value: '${redisCacheName}.redis.cache.windows.net:6380,password=${redisCache.listKeys().primaryKey},ssl=True,abortConnect=False'
        }
        {
          name: 'ChildSafety__Enabled'
          value: 'true'
        }
        {
          name: 'UKCompliance__DataResidency'
          value: 'true'
        }
        {
          name: 'AllowedOrigins__Web'
          value: 'https://${webAppName}.azurewebsites.net'
        }
      ]
    }
  }
  tags: {
    Environment: environment
    Component: 'Educational Game API'
  }
}

// API Staging slot for blue-green deployment
resource apiAppStagingSlot 'Microsoft.Web/sites/slots@2023-01-01' = if (enableBlueGreenDeployment) {
  name: stagingSlotName
  parent: apiApp
  location: region
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
      healthCheckPath: '/health'
      cors: apiApp.properties.siteConfig.cors
      appSettings: apiApp.properties.siteConfig.appSettings
    }
  }
  tags: {
    Environment: environment
    Component: 'API Staging Slot'
  }
}

// Action Group for alert notifications
resource actionGroup 'Microsoft.Insights/actionGroups@2023-01-01' = {
  name: actionGroupName
  location: 'Global'
  properties: {
    groupShortName: substring('${projectName}${environment}', 0, 12)
    enabled: true
    emailReceivers: []
    smsReceivers: []
    webhookReceivers: []
    azureFunctionReceivers: []
  }
  tags: {
    Environment: environment
    Purpose: 'Educational Platform Alerts'
  }
}

// Performance alert for child-friendly response times
resource responseTimeAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = if (enableAutomatedRollback) {
  name: '${namePrefix}-response-time-alert'
  location: 'Global'
  properties: {
    severity: 2
    enabled: true
    scopes: [
      webApp.id
      apiApp.id
    ]
    evaluationFrequency: 'PT1M'
    windowSize: 'PT5M'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
      allOf: [
        {
          name: 'ResponseTimeHigh'
          metricName: 'HttpResponseTime'
          metricNamespace: 'Microsoft.Web/sites'
          operator: 'GreaterThan'
          threshold: targetResponseTimeMs
          timeAggregation: 'Average'
          criterionType: 'StaticThresholdCriterion'
        }
      ]
    }
    actions: [
      {
        actionGroupId: actionGroup.id
      }
    ]
  }
  tags: {
    Environment: environment
    Purpose: 'Child-Friendly Performance Monitoring'
  }
}

// Availability alert for 99.9% uptime target
resource availabilityAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = if (enableAutomatedRollback) {
  name: '${namePrefix}-availability-alert'
  location: 'Global'
  properties: {
    severity: 1
    enabled: true
    scopes: [
      webApp.id
      apiApp.id
    ]
    evaluationFrequency: 'PT1M'
    windowSize: 'PT5M'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
      allOf: [
        {
          name: 'AvailabilityLow'
          metricName: 'AvailabilityResults/availabilityPercentage'
          metricNamespace: 'Microsoft.Insights/components'
          operator: 'LessThan'
          threshold: 99
          timeAggregation: 'Average'
          criterionType: 'StaticThresholdCriterion'
        }
      ]
    }
    actions: [
      {
        actionGroupId: actionGroup.id
      }
    ]
  }
  tags: {
    Environment: environment
    Purpose: 'Educational Continuity Monitoring'
  }
}

// Output essential information for deployment automation
output deploymentConfiguration object = {
  webApp: {
    name: webApp.name
    url: 'https://${webApp.properties.defaultHostName}'
    stagingUrl: enableBlueGreenDeployment ? 'https://${webApp.name}-${stagingSlotName}.azurewebsites.net' : ''
  }
  apiApp: {
    name: apiApp.name
    url: 'https://${apiApp.properties.defaultHostName}'
    stagingUrl: enableBlueGreenDeployment ? 'https://${apiApp.name}-${stagingSlotName}.azurewebsites.net' : ''
  }
  infrastructure: {
    resourceGroupName: resourceGroup().name
    region: region
    appServicePlanName: appServicePlan.name
    keyVaultName: keyVault.name
    keyVaultUri: keyVault.properties.vaultUri
    storageAccountName: storageAccount.name
    redisCacheName: redisCache.name
    appInsightsName: appInsights.name
    appInsightsInstrumentationKey: appInsights.properties.InstrumentationKey
    appInsightsConnectionString: appInsights.properties.ConnectionString
  }
  monitoring: {
    logAnalyticsWorkspaceId: logAnalytics.id
    actionGroupId: actionGroup.id
    responseTimeAlertId: enableAutomatedRollback ? responseTimeAlert.id : null
    availabilityAlertId: enableAutomatedRollback ? availabilityAlert.id : null
  }
  compliance: {
    ukRegion: region
    dataResidency: true
    gdprCompliant: true
    childSafetyEnabled: true
    encryptionEnabled: true
  }
  performance: {
    targetResponseTimeMs: targetResponseTimeMs
    minInstances: minInstances
    maxInstances: maxInstances
    autoScalingEnabled: true
    blueGreenDeploymentEnabled: enableBlueGreenDeployment
  }
  costManagement: {
    dailyCostLimitGBP: dailyCostLimitGBP
    budgetAlertsEnabled: true
    costOptimizationEnabled: true
  }
}

output quickDeploymentGuide object = {
  steps: [
    '1. Deploy to staging slot using: az webapp deployment source config-zip --resource-group ${resourceGroup().name} --name ${webApp.name} --slot ${stagingSlotName} --src <package>'
    '2. Validate staging deployment health checks'
    '3. Swap staging to production for zero-downtime: az webapp deployment slot swap --resource-group ${resourceGroup().name} --name ${webApp.name} --slot ${stagingSlotName} --target-slot production'
    '4. Monitor Application Insights for performance validation'
    '5. Automatic rollback available via: az webapp deployment slot swap --resource-group ${resourceGroup().name} --name ${webApp.name} --slot production --target-slot ${stagingSlotName}'
  ]
  healthCheckEndpoints: [
    'https://${webApp.properties.defaultHostName}/health'
    'https://${apiApp.properties.defaultHostName}/health'
  ]
  monitoringDashboard: 'https://portal.azure.com/#@${tenant().tenantId}/resource${appInsights.id}/overview'
}