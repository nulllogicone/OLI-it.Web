targetScope = 'resourceGroup'

param location string
param webAppName string
param appServicePlanResourceId string
param osType string = 'linux'
param linuxFxVersion string
param alwaysOn bool = true
param subnetResourceId string = ''
param appSettings object = {}
param testSlotAppSettings object = {}
param tags object = {}

var isLinux = toLower(osType) == 'linux'
var appSettingsArray = [for setting in items(appSettings): {
  name: setting.key
  value: string(setting.value)
}]
var testSlotAppSettingsArray = [for setting in items(empty(testSlotAppSettings) ? appSettings : testSlotAppSettings): {
  name: setting.key
  value: string(setting.value)
}]
var siteConfig = union({
  alwaysOn: alwaysOn
  http20Enabled: true
  ftpsState: 'Disabled'
  minTlsVersion: '1.2'
  vnetRouteAllEnabled: !empty(subnetResourceId)
  appSettings: appSettingsArray
}, isLinux ? {
  linuxFxVersion: linuxFxVersion
} : {})
var testSlotSiteConfig = union({
  alwaysOn: alwaysOn
  http20Enabled: true
  ftpsState: 'Disabled'
  minTlsVersion: '1.2'
  vnetRouteAllEnabled: !empty(subnetResourceId)
  appSettings: testSlotAppSettingsArray
}, isLinux ? {
  linuxFxVersion: linuxFxVersion
} : {})

resource webApp 'Microsoft.Web/sites@2023-12-01' = {
  name: webAppName
  location: location
  kind: isLinux ? 'app,linux' : 'app'
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlanResourceId
    httpsOnly: true
    clientAffinityEnabled: false
    virtualNetworkSubnetId: !empty(subnetResourceId) ? subnetResourceId : null
    siteConfig: siteConfig
  }
}


resource testSlot 'Microsoft.Web/sites/slots@2023-12-01' = {
  parent: webApp
  name: 'test'
  location: location
  kind: isLinux ? 'app,linux' : 'app'
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlanResourceId
    httpsOnly: true
    clientAffinityEnabled: false
    virtualNetworkSubnetId: !empty(subnetResourceId) ? subnetResourceId : null
    siteConfig: testSlotSiteConfig
  }
}

output webAppResourceId string = webApp.id
output defaultHostName string = webApp.properties.defaultHostName
output webAppIdentityPrincipalId string = webApp.identity.principalId
output testSlotIdentityPrincipalId string = testSlot.identity.principalId
output testSlotHostName string = testSlot.properties.defaultHostName
