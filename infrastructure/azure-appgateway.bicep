// Azure Application Gateway configuration for World Leaders Game
// Context: Path-based routing for educational game deployment
// Routes: / → Web App, /api → API App, /docs → Static Web App

param projectName string = 'worldleaders'
param environment string = 'prod'
param location string = 'uksouth'
param customDomainName string
param webAppBackendFqdn string
param apiAppBackendFqdn string
param docsBackendFqdn string

var applicationGatewayName = '${projectName}-${environment}-appgw'
var publicIpName = '${projectName}-${environment}-pip'
var vnetName = '${projectName}-${environment}-vnet'
var subnetName = 'appgw-subnet'

// Virtual Network for Application Gateway
resource vnet 'Microsoft.Network/virtualNetworks@2022-07-01' = {
  name: vnetName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/16'
      ]
    }
    subnets: [
      {
        name: subnetName
        properties: {
          addressPrefix: '10.0.1.0/24'
        }
      }
    ]
  }
  tags: {
    project: 'World Leaders Game'
    environment: environment
  }
}

// Public IP for Application Gateway
resource publicIp 'Microsoft.Network/publicIPAddresses@2022-07-01' = {
  name: publicIpName
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {
    publicIPAllocationMethod: 'Static'
    dnsSettings: {
      domainNameLabel: '${projectName}-${environment}-gw'
    }
  }
  tags: {
    project: 'World Leaders Game'
    environment: environment
  }
}

// Application Gateway
resource applicationGateway 'Microsoft.Network/applicationGateways@2022-07-01' = {
  name: applicationGatewayName
  location: location
  properties: {
    sku: {
      name: 'Standard_v2'
      tier: 'Standard_v2'
      capacity: 1
    }
    gatewayIPConfigurations: [
      {
        name: 'appGatewayIpConfig'
        properties: {
          subnet: {
            id: resourceId('Microsoft.Network/virtualNetworks/subnets', vnetName, subnetName)
          }
        }
      }
    ]
    frontendIPConfigurations: [
      {
        name: 'appGwPublicFrontendIp'
        properties: {
          publicIPAddress: {
            id: publicIp.id
          }
        }
      }
    ]
    frontendPorts: [
      {
        name: 'port_80'
        properties: {
          port: 80
        }
      }
      {
        name: 'port_443'
        properties: {
          port: 443
        }
      }
    ]
    backendAddressPools: [
      {
        name: 'webAppBackendPool'
        properties: {
          backendAddresses: [
            {
              fqdn: webAppBackendFqdn
            }
          ]
        }
      }
      {
        name: 'apiBackendPool'
        properties: {
          backendAddresses: [
            {
              fqdn: apiAppBackendFqdn
            }
          ]
        }
      }
      {
        name: 'docsBackendPool'
        properties: {
          backendAddresses: [
            {
              fqdn: docsBackendFqdn
            }
          ]
        }
      }
    ]
    backendHttpSettingsCollection: [
      {
        name: 'webAppHttpSettings'
        properties: {
          port: 443
          protocol: 'Https'
          cookieBasedAffinity: 'Enabled'
          requestTimeout: 30
          hostName: webAppBackendFqdn
          pickHostNameFromBackendAddress: false
        }
      }
      {
        name: 'apiHttpSettings'
        properties: {
          port: 443
          protocol: 'Https'
          cookieBasedAffinity: 'Disabled'
          requestTimeout: 30
          hostName: apiAppBackendFqdn
          pickHostNameFromBackendAddress: false
        }
      }
      {
        name: 'docsHttpSettings'
        properties: {
          port: 443
          protocol: 'Https'
          cookieBasedAffinity: 'Disabled'
          requestTimeout: 30
          hostName: docsBackendFqdn
          pickHostNameFromBackendAddress: false
        }
      }
    ]
    httpListeners: [
      {
        name: 'httpListener'
        properties: {
          frontendIPConfiguration: {
            id: resourceId(
              'Microsoft.Network/applicationGateways/frontendIPConfigurations',
              applicationGatewayName,
              'appGwPublicFrontendIp'
            )
          }
          frontendPort: {
            id: resourceId('Microsoft.Network/applicationGateways/frontendPorts', applicationGatewayName, 'port_80')
          }
          protocol: 'Http'
          hostName: customDomainName
        }
      }
    ]
    requestRoutingRules: [
      {
        name: 'defaultRule'
        properties: {
          ruleType: 'PathBasedRouting'
          priority: 100
          httpListener: {
            id: resourceId(
              'Microsoft.Network/applicationGateways/httpListeners',
              applicationGatewayName,
              'httpListener'
            )
          }
          urlPathMap: {
            id: resourceId(
              'Microsoft.Network/applicationGateways/urlPathMaps',
              applicationGatewayName,
              'pathBasedRouting'
            )
          }
        }
      }
    ]
    urlPathMaps: [
      {
        name: 'pathBasedRouting'
        properties: {
          defaultBackendAddressPool: {
            id: resourceId(
              'Microsoft.Network/applicationGateways/backendAddressPools',
              applicationGatewayName,
              'webAppBackendPool'
            )
          }
          defaultBackendHttpSettings: {
            id: resourceId(
              'Microsoft.Network/applicationGateways/backendHttpSettingsCollection',
              applicationGatewayName,
              'webAppHttpSettings'
            )
          }
          pathRules: [
            {
              name: 'apiRule'
              properties: {
                paths: [
                  '/api/*'
                ]
                backendAddressPool: {
                  id: resourceId(
                    'Microsoft.Network/applicationGateways/backendAddressPools',
                    applicationGatewayName,
                    'apiBackendPool'
                  )
                }
                backendHttpSettings: {
                  id: resourceId(
                    'Microsoft.Network/applicationGateways/backendHttpSettingsCollection',
                    applicationGatewayName,
                    'apiHttpSettings'
                  )
                }
              }
            }
            {
              name: 'docsRule'
              properties: {
                paths: [
                  '/docs/*'
                ]
                backendAddressPool: {
                  id: resourceId(
                    'Microsoft.Network/applicationGateways/backendAddressPools',
                    applicationGatewayName,
                    'docsBackendPool'
                  )
                }
                backendHttpSettings: {
                  id: resourceId(
                    'Microsoft.Network/applicationGateways/backendHttpSettingsCollection',
                    applicationGatewayName,
                    'docsHttpSettings'
                  )
                }
              }
            }
          ]
        }
      }
    ]
  }
  tags: {
    project: 'World Leaders Game'
    environment: environment
  }
  dependsOn: [
    vnet
  ]
}

// Output Application Gateway public IP for DNS configuration
output applicationGatewayPublicIp string = publicIp.properties.ipAddress
output applicationGatewayFqdn string = publicIp.properties.dnsSettings.fqdn
output dnsConfiguration object = {
  instruction: 'Add A record for ${customDomainName} pointing to ${publicIp.properties.ipAddress}'
  testUrl: 'http://${customDomainName}'
  apiTestUrl: 'http://${customDomainName}/api/health'
  docsTestUrl: 'http://${customDomainName}/docs'
}
