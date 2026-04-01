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
    keyVaultResourceId: existingKeyVaultResourceId
    keyVaultTenantId: subscription().tenantId
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
