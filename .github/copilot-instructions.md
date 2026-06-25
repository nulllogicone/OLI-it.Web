# Copilot Instructions for OLI-it.Web

## Git Rules (Always Apply)

- **Never commit** changes without the user reviewing them first.
- **Never push** without explicit user confirmation.
- Always show a `git diff` or file summary and wait for explicit user approval before proceeding with any `git commit` or `git push`.

## Build & Run

```powershell
# Restore and build
dotnet restore OLI-it.Web
dotnet build OLI-it.Web --configuration Release

# Run locally
dotnet run --project OLI-it.Web
# HTTP:  http://localhost:5113
# HTTPS: https://localhost:7119

# CI pipeline steps (mirrors GitHub Actions)
dotnet restore "OLI-it.Web"
dotnet build "OLI-it.Web" --configuration Release --no-restore
dotnet test "OLI-it.Web" --no-build
```

> There are no test projects yet. When adding tests, use xUnit in a new `OLI-it.Web.Tests` project.

### Troubleshooting Build & Run Errors

- If `dotnet build` fails, provide the full error message and check:
  - NuGet package dependencies are restored (`dotnet restore`)
  - Target framework matches the project configuration
  - Connection strings or secrets are properly configured
- If `dotnet run` fails, ensure the database connection is available and Azure Key Vault secrets are accessible.

## Architecture

**ASP.NET Core Razor Pages** on **.NET 10** backed by **SQL Server via EF Core (database-first)**.

```
Pages/          ← Razor Pages UI + code-behind (.cshtml / .cshtml.cs)
Services/       ← Business logic (injected into pages)
Endpoints/      ← Minimal API endpoints (currently: auth)
Data/           ← EF Core DbContext (OliItDbContext, 60+ DbSets)
Models/         ← Scaffolded EF entities (never edit by hand)
ViewComponents/ ← Reusable UI components (image thumbnails, galleries)
wwwroot/        ← Static assets; Bootstrap + jQuery, no build step
infra/          ← Azure Bicep IaC (App Service, Key Vault, Blob Storage)
docs/           ← Architecture docs, ADRs, domain glossary
```

Dependency flow: **Pages → Services → Data (DbContext)**. Pages should not call `DbContext` directly when a Service exists for that domain.

## Domain Language (German ↔ English)

The database schema uses German names — **preserve them exactly**. Do not rename or translate.

| German | English Meaning |
|--------|-----------------|
| `Stamm` | User / Author |
| `PostIt` | Message |
| `TopLab` | Answer / Response |
| `Angler` | Filter Profile |
| `Code` | Description / Tag |
| `Netz` | Net (wordspace network) |
| `Knoten` | Node |
| `Baum` | Tree |
| `Zweig` | Branch |

Full glossary: [`docs/german-english-quick-reference.md`](../docs/german-english-quick-reference.md)  
Entity details: [`docs/010-domain-entities.md`](../docs/010-domain-entities.md)

## Key Conventions

### Database-First EF Core
- The SQL Server schema is the source of truth. **Never create EF migrations.**
- To reflect schema changes, re-scaffold: see [`docs/ef-scaffolding-guide.md`](../docs/ef-scaffolding-guide.md).
  - If the scaffolding guide becomes outdated due to schema changes, notify the user and provide manual re-scaffolding steps or guidance to update the documentation.
- All entity classes live in `Models/` and are generated — do not hand-edit them.

### Razor Pages Pattern
- Each page has a `.cshtml` (view) and `.cshtml.cs` (PageModel) pair.
- Shared partials live in `Pages/Shared/` — `_Layout.cshtml`, `_Header.cshtml`, `_Sidebar.cshtml`, `_SidebarUnified.cshtml`, entity cards (`_StammCard`, `_PostItCard`, `_TopLabCard`, `_AnglerCard`, `_CodeCard`), child tables (`_Child*Table`), and `_TabsNavigation.cshtml`.
- Use `ViewComponents/` for reusable UI that needs injected services: `ImageThumbnailViewComponent` (single image) and `ImageGalleryViewComponent` (multi-image).
- `ImageThumbnailViewComponent` takes `dateiPath`, `altText`, `width`, `height` — see [`docs/developer-guide.md`](../docs/developer-guide.md) for full usage.

### Authentication
- Cookie-based auth via `CookieAuthenticationDefaults`. No ASP.NET Identity tables.
- Login is handled by `Endpoints/AuthenticationEndpoints.cs` (returns JSON) + `wwwroot/js/authentication.js` (AJAX).
- Rate limit: **5 login attempts per minute per IP** — do not remove this.

