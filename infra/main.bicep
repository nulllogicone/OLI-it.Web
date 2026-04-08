targetScope = 'resourceGroup'

param location string = resourceGroup().location
param webAppName string
param existingAppServicePlanResourceId string
param existingLogAnalyticsWorkspaceResourceId string
param existingTestKeyVaultResourceId string = ''
param existingProdKeyVaultResourceId string = ''
param webAppSubnetResourceId string = ''
param testDbConnectionKeyName string = ''
param prodDbConnectionKeyName string = ''
param storageConnectionKeyName string = ''
@allowed([
  'both'
  'testOnly'
  'prodOnly'
])
param deploymentMode string = 'both'
param imagesRootUrl string = ''
param osType string = 'windows'
param linuxFxVersion string = 'DOTNETCORE|8.0'
param alwaysOn bool = true
param tags object = {}

var testKeyVaultSubscriptionId = !empty(existingTestKeyVaultResourceId) ? split(existingTestKeyVaultResourceId, '/')[2] : ''
var testKeyVaultResourceGroupName = !empty(existingTestKeyVaultResourceId) ? split(existingTestKeyVaultResourceId, '/')[4] : ''
var testKeyVaultName = !empty(existingTestKeyVaultResourceId) ? split(existingTestKeyVaultResourceId, '/')[8] : ''
var prodKeyVaultSubscriptionId = !empty(existingProdKeyVaultResourceId) ? split(existingProdKeyVaultResourceId, '/')[2] : ''
var prodKeyVaultResourceGroupName = !empty(existingProdKeyVaultResourceId) ? split(existingProdKeyVaultResourceId, '/')[4] : ''
var prodKeyVaultName = !empty(existingProdKeyVaultResourceId) ? split(existingProdKeyVaultResourceId, '/')[8] : ''
var deployTest = deploymentMode != 'prodOnly'
var deployProd = deploymentMode != 'testOnly'

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
    deployProdSettings: deployProd
    deployTestSettings: deployTest
    subnetResourceId: webAppSubnetResourceId
    appSettings: deployProd ? union(
      {
        APPLICATIONINSIGHTS_CONNECTION_STRING: applicationInsights.outputs.connectionString
        ApplicationInsightsAgent_EXTENSION_VERSION: toLower(osType) == 'linux' ? '~3' : '~2'
        XDT_MicrosoftApplicationInsights_Mode: 'recommended'
      },
      (!empty(prodKeyVaultName) && !empty(prodDbConnectionKeyName)) ? {
        ConnectionStrings__OliItDb: '@Microsoft.KeyVault(VaultName=${prodKeyVaultName};SecretName=${prodDbConnectionKeyName})'
      } : {},
      (!empty(prodKeyVaultName) && !empty(storageConnectionKeyName)) ? {
        ConnectionStrings__OliItStorageConnectionString: '@Microsoft.KeyVault(VaultName=${prodKeyVaultName};SecretName=${storageConnectionKeyName})'
      } : {},
      !empty(imagesRootUrl) ? {
        ImagesRootUrl: imagesRootUrl
      } : {},
      {
        slot: 'production'
      }
    ) : {}
    testSlotAppSettings: deployTest ? union(
      {
        APPLICATIONINSIGHTS_CONNECTION_STRING: applicationInsights.outputs.connectionString
        ApplicationInsightsAgent_EXTENSION_VERSION: toLower(osType) == 'linux' ? '~3' : '~2'
        XDT_MicrosoftApplicationInsights_Mode: 'recommended'
      },
      (!empty(testKeyVaultName) && !empty(testDbConnectionKeyName)) ? {
        ConnectionStrings__OliItDb: '@Microsoft.KeyVault(VaultName=${testKeyVaultName};SecretName=${testDbConnectionKeyName})'
      } : {},
      (!empty(testKeyVaultName) && !empty(storageConnectionKeyName)) ? {
        ConnectionStrings__OliItStorageConnectionString: '@Microsoft.KeyVault(VaultName=${testKeyVaultName};SecretName=${storageConnectionKeyName})'
      } : {},
      !empty(imagesRootUrl) ? {
        ImagesRootUrl: imagesRootUrl
      } : {},
      {
        slot: 'test'
      }
    ) : {}
    slotSettingAppSettingNames: [
      'ConnectionStrings__OliItDb'
      'ConnectionStrings__OliItStorageConnectionString'
      'slot'
    ]
    tags: tags
  }
}

module keyVaultAccessProd './modules/keyVaultAccessPolicy.bicep' = if (deployProd && !empty(existingProdKeyVaultResourceId)) {
  name: 'kv-access-prod-${webAppName}'
  scope: resourceGroup(prodKeyVaultSubscriptionId, prodKeyVaultResourceGroupName)
  params: {
    keyVaultName: prodKeyVaultName
    objectId: webApp.outputs.webAppIdentityPrincipalId
  }
}

module keyVaultAccessTest './modules/keyVaultAccessPolicy.bicep' = if (deployTest && !empty(existingTestKeyVaultResourceId)) {
  name: 'kv-access-test-${webAppName}'
  scope: resourceGroup(testKeyVaultSubscriptionId, testKeyVaultResourceGroupName)
  params: {
    keyVaultName: testKeyVaultName
    objectId: webApp.outputs.testSlotIdentityPrincipalId
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
