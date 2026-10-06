# OLI-it — Motivation and transition overview

Updated: 2026-10-02
Status: draft; initial orientation, not a verified feature audit

## Purpose and ownership

Complete the transition from the stable production OLI-it system to modern workloads while preserving its protocol, data, and meaningful behavior. The aim is to finish the modern public UI and replace the legacy administration workflows, using the existing system as a behavioral reference. OLI-it is an open protocol for exchanging meaning, not just data.

## Development goals

- Complete the modern ASP.NET Core Razor Pages UI and required admin workflows.
- Support both SAPCT message flow and NKBZ Wortraum maintenance.
- Keep the UI usable by people and agents through clean HTML, predictable navigation, and explicit interactions.


## Where project knowledge lives

- This file: cross-repository overview, constraints, proposed milestones, current evidence, and next step.
- Existing `docs/` files: vision, domain rules, use cases, architecture, and developer guidance.
- `docs/070-decisions/`: accepted decisions and their reasons. Distinguish proposals from accepted decisions.
- GitHub issues: actionable work, acceptance criteria, feedback, and dependencies. Link rather than maintain a second detailed task backlog here.
- Codex and ChatGPT chats: exploration and execution history. Transfer durable conclusions into repository documentation; read relevant chats explicitly when needed.

For each completed slice, update its issue and any affected decision or status documents with evidence. Do not infer completion from an issue title or an old checkbox.

## Systems and initial evidence

| System | Role | Initial assessment | Evidence boundary |
|---|---|---|---|
| Azure DevOps OLI-it | Legacy production reference, including OLIWeb, KatWeb admin, WCF services and older experiments | Owner reports stable .NET Framework 4.8; replace/rewrite rather than migrate the application | README review only; production revision and classic pipeline definitions not inspected |
| GitHub nulllogicone.net | Modern semantic API: HTML, JSON, RDF; Ontop SPARQL over SQL Server | Owner reports fairly stable; READMEs describe deployment and remaining entity/ontology expansion | Local README review; runtime/API coverage not tested |
| GitHub OLI-it.Web | Modern public UI and potential admin replacement | READMEs describe .NET 10 Razor Pages, EF Core database-first, SQL Server, Bootstrap/vanilla JS and Azure hosting | UI basically works per owner; feature completion not audited |
| KatWeb replacement | Admin and vocabulary maintenance | Owner reports admin UI not yet migrated | Identify every required admin workflow before choosing implementation boundaries |

Local workspaces observed:

