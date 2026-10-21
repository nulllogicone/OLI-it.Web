# Resume the message-flow journey — 2026-10-21

Status: in progress; observations and interpretations kept separate
Scope: read-only OliWeb end-user walkthrough; no matching, registration, answers or credit operations

## Historical reconciliation

The 2026-10-03 creation notes are a historical snapshot, not the current feature status. The subsequent work was:

- Added PostIt creation and safe content updates on branch codex/send-message. Eleven focused SQL/handler tests passed locally and in CI.
- Commit 0c0261a was deployed to Azure test by [run 37159861723](https://github.com/nulllogicone/OLI-it.Web/actions/runs/37159861723); production was skipped in that feature-branch run.
- Created [Journey-20261004 — Modern UI](https://oliitrazorweb-test.azurewebsites.net/PostIt/21ad2a5b-e042-447b-8813-87a6cc50903d) through modern Azure test. The saved page showed 1.01 message credit, an initial Code and literal quotes, ampersands and Unicode.
- [PR #49](https://github.com/nulllogicone/OLI-it.Web/pull/49) was merged on 2026-10-04; the owner reported the merged version deployed. This note does not establish the current deployed revision.
- An experimental description editor and send service were removed before the implementation commit after the owner identified missing intrinsic NKBZ/OgIf logic. They are not part of the merged feature. The broader [story #48](https://github.com/nulllogicone/OLI-it.Web/issues/48) was not completed by this increment.
- Local implementation testing used a separate restored LocalDB database. Messages created there are not evidence of Azure writes. The Azure test creation linked above is a separate, explicitly verified record.

## Walkthrough objective

Choose one existing PostIt with at least one TopLab. Follow its actual UI links to Code, recipient Angler and answer in the legacy application, then inspect the same IDs in modern UI. Observe labels, coordinates, values, visibility and navigation before proposing explanations or code changes.

Questions to answer with the owner:

1. Which selections describe the author, message and recipient, and how does each coordinate retain its context?
2. How do the Code and Angler relate in this concrete example? What does the UI actually establish about matching, as distinct from our inference?
3. Who wrote the TopLab, and which UI links relate it to its PostIt and Stamm? A recorded answer alone does not establish that its author received the message through a match.
4. What is shown about deadlines, values, evaluation and status? Do not exercise payout or notification administration.

No new implementation is authorized by this walkthrough. Consult the papers and legacy behavior with the owner before designing modern description or sending.

## Current observations

Live walkthrough pending. Prior observations must not be presented as reverified on 2026-10-21.
