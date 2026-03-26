# ADR-0002: Stored Procedure for Matchmaking Logic

**Status:** Accepted  
**Date:** 2026-03-26  
**Decision Makers:** Development Team

## Context

The core feature of OLI-it is matchmaking: determining which messages (PostIt) should be delivered to which recipients based on their filter profiles (Angler). The matching algorithm evaluates:
- Author description criteria (Code → Olis markings)
- Recipient filter criteria (Angler → Ilos markings)
- Threshold values (get, fit)
- First-value and Second-value rules

This logic was implemented years ago as a **SQL Server stored procedure** and has been running successfully in production.

## Decision

The application will **invoke the existing stored procedure** for all matchmaking operations rather than reimplementing the algorithm in C# code.

1. Use EF Core's `FromSqlRaw()`, `ExecuteSqlRaw()`, or `FromSqlInterpolated()` to call the stored procedure
2. **Do not reimplement** the matching logic in C# 
3. **Do not modify** the stored procedure unless absolutely necessary
4. Document the stored procedure's signature and behavior

## Consequences

### Positive
- Proven, tested logic continues to work
- Likely optimized for SQL Server performance
- No risk of introducing bugs by reimplementation
- Database performs complex set operations efficiently
- Reduces C# code complexity

### Negative
- Logic is not in C# codebase, harder to debug
- Cannot easily unit test matching logic (requires database)
- Less portable if switching databases
- Stored procedure may be harder to maintain for developers unfamiliar with T-SQL

### Mitigation
- Create integration tests that verify matchmaking behavior via stored procedure
- Document stored procedure parameters, return values, and business rules
- Add logging/tracing around stored procedure calls for debugging
- Consider creating a service layer abstraction (`IMatchmakingService`) so implementation could theoretically be swapped

## Implementation Notes

```csharp
// Example invocation in MatchmakingService.cs
public async Task<IEnumerable<Match>> FindMatchesAsync(int messageId)
{
    var matches = await _context.Matches
        .FromSqlInterpolated($"EXEC sp_FindMatches {messageId}")
        .ToListAsync();
    
    return matches;
}
```

## Alternatives Considered

### Reimplement in C# with LINQ
Port the matching algorithm to C# using LINQ queries against EF entities.

**Rejected because:** High risk of introducing bugs; stored procedure is proven and optimized; significant development time required.

### Hybrid Approach
Load data in C#, perform matching logic in-memory.

**Rejected because:** Performance concerns with large datasets; loses database optimizations; more complex than calling stored procedure.

## Open Questions

- [ ] What is the exact name of the stored procedure(s)? *(to be discovered during scaffolding)*
- [ ] What are the parameters and return types?
- [ ] Does it handle batch matching or single message?
- [ ] Are there any triggers or scheduled jobs that call it?

## References

- `01-domain-entities.md` - Matchmaking glossary and rules
- `02-data-model.md` - Tables involved in matching (Olis, Ilos, get, fit)
