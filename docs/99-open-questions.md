# Open Questions

Last updated: 2026-04-08
Status: active

## Domain & Protocol

- OQ-001: Does matchmaking run eagerly (on message creation) or lazily (on recipient request)?
- ✅ OQ-002: Answered (Yes) - any logged-in user who can view a PostIt can answer by creating a TopLab, including the message author.
- OQ-003: Is there a limit to how many filter profiles a user can have?
- ✅ OQ-004: Answered - Bound- and Flow-KooK can take any value. Negative values mean the incentive direction for message flow is reversed.
- OQ-005: Are wordspace labels localized (EN/DE/ES per node name)?
- OQ-006: Should Criteria be in a single polymorphic table or split into DescriptionCriteria / FilterCriteria?

## Data Migration

- OQ-007: What is the current SQL Server schema (table names, PKs, FKs)?
- OQ-008: Are legacy user passwords migratable, or do users need to reset?
- OQ-009: Is historical transaction data to be migrated, or does balance carry over as a seed value?
- OQ-010: Are there deprecated tables in the legacy DB that should be ignored?

## Technical

- OQ-011: Target .NET version confirmed as .NET 10?
- OQ-012: Authentication: ASP.NET Core Identity, or custom session/cookie?
- OQ-013: EF Core approach: code-first migrations only, or scaffold from existing schema first?
- OQ-014: Is there an existing REST API in the legacy system to reference?

## UI/UX

- OQ-015: Is the Wordspace Navigator built as a Razor partial, a Blazor component, or JavaScript-driven?
- OQ-016: Which pages need to be mobile-responsive for MVP?
- OQ-017: Is dark/light theme toggle in scope?
- OQ-018: Is the RSS feed format defined? Should it be compatible with the existing XML output?
- OQ-021: [BL-111] StammStatisticsOverview — which activity data feeds the charts? (e.g. journal entries, answer counts, credit deltas, inbox delivery volume) Should the page be public or authenticated-only? Is there a preferred chart library (Chart.js, ApexCharts, etc.)?

## Delivery

- OQ-019: Who validates parity against the legacy system before go-live?
- OQ-020: Is the target a direct cutover or side-by-side staging?

## Change Log

- 2026-03-26: Initial questions from paper analysis and site review.
- 2026-04-08: Added OQ-021 for BL-111 (StammStatisticsOverview chart data + access model).
- 2026-04-08: Answered OQ-002 - logged-in users who can view a PostIt may answer it (create TopLab), including self-answer.
- 2026-04-08: Answered OQ-004 - Bound-/Flow-KooK are unbounded in sign; negative values reverse message-flow incentive direction.
