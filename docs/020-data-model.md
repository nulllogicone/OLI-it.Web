# Data Model

Last updated: 2026-10-06
Status: draft

## Approach

- **Existing database** — schema is already defined and must be used without changes
- EF Core Database-First via scaffolding
- SQL Server
- German table names from original implementation
- Matching logic implemented as stored procedure
- Domain model in `010-domain-entities.md` provides conceptual mapping

---

## German → English Entity Mapping

The database uses German naming conventions from the original implementation. This mapping helps navigate the schema:

### Kreislauf (Message Flow / SAPCT)

| German Table | English Concept | Purpose |
|--------------|-----------------|----------|
| **Stamm** | User | Author of messages, owner of filters |
| **Angler** | Filter Profile | Criteria to receive messages |
| **PostIt** | Message | Question, offer, or other content |
| **Code** | Description | Marks author + message + recipient |
| **TopLab** | Response/Answer | Reply to a PostIt |

### Wortraum (Wordspace / NKBZ)

| German Table | English Concept | Purpose |
|--------------|-----------------|----------|
| **Netz** | Net | Domain grouping |
| **Knoten** | Node | Aspect within a domain |
| **Baum** | Tree | Hierarchy structure |
| **Zweig** | Branch | Branch of a tree |

### Logic (OgIf)

| German Table | English Concept | Purpose |
|--------------|-----------------|----------|
| **Olis** | Message Marking | Criteria set by sender |
| **get** | Receiver Threshold | Required match level for recipient |
| **Ilos** | Filter Marking | Criteria set by recipient |
| **fit** | Sender Threshold | Required match level for sender |

---

## Working with the Generated Model

Keep the generated German entity, property, and DbSet names unchanged. Use descriptive English names for local variables and map to English-named DTOs at the UI or API boundary when useful. Do not edit generated entity classes by hand.

The DbContext exposes pluralized DbSet names. These examples use the current scaffolded names and fields:

```csharp
var userWithFilters = await context.Stamms
    .Include(stamm => stamm.Anglers)
    .FirstOrDefaultAsync(stamm => stamm.StammGuid == userGuid);

var recentMessages = await context.PostIts
    .OrderByDescending(postIt => postIt.Datum)
    .Take(10)
    .ToListAsync();

var nodesInNet = await context.Knotens
    .Where(knoten => knoten.NetzGuid == netzGuid)
    .OrderBy(knoten => knoten.Knoten1)
    .ToListAsync();
```


---

## Matching Logic

**Important:** The matchmaking algorithm is implemented as a **stored procedure** in the database. The application invokes this procedure rather than implementing matching logic in C# code. This procedure must continue to work without modification.

See [065-magic-match-logic.md](065-magic-match-logic.md) for the current SQL behavior of `oli.fischen` and `oli.beissen`.

---

## Tables (Existing Schema)

### Stamm (Users)

**Note:** Exact schema will be discovered via EF Core scaffolding from the existing database. Below is the conceptual structure based on known entities.

```sql
-- Kreislauf (Message Flow)
Stamm (...)         -- User/Author
Angler (...)        -- Filter Profile
PostIt (...)        -- Message
Code (...)          -- Description (author+message+recipient marking)
TopLab (...)        -- Response/Answer

-- Wortraum (Wordspace)
Netz (...)          -- Net
Knoten (...)        -- Node
Baum (...)          -- Tree
Zweig (...)         -- Branch

-- Logic (Matching)
Olis (...)          -- Message markings
get (...)           -- Receiver thresholds
Ilos (...)          -- Filter markings
fit (...)           -- Sender thresholds
```

---

## Scaffolding Strategy

1. Use `dotnet ef dbcontext scaffold` to generate entity classes from existing database
2. Place generated classes in `Models/` directory
3. Configure DbContext with existing connection string
4. **Do not modify database schema** — EF is read/write only, no migrations
5. Call existing stored procedures for matchmaking via `FromSqlRaw()` or `ExecuteSqlRaw()`

### Scaffold Command (example)

```bash
dotnet ef dbcontext scaffold "Server=...;Database=OLI_IT;..." \
  Microsoft.EntityFrameworkCore.SqlServer \
  --output-dir Models \
  --context-dir Data \
  --context OliItDbContext
```

---

## Notes

- Legacy German names preserved for compatibility
- Consider adding XML comments or extension methods for English naming in code
- Stored procedure names for matching: `oli.fischen` (batch sync) and `oli.beissen` (single pair decision)


