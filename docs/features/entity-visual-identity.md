# Entity Visual Identity — Styling Pattern

Each domain entity in OLI-it gets a distinct visual style that is immediately recognisable —
PostIts look like yellow sticky notes, Stamm like a tree trunk, etc.

## Architecture (3-layer CSS)

```
Layer 1 — Base card    .entity-card          shared structure (white bg, border, shadow, radius)
Layer 2 — Entity type  .postit-card          entity accent colour (left border stripe)
Layer 3 — Visual theme .sticky-note          full visual treatment (bg, shadow, shape, animation)
```

For table rows the same layers apply:

```
Layer 1 — Base table   .simple-table         clean table, standard padding
Layer 2 — Entity table .postit-table         border-spacing for visual row gaps
Layer 3 — Row theme    .sticky-note-row      entity background + shadow on each row
```

**Key principles:**
- Base characteristics live in **shared CSS classes** — no duplication between card and table row
- Entity-specific overrides are minimal and scoped (e.g. `.postit-card .card-meta-box`)
- `entity-card-header` grey background must be **overridden** for full-card colouring to work
- Table rows use `border-collapse: separate` + `border-spacing` instead of row margins
- Rows are **clickable via `onclick`** — no separate view-link column needed

---

## Implementation Steps

### 1. Add CSS classes to `wwwroot/css/site.css`

```css
/* Visual theme class (card) */
.sticky-note {
  background: linear-gradient(135deg, #fffacd 0%, #ffeb99 100%);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15),
              inset 0 1px 0 rgba(255, 255, 255, 0.8);
  border: none;
  border-radius: 2px;
  transform: rotate(-0.5deg);
  position: relative;
}
.sticky-note:hover {
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.2),
              inset 0 1px 0 rgba(255, 255, 255, 0.8);
  transform: rotate(-0.5deg) translateY(-2px);
}

/* Table row variant */
.sticky-note-row {
  background: linear-gradient(135deg, #fffacd 0%, #ffeb99 100%);
}

/* Table spacing */
.postit-table {
  border-collapse: separate;
  border-spacing: 0 8px;
}
.postit-table .sticky-note-row {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.12);
  transition: box-shadow 0.2s ease, transform 0.2s ease;
}
.postit-table .sticky-note-row:hover {
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.18);
  transform: translateY(-1px);
}
.postit-table .sticky-note-row td:first-child { border-radius: 4px 0 0 4px; }
.postit-table .sticky-note-row td:last-child  { border-radius: 0 4px 4px 0; }
```

### 2. Layout helpers for the card partial

```css
/* Full-card padding (replaces header/body split) */
.postit-card.sticky-note { padding: 16px 18px 14px; overflow: hidden; }

/* Kill inherited header background */
.postit-card .entity-card-header { background: transparent; border-bottom: none; }

/* Floating elements */
.postit-thumb-float   { float: left;  margin: 0 14px 10px 0; }
.postit-meta-float    { float: right; margin: 0 0 10px 14px; }
.postit-actions-float { float: right; display: flex; align-items: center; gap: 4px; }
.postit-title         { margin: 0 0 10px 0; font-size: 18px; }
.postit-body          { overflow: hidden; }
.postit-clearfix      { clear: both; }
```

### 3. Restructure the card partial (`_PostItCard.cshtml`)

Remove the `entity-card-header` / `entity-card-body` divs. Replace with:

```html
<div class="entity-card postit-card sticky-note">

    <!-- Thumbnail: floats top-left, clickable -->
    <a href="/postit/@Model.PostItGuid" class="postit-thumb-float">
        @await Component.InvokeAsync("ImageThumbnail", ...)
    </a>

    <!-- Actions: float top-right -->
    <div class="postit-actions-float">
        <!-- edit button (auth-gated) -->
        <a href="/stamm/@StammGuid" class="close-btn">&times;</a>
    </div>

    <!-- Meta box: float top-right, left of actions -->
    <div class="card-meta-box postit-meta-float">
        <div class="card-meta-datum">@Model.Datum</div>
        <div class="card-meta-value">@Model.KooK</div>
        <div class="card-meta-value">@Model.Hits <small>hits</small></div>
    </div>

    <h3 class="postit-title">@Model.Titel</h3>

    <div class="postit-body">
        <div class="entity-description">@Model.PostIt1</div>
        <!-- optional URL link -->
    </div>

    <div class="postit-clearfix"></div>
</div>
```

### 4. Update the table partial (`_ChildPostItsTable.cshtml`)

```html
<table class="simple-table postit-table">
    ...
    <tr class="sticky-note-row"
        onclick="window.location='/postit/@wurzel.PostItGuid'"
        style="cursor:pointer">
        <!-- no view-link column -->
    </tr>
```

### 5. Hot-reload workflow

```powershell
dotnet run --project OLI-it.Web
# → http://localhost:5113
# CSS and .cshtml changes reflect on browser refresh — no build step needed
```

---

## Entity Reference

| Entity | CSS theme class | Table class | Colour palette | Shape |
|--------|----------------|-------------|----------------|-------|
| PostIt | `.sticky-note` | `.postit-table` / `.sticky-note-row` | Yellow `#fffacd → #ffeb99` | Flat paper, 2px radius, slight rotation |
| Stamm  | `.trunk-card` *(pending)* | `.stamm-table` / `.trunk-row` *(pending)* | Warm brown `#fdf6f0 → #f0e0cc`, border `#5C3A1E` | Rounded trunk, 10px radius, full border |
| TopLab | *(pending)* | *(pending)* | Green `#228B22` accent | TBD |
| Angler | *(pending)* | *(pending)* | Blue `#1E90FF` accent | TBD |

---

## Stamm — Design Spec

Visual metaphor: **tree trunk cross-section** — warm bark-paper background, dark brown rounded border.

```css
.trunk-card {
  background: linear-gradient(135deg, #fdf6f0 0%, #f0e0cc 100%);
  box-shadow: 0 4px 14px rgba(92, 58, 30, 0.18),
              inset 0 1px 0 rgba(255, 255, 255, 0.7);
  border: 2px solid #5C3A1E;   /* full-perimeter border, not just left stripe */
  border-radius: 10px;
  position: relative;
}
.trunk-card:hover {
  box-shadow: 0 8px 22px rgba(92, 58, 30, 0.25);
  transform: translateY(-2px);
}
```

Key difference from PostIt:

| Property | PostIt (paper) | Stamm (trunk) |
|----------|---------------|---------------|
| `border` | `none` | `2px solid #5C3A1E` (full perimeter) |
| `border-radius` | `2px` | `10px` |
| `transform` | `rotate(-0.5deg)` | none (trunks are upright) |
| Shadow tint | neutral black | brown-tinted |

Files to change: `site.css`, `_StammCard.cshtml`, and any Stamm child-table partial.
