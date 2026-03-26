# Quick Reference: German ↔ English Entity Names

This is a quick lookup table for developers working with the codebase. For complete details, see `02-data-model.md`.

## Message Flow (Kreislauf / SAPCT)

| German | English | Description |
|--------|---------|-------------|
| `Stamm` | User | Platform user (author or recipient) |
| `Angler` | Filter Profile | User's criteria to receive messages |
| `PostIt` | Message | Question, offer, or content |
| `Code` | Description | Author's complete marking (self + message + recipient) |
| `TopLab` | Answer/Response | Reply to a received message |

## Wordspace (Wortraum / NKBZ)

| German | English | Description |
|--------|---------|-------------|
| `Netz` | Net | Domain grouping for concepts |
| `Knoten` | Node | Specific aspect within a domain |
| `Baum` | Tree | Hierarchical structure for categories |
| `Zweig` | Branch | Element in a tree hierarchy |

## Matching Logic (OgIf)

| German | English | Description |
|--------|---------|-------------|
| `Olis` | Message Marking | Sender's criteria/markings |
| `get` | Receiver Threshold | Match requirements for recipient |
| `Ilos` | Filter Marking | Recipient's filter criteria |
| `fit` | Sender Threshold | Match requirements for sender |

## Common Usage Patterns

### Querying Users
```csharp
// Get user by ID
var user = await _context.Stamm.FindAsync(userId);

// Get user with their filter profiles
var userWithFilters = await _context.Stamm
    .Include(s => s.Angler)
    .FirstOrDefaultAsync(s => s.StammId == userId);
```

### Querying Messages
```csharp
// Get recent messages
var messages = await _context.PostIt
    .Include(p => p.Stamm) // Author
    .Include(p => p.Code)  // Description
    .OrderByDescending(p => p.CreatedAt)
    .Take(10)
    .ToListAsync();
```

### Querying Wordspace
```csharp
// Get all nets
var nets = await _context.Netz.ToListAsync();

// Get nodes in a specific net
var nodes = await _context.Knoten
    .Where(k => k.NetzId == netId)
    .OrderBy(k => k.DisplayOrder)
    .ToListAsync();
```

## Pronunciation Guide (for non-German speakers)

- **Stamm** - SHTAHM (rhymes with "mom")
- **Angler** - AHN-glur
- **PostIt** - like the sticky note brand
- **Netz** - NETS (like "nets")
- **Knoten** - K'NOH-ten
- **Baum** - BOWM (rhymes with "down")
- **Zweig** - TSVYG (ts like "cats", vy like "view")

## Tips

1. **Use IntelliSense**: XML comments on entities provide English descriptions
2. **Variable Naming**: Use descriptive English names even when type is German
   ```csharp
   var author = stamm;
   var filterProfile = angler;
   ```
3. **DTOs**: Consider creating English-named DTOs for API/UI layers
4. **Consistency**: Be consistent with whichever naming approach your team chooses

## Related Documentation

- [01-domain-entities.md](01-domain-entities.md) - Full entity descriptions
- [02-data-model.md](02-data-model.md) - Complete schema mapping
- [07-decisions/ADR-0003-german-table-names.md](07-decisions/ADR-0003-german-table-names.md) - Why we preserve German names
