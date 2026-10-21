# Developer Guide

This guide covers local development setup, configuration, and implementation details for contributors.

---

## Development Setup

### Prerequisites

- .NET 10 SDK
- SQL Server (local or remote)
- Azure Blob Storage account (or a local substitute for images)

### Configuration

Update `appsettings.Development.json` with your local values:

```json
{
  "ConnectionStrings": {
    "OliItDb": "<your SQL Server connection string>"
  },
  "ImagesRootUrl": "https://oliit.blob.core.windows.net/oliupload"
}
```

**Configuration keys:**

| Key | Description |
|-----|-------------|
| `ConnectionStrings:OliItDb` | SQL Server connection string for the OLI-it database |
| `ImagesRootUrl` | Root URL for the Azure Blob Storage image repository. Combined with the relative `Datei` field to build full image URLs. Supports paths with or without a leading slash. |

### Running locally

```powershell
dotnet restore OLI-it.Web
dotnet run --project OLI-it.Web
# HTTP:  http://localhost:5113
# HTTPS: https://localhost:7119
```

On Ubuntu/Linux, trust the .NET dev certificate for browser HTTPS:

```bash
dotnet dev-certs https --export-path /tmp/localhost.crt --format PEM --no-password
certutil -d sql:$HOME/.pki/nssdb -A -t "P,," -n "localhost" -i /tmp/localhost.crt
```

### CI pipeline steps

The GitHub Actions workflow builds the application and runs the focused creation tests against an isolated database restored from the committed `data/null.bak`. These tests require Windows with SQL Server LocalDB and `sqlcmd` available:

```powershell
dotnet restore "OLI-it.Web"
dotnet build "OLI-it.Web" --configuration Release --no-restore
SqlLocalDB start MSSQLLocalDB
sqlcmd -S '(localdb)\MSSQLLocalDB' -d master -E -C -l 10 -b -Q 'SELECT 1'
dotnet test "OLI-it.Web.Tests" --configuration Release --filter "FullyQualifiedName~PostItCreationTests"
```

CI explicitly starts LocalDB and retries the connection probe up to six times before running tests. It fails if the database remains unavailable; it does not retry or skip failing tests. The wider matchmaking benchmark is outside this deployment gate.

