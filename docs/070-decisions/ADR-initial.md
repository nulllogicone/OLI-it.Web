# ADR-0001 — Criteria Table Design

Date: 2026-03-26
Status: proposed

## Context

Criteria records link wordspace elements (nodes/branches) to either a Description or a FilterProfile, along with first-value/second-value markings.
Two design options:
1. **Single polymorphic table** with `ContextType` + `ContextId` discriminator columns
2. **Two separate tables**: `DescriptionCriteria` and `FilterProfileCriteria`

## Decision

TBD — to be discussed in brainstorm session.

## Consequences

- Option 1: simpler schema, but no FK enforcement on `ContextId`.
- Option 2: clean FK integrity, but duplicated structure.

---

# ADR-0002 — Matchmaking Execution Model

Date: 2026-03-26
Status: proposed

## Context

The matchmaking algorithm evaluates all active message descriptions against all active filter profiles. Two execution models:
1. **Eager (on-write)**: run matching when a message is published or a filter profile is saved; persist results in `MessageDeliveries`.
2. **Lazy (on-read)**: compute matches at query time when user opens their inbox.

## Decision

TBD.

## Consequences

- Eager: faster inbox reads, but more writes and need to re-run on profile changes.
- Lazy: simple writes, but potentially expensive queries at read time; requires good indexing.

---

# ADR-0003 — Authentication Provider

Date: 2026-03-26
Status: proposed

## Context

Options:
1. **ASP.NET Core Identity** with cookie authentication
2. **Custom session/cookie** (lightweight, mimicking legacy)
3. **External provider** (OAuth, Entra ID)

## Decision

TBD — likely ASP.NET Core Identity for maximum integration with Razor Pages + EF Core.

## Consequences

- Identity adds `AspNetUsers`, `AspNetRoles`, etc. tables — may conflict or overlap with `Users` table design.
- Clarify whether to extend Identity's `IdentityUser` or maintain a separate `Users` table.
