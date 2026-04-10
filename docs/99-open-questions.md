# Open Questions

Last updated: 2026-04-09
Status: active

## Domain & Protocol

- ✅ OQ-001: Answered - Matchmaking runs eagerly, triggered when a Code or Angler record is created or changed.
- ✅ OQ-002: Answered (Yes) - any logged-in user who can view a PostIt can answer by creating a TopLab, including the message author.
- ✅ OQ-003: Answered - No limit; a user can have an unlimited number of filter profiles.
- ✅ OQ-004: Answered - Bound- and Flow-KooK can take any value. Negative values mean the incentive direction for message flow is reversed.
- ✅ OQ-005: Answered - Wordspace labels are localized. NKBZ tables already carry an English translation field; that is sufficient for MVP. Additional languages (ES, etc.) deferred to a later iteration.

## Data Migration

- ✅ OQ-007: Answered - Schema is already scaffolded into EF Core (see Models/ and OliItDbContext.cs).
- ✅ OQ-008: Answered - Legacy passwords are migrated as-is and remain usable. Hashing and salting is deferred to a future iteration.
- ✅ OQ-009: Answered - No data migration. The existing database stays unchanged; this project modernizes the UX only.
- ✅ OQ-010: Answered - N/A. Database is not being migrated; deprecated tables in the legacy DB are simply not used by the new app.

## Technical

- ✅ OQ-011: Answered - Target framework is .NET 10.
- ✅ OQ-012: Answered - Custom session/cookie authentication (not ASP.NET Core Identity).
- ✅ OQ-013: Answered - Scaffolded from the existing database schema.
- ✅ OQ-014: Answered - Yes. There is an existing modern REST API for JSON and RDF at https://nulllogicone.net.

## UI/UX

- OQ-015: Is the Wordspace Navigator built as a Razor partial, a Blazor component, or JavaScript-driven?
- ✅ OQ-016: Answered - All pages should be mobile-responsive for MVP.
- ✅ OQ-017: Answered - Not in scope yet; localization is higher priority.

## Delivery

- ✅ OQ-019: Answered - The current project team validates parity against the legacy system before go-live.
- ✅ OQ-020: Answered - Side-by-side staging.

## Change Log

- 2026-03-26: Initial questions from paper analysis and site review.
- 2026-04-08: Added OQ-021 for BL-111 (StammStatisticsOverview chart data + access model).
- 2026-04-08: Answered OQ-002 - logged-in users who can view a PostIt may answer it (create TopLab), including self-answer.
- 2026-04-08: Answered OQ-004 - Bound-/Flow-KooK are unbounded in sign; negative values reverse message-flow incentive direction.
- 2026-04-09: Answered OQ-001 - Matchmaking is eager; triggered on Code or Angler change.
- 2026-04-09: Answered OQ-003 - No limit on filter profiles per user.
- 2026-04-09: Answered OQ-005 - Labels localized via NKBZ English translation field; further languages deferred.
- 2026-04-09: Removed OQ-006 - question was based on a misunderstanding.
- 2026-04-09: Answered OQ-007 - Schema already scaffolded into EF Core.
- 2026-04-09: Answered OQ-008 - Legacy passwords migrated as-is; hashing/salting deferred.
- 2026-04-09: Answered OQ-009 & OQ-010 - No data migration; existing DB stays unchanged, project is a UX modernization.
- 2026-04-09: Answered OQ-011 - Target framework confirmed as .NET 10.
- 2026-04-09: Answered OQ-012 - Custom session/cookie authentication.
- 2026-04-09: Answered OQ-013 - EF model scaffolded from the existing database.
- 2026-04-09: Answered OQ-014 - Existing modern REST API for JSON/RDF at https://nulllogicone.net.
- 2026-04-09: Answered OQ-016 - All MVP pages must be mobile-responsive.
- 2026-04-09: Answered OQ-017 - Theme toggle deferred; localization prioritized first.
- 2026-04-09: Removed OQ-018 - RSS is no longer needed.
- 2026-04-09: Removed OQ-021 - StammStatisticsOverview was a non-essential idea and is out of scope.
- 2026-04-09: Answered OQ-019 - Parity validation before go-live is done by the current project team.
- 2026-04-09: Answered OQ-020 - Rollout strategy is side-by-side staging.
