targetScope = 'resourceGroup'

param location string
param applicationInsightsName string
param logAnalyticsWorkspaceResourceId string
param tags object = {}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsName
  location: location
  kind: 'web'
  tags: tags
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspaceResourceId
    IngestionMode: 'LogAnalytics'
  }
}

output applicationInsightsResourceId string = applicationInsights.id
output connectionString string = applicationInsights.properties.ConnectionString
