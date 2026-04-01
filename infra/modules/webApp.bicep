targetScope = 'resourceGroup'

param location string
param webAppName string
param appServicePlanResourceId string
param osType string = 'linux'
param linuxFxVersion string
param alwaysOn bool = true
param tags object = {}

var isLinux = toLower(osType) == 'linux'
var siteConfig = union({
  alwaysOn: alwaysOn
  http20Enabled: true
  ftpsState: 'Disabled'
  minTlsVersion: '1.2'
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
    siteConfig: siteConfig
  }
}

output webAppResourceId string = webApp.id
output defaultHostName string = webApp.properties.defaultHostName
output webAppIdentityPrincipalId string = webApp.identity.principalId