### Services
- `WortraumCacheService` — singleton; caches `Netz`/`Baum` hierarchies with 1-hour sliding expiration. Call `WarmupCacheAsync()` on startup, `InvalidateCache()` after writes.
- `AzureBlobStorageService` — singleton; wraps Azure Blob Storage for user image management.
- `SearchService` — scoped; handles full-text search across PostIt/Stamm/TopLab.
- `JournalService` — scoped; retrieves journal/activity log data.
- `ChartService` — scoped; provides data for the Charts page.

### Configuration & Secrets
- Local secrets managed via **User Secrets** (ID: `936429e2-4c07-4bde-9c3e-40e1f6531612`).
- In Azure, connection strings and keys come from **Azure Key Vault** (`oli-it-kv-test`).
- Never commit secrets or connection strings to source.

**Key config entries** (set in `appsettings.Development.json` or User Secrets):
| Key | Description |
|-----|-------------|
| `ConnectionStrings:OliItDb` | SQL Server connection string |
| `ImagesRootUrl` | Blob Storage base URL for images (e.g. `https://oliit.blob.core.windows.net/oliupload`) |

### Architecture Decisions (ADRs)
Active ADRs in `docs/070-decisions/`:
- **ADR-0001**: Database-first approach (no code-first migrations)
- **ADR-0002**: Matchmaking logic lives in SQL stored procedures, not C#
- **ADR-0003**: German table/column names must be preserved

### CI/CD
- Push to any branch → build + test + deploy to **test slot** on `oliitrazorweb` Azure App Service.
- Merge to `main` + manual approval → deploy to **production slot**.
- Infrastructure changes deploy via `infra-main-bicep.yml` (Bicep, also approval-gated for prod).

## Human Docs Refresh Mode (Markdown source + AI-generated HTML output)

When working on documentation:

- Treat `docs/*.md` as canonical source content.
- Generate and refresh `docs/human-overview/*.html` from canonical markdown during documentation sessions.
- Always read `docs/human-overview/human-output.config.yml` before generating HTML.
- Keep shared look-and-feel and behavior in:
  - `docs/human-overview/assets/style.css`
  - `docs/human-overview/assets/app.js`
- Each generated human-overview HTML page must embed metadata in a `doc-metadata` JSON block:
  - `doc_audience`
  - `doc_focus`
  - `context_tags`
  - `agent_load_hint`
  - `source_docs`
  - `last_updated`
- Preserve source traceability by keeping `source_docs` populated with canonical `docs/*.md` references only.
- Include a visible "What's changed" or "Highlights" section in updated human pages when source content changes.

## Docs Folder — Always Consider

Before answering questions about domain, architecture, decisions, or UI, **always consider the relevant files in `docs/`**. Consult them when context is needed; do not rely on memory alone.

| File | Summary |
|------|---------|
| `docs/README.md` | Documentation index and navigation guide |
| `docs/000-motivation.md` | Why OLI-it exists; problem statement and goals |
| `docs/001-vision.md` | Long-term product vision |
| `docs/010-domain-entities.md` | All domain entities with fields and relationships |
| `docs/020-data-model.md` | Database schema overview and ER relationships |
| `docs/030-use-cases.md` | User stories and use-case descriptions |
| `docs/040-ui-ia.md` | UI information architecture; page hierarchy |
| `docs/050-ui-wireframes.md` | Wireframe descriptions for key screens |
| `docs/060-architecture.md` | Technical architecture; layers, patterns, dependencies |
| `docs/070-decisions/ADR-0001-database-first-approach.md` | Decision: use existing DB schema, no EF migrations |
| `docs/070-decisions/ADR-0002-stored-procedure-matchmaking.md` | Decision: matchmaking in SQL stored procs, not C# |
| `docs/070-decisions/ADR-0003-german-table-names.md` | Decision: preserve German table/column names as-is |
| `docs/080-backlog.md` | Prioritised feature and bug backlog |
| `docs/990-open-questions.md` | Unresolved design and product questions |
| `docs/ef-scaffolding-guide.md` | How to re-scaffold EF models from the database |
| `docs/german-english-quick-reference.md` | German ↔ English entity name mapping |
| `docs/developer-guide.md` | Local setup, secrets config, ViewComponent usage, infra deployment |
| `docs/features/entity-visual-identity.md` | Visual identity rules for entity display |
| `docs/human-overview/README.html` | Human-overview workflow, HTML-only model, and metadata convention |
