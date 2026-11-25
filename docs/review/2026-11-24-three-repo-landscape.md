# Three-repository landscape review

Date: 2026-11-24
Status: draft; read-only source snapshot, runtime unverified

## Conclusion and scope

The modernization follows a strangler pattern: the legacy application stays live while two modern .NET 10 applications grow next to it, all sharing one SQL Server database. The direction is sound. The main risks lie in the seams between the repositories rather than inside any single one: two independently scaffolded copies of the data model, a database schema that is versioned only in the legacy repository, no recorded cut-over plan, and core semantic workflows that are not yet rebuilt.

Inspected snapshots, all with clean working trees on `main`:

| Repository | Commit | Role |
|---|---|---|
| OLI-it (Azure DevOps, private) | `21ea705f` | Live production: OliWeb (ASP.NET WebForms, .NET Framework 4.8), KatWeb admin (4.8), OliEngine (4.8), FunctionAppFischen (.NET 10 isolated worker) |
| [OLI-it.Web](https://github.com/nulllogicone/OLI-it.Web) | `c422528` | Razor Pages rewrite of the public site (.NET 10, EF Core 10) |
| [nulllogicone.net](https://github.com/nulllogicone/nulllogicone.net) | `37dba67` | Read-only semantic API: HTML/JSON/RDF entity routes and an Ontop SPARQL proxy (.NET 10, EF Core 9) |

This review builds on the [2026-10-10 fresh look](2026-10-10-claude-fresh-look.md) and the [semantic API consistency review](2026-10-19-semantic-api-consistency.md) and does not repeat their findings.

## Strengths

- **Database as the contract.** [ADR-0001](../070-decisions/ADR-0001-database-first-approach.md) and [ADR-0002](../070-decisions/ADR-0002-stored-procedure-matchmaking.md) keep the existing schema and the `fischen`/`beissen` stored procedures authoritative. Legacy and modern applications can run against the same data, and the matchmaking logic is not reimplemented.
- **Separation of UI and semantic API.** People use OLI-it.Web; machines use nulllogicone.net. The two do not depend on each other at runtime.
- **Evidence-based documentation.** The [live experience comparison](../live-experience/comparison.md) distinguishes verified, partial and untested behavior. Integration tests exercise the real stored procedures.
- **Legacy delivery is being stabilized.** In October 2026 the legacy repository gained YAML pipelines for OliWeb, KatWeb and the fischen function, and the function was upgraded to .NET 10.

## Findings

### 1. Duplicated, drifting data model

Both GitHub repositories contain the same 51 scaffolded EF Core entity files under `Models/`, with different namespaces. They have already diverged: `Stamm.Unterschrift` exists only in OLI-it.Web, and the API uses EF Core 9 while OLI-it.Web uses EF Core 10. Every schema change must be applied by hand twice.

Options: a shared model package built from one scaffolding step, or at least one documented scaffold command run against the same schema version in both repositories.

### 2. Schema is not versioned where the modern code lives

The full schema lives in the legacy repository (`Daten/SQL-Scripte`). OLI-it.Web holds only [`fischen.sql`](../sql/fischen.sql) and [`beissen.sql`](../sql/beissen.sql). If the database is the contract, it should be versioned where both modern repositories can build and test against it, for example as an SDK-style SQL database project that also produces the local test database.

### 3. Core semantic workflows remain open

Read-only views are largely rebuilt. The workflows that define OLI-it are not: wordspace navigation and marking (NKBZ/OgIf), describe and send, Angler filter profiles and delivery, and answering through TopLab with rewards. The [backlog](../080-backlog.md) lists BL-004 to BL-011 as not started, so the highest technical and domain risk is still ahead.

### 4. Shared-data changes must stay compatible with production

Because legacy and modern applications use the same tables, data rule changes must work for both at once. Password hashing (BL-109) is the clearest case: the legacy applications read the same password column, so hashing needs a coordinated transition (for example, dual verification during migration) rather than a change in OLI-it.Web alone.

### 5. No cut-over decision

No ADR describes how oli-it.com moves from the legacy site to the rewrite: a single switch, route-by-route proxying (which raises shared sign-in questions), or a new host name first. This decision shapes authentication, URL compatibility and the upload migration, and should precede further parity work.

### 6. Three places track work

Work items appear in Azure DevOps (for example PBI 681 for the admin pipeline), GitHub issues and these docs. The backlog was last updated on 2026-05-20, and several statuses are stale, as the 2026-10-10 review also noted.

### 7. Modern components remain in the legacy repository

FunctionAppFischen already targets .NET 10 but lives in Azure DevOps. The legacy repository also contains an older .NET Framework 4.6.1 `Anwendung/nulllogicone.net` project that the GitHub repository has replaced. Older experiments (`_older`, `Entwicklung_2`, FrontPage-era web folders) make up most of the legacy repository's roughly 14,000 tracked files.

### 8. Repository hygiene

- nulllogicone.net commits about 51 MB of Ontop CLI `.jar` files under `ontop/ontop-cli/lib`; a download script or container image would keep the repository small.
- nulllogicone.net references a release-candidate package, `Microsoft.AspNetCore.OpenApi 10.0.0-rc.2`.
- Legacy user uploads are committed to git in the legacy repository; OLI-it.Web stores attachments in Azure Blob Storage, so these files need migrating at cut-over.

## Suggested order of work

1. Make the database schema a shared, versioned contract and generate both model sets from it.
2. Record an ADR for cut-over and co-existence: authentication, password hashing, URL compatibility and upload migration.
3. Build the core workflows: wordspace navigator, describe and send, Angler, TopLab.
4. Archive inactive parts of the legacy repository so it reflects only what runs in production; decide where FunctionAppFischen belongs.
5. Choose one primary work tracker and link the others to it.

## Verification boundary

This is a static reading of local checkouts. Deployments, the shared production database, runtime behavior and the legacy repository's build health were not checked. Nothing was changed in any repository other than adding this document.
