param location string = resourceGroup().location

module storage 'storage.bicep' = {
  name: toLower('${resourceGroup().name}strg')
}

var shared_config = [
  {
    name: 'ORLEANS_AZURE_STORAGE_CONNECTION_STRING'
    value: format('DefaultEndpointsProtocol=https;AccountName=${storage.outputs.storageName};AccountKey=${storage.outputs.accountKey};EndpointSuffix=core.windows.net')
  }
]

var silo_config = [
  {
    name: 'ORLEANS_SILO_NAME'
    value: 'Orleans Silo'
  }
]

var dashboard_silo_config = [
  {
    name: 'ORLEANS_SILO_NAME'
    value: 'Orleans Dashboard'
  }
]

module logs 'logs-and-insights.bicep' = {
  name: 'logs-and-insights'
  params: {
    location: location
  }
}

resource vnet 'Microsoft.Network/virtualNetworks@2021-03-01' = {
  name: '${resourceGroup().name}vnet'
  location: resourceGroup().location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '172.17.0.0/16'
      ]
    }
    subnets: [
      {
        name: 'default'
        properties: {
          addressPrefix: '172.17.0.0/24'
          delegations: [
            {
              name: 'delegation'
              properties: {
                serviceName: 'Microsoft.Web/serverFarms'
              }
            }
          ]
        }
      }
    ]
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: '${resourceGroup().name}plan'
  location: resourceGroup().location
  kind: 'app'
  sku: {
    name: 'S1'
    capacity: 1
  }
}

module silo 'app-service.bicep' = {
  name: 'silo'
  params: {
    name: '${resourceGroup().name}-silo'
    appServicePlanId: appServicePlan.id
    vnetSubnetId: vnet.properties.subnets[0].id
    envVars: union(shared_config, silo_config)
  }
}

module dashboard 'app-service.bicep' = {
  name: 'dashboard'
  params: {
    name: '${resourceGroup().name}-dashboard'
    appServicePlanId: appServicePlan.id
    vnetSubnetId: vnet.properties.subnets[0].id
    envVars: union(shared_config, dashboard_silo_config)
  }
}

module client 'app-service.bicep' = {
  name: 'client'
  params: {
    name: '${resourceGroup().name}-client'
    appServicePlanId: appServicePlan.id
    vnetSubnetId: vnet.properties.subnets[0].id
    envVars: shared_config
  }
}
