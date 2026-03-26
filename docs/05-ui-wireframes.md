# UI Wireframes

Last updated: 2026-03-27
Status: in-progress

## Purpose

Low-fidelity sketches and interaction notes for key pages.  
Reference [04-ui-ia.md](04-ui-ia.md) for the full page inventory.

## Layout Structure

### Master Layout
```
┌─────────────────────────────────────────────────────────┐
│ Header                                                  │
│ [Logo] [Language: EN|DE|ES]          [Login/Username]  │
└─────────────────────────────────────────────────────────┘
┌──────────┬─────────────────────────────────────────────┐
│          │                                              │
│ Left     │ Main Content Area                           │
│ Sidebar  │                                              │
│ Menu     │                                              │
│          │                                              │
│          │                                              │
└──────────┴─────────────────────────────────────────────┘
```

### Browse Page - Stacked Layout
```
┌─────────────────────────────────────────────────────────┐
│ ┌─────────────────────────────────────────────────────┐ │
│ │ Stamm: Username                              [×]    │ │
│ │ Email | Balance | Registration Date                 │ │
│ └─────────────────────────────────────────────────────┘ │
│                                                         │
│ ┌─────────────────────────────────────────────────────┐ │
│ │ PostIt: Message Title                        [×]    │ │
│ │ Date | Balance | Type | Hits                        │ │
│ └─────────────────────────────────────────────────────┘ │
│                                                         │
│ ┌─────────────────────────────────────────────────────┐ │
│ │ TopLab: Answer Title                         [×]    │ │
│ │ Date | Reward | Type                                │ │
│ └─────────────────────────────────────────────────────┘ │
│                                                         │
│ Child Lists (shown when no deeper entity selected)     │
│ ┌─────────────────────────────────────────────────────┐ │
│ │ Table of child entities with [view] links          │ │
│ └─────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
```

The Browse page uses a stacked component architecture where:
- Selected entities appear as horizontal cards at the top
- Each card has a close button (×) to navigate back up the hierarchy
- Cards are color-coded by entity type:
  - 📋 Stamm (dark-red header)
  - 📝 PostIt (yellow header)
  - 🎣 Angler (blue header)
  - 💬 TopLab (green header)
  - 🔖 Code (grey header)
- Child entity lists appear below the stacked cards
- Clicking a child adds it to the stack

### Header Components
- **Logo**: Top-left, links to home (/)
- **Language Selector**: Next to logo (English default, Spanish, German)
- **Login/Logout**: Top-right for Stamm authentication

### Left Sidebar Menu (Anonymous Users)
- Register
- Timeline (Journal)
- Chart (Statistics)
- Search
- RSS Feed
- Links
- Terms and Conditions
- Imprint

### Left Sidebar Menu (Browse Page - Entity Stack)
Shows the current navigation stack:
- 📋 Stamm (root)
  - 📝 PostIt (child, indented)
    - 💬 TopLab (grandchild, further indented)
    - 🔖 Code (grandchild, further indented)
  - 🎣 Angler (child, indented)
  - 💬 TopLab (child - answers written by stamm)

The active entity in the stack is highlighted.

### Left Sidebar Menu (Context-Specific - Legacy Pages)
When viewing a **Stamm** detail page:
- Stamm Details (main)
- PostIt (messages created by stamm)
- Angler (filter profiles)
- TopLab (answers written)

When viewing a **PostIt** detail page:
- PostIt Details (main)
- Codes (descriptions)
- TopLab (answers)

## Navigation Flow

### Browse Page URLs
The Browse page uses query parameters to maintain state:

- `/browse?stamm={guid}` - Shows Stamm card + child lists
- `/browse?stamm={guid}&postit={guid}` - Shows Stamm + PostIt cards + PostIt child lists
- `/browse?stamm={guid}&postit={guid}&toplab={guid}` - Shows Stamm + PostIt + TopLab cards
- `/browse?stamm={guid}&angler={guid}` - Shows Stamm + Angler cards
- `/browse?stamm={guid}&postit={guid}&code={guid}` - Shows Stamm + PostIt + Code cards

Each entity card has a close button that navigates back to its parent in the hierarchy.

## Pages to wireframe (priority order)

1. **Browse** - Stacked entity view (IMPLEMENTED)
2. **Home / Journal** - List of recent messages
3. **Stamm Detail** - User profile with child entity navigation (Legacy)
4. **PostIt Detail** - Message detail with answers (Legacy)
5. Message Detail (with answers)
6. Create Message (with Wordspace Navigator)
7. Filter Profile Editor (with Wordspace Navigator)
8. Inbox
9. Admin: Wordspace Manager

## Status

Stacked layout implemented for Browse page with:
- ✅ Horizontal entity cards with color coding
- ✅ Close buttons for navigation
- ✅ Entity stack sidebar showing hierarchy
- ✅ Child entity lists below cards
- ✅ Query parameter-based routing

Legacy detail pages remain available for direct access.

## Change Log

- 2026-03-27: Added stacked Browse page layout and entity card architecture.
- 2026-03-27: Added layout structure, header components, and context-specific menu navigation.
- 2026-03-26: Stub created.
