targetScope = 'resourceGroup'

param location string = resourceGroup().location
param webAppName string
param existingAppServicePlanResourceId string
param existingLogAnalyticsWorkspaceResourceId string
param existingKeyVaultResourceId string = ''
param webAppSubnetResourceId string = ''
param keyVaultSecretUri string = ''
param osType string = 'windows'
param linuxFxVersion string = 'DOTNETCORE|8.0'
param alwaysOn bool = true
param tags object = {}

var keyVaultSubscriptionId = !empty(existingKeyVaultResourceId) ? split(existingKeyVaultResourceId, '/')[2] : ''
var keyVaultResourceGroupName = !empty(existingKeyVaultResourceId) ? split(existingKeyVaultResourceId, '/')[4] : ''
var keyVaultName = !empty(existingKeyVaultResourceId) ? split(existingKeyVaultResourceId, '/')[8] : ''

module applicationInsights './modules/applicationInsights.bicep' = {
  name: 'appi-${webAppName}'
  params: {
    location: location
    applicationInsightsName: '${webAppName}-appi'
    logAnalyticsWorkspaceResourceId: existingLogAnalyticsWorkspaceResourceId
    tags: tags
  }
}

module webApp './modules/webApp.bicep' = {
  name: 'webApp-${webAppName}'
  params: {
    location: location
    webAppName: webAppName
    appServicePlanResourceId: existingAppServicePlanResourceId
    osType: osType
    linuxFxVersion: linuxFxVersion
    alwaysOn: alwaysOn
    subnetResourceId: webAppSubnetResourceId
    appSettings: union(
      {
        APPLICATIONINSIGHTS_CONNECTION_STRING: applicationInsights.outputs.connectionString
        APPINSIGHTS_INSTRUMENTATIONKEY: applicationInsights.outputs.instrumentationKey
      },
      !empty(keyVaultSecretUri) ? {
        'ConnectionStrings__OliItDb': keyVaultSecretUri
      } : {}
    )
    tags: tags
  }
}

module keyVaultAccess './modules/keyVaultAccessPolicy.bicep' = if (!empty(existingKeyVaultResourceId)) {
  name: 'kv-access-${webAppName}'
  scope: resourceGroup(keyVaultSubscriptionId, keyVaultResourceGroupName)
  params: {
    keyVaultName: keyVaultName
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
output applicationInsightsResourceId string = applicationInsights.outputs.applicationInsightsResourceId
output applicationInsightsConnectionString string = applicationInsights.outputs.connectionString
