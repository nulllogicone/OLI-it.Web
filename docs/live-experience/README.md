# Live experience journey

Updated: 2026-10-21  
Status: draft

## Purpose

Explore the legacy and modern test applications together. Compare the same user intent in each, preserve meaningful protocol behavior, and distinguish missing behavior from opportunities to improve the experience. Start with [the transition overview](../000-motivation.md).

The initial journey and PostIt create/update increment were merged through [PR #49](https://github.com/nulllogicone/OLI-it.Web/pull/49). Further walkthrough notes remain local until reviewed. Credentials must never be recorded here.

## Environments

| Application | Test URL | Source |
|---|---|---|
| Legacy .NET Framework 4.8 (owner description) | https://oliweb-test.azurewebsites.net/ | Azure DevOps OLI-it repository |
| Modern Razor Pages | https://oliitrazorweb-test.azurewebsites.net/ | This GitHub repository |

The owner authorizes exploration using the supplied test account and explicitly confirms both test applications share the same database. Legacy creation was verified on 2026-10-03; modern creation was verified in Azure test on 2026-10-04 after deployment of commit `0c0261a`. Registration and TopLab creation remain untested. Description and sending are deferred until the intrinsic NKBZ/OgIf behavior is understood with the owner. Outbound notification behavior and the current deployed revisions are unverified.

Scope follows the transition overview: OliWeb end-user workflows. KatWeb vocabulary administration, reward payouts and notification email administration are outside this repository's migration scope.

## How we work

1. Pick one scenario and its expected result together.
2. Perform the same scenario in both test applications, using a distinctive test-data prefix such as `Journey-YYYYMMDD-` when creating records.
3. Record steps, observed results, language, environment, relevant record references and screenshots. Separate observation, interpretation, idea and unresolved question.
4. Explain domain behavior with paper page references, code and database evidence where needed. A navigation link alone does not prove the feature works.
5. Carry accepted domain explanations into the existing docs, decisions into `070-decisions/`, and actionable work into GitHub issues. Link existing issues rather than duplicate their backlog here.
6. Before merging, condense the journey into a reviewed comparison and decisions. Keep only useful, publication-safe evidence; remove redundant notes and redact private data. Do not push credentials, cookies, personal account details or unreviewed screenshots.

## Walkthrough sequence

| Scenario | What to establish |
|---|---|
| Sign-in and orientation | Account identity, navigation, language and errors |
| Stamm registration/profile | Creation, validation, ownership and discoverability |
| PostIt and Code | Composition plus markings: from whom, about what, to whom |
| Angler and delivery | Recipient filters, mutual requirements and matched messages |
| TopLab | Answer creation, visibility, evaluation and credit effects |
| Wortraum / NKBZ | Navigate and maintain Netz, Knoten, Baum and Zweig; translations and reusable structures |
| OgIf | Explain expected matches and non-matches from papers; trace actual implementation and compare outcomes |

These are investigation topics, not verified feature claims or a duplicate implementation backlog.

## Evidence and findings

- [2026-10-03: access baseline](sessions/2026-10-03-access.md)
- [2026-10-03: message creation](sessions/2026-10-03-message-creation.md)
- [2026-10-21: journey reset and read-only message-flow walkthrough](sessions/2026-10-21-message-flow.md)
- [Comparison matrix](comparison.md)
- [Paper and implementation references](references.md)
- Screenshots: `evidence/YYYY-MM-DD/`, linked from session notes. Capture only what is needed to establish a finding.

For later sessions, record: purpose, preconditions, exact steps, expected/observed result per application, evidence, interpretation, open questions, and accepted conclusions. Add deployed revision when available; do not assume a local checkout equals deployed code.