- Legacy: `C:/Users/luchtfr/source/repos/DevOps/nulllogicone/OLI-it`
- Modern UI: `C:/Users/luchtfr/source/repos/GitHub/nulllogicone/OLI-it.Web`
- Semantic API: `C:/Users/luchtfr/source/repos/GitHub/nulllogicone/nulllogicone.net` (readable; outside this chat's configured writable roots)

## Protocol and compatibility constraints

- Preserve the three root pillars: **from whom – about what – to whom**. Add agent types/skills/purpose below them.
- SAPCT describes message flow: Stamm, Angler, PostIt, Code, TopLab. NKBZ describes Wortraum: Netz, Knoten, Baum, Zweig. OgIf and KooK add logic and reward semantics.
- Existing modern documentation specifies reuse of the SQL database/schema and matchmaking procedures. Treat this as the initial compatibility baseline; any schema or behavior change needs an explicit decision.
- Retain Razor Pages as the current implementation baseline. The SPA issue is an experiment proposal, not an accepted replacement decision.
- Broad UI localization is currently deferred in the modern README. Vocabulary translation and child-record maintenance remain relevant admin requirements.

## Proposed delivery sequence

1. **Establish the baseline.** Audit code and representative legacy workflows; record implemented/partial/missing/out-of-scope with evidence. Identify deployed commit, classic CI/CD configuration, environments, database dependencies, and modern deployment behavior.
2. **Complete one communication cycle.** Registration/login; message creation and semantic marking; filter profile management; matching/inbox; answers; ratings and credit effects. Verify against known legacy examples, including permissions and failure cases.
3. **Replace required admin workflows.** Inventory KatWeb; implement vocabulary CRUD, relationships, translations, child records and access control. Document maintenance walkthroughs. These capabilities can support agent vocabulary without manually editing production data.
4. **Validate replacement readiness.** API compatibility, operational requirements, representative data, deployment isolation, side-by-side acceptance, rollback and cutover criteria. Retire legacy components only after their replacements are accepted.
5. **Extend deliberately.** AI participants, assistant functionality, analytics, matching optimization and UI experiments according to agreed priorities. Some may proceed earlier when independent of parity work.

This sequence is proposed, not a scheduling commitment. Each milestone needs measurable acceptance criteria after the feature audit.

## Existing issue map

Issue bodies were retrieved on 2026-10-02. The search result did not supply reliable issue state; this list does not claim these items remain open or unimplemented. Comments and PR history were not audited.

- Account creation: [#8](https://github.com/nulllogicone/OLI-it.Web/issues/8).
- Vocabulary/admin and agent semantics: [#45](https://github.com/nulllogicone/OLI-it.Web/issues/45), [#46](https://github.com/nulllogicone/OLI-it.Web/issues/46).
- Matchmaking optimization and baseline comparison suite: [#41](https://github.com/nulllogicone/OLI-it.Web/issues/41), [#42](https://github.com/nulllogicone/OLI-it.Web/issues/42). Separate intentional semantic changes, such as NOT support, from performance-only parity checks.
- Development/deployment reliability: [#34](https://github.com/nulllogicone/OLI-it.Web/issues/34), [#37](https://github.com/nulllogicone/OLI-it.Web/issues/37).
- Localization: [#9](https://github.com/nulllogicone/OLI-it.Web/issues/9).
- Product assistant and analytics: [#4](https://github.com/nulllogicone/OLI-it.Web/issues/4), [#38](https://github.com/nulllogicone/OLI-it.Web/issues/38).
- Design and discovery: #15–18, #20, #22, #25, #27–30; #25 and #28 have matching descriptions and need reconciliation before new work.
- SPA experiment: [#33](https://github.com/nulllogicone/OLI-it.Web/issues/33).

## Imported chat context

The app exposed the ChatGPT project **OLI-it**. Two project chats were visible in the current recent-chat listing and read:

- **Brainstorm AI Agents**: Frederic's correction fixes the three root pillars and places extensions below them. The assistant's proposed Baum/type/skill structures remain proposals.
- **Open Source Oli It Funding**: visible messages establish Markdown preference and the mission phrase above. The reported downloadable mission file itself was not retrieved; no detailed funding strategy was available in the returned history.

The related chat **Create Oli It Project**, listed outside the project, was also read. It establishes separation of OLI-it from private/family context. This is not an exhaustive import of older or archived project chats, files, or project instructions.

## Documentation reconciliation needed

- Root README links use names such as `00-vision.md`, `01-domain-entities.md`, `07-decisions/`, and `08-backlog.md`; the local files use `001-vision.md`, `010-domain-entities.md`, `070-decisions/`, and `080-backlog.md`.
- The unfinished, conflicting proposals formerly in `070-decisions/ADR-initial.md` were removed; accepted decisions are recorded in the numbered ADRs.
- Vision, backlog and open questions differ on RSS scope. The backlog records RSS as completed because it was removed from scope, while the vision still lists it as required.
- Backlog status may lag implementation; it must be verified against code and behavior.
- Wortraum README performance figures and production-readiness statements are documentation claims, not measurements verified in this review.
- TestData README describes `data/null.bak` but its sample configuration uses another backup path. Verify actual configuration when running the suite.

## Next concrete step

Build an evidence-backed feature parity matrix for public UI and KatWeb admin, linked to existing issues. Start with the complete communication cycle and vocabulary maintenance. Reconcile documentation against that evidence before assigning new feature priorities.

## Review scope

Read README-named files found with `rg --files` in the legacy and modern UI repositories (8 and 7 respectively), and 5 in the adjacent semantic API repository, including historical/vendor READMEs. Also read modern vision, motivation, backlog, open questions and initial decision proposals, plus the chats and issue bodies above. Ignored/generated files were excluded. No application builds, tests, production inspection, full Git history review, or classic pipeline inspection were performed. Application code and deployed systems were not modified.
