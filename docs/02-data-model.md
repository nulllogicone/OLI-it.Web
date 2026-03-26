# Data Model

Last updated: 2026-03-26
Status: draft

## Approach

- **Existing database** — schema is already defined and must be used without changes
- EF Core Database-First via scaffolding
- SQL Server
- German table names from original implementation
- Matching logic implemented as stored procedure
- Domain model in `01-domain-entities.md` provides conceptual mapping

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

## Matching Logic

**Important:** The matchmaking algorithm is implemented as a **stored procedure** in the database. The application invokes this procedure rather than implementing matching logic in C# code. This procedure must continue to work without modification.

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
- Stored procedure name(s) for matching: **TBD** (discover during scaffolding
  DescriptionId INT IDENTITY PK,
  MessageId     INT NOT NULL UNIQUE FK → Messages
)
```

### FilterProfiles

```sql
FilterProfiles (
  FilterProfileId INT IDENTITY PK,
  UserId          INT NOT NULL FK → Users,
  Name            NVARCHAR(200) NOT NULL,
  IsActive        BIT NOT NULL DEFAULT 1,
  CreatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE()
)
```

### Criteria

Stores markings for both Descriptions and FilterProfiles.

```sql
Criteria (
  CriterionId  INT IDENTITY PK,
  ContextType  TINYINT NOT NULL,      -- 1=Description, 2=FilterProfile
  ContextId    INT NOT NULL,          -- FK to DescriptionId or FilterProfileId
  ElementType  TINYINT NOT NULL,      -- 1=Node, 2=Branch
  ElementId    INT NOT NULL,          -- FK to NodeId or BranchId
  FirstValue   TINYINT NOT NULL,      -- 1, 2, or 3
  SecondValue  TINYINT NOT NULL       -- 0, 2, or 3
)
-- Index: (ContextType, ContextId) for fast lookup
```

### Answers

```sql
Answers (
  AnswerId   INT IDENTITY PK,
  MessageId  INT NOT NULL FK → Messages,
  AuthorId   INT NOT NULL FK → Users,
  Content    NVARCHAR(MAX) NOT NULL,
  IsActive   BIT NOT NULL DEFAULT 1,
  CreatedAt  DATETIME2 NOT NULL DEFAULT GETUTCDATE()
)
```

### Ratings

```sql
Ratings (
  RatingId       INT IDENTITY PK,
  AnswerId       INT NOT NULL FK → Answers,
  GivenByUserId  INT NOT NULL FK → Users,
  Score          INT NOT NULL,
  CreatedAt      DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  UNIQUE (AnswerId, GivenByUserId)   -- one rating per user per answer
)
```

### Transactions

```sql
Transactions (
  TransactionId     INT IDENTITY PK,
  UserId            INT NOT NULL FK → Users,
  RelatedMessageId  INT NULL FK → Messages,
  RelatedAnswerId   INT NULL FK → Answers,
  Amount            DECIMAL(18,4) NOT NULL,
  Reason            TINYINT NOT NULL,   -- enum: 1=Delivery, 2=Answer, 3=Rating, ...
  CreatedAt         DATETIME2 NOT NULL DEFAULT GETUTCDATE()
)
```

### MessageDeliveries

Junction table recording which messages were delivered to which filter profiles (matchmaking results).

```sql
MessageDeliveries (
  DeliveryId      INT IDENTITY PK,
  MessageId       INT NOT NULL FK → Messages,
  FilterProfileId INT NOT NULL FK → FilterProfiles,
  DeliveredAt     DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
  UNIQUE (MessageId, FilterProfileId)
)
```

---

## EF Core Notes

- Use `HasQueryFilter` for soft-delete (`IsActive = 1`) on User, Message, Answer.
- Criteria polymorphism: use discriminator column (`ContextType`) or separate tables (to be decided).
- `WordspaceBranch` self-referencing hierarchy: use `HasOne/WithMany` with `ParentBranchId`.
- Consider `IEntityTypeConfiguration<T>` classes per entity for clean mapping.
- Migrations: one migration per significant schema change, named descriptively.

---

## Open Questions

- Should Criteria be split into two tables (DescriptionCriteria / FilterCriteria) for cleaner FKs?
- Should MessageDeliveries be computed at write-time or query-time?
- What indexes are needed on Criteria for matchmaking performance?
- Confirm legacy column names and types before first migration.

## Change Log

- 2026-03-26: Initial schema derived from domain entities and paper model.
