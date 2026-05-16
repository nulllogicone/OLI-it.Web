# Dev Dashboard

Interactive personal workspace for OLI-it.Web development — todos, ideas, and discussions, all stored as plain markdown files.

## Start

```bash
cd slop
npm install
npm start
```

Opens at **http://localhost:3456**

## Usage

- **Kanban board** per category with status columns
- **Quick add**: type in the footer of any column and press `Enter`
- **Edit / Delete**: click any card to open the edit modal
- **Add Item** button in the header for a full form

## Data files

All data lives in plain markdown under `data/` — edit them directly in any editor:

| File | Contents |
|------|----------|
| `data/todos.md` | Developer todos |
| `data/ideas.md` | Feature ideas |
| `data/discussions.md` | Open discussions |

### Item format

```markdown
## TODO-001: Title of the item
**Status:** in-progress
**Priority:** high
**Created:** 2026-05-13
**Tags:** auth, api
**Notes:** Any extra context goes here
```

Status values per type:

| Type | Statuses |
|------|----------|
| Todos | `todo` → `in-progress` → `done` / `blocked` |
| Ideas | `new` → `exploring` → `accepted` / `rejected` |
| Discussions | `open` → `in-progress` → `resolved` |

## Port

Set `PORT=XXXX` before starting to use a different port.
