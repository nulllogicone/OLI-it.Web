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

### CI pipeline steps

These mirror what the GitHub Actions workflow executes:

```powershell
dotnet restore "OLI-it.Web"
dotnet build "OLI-it.Web" --configuration Release --no-restore
dotnet test "OLI-it.Web" --no-build
```

> There are no test projects yet. When adding tests, use xUnit or NUnit in a new `OLI-it.Web.Tests` project.

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
