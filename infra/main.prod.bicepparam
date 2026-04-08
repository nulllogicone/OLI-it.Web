using './main.bicep'

param location = 'westeurope'
param webAppName = 'oliitrazorweb'
param existingAppServicePlanResourceId = '/subscriptions/33dd8226-abb3-4f36-b1f0-059e18b9570a/resourceGroups/default-web-westeurope/providers/Microsoft.Web/serverFarms/Default0'
param existingLogAnalyticsWorkspaceResourceId = '/subscriptions/33dd8226-abb3-4f36-b1f0-059e18b9570a/resourcegroups/default-web-westeurope/providers/microsoft.operationalinsights/workspaces/oliitwebloganalyticsworkspace'
param existingTestKeyVaultResourceId = '/subscriptions/33dd8226-abb3-4f36-b1f0-059e18b9570a/resourceGroups/Default-Web-WestEurope/providers/Microsoft.KeyVault/vaults/oli-it-kv-test'
param existingProdKeyVaultResourceId = '/subscriptions/33dd8226-abb3-4f36-b1f0-059e18b9570a/resourceGroups/Default-Web-WestEurope/providers/Microsoft.KeyVault/vaults/oli-it-kv-prod'
param webAppSubnetResourceId = '/subscriptions/33dd8226-abb3-4f36-b1f0-059e18b9570a/resourceGroups/Default-Network-WestEurope/providers/Microsoft.Network/virtualNetworks/OLI-it-VNet/subnets/frontend'
param testDbConnectionKeyName = 'null-test-connection'
param prodDbConnectionKeyName = 'null-connection'
param storageConnectionKeyName = 'OliItStorageConnectionString'
param deploymentMode = 'prodOnly'
param imagesRootUrl = 'https://oliit.blob.core.windows.net/oliupload'

param osType = 'windows'
param linuxFxVersion = 'DOTNETCORE|8.0'
param alwaysOn = true
param tags = {
  environment: 'production'
  application: 'OLI-it.Web'
}
