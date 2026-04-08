# OLI-it Web Application

## Configuration

### Infrastructure Deployment Parameters

Infrastructure is deployed from `infra/main.bicep` using environment-specific parameter files:

- `infra/main.test.bicepparam` for test slot-oriented values
- `infra/main.prod.bicepparam` for production values

The GitHub Actions workflow `.github/workflows/infra-main-bicep.yml` selects the parameter file based on the workflow input:

- `environment: test` -> test deployment job using `infra/main.test.bicepparam`
- `environment: production` -> production deployment job using `infra/main.prod.bicepparam` (GitHub Environment `production` approval gate)

The legacy single-file parameter set `infra/main.bicepparam` has been removed.

Deployment mode is now explicit in parameters:

- `infra/main.test.bicepparam` sets `deploymentMode = 'testOnly'`
- `infra/main.prod.bicepparam` sets `deploymentMode = 'prodOnly'`

Fresh deployment behavior (empty resource group):

- A test deployment can run first and will provision the App Service and test slot infrastructure.
- Production-specific app settings and production Key Vault access policy are only applied in the production deployment.

### Environment Variables / App Settings

The application uses the following configuration settings in `appsettings.json`:

- **ConnectionStrings:OliItDb** - SQL Server connection string for the OLI-it database
- **ImagesRootUrl** - Root URL for the shared Azure Blob Storage image repository
  - Example: `https://oliit.blob.core.windows.net/oliupload`
  - This URL is combined with relative paths from the `Datei` field to construct full image URLs
  - Supports paths with or without leading slash (e.g., both `/photo.jpg` and `photo.jpg` work)

### Development Setup

1. Update `appsettings.Development.json` with your local database connection string
2. Configure the `ImagesRootUrl` to point to your Azure Blob Storage or local image server
   - Example: `"ImagesRootUrl": "https://oliit.blob.core.windows.net/oliupload"`
3. Run the application using `dotnet run` or Visual Studio

---

## Image Thumbnail Feature

The application includes a reusable **ImageThumbnail View Component** that displays image thumbnails from Azure Blob Storage for Stamm, PostIt, and TopLab entities.

### How It Works

1. **Data Model**: Each entity (Stamm, PostIt, TopLab) has a `Datei` field that stores the relative path to an image
2. **Configuration**: The `ImagesRootUrl` setting defines the base URL for the image repository
3. **URL Construction**: The component combines `ImagesRootUrl` + `Datei` to create the complete image URL
   - Example: `https://oliit.blob.core.windows.net/oliupload` + `/photo.jpg` → `https://oliit.blob.core.windows.net/oliupload/photo.jpg`

### Path Handling

The component automatically handles different path formats:
- Paths starting with `/`: Used as-is (e.g., `/documents/image.jpg`)
- Paths without `/`: Automatically prefixed (e.g., `image.jpg` → `/image.jpg`)
- Empty or null paths: No thumbnail displayed

### Where Thumbnails Appear

#### 1. **Tables** (Browse Page)
Thumbnails appear in the **first column** (80x80 pixels) for:
- **Stamm List** (Home page) - Recent users
- **PostIt List** - Messages under a Stamm
- **TopLab List** - Answers under a Stamm or PostIt

#### 2. **Cards** (Browse Page)
Thumbnails appear in the **top-right corner** (60x60 pixels, circular) for:
- **PostIt Card** - In the header next to the close button
- **TopLab Card** - In the header next to the close button
- **Stamm Card** - In the body section (150x150 pixels)

#### 3. **Detail Pages**
Thumbnails appear below entity details (150x150 pixels) for:
- Stamm detail page (`/stamm/{id}`)
- PostIt detail page (`/postit/{id}`)
- TopLab detail page (`/toplab/{id}`)

### Error Handling

- **Missing Images**: If an image fails to load, a gray placeholder with "No Image" text is displayed
- **Debug Info**: Hover over any thumbnail to see the full constructed image URL
- **Graceful Degradation**: If no `Datei` path exists, the thumbnail area is simply not rendered

### Visual Effects

- **Hover Effect**: Thumbnails zoom slightly (1.05x) and show a shadow on hover
- **Cursor**: Changes to pointer on hover to indicate interactivity
- **Lazy Loading**: Images use native browser lazy loading for better performance

### Component Usage

The ImageThumbnail component can be invoked in any Razor view:

```razor
@await Component.InvokeAsync("ImageThumbnail", new { 
    dateiPath = Model.Datei, 
    altText = "Description", 
    width = 150, 
    height = 150 
})
```

**Parameters:**
- `dateiPath` (string, required): Relative path from the `Datei` field
- `altText` (string, optional): Alt text for accessibility (default: "Image")
- `width` (int, optional): Thumbnail width in pixels (default: 150)
- `height` (int, optional): Thumbnail height in pixels (default: 150)

### File Structure

```
OLI-it.Web/
├── ViewComponents/
│   └── ImageThumbnailViewComponent.cs          # Component logic
├── Pages/
│   └── Shared/
│       └── Components/
│           └── ImageThumbnail/
│               └── Default.cshtml              # Component view template
└── wwwroot/
    └── css/
        └── site.css                            # Thumbnail styling
```

### CSS Classes

- `.image-thumbnail` - Container for the thumbnail
- `.thumbnail-img` - The image element
- `.header-thumbnail` - Thumbnail in card headers (circular)
- `.entity-file-section` - Section containing thumbnails in card bodies

### Example Data

```sql
-- Example Datei values in the database:
'/documents/profile-photo.jpg'
'/images/2024/screenshot.png'
'uploads/document.jpg'  -- Works without leading slash
```

### Troubleshooting

**Thumbnails not appearing?**
1. Check that `ImagesRootUrl` is configured in `appsettings.json`
2. Verify the `Datei` field has a value in the database
3. Hover over the empty space to see if there's a "No Image" placeholder
4. Check the browser console for image loading errors
5. Verify the Azure Blob Storage URL is accessible

**Wrong images loading?**
1. Hover over the thumbnail to see the full constructed URL
2. Verify the `Datei` path matches the actual file name in blob storage
3. Check for case sensitivity in file names

---

## Database Schema

The following entities support image thumbnails via the `Datei` field:
- **Stamm** - User/member entity
- **PostIt** - Message/question entity  
- **TopLab** - Answer/response entity

