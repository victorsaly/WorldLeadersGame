// Azure PostgreSQL Flexible Server for World Leaders Educational Game
// Cost-optimized configuration for educational use

@description('Environment name (dev, staging, prod)')
param environment string = 'prod'

@description('Location for all resources')
param location string = resourceGroup().location

@description('PostgreSQL server administrator login name')
@secure()
param administratorLogin string

@description('PostgreSQL server administrator password')
@secure()
param administratorPassword string

@description('Database name')
param databaseName string = 'worldleaders'

@description('Application name for resource naming')
param appName string = 'worldleaders'

// Variables for resource naming
var postgresServerName = '${appName}-postgres-${environment}'
var databaseNameFull = '${databaseName}_${environment}'

// Cost-optimized PostgreSQL Flexible Server
resource postgresServer 'Microsoft.DBforPostgreSQL/flexibleServers@2023-03-01-preview' = {
  name: postgresServerName
  location: location
  sku: {
    name: 'Standard_B1ms'  // Burstable tier - cost effective for educational use
    tier: 'Burstable'
  }
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorPassword
    version: '15'  // Latest stable PostgreSQL version
    storage: {
      storageSizeGB: 32  // Minimum size for cost optimization
      autoGrow: 'Enabled'  // Allow growth when needed
    }
    backup: {
      backupRetentionDays: 7  // Minimum retention for cost optimization
      geoRedundantBackup: 'Disabled'  // Disabled for cost optimization
    }
    highAvailability: {
      mode: 'Disabled'  // Disabled for cost optimization in educational environment
    }
    network: {
      // Public network access is controlled via firewall rules
    }
    authConfig: {
      activeDirectoryAuth: 'Disabled'
      passwordAuth: 'Enabled'
    }
  }

  tags: {
    Environment: environment
    Application: 'WorldLeadersGame'
    Purpose: 'Educational'
    'Cost-Center': 'Educational-Technology'
  }
}

// Firewall rule to allow Azure services
resource allowAzureServices 'Microsoft.DBforPostgreSQL/flexibleServers/firewallRules@2023-03-01-preview' = {
  parent: postgresServer
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'  // Special range that allows Azure services
  }
}

// Optional: Allow specific IP ranges for development
resource allowDevelopment 'Microsoft.DBforPostgreSQL/flexibleServers/firewallRules@2023-03-01-preview' = if (environment == 'dev') {
  parent: postgresServer
  name: 'AllowDevelopment'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '255.255.255.255'  // Open for development only
  }
}

// Create the database
resource database 'Microsoft.DBforPostgreSQL/flexibleServers/databases@2023-03-01-preview' = {
  parent: postgresServer
  name: databaseNameFull
  properties: {
    charset: 'UTF8'
    collation: 'en_US.utf8'
  }
}

// Outputs for App Service configuration
output postgresServerName string = postgresServer.name
output postgresServerFQDN string = postgresServer.properties.fullyQualifiedDomainName
output databaseName string = database.name
output connectionStringFormat string = 'Host={serverFQDN};Database={databaseName};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true'

// Cost monitoring outputs
output estimatedMonthlyCost string = 'Approximately £15-25/month for Burstable B1ms instance'
output costOptimizations string = 'Using Burstable tier, minimal storage, disabled HA and geo-backup for educational cost optimization'
