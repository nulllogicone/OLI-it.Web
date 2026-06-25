# Refresh Human Overview Docs

Use this prompt to run docs refresh mode:

1. Treat `docs/*.md` as canonical source of truth.
2. Read `docs/human-overview/human-output.config.yml` first and follow it strictly.
3. Update markdown first when source content needs changes.
4. Regenerate or align `docs/human-overview/*.html` pages defined in config.
5. Keep each generated HTML page metadata block in `doc-metadata` JSON with:
   - `doc_audience`
   - `doc_focus`
   - `context_tags`
   - `agent_load_hint`
   - `source_docs`
   - `last_updated`
6. Keep style and interaction in shared assets:
   - `docs/human-overview/assets/style.css`
   - `docs/human-overview/assets/app.js`
7. Include a visible "What's changed" highlights section for updated pages.
8. Preserve stable links and image references.
9. Provide a short summary:
   - changed markdown files
   - refreshed HTML files
   - any open follow-up items
