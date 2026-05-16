# Copilot Instructions for OLI-it.Web

## Git Rules (Always Apply)

- **Never commit** changes without the user reviewing them first.
- **Never push** without explicit user confirmation.
- Always show a `git diff` or file summary and wait for approval before any `git commit` or `git push`.

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

> There are no test projects yet. When adding tests, use xUnit or NUnit in a new `OLI-it.Web.Tests` project.

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
Entity details: [`docs/01-domain-entities.md`](../docs/01-domain-entities.md)

## Key Conventions

### Database-First EF Core
- The SQL Server schema is the source of truth. **Never create EF migrations.**
- To reflect schema changes, re-scaffold: see [`docs/ef-scaffolding-guide.md`](../docs/ef-scaffolding-guide.md).
- All entity classes live in `Models/` and are generated — do not hand-edit them.

### Razor Pages Pattern
- Each page has a `.cshtml` (view) and `.cshtml.cs` (PageModel) pair.
- Shared partials live in `Pages/Shared/` — `_StammCard.cshtml`, `_PostItCard.cshtml`, `_TabsNavigation.cshtml`, etc.
- Use `ViewComponents/` for reusable UI that needs injected services (e.g., `ImageThumbnailViewComponent`).

### Authentication
- Cookie-based auth via `CookieAuthenticationDefaults`. No ASP.NET Identity tables.
- Login is handled by `Endpoints/AuthenticationEndpoints.cs` (returns JSON) + `wwwroot/js/authentication.js` (AJAX).
- Rate limit: **5 login attempts per minute per IP** — do not remove this.

### Services
- `WortraumCacheService` — singleton; caches `Netz`/`Baum` hierarchies with 1-hour sliding expiration. Call `WarmupCacheAsync()` on startup, `InvalidateCache()` after writes.
- `AzureBlobStorageService` — singleton; wraps Azure Blob Storage for user image management.

### Configuration & Secrets
- Local secrets managed via **User Secrets** (ID: `936429e2-4c07-4bde-9c3e-40e1f6531612`).
- In Azure, connection strings and keys come from **Azure Key Vault** (`oli-it-kv-test`).
- Never commit secrets or connection strings to source.

### Architecture Decisions (ADRs)
Active ADRs in `docs/07-decisions/`:
- **ADR-0001**: Database-first approach (no code-first migrations)
- **ADR-0002**: Matchmaking logic lives in SQL stored procedures, not C#
- **ADR-0003**: German table/column names must be preserved

### CI/CD
- Push to any branch → build + test + deploy to **test slot** on `oliitrazorweb` Azure App Service.
- Merge to `main` + manual approval → deploy to **production slot**.
- Infrastructure changes deploy via `infra-main-bicep.yml` (Bicep, also approval-gated for prod).

### Stakeholder Dashboard
A pre-build step runs `docs/generate-dashboard.ps1` to produce an HTML dashboard from the docs. This is triggered automatically during `dotnet build`. The prompt that drives it is in `.github/prompts/generate-dashboard.prompt.md`.
