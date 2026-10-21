# Claude fresh look review — 2026-10-10

Status: draft
Scope: one-time review of OLI-it.Web (`origin/main` at `669f66d`) against its documentation and the legacy
WebForms application (Azure DevOps `nulllogicone/OLI-it`, `main`), plus a proposal for how AI coding agents
(GitHub Copilot, Codex, Claude Code) work together on this repository.

This is a point-in-time snapshot, not an authoritative home for any definition. Durable conclusions belong in
the owning documents listed in [docs/README.md](../README.md); work items belong in
[GitHub issues](https://github.com/nulllogicone/OLI-it.Web/issues). Security-sensitive findings are
deliberately kept out of this public document and are handled separately.

## Summary

- The application has a sensible structure (Razor Pages, scaffolded EF Core model, a few services) and the
  newest code (PostIt creation) is of good quality with meaningful tests.
- Typical "vibe coding" debt: duplicated logic, inconsistent patterns between features, magic values,
  data access spread across page models.
- Several documents describe plans as if they were implemented, or describe implemented features as not
  started. The backlog is the most outdated.
- Legacy parity covers most read-only views; most write interactions beyond creating and editing are missing.
- Agent instructions are split across two files that have drifted; no Claude Code instructions exist.

## Code and architecture

| Area | Observation | Evidence |
|------|-------------|----------|
| Sign-in | Claims/sign-in logic exists three times; `LoginModel.OnPostAsync` and `Logout.OnPostAsync` are unused because the UI calls `/api/login` and `/api/logout`. | `OLI-it.Web/Endpoints/AuthenticationEndpoints.cs`, `Pages/Login.cshtml.cs`, `Pages/Register.cshtml.cs` |
| Edit permissions | Viewing the PostIt edit page is allowed for any connected Stamm, saving only for the author, so non-authors get a 403 after editing. | `Pages/PostIt/Index.cshtml.cs`, `Pages/PostIt/Edit.cshtml.cs` |
| Data access | 18 page models query `OliItDbContext` directly; only Search, Journal, Chart, Wortraum and PostIt creation use services. `PostIt/Index` loads Spiegel rows per Code (N+1). | `Pages/**/*.cshtml.cs`, `Services/` |
| Magic values | Author lookup `StammZust == 1` repeated in about eight files; message type `"txt"` as a string literal. | `Pages/PostIt/*`, `Pages/Stamm/*` |
| Views | `_SidebarUnified.cshtml` uses `@model dynamic` and catches binder exceptions; sidebar selection via `ViewData["Sidebar"]`. `Wortraum.cshtml` (≈900 lines) carries inline script and style. Unused partials: `_SidebarPostIt`, `_SidebarStamm`, `_CodeCard`. | `Pages/Shared/`, `Pages/Wortraum.cshtml` |
| Style | Mixed namespace styles and constructor styles; view components in both `Views/Shared/Components` and `Pages/Shared/Components`. | — |
| Passwords | The modern app compares and stores `Stamm.Unterschrift` as the legacy app does ([OQ-008](../990-open-questions.md)). Because both applications share one database, hashing cannot be introduced in OLI-it.Web alone. | `AuthenticationEndpoints.cs`, `Register.cshtml.cs` |
| Tests | `PostItCreationTests` are meaningful (accounting, retry idempotency, rollback, tampering). `MatchmakingTests` return early, reporting success, when the test backup is missing. No tests for sign-in, edit authorization, search, journal, charts, Wortraum. | `OLI-it.Web.Tests/` |
| CI/CD | CI runs only `--filter PostItCreationTests`. Every branch push deploys to the shared `test` slot, including documentation-only changes. | `.github/workflows/oliitrazorweb.yml` |
| Solution | `OLI-it.Web.slnx` lists documentation files under their old names (`docs/00-vision.md` …). | `OLI-it.Web.slnx` |
| Infra | `linuxFxVersion` parameter is unused on the Windows App Service; the .NET runtime is not pinned; `infra/main-fresh.json` looks like a stale compiled template. | `infra/` |

## Documentation versus implementation

Mismatches to reconcile in the owning documents (no scope or behavior change implied):

- [080-backlog.md](../080-backlog.md): BL-001 claims ASP.NET Identity and BL-002 an initial migration
  (neither exists; see [ADR-0001](../070-decisions/ADR-0001-database-first-approach.md)). BL-006, BL-013,
  BL-014 and BL-111 are "not started" although create/edit, charts, search and statistics pages exist.
  BL-003 is "in progress" although registration works. BL-017 (RSS) is marked "completed" in the sense of
  "removed from scope", while [001-vision.md](../001-vision.md) still lists RSS as a goal.
- [060-architecture.md](../060-architecture.md): references ADR-0004/ADR-0005 (do not exist), a
  `ViewModels/` folder and migrations (do not exist), and `/{lang}/` routing with `.resx` (not implemented);
  `Endpoints/`, `ViewComponents/` and `Views/` are not described.
- [020-data-model.md](../020-data-model.md), [010-domain-entities.md](../010-domain-entities.md) and ADR-0001
  list `Olis`, `get`, `Ilos`, `fit` as tables; in the scaffolded model they are columns of `Ringe` and `Löcher`.
- [010-domain-entities.md](../010-domain-entities.md) and [030-use-cases.md](../030-use-cases.md) state that
  passwords are hashed; the implementation keeps legacy behavior (see OQ-008).
- [ADR-0002](../070-decisions/ADR-0002-stored-procedure-matchmaking.md) says a UI button and a queue/Azure
  Function path invoke `fischen`. That matches the legacy application; OLI-it.Web does not call `fischen`
  or `beissen` yet. When matching runs is described differently in ADR-0002, 065, 010 and OQ-001.
- [developer-guide.md](../developer-guide.md) and `.github/copilot-instructions.md` say there are no test
  projects; the latter also links a missing `docs/ef-scaffolding-guide.md` and recommends a cache warm-up
  call that the code does not make.
- [040-ui-ia.md](../040-ui-ia.md) routes (`/Messages`, `/FilterProfiles`, `/Inbox`, `/Admin/Wordspace`) differ
  from the implemented routes (`/postit/…`, `/stamm/{id}/angler`, …).
- [docs/README.md](../README.md) does not list `philosophy.md`, `developer-guide.md`,
  `090-legacy-login-observability.md`, `features/` or `design-overview-page/`; an empty line inside its file
  table breaks the table rendering.

Repeated explanations that should collapse to one home with links: German→English mapping (home:
020-data-model), stack table, build/run/test steps (already drifted between README, developer guide and
Copilot instructions), infrastructure deployment notes, and the "localization deferred" note (six places).

Decisions made in code without an ADR: custom cookie authentication instead of Identity, legacy password
compatibility, idempotent PostIt creation (ticket plus SQL application lock, credit via `oli.zahlen`),
HTML-encoding on store for legacy compatibility, the committed test database backup `data/null.bak`
(confirmed on 2026-10-10 to be public test data), and a single shared test slot for all branches.

## Legacy parity

Reference: legacy OliWeb pages and user controls on `main`. KatWeb (admin), OLIService, OLIXml beyond RSS and
nulllogicone.net are out of scope per [000-motivation.md](../000-motivation.md).

| Legacy feature | OLI-it.Web | Notes |
|----------------|-----------|-------|
| Login / logout | done | same credential behavior as legacy |
| Register Stamm | done | |
| Stamm view / edit | done | |
| Stamm inbox, news, shortcuts | missing | |
| PostIt create | done | `PostItCreationService` |
| PostIt edit | done | |
| Semantic marking (Code, Wortraum picker) | partial | creation adds a default Code; Wortraum is read-only |
| Angler create / edit, Löcher | missing | Angler page is read-only |
| Matching and catch lists | partial | catch lists are shown; PostIts created in OLI-it.Web never trigger `fischen` |
| TopLab (answer) create | missing | read-only views |
| Rating, comments, credit payouts | missing | only `oli.zahlen` on create |
| Journal, Charts, Search | done | |
| RSS feeds | not built | removed from scope per BL-017; still a goal in 001-vision |
| Sitemaps, multiple languages | missing / deferred | |

Both applications write to the same tables (Stamm, PostIt, Code, Ringe, credit balances). The modern
application's application lock is not honored by the legacy application.

## Agent collaboration

### Current state

- `AGENTS.md`: documentation and commit rules.
- `.github/copilot-instructions.md`: build, architecture, glossary, docs index and commit rules; partly
  outdated and duplicating `AGENTS.md` and `docs/README.md`.
- No `CLAUDE.md`. `.vscode/settings.json` auto-approves `az` terminal commands for chat agents.
- Branch prefixes already show who did the work: `copilot/*` (Copilot coding agent), `codex/*` (Codex),
  `agents/*` (VS Code agent sessions), `frederic-luchting/*` (maintainer).

### Proposal (this round)

1. **One instruction source.** `AGENTS.md` becomes canonical (read natively by Codex and Copilot) and gains
   short sections for commands, architecture, conventions, definition of done and where the legacy code lives.
   `CLAUDE.md` imports `AGENTS.md` and adds only Claude-specific notes; `.github/copilot-instructions.md`
   shrinks to a pointer plus Copilot-specific notes.
2. **Roles by strength.**
   - Copilot coding agent: small, well-specified GitHub issues with acceptance criteria → pull request (`copilot/*`).
   - Codex: longer feature journeys and live-experience documentation (`codex/*`).
   - Claude Code: cross-repository legacy analysis, refactoring, reviews, documentation reconciliation (`claude/*`).
   - Maintainer: writes issues, integrates, and is the only one who commits to `main`.
3. **One issue, one branch.** Before starting work an agent asks whether to stay on the current branch or
   create a new branch in the same repository; separate worktrees only on request.

### Future features

- **Cross-review:** each agent pull request is reviewed by a different agent before merge.
- **CI as referee:** run the full test suite, skip (not pass) tests whose prerequisites are missing, deploy to
  the test slot only from `main` or by manual dispatch.
- **Shared state in GitHub and docs:** issues labeled per agent; a legacy parity table that links issues
  rather than duplicating the backlog.
- **Legacy access for Claude Code:** add the legacy checkout as an additional working directory in an
  uncommitted local settings file, so no machine paths enter this public repository.

## Proposed order of work

Each step starts by asking which branch to use; changes stay uncommitted until the maintainer reviews them.

1. Resolve the security-sensitive findings (handled outside this document).
2. Agent setup: consolidate `AGENTS.md`, add `CLAUDE.md`, slim the Copilot instructions, add an issue
   template with acceptance criteria, fix the stale `.slnx` entries.
3. Documentation reconciliation: the mismatches above, deduplication, ADRs for the undocumented decisions,
   and a legacy parity document.
4. Small code fixes, suitable as Copilot issues: one edit-permission rule for GET and POST, a single sign-in
   service, constants for `StammZust` and message types, removal of dead handlers and partials.
5. Items that need a design decision first: password hashing coordinated with the legacy application,
   triggering `fischen` after PostIt creation, and the next parity features.
