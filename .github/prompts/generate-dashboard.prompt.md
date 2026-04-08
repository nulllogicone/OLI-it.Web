---
mode: agent
description: "Regenerate the OLI-it.Web stakeholder dashboard HTML from the project markdown files."
---

# Regenerate Stakeholder Dashboard

Run the dashboard generator script to produce an up-to-date `docs/dashboard.html` from the current project markdown files.

## Steps

1. Run the generator script:

```pwsh
pwsh -File "${workspaceFolder}/docs/generate-dashboard.ps1" -Open
```

2. Verify the output file exists at `docs/dashboard.html`.

3. If the script fails, diagnose the error:
   - Check that `docs/08-backlog.md`, `docs/99-open-questions.md`, and `docs/07-decisions/ADR-*.md` all exist.
   - Report any parse warnings and which section was affected.

## What the dashboard contains

| Section | Source file |
|---------|-------------|
| Delivery progress (phase bars) | `docs/08-backlog.md` |
| Phase 1 Kanban board | `docs/08-backlog.md` — Phase 1 table |
| Phase 2 Kanban board | `docs/08-backlog.md` — Phase 2 table |
| Open Questions | `docs/99-open-questions.md` |
| Architecture Decisions | `docs/07-decisions/ADR-*.md` |
| Change Log | `docs/08-backlog.md` — Change Log section |

## Updating backlog status

To reflect work that has been completed, update the `Status` column in `docs/08-backlog.md` using one of these values:

| Value | Kanban column |
|-------|---------------|
| `not started` | Backlog |
| `in progress` | In Progress |
| `✅ completed` | Done |
| `blocked` | Blocked |

Then re-run this prompt to regenerate the dashboard.

## Future: DocFx + GitHub Pages

When the team is ready to publish docs publicly:
1. Install DocFx: `dotnet tool install -g docfx`
2. Run `docfx init` at the repo root and point it at the `docs/` folder.
3. Add a GitHub Actions workflow that runs `docfx build` and deploys to the `gh-pages` branch.
4. The existing markdown files are already structured and will render cleanly with DocFx.
