# UI Information Architecture

Last updated: 2026-03-26
Status: draft

## Navigation Structure

```
/ (Home / Journal)          ← public
/Register                   ← public
/Login                      ← public
/Logout                     ← authenticated

/Journal                    ← public — UC-008
/Charts                     ← public — UC-009
/Search                     ← public — UC-010

/Messages/Create            ← authenticated — UC-003
/Messages/{id}              ← public (read), authenticated (answer/rate)
/Messages/{id}/Edit         ← authenticated (author only)

/FilterProfiles             ← authenticated — UC-004
/FilterProfiles/Create      ← authenticated
/FilterProfiles/{id}/Edit   ← authenticated

/Inbox                      ← authenticated — UC-005 (matched messages)
/Profile                    ← authenticated — UC-011

/Admin/Wordspace            ← admin — UC-012
/Admin/Wordspace/Nets
/Admin/Wordspace/Nets/{id}/Nodes
/Admin/Wordspace/Trees
/Admin/Wordspace/Trees/{id}/Branches
```

---

## Key Pages

### Home / Journal (`/`)

Visible to: everyone  
Content:
- Latest messages in reverse chronological order
- Each item: title, author, date, answer count, value indicator
- Pagination
- Search bar in header

---

### Message Detail (`/Messages/{id}`)

Visible to: everyone (read); any authenticated user who can view the message may answer  
Content:
- Message title, content, author, date, value, time limit
- Wordspace description summary (collapsed by default, expandable)
- Answer list (each with author, content, rating)
- Answer form (logged-in users; self-answer allowed)
- Rate buttons (author only, per answer)

---

### Create / Edit Message (`/Messages/Create`, `/Messages/{id}/Edit`)

Visible to: authenticated users  
Content:
- Title, content, value, time limit fields
- Wordspace navigator: tree/node selector with first-value / second-value dot controls
- Preview of current description markings
- Save / Cancel

---

### Filter Profiles (`/FilterProfiles`)

Visible to: authenticated users  
Content:
- List of user's filter profiles (name, active state, criterion count)
- Create new / Edit / Activate / Deactivate / Delete actions

---

### Filter Profile Editor (`/FilterProfiles/Create`, `/FilterProfiles/{id}/Edit`)

Visible to: authenticated users  
Content:
- Profile name field
- Wordspace navigator (same component as message description)
- Preview of markings
- Save / Cancel

---

### Inbox (`/Inbox`)

Visible to: authenticated users  
Content:
- Messages matched to any of the user's active filter profiles
- sorted by delivery date
- Link to message detail

---

### Charts (`/Charts`)

Visible to: everyone  
Content:
- Toggle: sort by points / sort by money
- Message ranking table: rank, title, author, score/amount
- User ranking table: rank, username, balance/points

---

### Search (`/Search?q=...`)

Visible to: everyone  
Content:
- Search input
- Results grouped by: Messages | Answers | Users
- Each result links to detail page

---

### Profile (`/Profile`)

Visible to: authenticated user (own)  
Content:
- Username, display name, language preference, email
- Credit balance
- Transaction history (date, amount, reason, related entity link)

---

### Admin: Wordspace (`/Admin/Wordspace/*`)

Visible to: admin role only  
Content:
- List of nets, with expandable node hierarchy (CRUD)
- List of trees, with expandable branch hierarchy (CRUD)
- Ordering controls for display order

---

## Shared Components

| Component | Used On |
|-----------|---------|
| Header nav (logo, journal, charts, search, login/register or user menu) | All pages |
| Wordspace Navigator | Message Create/Edit, Filter Profile Create/Edit |
| Message Card (title, author, date, answer count) | Journal, Inbox, Search |
| Answer Block (content, author, date, rating) | Message Detail |
| Pagination | Journal, Inbox, Search |
| Language Switcher (EN / DE / ES) | Header |

---

## Wordspace Navigator Component

This is the most complex UI component. It must:
- Render the hierarchical wordspace (nets → nodes, trees → branches)
- Allow marking each element with first-value (1/2/3) and second-value (0/2/3) represented as colored dots
- Enforce the rule: first-value 3 can only be used once per net or tree structure
- Show expert mode / simple mode toggle (simple mode hides second-value)
- Persist markings as Criteria records on save

Visual reference: colored dot selectors from the paper (green/blue/black × red/yellow/white)

---

## Open Questions

- Which pages require login-wall redirect vs. graceful degradation?
- Is the Inbox separate from Journal or a filtered view of Journal?
- Mobile layout priority: which pages are critical for responsive first?
- Should the wordspace navigator be a partial page or a full modal/panel?

## Change Log

- 2026-03-26: Initial IA from paper and live site analysis.
- 2026-04-08: Updated message answer access rule per OQ-002.
