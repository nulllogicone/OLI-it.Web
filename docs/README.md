# OLI-it.Web — Documentation Index

Last updated: 2026-10-07

## How to use these docs

- Each file has a `Status` header: **draft → reviewed → final**.
- IDs cross-link files: `UC-001`, `ENT-User`, `ADR-0001`.
- Refine incrementally; record every decision in `070-decisions/`.
- Park unresolved items in `990-open-questions.md`.
- **German→English mapping:** See [020-data-model.md](020-data-model.md) for database entity names.
- **Delivery note:** broad page-content localization rollout is currently deferred until core feature parity progresses further.

## Document Responsibilities

Read from principles to specifics: [philosophy](philosophy.md), [migration context](000-motivation.md), [application goals](001-vision.md), [domain concepts](010-domain-entities.md), then [database compatibility](020-data-model.md) and implementation details. This is a reading route, not a claim that the protocol depends on this SQL implementation.

Each definition or detailed explanation has one authoritative home. Other documents give only the context needed for their purpose and link a short, meaningful phrase to that home. Preserve complementary perspectives rather than merging documents or repeating their contents. The durable editing rule lives in [AGENTS.md](../AGENTS.md#documentation-structure-and-linking).

## Infrastructure Deployment Notes

- Infra template: `infra/main.bicep`
- Test parameters: `infra/main.test.bicepparam`
- Production parameters: `infra/main.prod.bicepparam`
- The legacy `infra/main.bicepparam` file is no longer used.
- CI workflow `.github/workflows/infra-main-bicep.yml` has split jobs:
	- test deployment job (`environment: test` or `push` to `main`)
	- production deployment job (`environment: production`) with GitHub Environment approval gate.
- First deployment to an empty resource group can be run as test first; production-specific settings are applied only by production deployment.

## Files

Live application exploration: [Live experience journey](live-experience/README.md) records side-by-side test-slot walkthroughs, evidence and research references. Initial access baseline: 2026-10-03; feature parity remains unverified.

| File | Purpose | Status |
|------|---------|--------|
| [philosophy.md](philosophy.md) | Technology-independent protocol principles and design compass | draft |
| [000-motivation.md](000-motivation.md) | Migration rationale, current transition boundaries, evidence, and proposed sequence | draft |
| [001-vision.md](001-vision.md) | Application outcomes, success criteria, and draft phased goals | draft |
| [010-domain-entities.md](010-domain-entities.md) | Conceptual entities, relationships, and business rules | draft |
| [020-data-model.md](020-data-model.md) | SQL/EF naming map, generated-model usage, and database compatibility | draft |
| [030-use-cases.md](030-use-cases.md) | User stories and acceptance criteria | draft |
| [040-ui-ia.md](040-ui-ia.md) | Information architecture, screens, navigation | draft |
| [050-ui-wireframes.md](050-ui-wireframes.md) | Low-fidelity wireframes and interaction notes | stub |
| [060-architecture.md](060-architecture.md) | Technical architecture, Database-First approach | draft |
| [065-magic-match-logic.md](065-magic-match-logic.md) | SQL matchmaking behavior (`fischen`/`beissen`) | draft |
| [070-decisions/](070-decisions/) | Architecture Decision Records (ADRs) | draft |
| [080-backlog.md](080-backlog.md) | Prioritized MVP slices | stub |
| [990-open-questions.md](990-open-questions.md) | Unresolved items parking lot | draft |


