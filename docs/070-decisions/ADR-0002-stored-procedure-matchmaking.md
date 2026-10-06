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


## Alternatives Considered

### Reimplement in C# with LINQ
Port the matching algorithm to C# using LINQ queries against EF entities.

**Rejected because:** High risk of introducing bugs; stored procedure is proven and optimized; significant development time required.

### Hybrid Approach
Load data in C#, perform matching logic in-memory.

**Rejected because:** Performance concerns with large datasets; loses database optimizations; more complex than calling stored procedure.

## Matchmaking Operations

- `oli.beissen(@CodeGuid, @AnglerGuid)` evaluates one Code/Angler pair and returns whether they match.
- `oli.fischen(@CodeGuid, @AnglerGuid)` calls `beissen` and updates `Spiegel` by inserting matching pairs and deleting non-matching pairs.
- `fischen` supports a single pair, one Code against all Angler records, one Angler against all Code records, or a full Code-by-Angler match run. A `00000` GUID indicates that side should include all records; when neither side is specified, it runs the full NxM match.
- These are the only stored procedures known to affect matching in `Spiegel`.
- After a Code or Angler is updated in the UI, the user can click a button to invoke the procedure. An asynchronous path is also implemented: the application queues a message containing `CodeGuid` and `AnglerGuid`, and an Azure Function processes it and calls the procedure.
- No additional constraints from related views, triggers, or callers are known. This has not been independently verified against the database objects.
- No database trigger or scheduled job was identified in the discussion; the described invocation paths are initiated by the application UI or queue-triggered Azure Function.

## Open Questions

- [ ] Verify whether related views, database triggers, or other callers add matching constraints beyond `fischen` and `beissen`.
- [ ] Verify whether any database triggers or scheduled jobs invoke either procedure.

## References

- `010-domain-entities.md` - Matchmaking glossary and rules
- `020-data-model.md` - Tables involved in matching (Olis, Ilos, get, fit)
- `065-magic-match-logic.md` - Current SQL behavior and rule flow
