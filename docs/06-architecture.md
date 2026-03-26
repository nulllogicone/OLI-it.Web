# Architecture

Last updated: 2026-03-26
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

See [07-decisions/](07-decisions/) for ADRs.

**Confirmed:**
- Database-First approach (existing schema) → ADR-0001
- Matching via stored procedure (no C# implementation) → ADR-0002
- German table names preserved for compatibility → ADR-0003

Pending decisions:
- Authentication provider (ASP.NET Identity vs. existing auth) → ADR-0004
- English naming strategy in C# code (extension methods, DTOs) → ADR-0005

## Change Log

- 2026-03-26: Stub created. Stack confirmed from project file inspection.
