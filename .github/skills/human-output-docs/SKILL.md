---
name: human-output-docs
description: "Regenerate docs/human-overview HTML from canonical docs/*.md using human-output config, preserving navigation, metadata, links, and modern look-and-feel."
argument-hint: "Optional: list changed docs or specific human-overview pages to refresh."
---

# Human Output Docs Regeneration

Use this skill when you need AI to refresh human-facing HTML documentation from markdown source files.

## When to use

- "Regenerate human docs"
- "Refresh overview pages from docs markdown"
- "Update human-output HTML after docs changes"
- "Keep docs look and feel while syncing content"

## Required project contract

1. Canonical source: `docs/*.md`
2. Human output config: `docs/human-overview/human-output.config.yml`
3. Generated target: `docs/human-overview/*.html`
4. Shared presentation assets:
   - `docs/human-overview/assets/style.css`
   - `docs/human-overview/assets/app.js`

## Procedure

1. Read [generation rules](./references/generation-rules.md).
2. Read human output config from `docs/human-overview/human-output.config.yml`.
3. Load mapped markdown source docs.
4. Regenerate listed output pages with consistent navigation and metadata.
5. Add/update "What's changed" highlights based on source updates.
6. Ensure links and image paths remain valid from `docs/human-overview/`.

## Page structure baseline

Use [page skeleton](./references/page-skeleton.html) as a baseline and adapt page body content per source docs.
