targetScope = 'resourceGroup'

param location string
param webAppName string
param appServicePlanResourceId string
param osType string = 'linux'
param linuxFxVersion string
param alwaysOn bool = true
param tags object = {}
param keyVaultResourceId string = ''
param keyVaultTenantId string = ''

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

// Configure Key Vault access if KeyVault is provided
resource keyVaultAccessPolicy 'Microsoft.KeyVault/vaults/accessPolicies@2023-07-01' = if (!empty(keyVaultResourceId)) {
  name: '${split(keyVaultResourceId, '/')[8]}/add'
  properties: {
    accessPolicies: [
      {
        tenantId: keyVaultTenantId
        objectId: webApp.identity.principalId
        permissions: {
          secrets: [
            'get'
            'list'
          ]
        }
      }
    ]
  }
}

output webAppResourceId string = webApp.id
output defaultHostName string = webApp.properties.defaultHostName
output webAppIdentityPrincipalId string = webApp.identity.principalId
