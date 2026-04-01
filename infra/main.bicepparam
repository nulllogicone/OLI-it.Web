using './main.bicep'

// Fill in all values marked with TODO before deployment.
param location = 'westeurope'
param webAppName = 'oliitrazorweb'
param existingAppServicePlanResourceId = '/subscriptions/33dd8226-abb3-4f36-b1f0-059e18b9570a/resourceGroups/default-web-westeurope/providers/Microsoft.Web/serverFarms/Default0'
param existingLogAnalyticsWorkspaceResourceId = '/subscriptions/33dd8226-abb3-4f36-b1f0-059e18b9570a/resourcegroups/default-web-westeurope/providers/microsoft.operationalinsights/workspaces/oliitwebloganalyticsworkspace'
param existingKeyVaultResourceId = '/subscriptions/33dd8226-abb3-4f36-b1f0-059e18b9570a/resourceGroups/Default-Web-WestEurope/providers/Microsoft.KeyVault/vaults/oli-it-kv-test'
param webAppSubnetResourceId = '/subscriptions/33dd8226-abb3-4f36-b1f0-059e18b9570a/resourceGroups/Default-Network-WestEurope/providers/Microsoft.Network/virtualNetworks/OLI-it-VNet/subnets/frontend'
param keyVaultSecretUri = '@Microsoft.KeyVault(SecretUri=https://oli-it-kv-test.vault.azure.net/secrets/null-test-connection/)'
param imagesRootUrl = 'https://www.oli-it.com/images/'

// Optional overrides
param osType = 'windows'
param linuxFxVersion = 'DOTNETCORE|8.0'
param alwaysOn = true
param tags = {
  environment: 'dev'
  application: 'OLI-it.Web'
}
