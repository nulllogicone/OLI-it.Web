# Generation Rules

## Objective

Produce human-friendly HTML pages in `docs/human-overview/` from canonical markdown in `docs/*.md`.

## Rules

1. Always read `docs/human-overview/human-output.config.yml` first.
2. Use markdown sources listed in config `pages[*].source_docs`.
3. Preserve or improve global navigation on every output page.
4. Include metadata JSON in every page:
   - `doc_audience`
   - `doc_focus`
   - `context_tags`
   - `agent_load_hint`
   - `source_docs`
   - `last_updated`
5. Include a visible update-highlights block when content changed.
6. Keep generated HTML readable, semantic, and accessible.
7. Keep pathing local and relative (no broken relative links).

## Validation checklist

- [ ] All configured output files exist
- [ ] Metadata JSON is present on each page
- [ ] `source_docs` values point to existing `docs/*.md` files
- [ ] Navigation links are consistent across pages
- [ ] CSS/JS references point to `assets/style.css` and `assets/app.js`
