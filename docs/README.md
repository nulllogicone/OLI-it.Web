# OLI-it.Web — Documentation Index

Last updated: 2026-05-20

## How to use these docs

- Each file has a `Status` header: **draft → reviewed → final**.
- IDs cross-link files: `UC-001`, `ENT-User`, `ADR-0001`.
- Refine incrementally; record every decision in `070-decisions/`.
- Park unresolved items in `990-open-questions.md`.
- **German→English mapping:** See [german-english-quick-reference.md](german-english-quick-reference.md) for database entity names.
- **Delivery note:** broad page-content localization rollout is currently deferred until core feature parity progresses further.

## Human-friendly overview docs

- For fast owner-level reading, open [human-overview/index.html](human-overview/index.html).
- Human-overview workflow and metadata conventions are documented in [human-overview/README.html](human-overview/README.html).
- Existing deep technical markdown docs remain unchanged and are still the source specification.
- Local preview for rendered markdown links:
  - Start a local server from repository root: `python -m http.server 8080`
  - Open: `http://localhost:8080/docs/human-overview/index.html`

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

| File | Purpose | Status |
|------|---------|--------|
| [german-english-quick-reference.md](german-english-quick-reference.md) | Quick lookup: German↔English table names | draft |
| [human-overview/](human-overview/README.html) | Curated human-readable HTML overview pages with embedded metadata | draft |
| [ef-scaffolding-guide.md](ef-scaffolding-guide.md) | How to scaffold existing database with EF Core | draft |
| [000-motivation.md](000-motivation.md) | Why OLI-it.Web is being built, core drivers | draft |
| [001-vision.md](001-vision.md) | Product intent, goals, success criteria | draft |
| [010-domain-entities.md](010-domain-entities.md) | Business entities, relationships, rules (with German mapping) | draft |
| [020-data-model.md](020-data-model.md) | Existing database schema, German→English mapping | draft |
| [030-use-cases.md](030-use-cases.md) | User stories and acceptance criteria | draft |
| [040-ui-ia.md](040-ui-ia.md) | Information architecture, screens, navigation | draft |
| [050-ui-wireframes.md](050-ui-wireframes.md) | Low-fidelity wireframes and interaction notes | stub |
| [060-architecture.md](060-architecture.md) | Technical architecture, Database-First approach | draft |
| [065-magic-match-logic.md](065-magic-match-logic.md) | SQL matchmaking behavior (`fischen`/`beissen`) | draft |
| [070-decisions/](070-decisions/) | Architecture Decision Records (ADRs) | draft |
| [080-backlog.md](080-backlog.md) | Prioritized MVP slices | stub |
| [990-open-questions.md](990-open-questions.md) | Unresolved items parking lot | draft |

## Templates
See [templates/](templates/) for entity, use-case, ADR and backlog-item templates.
