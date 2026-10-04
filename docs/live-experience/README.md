# Live experience journey

Updated: 2026-10-03  
Status: draft

## Purpose

Explore the legacy and modern test applications together. Compare the same user intent in each, preserve meaningful protocol behavior, and distinguish missing behavior from opportunities to improve the experience. Start with [the transition overview](../000-motivation.md).

Working branch: `codex/live-experience-journey`. Keep this exploration local until its documentation and screenshots have been reviewed for publication. Credentials must never be recorded here.

## Environments

| Application | Test URL | Source |
|---|---|---|
| Legacy .NET Framework 4.8 (owner description) | https://oliweb-test.azurewebsites.net/ | Azure DevOps OLI-it repository |
| Modern Razor Pages | https://oliitrazorweb-test.azurewebsites.net/ | This GitHub repository |

The owner authorizes exploration using the supplied test account. Registration, PostIt creation and TopLab creation are proposed subsequent walkthroughs. They have not been performed in this baseline session. Database isolation, outbound notification behavior and deployed revisions are not yet verified; establish those details before interpreting cross-application writes or delivery effects.

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
- [Comparison matrix](comparison.md)
- [Paper and implementation references](references.md)
- Screenshots: `evidence/YYYY-MM-DD/`, linked from session notes. Capture only what is needed to establish a finding.

For later sessions, record: purpose, preconditions, exact steps, expected/observed result per application, evidence, interpretation, open questions, and accepted conclusions. Add deployed revision when available; do not assume a local checkout equals deployed code.
