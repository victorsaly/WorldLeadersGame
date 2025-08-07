@description('Auto-scaling App Service Plan for Educational Platform - UK South Region')
@description('Optimized for 1000+ concurrent users with child-friendly performance requirements')

@allowed(['dev', 'staging', 'production'])
param environment string = 'production'

@allowed(['uksouth', 'ukwest'])
param region string = 'uksouth'

@description('Educational platform name prefix')
param appNamePrefix string = 'worldleaders'

@minValue(1)
@maxValue(20)
param minInstances int = 2

@minValue(2)
@maxValue(50)
param maxInstances int = 10

@description('Target CPU percentage for scaling out (educational peak times)')
@minValue(50)
@maxValue(90)
param scaleOutCpuThreshold int = 70

@description('Target CPU percentage for scaling in (off-peak hours)')
@minValue(20)
@maxValue(60)
param scaleInCpuThreshold int = 30

// Variables for educational platform optimization
var appServicePlanName = '${appNamePrefix}-asp-${environment}-${region}'
var autoScaleSettingName = '${appServicePlanName}-autoscale'
var skuName = environment == 'production' ? 'P2v3' : 'P1v3' // Premium for production scaling
var skuTier = 'PremiumV3'
var workerSize = environment == 'production' ? '2' : '1' // Larger workers for production

// Educational usage patterns (UK school hours optimization)
var schoolHoursStart = '09:00'
var schoolHoursEnd = '16:00'
var timeZone = 'GMT Standard Time'

resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: appServicePlanName
  location: region
  kind: 'linux'
  properties: {
    reserved: true // Linux App Service Plan
    targetWorkerCount: minInstances
    targetWorkerSizeId: int(workerSize)
    // Educational platform optimization
    zoneRedundant: environment == 'production' ? true : false
    // Performance optimization for child-friendly response times
    hyperV: false
    isSpot: false
    spotExpirationTime: null
  }
  sku: {
    name: skuName
    tier: skuTier
    size: skuName
    family: 'Pv3'
    capacity: minInstances
  }
  tags: {
    Environment: environment
    Region: region
    Purpose: 'Educational Platform'
    TargetUsers: '1000+ concurrent'
    ChildSafety: 'COPPA compliant'
    DataResidency: 'UK'
    CostCenter: 'Education'
    AutoScaling: 'Enabled'
  }
}

// Auto-scaling settings optimized for educational usage patterns
resource autoScaleSettings 'Microsoft.Insights/autoscalesettings@2022-10-01' = {
  name: autoScaleSettingName
  location: region
  properties: {
    enabled: true
    targetResourceUri: appServicePlan.id
    // Educational platform profiles
    profiles: [
      {
        name: 'School Hours Profile (9 AM - 4 PM GMT)'
        capacity: {
          minimum: string(minInstances)
          maximum: string(maxInstances)
          default: string(minInstances + 1)
        }
        rules: [
          // Scale out rule for peak educational hours
          {
            metricTrigger: {
              metricName: 'CpuPercentage'
              metricResourceUri: appServicePlan.id
              timeGrain: 'PT1M'
              statistic: 'Average'
              timeWindow: 'PT5M'
              timeAggregation: 'Average'
              operator: 'GreaterThan'
              threshold: scaleOutCpuThreshold
            }
            scaleAction: {
              direction: 'Increase'
              type: 'ChangeCount'
              value: '2' // Scale out by 2 instances for faster response
              cooldown: 'PT5M'
            }
          }
          // Scale in rule for normal operation
          {
            metricTrigger: {
              metricName: 'CpuPercentage'
              metricResourceUri: appServicePlan.id
              timeGrain: 'PT1M'
              statistic: 'Average'
              timeWindow: 'PT15M'
              timeAggregation: 'Average'
              operator: 'LessThan'
              threshold: scaleInCpuThreshold
            }
            scaleAction: {
              direction: 'Decrease'
              type: 'ChangeCount'
              value: '1'
              cooldown: 'PT10M'
            }
          }
          // Memory-based scaling for child-friendly performance
          {
            metricTrigger: {
              metricName: 'MemoryPercentage'
              metricResourceUri: appServicePlan.id
              timeGrain: 'PT1M'
              statistic: 'Average'
              timeWindow: 'PT5M'
              timeAggregation: 'Average'
              operator: 'GreaterThan'
              threshold: 80
            }
            scaleAction: {
              direction: 'Increase'
              type: 'ChangeCount'
              value: '1'
              cooldown: 'PT5M'
            }
          }
        ]
        recurrence: {
          frequency: 'Week'
          schedule: {
            timeZone: timeZone
            days: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday']
            hours: [9]
            minutes: [0]
          }
        }
        fixedDate: null
      }
      {
        name: 'Off-Peak Hours Profile (Evenings & Weekends)'
        capacity: {
          minimum: string(minInstances)
          maximum: string(max(4, minInstances + 2)) // Lower max for off-peak
          default: string(minInstances)
        }
        rules: [
          // Conservative scaling for off-peak hours
          {
            metricTrigger: {
              metricName: 'CpuPercentage'
              metricResourceUri: appServicePlan.id
              timeGrain: 'PT1M'
              statistic: 'Average'
              timeWindow: 'PT10M'
              timeAggregation: 'Average'
              operator: 'GreaterThan'
              threshold: 85 // Higher threshold for off-peak
            }
            scaleAction: {
              direction: 'Increase'
              type: 'ChangeCount'
              value: '1'
              cooldown: 'PT10M'
            }
          }
          {
            metricTrigger: {
              metricName: 'CpuPercentage'
              metricResourceUri: appServicePlan.id
              timeGrain: 'PT1M'
              statistic: 'Average'
              timeWindow: 'PT20M'
              timeAggregation: 'Average'
              operator: 'LessThan'
              threshold: 25 // Lower threshold for off-peak
            }
            scaleAction: {
              direction: 'Decrease'
              type: 'ChangeCount'
              value: '1'
              cooldown: 'PT15M'
            }
          }
        ]
        recurrence: {
          frequency: 'Week'
          schedule: {
            timeZone: timeZone
            days: ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday']
            hours: [16]
            minutes: [0]
          }
        }
        fixedDate: null
      }
    ]
    // Notifications for scaling events
    notifications: [
      {
        operation: 'Scale'
        email: {
          sendToSubscriptionAdministrator: true
          sendToSubscriptionCoAdministrators: false
          customEmails: []
        }
      }
    ]
  }
  tags: {
    Environment: environment
    Region: region
    Purpose: 'Educational Auto-scaling'
    OptimizedFor: 'UK School Hours'
  }
}

// Output values for use in other templates
output appServicePlanId string = appServicePlan.id
output appServicePlanName string = appServicePlan.name
output autoScaleSettingsId string = autoScaleSettings.id
output resourceGroupName string = resourceGroup().name
output region string = region
output skuInfo object = {
  name: skuName
  tier: skuTier
  capacity: minInstances
}
output scalingConfiguration object = {
  minInstances: minInstances
  maxInstances: maxInstances
  scaleOutThreshold: scaleOutCpuThreshold
  scaleInThreshold: scaleInCpuThreshold
  schoolHours: '${schoolHoursStart} - ${schoolHoursEnd} ${timeZone}'
}