On 2026-10-21, [push run 186](https://github.com/nulllogicone/OLI-it.Web/actions/runs/39654872813) failed during fixture initialization with a LocalDB pre-login connection timeout. All 11 tests inherited that setup failure. [PR run 187](https://github.com/nulllogicone/OLI-it.Web/actions/runs/39655640013) subsequently passed all 11 tests for branch head `f50de68`. This supports an intermittent startup problem; the explicit readiness check addresses that dependency. The package vulnerability warnings in both runs were separate from the test failure.

---

## Feature worktrees and online previews

Verified against the local workflow and GitHub `main` on 2026-10-21: [the application workflow](../.github/workflows/oliitrazorweb.yml) supports deploying feature branches to the shared Azure App Service `test` slot. Git history records the change as `c231773` ("deploy any branch", 2026-04-02).

The documented preview address is <https://oliitrazorweb-test.azurewebsites.net/>. This configuration review does not establish which commit is currently deployed or whether a new deployment will succeed.

Suggested review cycle:

1. Develop an experiment in a separate worktree on a feature branch, for example `codex/47-fishing-celebration` for [issue #47](https://github.com/nulllogicone/OLI-it.Web/issues/47).
2. Prepare the implementation and validate it locally; leave changes available for the owner's review. Commit and push only when explicitly requested by the owner.
3. A push that matches the workflow's path filters builds, runs the selected `PostItCreationTests`, publishes, and deploys to `test`. The workflow also exposes a manual `workflow_dispatch` trigger. Pull-request events build and test but skip deployment.
4. After deployment succeeds, record the branch, commit, workflow run and preview URL so the owner can review the actual deployed version. Decide whether to revise, abandon, or merge the experiment after online review.

All branches share the same test slot; there is no separate preview environment per worktree or branch. Another successful deployment can replace the experiment, and the workflow currently has no deployment concurrency control. Coordinate preview use while reviewing a feature. A local worktree alone does not deploy anything.

Feature branches cannot enter the production job: it requires `refs/heads/main` and a non-pull-request event. Merging to `main` starts the deployment path toward production, subject to the configured GitHub `production` environment protections. Verify those protections before relying on an approval gate. Test-slot database and storage isolation must also be checked before experiments that modify data; a separate worktree does not isolate hosted data.

---

## Infrastructure Deployment

Infrastructure is defined in `infra/main.bicep` and deployed via GitHub Actions (`.github/workflows/infra-main-bicep.yml`).

### Parameter files

| File | Used for |
|------|----------|
| `infra/main.test.bicepparam` | Test slot deployment (`deploymentMode = 'testOnly'`) |
| `infra/main.prod.bicepparam` | Production deployment (`deploymentMode = 'prodOnly'`) |

The workflow selects the parameter file based on the `environment` input:

- `environment: test` → test deployment job
- `environment: production` → production deployment job (requires GitHub Environment `production` approval)

**Fresh deployment behavior** (empty resource group):

- A test deployment can run first and will provision the App Service and test slot infrastructure.
- Production-specific app settings and Key Vault access policy are only applied in the production deployment.

---

## Secrets & Configuration

- Local secrets are managed via **User Secrets** (ID: `936429e2-4c07-4bde-9c3e-40e1f6531612`).
- In Azure, connection strings and keys come from **Azure Key Vault** (`oli-it-kv-test`).
- Never commit secrets or connection strings to source.

---

## ImageThumbnail View Component

A reusable component that displays image thumbnails from Azure Blob Storage for `Stamm`, `PostIt`, and `TopLab` entities.

### How it works

1. Each entity has a `Datei` field storing a relative image path.
2. `ImagesRootUrl` (config) defines the blob storage base URL.
3. The component combines both to form the full image URL.
   - Example: `https://oliit.blob.core.windows.net/oliupload` + `/photo.jpg` → full URL

### Usage

```razor
@await Component.InvokeAsync("ImageThumbnail", new { 
    dateiPath = Model.Datei, 
    altText = "Description", 
    width = 150, 
    height = 150 
})
```

**Parameters:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `dateiPath` | string | required | Relative path from the `Datei` field |
| `altText` | string | `"Image"` | Alt text for accessibility |
| `width` | int | `150` | Width in pixels |
| `height` | int | `150` | Height in pixels |

### Where thumbnails appear

| Location | Size | Format |
|----------|------|--------|
| Tables (Browse page: Stamm, PostIt, TopLab lists) | 80×80 | Square |
| Card headers (PostIt, TopLab) | 60×60 | Circular |
| Stamm card body | 150×150 | Square |
| Detail pages (`/stamm/{id}`, `/postit/{id}`, `/toplab/{id}`) | 150×150 | Square |

### Path handling

- Paths with leading `/`: used as-is
- Paths without `/`: slash is prepended automatically
- Empty / null: thumbnail area not rendered

### Error handling & visual effects

- Failed image load → gray "No Image" placeholder
- Hover → tooltip showing the full constructed URL
- Hover effect: 1.05× zoom + shadow
- Native browser lazy loading

### File structure

```
OLI-it.Web/
├── ViewComponents/
│   └── ImageThumbnailViewComponent.cs
├── Pages/Shared/Components/ImageThumbnail/
│   └── Default.cshtml
└── wwwroot/css/site.css          ← thumbnail CSS classes
```

### CSS classes

| Class | Purpose |
|-------|---------|
| `.image-thumbnail` | Container |
| `.thumbnail-img` | Image element |
| `.header-thumbnail` | Circular variant for card headers |
| `.entity-file-section` | Section wrapping thumbnails in card bodies |

### Troubleshooting

**Thumbnails not appearing?**
1. Check `ImagesRootUrl` is set in `appsettings.json`.
2. Verify the `Datei` field has a value in the database.
3. Hover over the empty area — a "No Image" placeholder should be visible.
4. Check the browser console for loading errors.
5. Verify Azure Blob Storage is accessible.

**Wrong image loading?**
1. Hover over the thumbnail to see the full constructed URL.
2. Verify the `Datei` path matches the actual filename in blob storage (case-sensitive).
