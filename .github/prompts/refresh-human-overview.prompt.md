# Refresh Human Overview Docs

Use this prompt to run docs refresh mode:

1. Treat `docs/*.md` and `docs/human-overview/*.md` as source of truth.
2. Update markdown first based on requested changes.
3. Keep or improve context headers in `docs/human-overview/*.md`:
   - `doc_focus`
   - `context_tags`
   - `agent_load_hint`
   - `source_docs`
   - `last_updated`
4. Regenerate or align these HTML pages for human reading:
   - `docs/human-overview/index.html`
   - `docs/human-overview/project-at-a-glance.html`
   - `docs/human-overview/progress-and-next-steps.html`
   - `docs/human-overview/documentation-reading-guide.html`
5. Preserve existing markdown files and technical docs.
6. Keep style and interaction in shared assets:
   - `docs/human-overview/assets/style.css`
   - `docs/human-overview/assets/app.js`
7. Provide a short summary:
   - changed markdown files
   - refreshed HTML files
   - any open follow-up items
