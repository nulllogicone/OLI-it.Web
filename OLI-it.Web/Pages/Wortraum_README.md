# Wortraum Playground - NKBZ Hierarchy

## Overview
This playground page demonstrates the hierarchical navigation structure for the Wortraum (word space) system with **Netz**, **Knoten**, **Baum**, and **Zweig** entities.

This page specifically displays the **Root Netz** which serves as the entry point for classifying PostIt messages by describing the **author**, **content**, and **desired recipient characteristics**.

## URL
Navigate to: `/Wortraum`

## Root Netz
- **Netz GUID**: `76035F19-F4AE-4D58-A388-4BBC72C51CEF` (hardcoded as constant)
- **Purpose**: Special entry point vocabulary for PostIt classification
- **Display**: Automatically expanded to show all Knoten nodes by default

## Hierarchical Structure

```
Netz (Network) - Blue hierarchy
├── Knoten (Node) - Blue
    ├── → WeiterNetzGuid (links to another Netz) ✓ NOW RENDERS!
    │   └── Netz (referenced network)
    │       └── Knoten (nodes in referenced network)
    │           └── ... (continues recursively)
    └── → WeiterBaumGuid (links to a Baum)
        └── Baum (Tree) - Green hierarchy
            └── Zweige (Branches) - Green
                ├── → WeiterNetzGuid (links to Netz)
                └── → WeiterBaumGuid (links to another Baum)
```

### Navigation Support
- **Knoten → Netz**: When a Knoten has `WeiterNetzGuid`, the referenced Netz is loaded and displayed as a child
- **Knoten → Baum**: When a Knoten has `WeiterBaumGuid`, the referenced Baum is loaded and displayed as a child
- **Recursive Loading**: Referenced Netze can contain their own Knoten which can reference more Netze
- **Depth Limit**: Maximum depth of 5 levels to prevent infinite recursion
- **Cycle Detection**: Visited Netz tracking prevents infinite loops in circular references

## Features

### Visual Elements
- **Tree Connectors**: `T` for parent nodes, `├` for child nodes
- **Expand/Collapse Icons**: `▶` (collapsed) and `▼` (expanded)
- **Color Coding**:
  - Blue text: Netz and Knoten (Network hierarchy)
  - Green text: Baum and Zweig (Tree hierarchy)
  - Orange text: Navigation indicators (→ Netz, → Baum)
- **Interactive**: Click on any node header to expand/collapse its children

### Entity Relationships
Based on your EF Core models:

1. **Netz** has many **Knoten** (via `ICollection<Knoten> Knotens`)
2. **Knoten** belongs to one **Netz** (via `Guid NetzGuid` and `Netz Netz`)
3. **Knoten** can point to:
   - Another **Netz** (via `Guid? WeiterNetzGuid`)
   - A **Baum** (via `Guid? WeiterBaumGuid`)
4. **Baum** has many **Zweig** (via `ICollection<Zweig> Zweigs`)
5. **Zweig** belongs to one **Baum** (via `Guid BaumGuid` and `Baum Baum`)
6. **Zweig** can point to:
   - A **Netz** (via `Guid? WeiterNetzGuid`)
   - Another **Baum** (via `Guid? WeiterBaumGuid`)

## Implementation Details

### Backend (Wortraum.cshtml.cs)
- Loads all Netz with their Knoten using `.Include()`
- Loads all Baum with their Zweig using `.Include()`
- Builds view models for efficient rendering
- Resolves WeiterBaumGuid references to display Baum hierarchies under Knoten

### Frontend (Wortraum.cshtml)
- Pure HTML/CSS/JavaScript (no external dependencies)
- Monospace font for tree-like appearance
- Click handlers for expand/collapse functionality
- Hover effects for better UX

## Future Enhancements

### For PostIt Marking/Coding
When integrating this into the PostIt marking workflow:

1. **Selection Mechanism**: Add checkboxes or click-to-select on nodes
2. **Multi-level Selection**: Allow selecting at any level (Netz, Knoten, Baum, Zweig)
3. **Breadcrumb Display**: Show selected path (e.g., "author > sex > female")
4. **Colored Dots**: Implement the colored dots system from your original application
5. **Search/Filter**: Add search to quickly find specific nodes in large hierarchies
6. **Persistence**: Save selected nodes to PostIt entity

### Navigation Links
Currently shows navigation hints (→ Netz, → Baum) but doesn't follow them. Future:
- Make WeiterNetzGuid and WeiterBaumGuid clickable
- Navigate/expand to linked hierarchies dynamically
- Show breadcrumb trail of navigation path

## Testing
The page will load the specific root Netz with GUID `76035F19-F4AE-4D58-A388-4BBC72C51CEF`.

**Expected behavior:**
- Root Netz is displayed with its Knoten level already expanded
- All Knoten are visible immediately
- Click on Knoten with Baum references to explore deeper
- Use the control buttons to expand/collapse multiple levels at once

**If the page shows an error:**
Ensure the Netz with GUID `76035F19-F4AE-4D58-A388-4BBC72C51CEF` exists in your database.

## Styling Notes
- Colors match the hierarchy from your reference image
- Monospace font gives a clean tree structure
- Responsive design works on different screen sizes
- Consistent with the rest of your OLI-it.Web application styling
