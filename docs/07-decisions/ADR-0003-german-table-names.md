# ADR-0003: Preserve German Table Names in Database

**Status:** Accepted  
**Date:** 2026-03-26  
**Decision Makers:** Development Team

## Context

The existing database uses German naming conventions from the original implementation by Frederic Luchting. Examples:
- `Stamm` (User)
- `PostIt` (Message)
- `Angler` (Filter Profile)
- `Netz` (Net), `Knoten` (Node)
- `Baum` (Tree), `Zweig` (Branch)

The database is in production and contains existing data.

## Decision

**The German table names will be preserved in the database without modification.**

Application-level strategies for clarity:
1. Generated EF entity classes will retain German names (e.g., `Stamm`, `PostIt`)
2. Create comprehensive German↔English mapping documentation
3. Use XML comments on entity classes to provide English descriptions
4. Consider creating English-named DTOs, ViewModels, or service interfaces
5. Use extension methods or partial classes to add English-named properties/methods

## Consequences

### Positive
- No database migration risk
- No breaking changes to existing stored procedures or queries
- No data migration needed
- Preserves historical context and original design intent
- Maintains compatibility with any external tools or reports

### Negative
- Developers must learn German→English mapping
- Code may be less immediately readable for non-German speakers
- Onboarding documentation is critical

### Mitigation
- **Documentation:** Maintain `02-data-model.md` with complete mapping table
- **IDE Support:** Use XML comments so IntelliSense shows English descriptions
- **Code Style:** Encourage using descriptive variable names even if types are German
  ```csharp
  // Good
  var user = await _context.Stamm.FindAsync(userId);
  
  // Also good with DTO
  var userDto = mapper.Map<UserDto>(stamm);
  ```

## Naming Strategy Examples

### Option 1: Use German Entities Directly
```csharp
public class HomePageModel : PageModel
{
    public async Task<List<PostIt>> GetRecentMessages()
    {
        return await _context.PostIt
            .Include(p => p.Stamm) // Author
            .OrderByDescending(p => p.CreatedAt)
            .Take(10)
            .ToListAsync();
    }
}
```

### Option 2: DTOs with English Names
```csharp
public class MessageDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public UserDto Author { get; set; }
}

// Mapping
var messageDto = new MessageDto
{
    Id = postIt.PostItId,
    Title = postIt.Title,
    Author = MapUser(postIt.Stamm)
};
```

### Option 3: Extension Methods
```csharp
public static class StammExtensions
{
    public static string GetDisplayName(this Stamm user)
    {
        return user.DisplayName ?? user.Username;
    }
}
```

## Alternatives Considered

### Rename Tables to English
Rename all database tables to English equivalents.

**Rejected because:** 
- High risk to production database
- Requires updating all stored procedures
- Requires data migration coordination
- May break external integrations

### Use Synonyms
Create SQL synonyms with English names pointing to German tables.

**Rejected because:**
- Adds complexity without solving code-level naming
- EF scaffolding would need manual configuration
- Doesn't work with stored procedures

## References

- `02-data-model.md` - Complete German↔English mapping table
- `01-domain-entities.md` - Entity descriptions with both names
