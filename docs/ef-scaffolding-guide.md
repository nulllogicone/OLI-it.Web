# EF Core Scaffolding Guide

This guide documents how to scaffold the existing database into EF Core entity classes.

## Prerequisites

- .NET 10 SDK installed
- Connection string to the production/staging database
- EF Core tools installed globally or locally

### Install EF Core Tools (if needed)

```bash
dotnet tool install --global dotnet-ef
# or update if already installed
dotnet tool update --global dotnet-ef
```

## Scaffolding Commands

### Full Database Scaffold

Scaffold all tables from the database:

```bash
dotnet ef dbcontext scaffold "Server=YOUR_SERVER;Database=OLI_IT;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True" ^
  Microsoft.EntityFrameworkCore.SqlServer ^
  --output-dir Models ^
  --context-dir Data ^
  --context OliItDbContext ^
  --force
```

### Scaffold Specific Tables

If you only want to scaffold certain tables:

```bash
dotnet ef dbcontext scaffold "YOUR_CONNECTION_STRING" ^
  Microsoft.EntityFrameworkCore.SqlServer ^
  --output-dir Models ^
  --context-dir Data ^
  --context OliItDbContext ^
  --table Stamm ^
  --table PostIt ^
  --table Angler ^
  --table Code ^
  --table TopLab ^
  --table Netz ^
  --table Knoten ^
  --table Baum ^
  --table Zweig ^
  --table Olis ^
  --table Ilos ^
  --table get ^
  --table fit ^
  --force
```

## Command Options Explained

| Option | Description |
|--------|-------------|
| `--output-dir Models` | Place entity classes in Models folder |
| `--context-dir Data` | Place DbContext in Data folder |
| `--context OliItDbContext` | Name the DbContext class |
| `--force` | Overwrite existing files |
| `--table TableName` | Only scaffold specific tables |
| `--no-onconfiguring` | Don't include connection string in DbContext |
| `--data-annotations` | Use data annotations instead of Fluent API |

## Post-Scaffolding Steps

### 1. Move Connection String to Configuration

Remove hardcoded connection string from `OliItDbContext.OnConfiguring()` and use dependency injection.

**In `OliItDbContext.cs`:**
```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    // Remove or comment out the connection string
    // optionsBuilder.UseSqlServer("...");
}
```

**In `Program.cs`:**
```csharp
builder.Services.AddDbContext<OliItDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OliItDb")));
```

**In `appsettings.json`:**
```json
{
  "ConnectionStrings": {
    "OliItDb": "Server=YOUR_SERVER;Database=OLI_IT;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True"
  }
}
```

### 2. Add XML Comments

Add English descriptions to German-named classes:

```csharp
/// <summary>
/// User entity (German: Stamm - "stem" or "base")
/// Represents a platform user who can be both author and recipient.
/// </summary>
public partial class Stamm
{
    // ...
}
```

### 3. Create Partial Classes (optional)

Extend scaffolded entities without modifying generated code:

```csharp
// Models/Stamm.partial.cs
public partial class Stamm
{
    /// <summary>
    /// Gets the display name or falls back to username
    /// </summary>
    public string GetDisplayNameOrUsername() => DisplayName ?? Username;
}
```

### 4. Discover Stored Procedures

Check for matchmaking stored procedures:

```sql
-- Run in SQL Server Management Studio or Azure Data Studio
SELECT 
    name,
    type_desc,
    create_date,
    modify_date
FROM sys.procedures
WHERE name LIKE '%match%'
   OR name LIKE '%oli%'
   OR name LIKE '%fit%'
ORDER BY name;
```

Common patterns:
- `sp_FindMatches`
- `usp_MatchMessages`
- `proc_Matchmaking`

### 5. Test the Scaffolding

Create a simple test to verify DbContext works:

```csharp
// Test in a Razor Page or controller
var userCount = await _context.Stamm.CountAsync();
var messageCount = await _context.PostIt.CountAsync();

Console.WriteLine($"Users: {userCount}, Messages: {messageCount}");
```

## Troubleshooting

### "Connection string is invalid"
- Check server name, database name, credentials
- Ensure SQL Server allows remote connections
- Try adding `TrustServerCertificate=True` if using self-signed cert

### "Table name conflicts with entity name"
- EF will pluralize/singularize by default
- Use `--table` to explicitly specify tables
- Configure with Fluent API: `.ToTable("Stamm")`

### "Cannot find dotnet-ef"
```bash
dotnet tool install --global dotnet-ef
```

### "Duplicate property names"
- May occur if column names differ only by case
- Use Fluent API to manually map in `OnModelCreating`

## Best Practices

1. **Version Control**: Commit scaffolded files, but document that they're generated
2. **Documentation**: Add XML comments immediately after scaffolding
3. **Don't Modify Generated Code Directly**: Use partial classes instead
4. **Re-scaffolding**: Use `--force` carefully - it overwrites all changes
5. **Connection Strings**: Never commit connection strings with real credentials

## Next Steps After Scaffolding

1. Document discovered stored procedure names in ADR-0002
2. Create service layer (e.g., `MatchmakingService`) to call stored procedures
3. Build ViewModels/DTOs with English names for Razor Pages
4. Create integration tests for database operations
5. Set up proper connection string management (User Secrets, Azure Key Vault)

## References

- [Microsoft EF Core Scaffolding Docs](https://learn.microsoft.com/en-us/ef/core/managing-schemas/scaffolding/)
- [ADR-0001: Database-First Approach](07-decisions/ADR-0001-database-first-approach.md)
- [german-english-quick-reference.md](german-english-quick-reference.md)
