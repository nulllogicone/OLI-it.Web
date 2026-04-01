targetScope = 'resourceGroup'

param location string = resourceGroup().location
param webAppName string
param existingAppServicePlanResourceId string
param existingLogAnalyticsWorkspaceResourceId string
param existingKeyVaultResourceId string = ''
param osType string = 'windows'
param linuxFxVersion string = 'DOTNETCORE|8.0'
param alwaysOn bool = true
param tags object = {}

var keyVaultSubscriptionId = !empty(existingKeyVaultResourceId) ? split(existingKeyVaultResourceId, '/')[2] : ''
var keyVaultResourceGroupName = !empty(existingKeyVaultResourceId) ? split(existingKeyVaultResourceId, '/')[4] : ''
var keyVaultName = !empty(existingKeyVaultResourceId) ? split(existingKeyVaultResourceId, '/')[8] : ''

module webApp './modules/webApp.bicep' = {
  name: 'webApp-${webAppName}'
  params: {
    location: location
    webAppName: webAppName
    appServicePlanResourceId: existingAppServicePlanResourceId
    osType: osType
    linuxFxVersion: linuxFxVersion
    alwaysOn: alwaysOn
    tags: tags
  }
}

module keyVaultAccess './modules/keyVaultAccessPolicy.bicep' = if (!empty(existingKeyVaultResourceId)) {
  name: 'kv-access-${webAppName}'
  scope: resourceGroup(keyVaultSubscriptionId, keyVaultResourceGroupName)
  params: {
    keyVaultName: keyVaultName
    tenantId: subscription().tenantId
    objectId: webApp.outputs.webAppIdentityPrincipalId
  }
}

module webAppDiagnostics './modules/diagnosticSettings.bicep' = {
  name: 'diag-${webAppName}'
  params: {
    targetWebAppName: webAppName
    diagnosticSettingName: '${webAppName}-to-law'
    logAnalyticsWorkspaceResourceId: existingLogAnalyticsWorkspaceResourceId
  }
}

output webAppResourceId string = webApp.outputs.webAppResourceId
output webAppDefaultHostName string = webApp.outputs.defaultHostName
