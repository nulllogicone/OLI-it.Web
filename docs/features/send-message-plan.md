# Send-message implementation plan

Updated: 2026-10-03
Status: proposed; implementation not started
Branch: codex/send-message
Base inspected: 3899766

## Outcome

A signed-in author can create a PostIt in modern OLI-it.Web, describe author/content/desired recipient in the existing Wortraum, and send it through the existing mutual matching procedures. Preserve the existing database schema and protocol semantics.

Deliver in reviewable increments. Compose-and-save is useful independently, but do not claim delivery is implemented until marking, matching and visible results work.

## Verified baseline

- Legacy UI created `Journey-20261003 — Legacy UI`, PostIt `5456bfb3-8eeb-4a84-854c-2d556c1032a8`; it is readable in modern UI. The owner confirms the slots share the database.
- Modern Stamm and PostIt navigation exposes no creation action. `Pages/PostIt/Edit.cshtml.cs` requires an existing ID on GET and POST.
- `Pages/Code/Index.cshtml.cs` and `Pages/PostIt/Code.cshtml.cs` load existing records; they have no mutation handlers. `Pages/Wortraum.cshtml.cs` browses the hierarchy, including lazy child loading; marking persistence is not implemented there.
- Keep the three pillars: from whom, about what, to whom. Reuse the schema, `oli.fischen` and `oli.beissen`, per ADR-0001 and ADR-0002.
- Legacy references: `Anwendung/OLIWeb/Sites/Edit/PostItMaker.aspx.cs`, `Entwicklung/OliEngine/OliMiddleTier/OLIs/PostIt.cs`, `Entwicklung/OliEngine/OliCommon.cs`, and `Entwicklung/OliEngine/OliDataAccess/Functions/Zahlmeister.cs` in the DevOps repository.

## Increment 1: compose and save

1. Add an authenticated `/postit/create` Razor Page. Add a clear `New message` entry point to the signed-in user's sidebar and own PostIt list. When browsing another Stamm, the action must create for the signed-in author, never for the viewed profile.
2. Use a dedicated input model: title, required nonblank body, plain-text type initially, optional URL/image selection and deadline/value controls as the verified rules permit. Enforce database length/precision limits. Preserve entered data on errors. Cancel and GET must create no records or credit entries.
3. Put creation in a scoped application service, independent of the PageModel. Derive author identity from authentication claims and recheck account existence server-side. Use anti-forgery protection and owner checks for all later mutations.
4. Persist PostIt, author Wurzeln (`StammZust = 1`), initial Code and required credit records atomically. Generate stable IDs for duplicate-submit handling so double clicks/retries cannot charge twice or create duplicate messages. Redirect after success.
5. Preserve the legacy initialization contract after verifying procedures/triggers against an isolated SQL Server fixture: text type, timestamp, zero hits, default Flow-KooK 0.01, default Code comment and unscanned state, author link, initial PostItKonto entry, and `oli.zahlen` transfer of 1 KooK with 10-day deadline. Inspect SQL transaction behavior before wrapping calls. Do not reproduce ledger effects manually if the procedure already owns them.
6. Inspect automatic ShortCuts copying and event/notification behavior. Support required legacy semantics, or explicitly document an agreed narrower first increment; do not silently omit them.
7. Redirect to the message's description step with `Message saved; describe it to find recipients`. Until increment 2 exists, show the saved detail and clearly state that delivery is not yet available.
8. Store new plain text consistently with modern Razor encoding; verify ampersands, quotes, Unicode and markup-like input in both UIs. Legacy encodes text before storage, so do not assume one storage convention fixes historical content. Avoid enabling arbitrary HTML composition in this first increment.

## Increment 2: describe the message

1. Add an owner-authorized Code editor, reusing Wortraum navigation and lazy loading without putting per-user markings in the shared vocabulary cache.
2. Load and persist Ringe coordinates and directional OLIs/get values. Validate IDs and hierarchy relationships against the vocabulary, and enforce allowed combinations on the server.
3. Explain strictness and receiver thresholds in user terms. Keep selected criteria visible and editable; verify reload persistence and keyboard operation.
4. Trace default node/branch markings, structural constraints and automatic ShortCuts behavior against legacy.
5. Read the relevant PDF papers before finalizing marking semantics, with title/page references: ISWC 2011 submission in `wwwroot`, Soulmate, 0L1 and the RDF mediation thesis in the legacy repository. Keep paper intent, implemented SQL behavior and proposed extensions distinct. Resolve NOT/exclusion questions before exposing ambiguous controls.

