@description('Base name for resources')
param appName string = 'librarymanager'

@description('Region')
param location string = resourceGroup().location

@description('Tag suffix')
param suffix string = 'demo'

var uniqueSuffix = uniqueString(resourceGroup().id)
var webAppName = '${appName}-${suffix}-${uniqueSuffix}'
var appServicePlanName = '${appName}-plan-${suffix}'

resource appServicePlan 'Microsoft.Web/serverfarms@2025-03-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'F1'
    tier: 'Free'
  }
}

resource webApp 'Microsoft.Web/sites@2025-03-01' = {
  name: webAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      netFrameworkVersion: 'v10.0'
      appSettings: [
        {
          name: 'Database__Provider'
          value: 'Sqlite'
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Demo'
        }
        {
          name: 'SEED_DEMO_DATA'
          value: 'true'
        }
        {
          name: 'ConnectionStrings__DefaultConnection'
          value: 'Data Source=D:\\home\\library.db'
        }
      ]
    }
  }
}

output webAppUrl string = 'https://${webApp.properties.defaultHostName}'
output webAppName string = webApp.name
