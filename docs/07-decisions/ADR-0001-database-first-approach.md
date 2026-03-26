# ADR-0001: Database-First Approach with Existing Schema

**Status:** Accepted  
**Date:** 2026-03-26  
**Decision Makers:** Development Team

## Context

The OLI-it.Web application needs to connect to an existing SQL Server database that was created years ago with German table names. The database contains the complete schema for:
- User management (Stamm)
- Message flow (PostIt, Code, TopLab, Angler)
- Wordspace structure (Netz, Knoten, Baum, Zweig)
- Matching logic (Olis, Ilos, get, fit)

The matching algorithm is implemented as a stored procedure that must continue to work without modification.

## Decision

We will use **EF Core Database-First** approach via scaffolding:

1. Use `dotnet ef dbcontext scaffold` to generate entity classes from the existing database
2. Place generated entities in `Models/` directory
3. **Do not modify the database schema** - the application is read/write only
4. **Do not use EF migrations** - schema changes are not permitted
5. Call existing stored procedures for matchmaking via `FromSqlRaw()` or similar
6. Preserve German table names in the database
7. Use generated entity classes directly or create DTOs/ViewModels with English names for clarity in application code

## Consequences

### Positive
- No risk of breaking existing database structure
- Existing stored procedure logic continues to work
- Faster initial setup (no schema design needed)
- Can immediately start building UI and business logic

### Negative
- Entity class names will be in German, may require documentation/comments
- Cannot use EF migrations for schema evolution
- Limited control over entity class structure (can be customized post-scaffold)
- Must coordinate any schema changes with database administrator

### Mitigation
- Create comprehensive German↔English mapping documentation (see `02-data-model.md`)
- Consider creating English-named DTOs or extension methods for clarity
- Add XML comments to generated entity classes explaining their purpose
- Use partial classes to extend generated entities without modifying scaffolded code

## Alternatives Considered

### Code-First with Reverse Engineering
Generate initial entities via scaffolding, then switch to Code-First with migrations.

**Rejected because:** Stored procedure must remain unchanged; migrations would create schema drift risk.

### Manual Entity Creation
Hand-write entity classes to match database schema with English names.

**Rejected because:** Error-prone, time-consuming, and scaffolding provides accurate mapping including relationships.

## References

- `02-data-model.md` - Database schema and German→English mapping
- EF Core scaffolding documentation: https://learn.microsoft.com/en-us/ef/core/managing-schemas/scaffolding/