## Increment 3: send and show results

1. Add an authenticated, anti-forgery-protected `Send message` action scoped to an owned Code. Require a usable description; an empty Code must not produce a misleading delivery success.
2. Invoke parameterized `oli.fischen` for the specific Code and all applicable Anglers, following its verified signature. Preserve `oli.beissen` semantics; do not implement a second matching algorithm.
3. Inspect caller/trigger behavior, `Gescannt` state and the News/Wurzeln/inbox materialization path. Spiegel matching alone is not proof that the recipient feed is populated or notifications are sent.
4. Show saved/sent state, match count and zero-match feedback, with links to review recipients and refine markings. Retries must not duplicate matches, ledger effects or notifications.
5. Verify the same result through legacy UI and a controlled recipient Angler. Confirm delivery state when description or message changes after sending. Keep registration, TopLab creation and vocabulary administration outside this implementation.

## Validation

- Build the solution in Release. Add focused tests for input validation, authorization, duplicate submission, atomic rollback, default initialization and SQL procedure integration. Use SQL Server rather than EF in-memory tests for transaction/procedure/trigger behavior.
- Extend the existing test project with the needed application reference and HTTP test harness. Its current fixture restores a LocalDB backup into a fixed database name; isolate new fixtures and avoid parallel destructive fixture interference.
- Verify body rendering across both UIs; invalid forms must write nothing. Test failures after each persistence stage leave no partial PostIt, Code, Wurzeln or accounting effects.
- For matching, compare known positive, negative and zero-result cases against legacy procedures and recipient visibility. Validate repeat sending and concurrent changes.
- Browser acceptance: signed-out access, own/other Stamm navigation, compose/cancel/save, reload, mark/send, zero matches, recipient view, small-screen layout, and keyboard navigation.
- Complete the deferred live test with title `Journey-20261003 — Modern UI` (or a new date prefix when performed) and verify it in both applications. Record IDs and publication-safe screenshots.

## Existing feature-branch CI/CD

Inspected `.github/workflows/oliitrazorweb.yml` and GitHub on 2026-10-03:

- Pushes to all branches run when `OLI-it.Web/**`, `docs/**` or the application workflow changes. Manual dispatch is also supported.
- Pull requests build only. Non-PR runs deploy the artifact to App Service `oliitrazorweb`, slot `test`, using GitHub environment `test` and Azure OIDC.
- Production runs only for `refs/heads/main`, after test deployment. A feature-branch push does not satisfy that condition.
- The test environment currently has no protection rules or deployment branch restriction.
- [Feature-branch run 37149074641](https://github.com/nulllogicone/OLI-it.Web/actions/runs/37149074641) succeeded: build and deploy-test succeeded, deploy-prod skipped. An earlier agents/html-markdown-docs-enhancement branch also has a successful run.
- This is a shared test slot, not one preview per branch. Another branch push can replace the version being tested. There is no deployment concurrency group in the inspected workflow.

Before using CI as a verification gate:

1. Correct the test command: it currently targets the web project with `--no-build` and no matching Release configuration, rather than `OLI-it.Web.Tests`. Ensure tests are built/run and unavailable database fixtures are reported honestly.
2. Include test-project paths in CI triggers. Separate fast required tests from database tests and provision their prerequisites explicitly; a skipped database suite is not validation.
3. Consider serializing shared-slot deployments without cancelling an active deployment. Verify the deployed commit before each walkthrough.
4. After local checks and user-requested commit/push, push `codex/send-message` or manually dispatch its ref. Verify build and deploy-test, then perform live acceptance at https://oliitrazorweb-test.azurewebsites.net/.
5. Restore a known-good version through the same test deployment workflow if needed. Treat test-data cleanup separately from deployment rollback.

No code, workflow, database or remote deployment was changed during planning. This plan remains local and uncommitted; pushing documentation alone currently also triggers test deployment.

## Next implementation task

Inspect `oli.zahlen`, initialization triggers/defaults, automatic ShortCuts and related event behavior in an isolated database; then implement increment 1 with meaningful persistence tests. Continue marking and sending as separate increments on this branch. Record resolved semantics in existing domain docs and accepted design decisions in `docs/070-decisions/`.
