# Architecture

Last updated: 2026-05-20
Status: stub

## Stack

| Layer | Technology |
|-------|-----------|
| Web framework | ASP.NET Core Razor Pages (.NET 10) |
| ORM | Entity Framework Core (Database-First / Scaffolding) |
| Database | SQL Server (existing schema, German names) |
| Auth | ASP.NET Core Identity (TBD — see OQ-012) |
| Languages | C# |
| Front-end | Razor Pages + Bootstrap + vanilla JS (minimal) |

**Important:** The database schema already exists with German table names (Stamm, Angler, PostIt, etc.) and must not be modified. Matching logic is implemented as a stored procedure.

## Project Structure (proposed)

```
OLI-it.Web/
  Pages/           ← Razor Pages (UI)
  Models/          ← EF Core entity classes
  Data/            ← DbContext, migrations, configurations
  Services/        ← domain services (matchmaking, transactions)
  ViewModels/      ← page-specific view models
  wwwroot/         ← static assets
```

## Key Architectural Decisions

See [070-decisions/](070-decisions/) for ADRs.

**Confirmed:**
- Database-First approach (existing schema) → ADR-0001
- Matching via stored procedure (no C# implementation) → ADR-0002
- German table names preserved for compatibility → ADR-0003

Pending decisions:
- Authentication provider (ASP.NET Identity vs. existing auth) → ADR-0004
- English naming strategy in C# code (extension methods, DTOs) → ADR-0005

## Localization and SEO URL strategy

- Supported UI cultures: `en`, `de`, `es`
- Default/fallback culture: `en`
- URL strategy: language path prefix `/{lang}/...` (example: `/de/search?q=...`)
- Non-prefixed URLs remain valid and resolve with English defaults.
- Unsupported two-letter language prefixes are redirected to the English equivalent URL.
- Shared chrome strings can be localized via `.resx` resources with English fallback.
- The layout can emit SEO metadata (`html lang`, canonical URL, `hreflang` alternates for EN/DE/ES and `x-default`).
- **Current product decision:** broad page-content localization is deferred until core feature-completeness milestones are further along.

## Change Log

- 2026-03-26: Stub created. Stack confirmed from project file inspection.
- 2026-05-20: Added localization architecture details (path-prefix culture routing, English fallback, and SEO metadata behavior).
- 2026-05-20: Marked localization rollout as deferred in delivery sequencing to prioritize feature completeness first.
