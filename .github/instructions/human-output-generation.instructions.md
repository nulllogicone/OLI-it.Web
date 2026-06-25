---
description: "Use when generating docs/human-overview HTML from docs/*.md. Covers canonical markdown source-of-truth, human-output config loading, navigation consistency, metadata traceability, update highlights, and preserving look-and-feel."
---
# Human Output Generation Rules

Use these rules whenever the task involves documentation refresh, human-facing overview pages, or docs HTML generation.

## Source-of-truth contract

- Canonical content source is `docs/*.md` (including subfolders).
- `docs/human-overview/*.html` are generated presentation artifacts for human readers.
- Do not invent facts that are not supported by canonical markdown.

## Required inputs before generating HTML

1. Read `docs/human-overview/human-output.config.yml`.
2. Read all source markdown files referenced by that config.
3. Read existing output HTML only to preserve navigation, layout intent, and stable anchors when possible.

## Look-and-feel contract

- Keep shared styling in `docs/human-overview/assets/style.css`.
- Keep shared client behavior in `docs/human-overview/assets/app.js`.
- Maintain a modern, readable layout with:
  - clear header and summary
  - consistent navigation between pages
  - scannable cards/sections
  - strong contrast and mobile-friendly structure

## Metadata contract in every human HTML page

Every generated page must include:

```html
<script type="application/json" id="doc-metadata">{...}</script>
```

With keys:
- `doc_audience`
- `doc_focus`
- `context_tags`
- `agent_load_hint`
- `source_docs`
- `last_updated`

`source_docs` must only include canonical markdown files under `docs/`.

## Update-highlights contract

- When source markdown changes, visibly summarize what changed in regenerated HTML pages.
- Use concise bullets and link highlights back to relevant sections.
- Keep this section human-focused (impact and meaning), not commit-log style.

## Link and asset stability

- Preserve existing output URLs/file names unless explicitly asked to rename.
- Use relative links that work from `docs/human-overview/`.
- Keep image references stable and rooted in known asset paths.
- Avoid broken links by validating internal navigation paths while editing.
