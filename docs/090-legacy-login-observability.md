# Legacy OliWeb login observability

Updated: 2026-10-07
Status: reviewed source and Azure resource metadata; live environment-discovery query verified; workbook rendering unverified

## Purpose

This document owns the operational queries for observing legacy OliWeb password logins during the [migration](000-motivation.md). Application Insights is sufficient for this reporting use case; Event Grid is only needed if a login must trigger downstream processing.

## Existing signal and limits

In the DevOps repository, `Anwendung/OLIWeb/Controls/Floor/EinAusLoggen.ascx.cs`, method `showStamm`, calls `TrackEvent("Login", props)` after `OliUser.ShowStamm(name, password)`. Properties are `StammTextBox` (entered name), `Success` (boolean converted to string), and `Machine`. `Entwicklung/OliEngine/OliMiddleTier/OLIs/OliUser.cs` returns true after successful authentication and false on failure. Filter `Success` to avoid counting attempts to view a profile as successful logins. Use the explicit name property; authenticated-user context is not established by this event code.

Remembered-cookie authentication in `Global.asax.cs`, `Session_Start`, calls `ShowStamm` directly without emitting `Login`. These queries therefore cover explicit password logins, not all restored authenticated sessions. The checked-out source is not verified against the deployed production revision.

`ApplicationInsights.config` includes adaptive sampling for events. `sum(itemCount)` gives a sampling-adjusted estimate; individual rows and distinct users can be incomplete. This is operational telemetry, not a complete audit trail. Timestamps are UTC. Telemetry arrival is delayed.

Azure metadata confirms the Application Insights resource **oliweb**, resource group **Default-Web-westeurope**, and backing workspace **oliitwebloganalyticsworkspace**. The production App Service is **oliweb**. A live 30-day aggregate query on 2026-10-07 confirmed `Login` events with `True` and `False` outcomes under roles `oliweb` and `oliweb-test`. Runtime app settings and the deployed code revision were not inspected.

## Where to run

In Azure Portal, open **Application Insights → oliweb → Logs**. Use KQL mode if the simple query interface is displayed. Set the portal time range to include the last 30 days.

First discover the environment values actually present:

```kql
customEvents
| where timestamp > ago(30d)
| where name == "Login"
| summarize StoredEvents=count(), EstimatedEvents=sum(itemCount),
    LastSeen=max(timestamp)
    by cloud_RoleName, Success=tostring(customDimensions.Success)
| order by LastSeen desc
```

The live query confirmed production reports `cloud_RoleName == "oliweb"` and test reports `oliweb-test`; both use the same Application Insights resource. The following queries exclude test. Recheck discovery after hosting or telemetry configuration changes. If roles become empty or indistinguishable, verify another environment discriminator before calling the results production-only.

### Recent successful password logins

```kql
customEvents
| where timestamp > ago(30d)
| where name == "Login" and cloud_RoleName == "oliweb"
| where tostring(customDimensions.Success) =~ "true"
| project LoginTimeUtc=timestamp,
    UserName=tostring(customDimensions.StammTextBox),
    SamplingWeight=itemCount
| order by LoginTimeUtc desc
| take 200
```

### Successful logins per day

```kql
customEvents
| where timestamp > ago(30d)
| where name == "Login" and cloud_RoleName == "oliweb"
| where tostring(customDimensions.Success) =~ "true"
| summarize EstimatedLogins=sum(itemCount) by bin(timestamp, 1d)
| render timechart
```

From the workspace's **Logs** instead, use `AppEvents`, `TimeGenerated`, `Name`, `AppRoleName`, `Properties` and `ItemCount` in place of `customEvents`, `timestamp`, `name`, `cloud_RoleName`, `customDimensions` and `itemCount`. Also scope to the oliweb component using `_ResourceId`; a workspace can contain multiple applications.

## Workbook

The [workbook template](workbooks/legacy-oliweb-logins.workbook) contains environment discovery, recent successful logins and a daily chart, initially covering 30 days. Its production filter uses the verified role above.

Open **Application Insights → oliweb → Workbooks → New → Edit → Advanced Editor**, choose the gallery template JSON view if offered, paste the file contents and apply. Ensure query items select **oliweb** as the Application Insights resource. Save as **Legacy OliWeb password logins**. Change the lookback period in query editors as needed. The template has not been rendered or imported into Azure in this review.

The workbook displays names already stored in telemetry. Keep access consistent with access to those logs; do not copy actual login records into public repository files. No Azure resources or application behavior were changed.

## References

- [Application Insights telemetry model and table names](https://learn.microsoft.com/en-us/azure/azure-monitor/app/data-model-complete)
- [Sampling weights in log-based metrics](https://learn.microsoft.com/en-us/azure/azure-monitor/app/metrics-overview)
- [Create or edit an Azure Workbook](https://learn.microsoft.com/en-us/azure/azure-monitor/visualize/workbooks-create-workbook)
