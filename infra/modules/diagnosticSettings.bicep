targetScope = 'resourceGroup'

param targetWebAppName string

param diagnosticSettingName string

param logAnalyticsWorkspaceResourceId string

resource targetResource 'Microsoft.Web/sites@2023-12-01' existing = {
  name: targetWebAppName
}

resource webAppDiagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  scope: targetResource
  name: diagnosticSettingName
  properties: {
    workspaceId: logAnalyticsWorkspaceResourceId
    logs: [
      {
        categoryGroup: 'allLogs'
        enabled: true
      }
    ]
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
      }
    ]
  }
}
