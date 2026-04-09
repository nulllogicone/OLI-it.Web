#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Generates a Kanban-style HTML stakeholder dashboard from OLI-it.Web project markdown files.

.DESCRIPTION
    Reads the project markdown files (backlog, open questions, ADRs) and produces a
    self-contained HTML dashboard for product owners, scrum masters, and stakeholders.

.PARAMETER OutputPath
    Path for the generated HTML file. Defaults to docs\dashboard.html (same folder as script).

.PARAMETER Open
    If specified, opens the generated file in the default browser after generation.

.EXAMPLE
    .\generate-dashboard.ps1
    .\generate-dashboard.ps1 -Open
    .\generate-dashboard.ps1 -OutputPath "..\dashboard.html" -Open
#>
param(
    [string]$OutputPath = (Join-Path $PSScriptRoot "dashboard.html"),
    [switch]$Open
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$docsPath    = $PSScriptRoot
$decPath     = Join-Path $docsPath "07-decisions"
$backlogPath = Join-Path $docsPath "08-backlog.md"
$oqPath      = Join-Path $docsPath "99-open-questions.md"

Write-Host "Generating OLI-it.Web stakeholder dashboard..." -ForegroundColor Cyan

# ─── Helpers ─────────────────────────────────────────────────────────────────
function HtmlEnc([string]$s) {
    $s -replace '&','&amp;' -replace '<','&lt;' -replace '>','&gt;' -replace '"','&quot;'
}

function Get-StatusClass([string]$status) {
    $s = ($status -replace '\u2705', '' -replace '\s+', ' ').Trim().ToLower()
    if ($s -match 'completed|done')   { return 'done' }
    if ($s -match 'in.?progress')     { return 'progress' }
    if ($s -match 'blocked')          { return 'blocked' }
    return 'todo'
}

# ─── Parse Backlog ────────────────────────────────────────────────────────────
$bl        = Get-Content $backlogPath -Raw
$blUpdated = if ($bl -match 'Last updated:\s*(.+)') { $matches[1].Trim() } else { 'unknown' }

# Phase 1: | BL-NNN | Title | UseCase | EntityImpact | Status |
$phase1 = [System.Collections.Generic.List[pscustomobject]]::new()
$p1sec  = if ($bl -match '(?s)(## Phase 1.*?)(?=## Phase 2)') { $matches[1] } else { '' }
foreach ($m in [regex]::Matches($p1sec, '\|\s*(BL-\d+)\s*\|\s*([^|]+?)\s*\|\s*([^|]+?)\s*\|\s*([^|]+?)\s*\|\s*([^|]+?)\s*\|')) {
    $phase1.Add([pscustomobject]@{
        Id      = $m.Groups[1].Value.Trim()
        Title   = $m.Groups[2].Value.Trim()
        UseCase = $m.Groups[3].Value.Trim()
        Entity  = $m.Groups[4].Value.Trim()
        Status  = $m.Groups[5].Value.Trim()
    })
}

# Phase 2: | BL-NNN | Title | Notes | Status |
$phase2 = [System.Collections.Generic.List[pscustomobject]]::new()
$p2sec  = if ($bl -match '(?s)(## Phase 2.*?)(?=## Change Log)') { $matches[1] } else { '' }
foreach ($m in [regex]::Matches($p2sec, '\|\s*(BL-\d+)\s*\|\s*([^|]+?)\s*\|\s*([^|]+?)\s*\|\s*([^|]+?)\s*\|')) {
    $phase2.Add([pscustomobject]@{
        Id     = $m.Groups[1].Value.Trim()
        Title  = $m.Groups[2].Value.Trim()
        Notes  = $m.Groups[3].Value.Trim()
        Status = $m.Groups[4].Value.Trim()
    })
}

# Change log (last 10)
$changelog = [System.Collections.Generic.List[string]]::new()
if ($bl -match '(?s)## Change Log\s*\n(.+)$') {
    foreach ($m in [regex]::Matches($matches[1], '-\s+(.+)')) {
        $changelog.Add($m.Groups[1].Value.Trim())
    }
}

# ─── Parse Open Questions ─────────────────────────────────────────────────────
$oq       = Get-Content $oqPath -Raw
$oqGroups = [System.Collections.Generic.List[pscustomobject]]::new()
$curGrp   = $null
foreach ($line in ($oq -split "`n")) {
    $line = $line.TrimEnd()
    if ($line -match '^## (.+)') {
        $curGrp = [pscustomobject]@{ Name = $matches[1].Trim(); Items = [System.Collections.Generic.List[string]]::new() }
        $oqGroups.Add($curGrp)
    } elseif ($line -match '^-\s+((?:\u2705\s*)?OQ-\d+:.+)' -and $curGrp) {
        $curGrp.Items.Add($matches[1].Trim())
    }
}
$oqGroups = @($oqGroups | Where-Object { $_.Name -notmatch 'Change Log' -and $_.Items.Count -gt 0 })
$totalOQ  = ($oqGroups | ForEach-Object { $_.Items.Count } | Measure-Object -Sum).Sum

# ─── Parse ADRs (specific files only, skip ADR-initial) ──────────────────────
$adrs = [System.Collections.Generic.List[pscustomobject]]::new()
Get-ChildItem $decPath -Filter "ADR-*.md" -ErrorAction SilentlyContinue |
    Where-Object { $_.Name -ne 'ADR-initial.md' } |
    Sort-Object Name |
    ForEach-Object {
        $c      = Get-Content $_.FullName -Raw
        $title  = if ($c -match '^#\s+(.+)')             { $matches[1].Trim() } else { $_.BaseName }
        $status = if ($c -match '\*\*Status:\*\*\s*(.+)') { $matches[1].Trim() } else { 'unknown' }
        $date   = if ($c -match '\*\*Date:\*\*\s*(.+)')   { $matches[1].Trim() } else { '' }
        $adrs.Add([pscustomobject]@{ Title = $title; Status = $status; Date = $date })
    }

# ─── Metrics ─────────────────────────────────────────────────────────────────
$p1Done   = @($phase1 | Where-Object { (Get-StatusClass $_.Status) -eq 'done' }).Count
$p1InProg = @($phase1 | Where-Object { (Get-StatusClass $_.Status) -eq 'progress' }).Count
$p1Total  = $phase1.Count
$p1Pct    = if ($p1Total -gt 0) { [math]::Round(($p1Done / $p1Total) * 100) } else { 0 }

$p2Done   = @($phase2 | Where-Object { (Get-StatusClass $_.Status) -eq 'done' }).Count
$p2InProg = @($phase2 | Where-Object { (Get-StatusClass $_.Status) -eq 'progress' }).Count
$p2Total  = $phase2.Count
$p2Pct    = if ($p2Total -gt 0) { [math]::Round(($p2Done / $p2Total) * 100) } else { 0 }

$genDate = Get-Date -Format "yyyy-MM-dd HH:mm"

# ─── Kanban Board Builder ────────────────────────────────────────────────────
function Build-KanbanCard($item, [bool]$showUseCase) {
    $id    = HtmlEnc $item.Id
    $title = HtmlEnc $item.Title
    $sub   = ''
    if ($showUseCase -and $item.UseCase -and $item.UseCase -ne '-' -and $item.UseCase -ne '&mdash;') {
        $sub = "<div class='kcard-sub'>UC: $(HtmlEnc $item.UseCase)</div>"
    } elseif (-not $showUseCase -and $item.Notes -and $item.Notes -ne '') {
        $sub = "<div class='kcard-sub'>$(HtmlEnc $item.Notes)</div>"
    }
    return "<div class='kcard'><span class='kcard-id'>$id</span><div class='kcard-title'>$title</div>$sub</div>"
}

function Build-KanbanBoard($items, [bool]$showUseCase = $false) {
    $buckets = @{
        todo     = @($items | Where-Object { (Get-StatusClass $_.Status) -eq 'todo' })
        progress = @($items | Where-Object { (Get-StatusClass $_.Status) -eq 'progress' })
        done     = @($items | Where-Object { (Get-StatusClass $_.Status) -eq 'done' })
        blocked  = @($items | Where-Object { (Get-StatusClass $_.Status) -eq 'blocked' })
    }

    $cols = @(
        @{ key = 'todo';     label = 'Backlog';     css = 'kcol-todo' }
        @{ key = 'progress'; label = 'In Progress'; css = 'kcol-progress' }
        @{ key = 'done';     label = 'Done';        css = 'kcol-done' }
    )
    if ($buckets.blocked.Count -gt 0) {
        $cols += @{ key = 'blocked'; label = 'Blocked'; css = 'kcol-blocked' }
    }

    $board = "<div class='kboard'>"
    foreach ($col in $cols) {
        $colItems = $buckets[$col.key]
        $count    = $colItems.Count
        $cards    = if ($count -gt 0) {
            ($colItems | ForEach-Object { Build-KanbanCard $_ $showUseCase }) -join ''
        } else {
            "<div class='kcard-empty'>Nothing here yet</div>"
        }
        $board += "<div class='kcol $($col.css)'>"
        $board += "<div class='kcol-header'>$($col.label)<span class='kcol-count'>$count</span></div>"
        $board += "<div class='kcol-body'>$cards</div>"
        $board += "</div>"
    }
    $board += "</div>"
    return $board
}

# ─── Open Questions HTML ──────────────────────────────────────────────────────
$oqHtml = ''
foreach ($grp in $oqGroups) {
    $oqHtml += "<h5 class='oq-group-header'>$(HtmlEnc $grp.Name)</h5><ul class='oq-list'>"
    foreach ($q in $grp.Items) {
        if ($q -match '^(\u2705\s+)?(OQ-\d+):(.+)$') {
            $check = if ($matches[1]) { '✅ ' } else { '' }
            $oqHtml += "<li><span class='oq-id'>$check$($matches[2])</span> $(HtmlEnc $matches[3].Trim())</li>"
        } else {
            $oqHtml += "<li>$(HtmlEnc $q)</li>"
        }
    }
    $oqHtml += '</ul>'
}

# ─── ADR Table HTML ───────────────────────────────────────────────────────────
$adrHtml = ''
foreach ($adr in $adrs) {
    $s = $adr.Status.ToLower()
    $badge = switch ($s) {
        'accepted'   { "<span class='badge adr-accepted'>Accepted</span>" }
        'proposed'   { "<span class='badge adr-proposed'>Proposed</span>" }
        'rejected'   { "<span class='badge adr-rejected'>Rejected</span>" }
        'superseded' { "<span class='badge adr-superseded'>Superseded</span>" }
        default      { "<span class='badge adr-unknown'>$(HtmlEnc $adr.Status)</span>" }
    }
    $adrHtml += "<tr><td>$(HtmlEnc $adr.Title)</td><td>$(HtmlEnc $adr.Date)</td><td>$badge</td></tr>"
}

# ─── Change Log HTML ──────────────────────────────────────────────────────────
$clHtml = ($changelog | Select-Object -Last 10 | ForEach-Object { "<li>$(HtmlEnc $_)</li>" }) -join ''

# ─── Kanban Boards ────────────────────────────────────────────────────────────
$p1Board = Build-KanbanBoard $phase1 $true
$p2Board = Build-KanbanBoard $phase2 $false

# ─── Dashboard HTML ───────────────────────────────────────────────────────────
# Note: $() expressions below are PowerShell interpolation — intentional.
$html = @"
<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>OLI-it.Web &mdash; Stakeholder Dashboard</title>
<link rel="stylesheet"
  href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
  integrity="sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH"
  crossorigin="anonymous">
<style>
body { background:#f0f2f5; font-family:'Segoe UI',system-ui,sans-serif; }
.dash-header { background:linear-gradient(135deg,#1a237e 0%,#0d47a1 100%); color:#fff; padding:1.75rem 0 1.5rem; margin-bottom:1.75rem; }
.dash-header h1 { font-size:1.7rem; font-weight:700; margin:0; letter-spacing:-.01em; }
.dash-header .sub { opacity:.75; font-size:.85rem; margin-top:.3rem; }
.metric-card { background:#fff; border-radius:10px; padding:1.1rem 1rem; text-align:center; box-shadow:0 1px 4px rgba(0,0,0,.08); height:100%; }
.metric-value { font-size:2.4rem; font-weight:700; line-height:1; }
.metric-label { font-size:.7rem; color:#888; text-transform:uppercase; letter-spacing:.06em; margin-top:.3rem; }
.mv-gray  { color:#546e7a; }
.mv-green { color:#2e7d32; }
.mv-amber { color:#e65100; }
.mv-blue  { color:#1565c0; }
.section-card { background:#fff; border-radius:10px; padding:1.4rem 1.5rem; box-shadow:0 1px 4px rgba(0,0,0,.08); margin-bottom:1.5rem; }
.section-card h2 { font-size:1rem; font-weight:700; color:#1a237e; border-bottom:2px solid #e3f2fd; padding-bottom:.4rem; margin-bottom:1rem; text-transform:uppercase; letter-spacing:.04em; }
.progress { height:8px; border-radius:4px; }
.prog-label { font-size:.8rem; color:#555; margin-bottom:.2rem; display:flex; justify-content:space-between; }
.kboard { display:flex; gap:.85rem; overflow-x:auto; padding-bottom:.5rem; }
.kcol { flex:0 0 250px; border-radius:8px; overflow:hidden; border:1px solid #e0e0e0; background:#fafafa; }
.kcol-header { padding:.55rem .85rem; font-weight:700; font-size:.72rem; text-transform:uppercase; letter-spacing:.07em; display:flex; justify-content:space-between; align-items:center; }
.kcol-count { background:rgba(255,255,255,.35); border-radius:12px; padding:.05em .55em; font-size:.9em; }
.kcol-body { padding:.55rem; display:flex; flex-direction:column; gap:.4rem; max-height:500px; overflow-y:auto; }
.kcol-todo     .kcol-header { background:#78909c; color:#fff; }
.kcol-progress .kcol-header { background:#1565c0; color:#fff; }
.kcol-done     .kcol-header { background:#2e7d32; color:#fff; }
.kcol-blocked  .kcol-header { background:#c62828; color:#fff; }
.kcard { background:#fff; border-radius:6px; padding:.55rem .7rem; box-shadow:0 1px 3px rgba(0,0,0,.07); border-left:3px solid #bdbdbd; }
.kcol-progress .kcard { border-color:#1976d2; }
.kcol-done     .kcard { border-color:#388e3c; }
.kcol-blocked  .kcard { border-color:#d32f2f; }
.kcard-id    { font-size:.65rem; font-weight:700; color:#9e9e9e; text-transform:uppercase; display:block; margin-bottom:2px; }
.kcard-title { font-size:.8rem; line-height:1.35; color:#212121; }
.kcard-sub   { font-size:.7rem; color:#bdbdbd; margin-top:3px; }
.kcard-empty { text-align:center; color:#ccc; padding:1.5rem .5rem; font-size:.82rem; }
.oq-group-header { font-size:.75rem; font-weight:700; color:#78909c; text-transform:uppercase; letter-spacing:.06em; margin:1rem 0 .3rem; border-bottom:1px solid #f0f0f0; padding-bottom:.2rem; }
.oq-group-header:first-child { margin-top:0; }
.oq-list { padding-left:1.1rem; margin:0 0 .4rem; }
.oq-list li { font-size:.82rem; margin-bottom:.3rem; color:#333; }
.oq-id { font-weight:700; color:#e65100; font-size:.75rem; }
.adr-accepted   { background:#d4edda; color:#155724; font-size:.75rem; }
.adr-proposed   { background:#fff3cd; color:#856404; font-size:.75rem; }
.adr-rejected   { background:#f8d7da; color:#721c24; font-size:.75rem; }
.adr-superseded { background:#d6d8db; color:#383d41; font-size:.75rem; }
.adr-unknown    { background:#e2e3e5; color:#383d41; font-size:.75rem; }
.cl-list { list-style:none; padding:0; margin:0; }
.cl-list li { font-size:.8rem; border-left:3px solid #e3f2fd; padding:.2rem .6rem; margin-bottom:.25rem; color:#444; }
.cl-list li:first-child { border-color:#1565c0; font-weight:600; }
.dash-footer { text-align:center; color:#aaa; font-size:.72rem; padding:1.5rem 0 2rem; }
@media (max-width:768px) { .kboard { flex-direction:column; } .kcol { flex:none; } }
</style>
</head>
<body>

<div class="dash-header">
  <div class="container">
    <div class="d-flex justify-content-between align-items-start flex-wrap gap-2">
      <div>
        <h1>OLI-it.Web &mdash; Stakeholder Dashboard</h1>
        <div class="sub">0L1 Open Messaging Platform &bull; Generated: $genDate &bull; Backlog: $blUpdated</div>
      </div>
      <div class="d-flex gap-2 align-items-center mt-1">
        <span class="badge bg-warning text-dark fs-6 px-3 py-2">Phase 1 &mdash; MVP Parity</span>
      </div>
    </div>
  </div>
</div>

<div class="container pb-4">

  <!-- Metrics -->
  <div class="row g-3 mb-4">
    <div class="col-6 col-sm-4 col-lg">
      <div class="metric-card"><div class="metric-value mv-gray">$p1Total</div><div class="metric-label">Phase 1 Items</div></div>
    </div>
    <div class="col-6 col-sm-4 col-lg">
      <div class="metric-card"><div class="metric-value mv-green">$p1Done</div><div class="metric-label">Phase 1 Done</div></div>
    </div>
    <div class="col-6 col-sm-4 col-lg">
      <div class="metric-card"><div class="metric-value mv-blue">$p1InProg</div><div class="metric-label">Phase 1 In&nbsp;Progress</div></div>
    </div>
    <div class="col-6 col-sm-4 col-lg">
      <div class="metric-card"><div class="metric-value mv-gray">$p2Total</div><div class="metric-label">Phase 2 Items</div></div>
    </div>
    <div class="col-6 col-sm-4 col-lg">
      <div class="metric-card"><div class="metric-value mv-green">$p2Done</div><div class="metric-label">Phase 2 Done</div></div>
    </div>
    <div class="col-6 col-sm-4 col-lg">
      <div class="metric-card"><div class="metric-value mv-amber">$totalOQ</div><div class="metric-label">Open Questions</div></div>
    </div>
  </div>

  <!-- Progress -->
  <div class="section-card">
    <h2>Delivery Progress</h2>
    <div class="mb-3">
      <div class="prog-label"><span>Phase 1 &mdash; MVP Parity</span><strong>$p1Done / $p1Total &nbsp;($p1Pct%)</strong></div>
      <div class="progress"><div class="progress-bar bg-primary" role="progressbar" style="width:$p1Pct%" aria-valuenow="$p1Pct" aria-valuemin="0" aria-valuemax="100"></div></div>
    </div>
    <div>
      <div class="prog-label"><span>Phase 2 &mdash; Enhancements</span><strong>$p2Done / $p2Total &nbsp;($p2Pct%)</strong></div>
      <div class="progress"><div class="progress-bar bg-success" role="progressbar" style="width:$p2Pct%" aria-valuenow="$p2Pct" aria-valuemin="0" aria-valuemax="100"></div></div>
    </div>
  </div>

  <!-- Phase 1 Kanban -->
  <div class="section-card">
    <h2>Phase 1 &mdash; MVP Parity &nbsp;<small class="text-muted fw-normal text-lowercase">($p1Total items)</small></h2>
    $p1Board
  </div>

  <!-- Phase 2 Kanban -->
  <div class="section-card">
    <h2>Phase 2 &mdash; Enhancements &nbsp;<small class="text-muted fw-normal text-lowercase">($p2Total items)</small></h2>
    $p2Board
  </div>

  <!-- Open Questions + ADRs -->
  <div class="row g-3 mb-0">
    <div class="col-lg-6">
      <div class="section-card h-100">
        <h2>Open Questions ($totalOQ)</h2>
        $oqHtml
      </div>
    </div>
    <div class="col-lg-6">
      <div class="section-card h-100">
        <h2>Architecture Decisions</h2>
        <table class="table table-sm mb-0">
          <thead class="table-light"><tr><th>Decision</th><th style="width:110px">Date</th><th style="width:100px">Status</th></tr></thead>
          <tbody>$adrHtml</tbody>
        </table>
      </div>
    </div>
  </div>

  <!-- Change Log -->
  <div class="section-card mt-3">
    <h2>Recent Changes</h2>
    <ul class="cl-list">$clHtml</ul>
  </div>

</div>

<div class="dash-footer">
  <div class="container">
    Generated by <code>docs/generate-dashboard.ps1</code> from project markdown files &bull; OLI-it.Web (nulllogicone) &bull; $genDate
  </div>
</div>

</body>
</html>
"@

$outputDir = Split-Path -Path $OutputPath -Parent
if (-not [string]::IsNullOrWhiteSpace($outputDir)) {
  New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
}

$html | Set-Content -Path $OutputPath -Encoding UTF8 -NoNewline

Write-Host "  Dashboard written to: $OutputPath" -ForegroundColor Green

if ($Open) {
    Start-Process $OutputPath
}
