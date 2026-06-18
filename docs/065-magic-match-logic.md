# Magic Match Logic (`oli.fischen` + `oli.beissen`)

Last updated: 2026-06-18  
Status: draft

## Scope

This document describes the current SQL Server implementation of matchmaking for delivering `PostIt` messages to recipients.

- Pair evaluator: `oli.beissen(@CodeGuid, @AnglerGuid)`
- Batch/orchestration runner: `oli.fischen(@CodeGuid, @AnglerGuid)`

## Data perspective

The algorithm matches sender-side **Code** markings against recipient-side **Angler** markings in NKBZ coordinates:

- `NetzGuid`, `BaumGuid`, `ZweigGuid`, `KnotenGuid`
- Sender side (`Ringe`): `OLIs` (strictness), `get` (required recipient threshold)
- Recipient side (`Löcher`): `ILOS` (strictness), `fit` (required sender threshold)

Result pairs are persisted in `Spiegel(CodeGuid, AnglerGuid)`.

## `beissen` decision logic (single pair)

`beissen` returns:

- `0` = match
- `-1` = no match (fails fast on first violated rule)

### 1) Preconditions

1. Code must have at least one `Ringe` row.
2. Angler must have at least one `Löcher` row.

If either side is empty, no match.

### 2) MUST rules (`OLIs=3` / `ILOS=3`) are enforced both directions

1. For every sender `Ringe` with `OLIs=3`, a recipient `Löcher` with same `KnotenGuid` must exist, and:
   - `Löcher.ILOS >= Ringe.get`
   - if sender `ZweigGuid` is set, `Löcher.ZweigGuid` must be equal
2. For every recipient `Löcher` with `ILOS=3`, a sender `Ringe` with same `KnotenGuid` must exist, and:
   - `Ringe.OLIs >= Löcher.fit`
   - if recipient `ZweigGuid` is set, `Ringe.ZweigGuid` must be equal

### 3) SHOULD rules (`OLIs=2` / `ILOS=2`) with threshold >=2 are also enforced both directions

The procedure enforces "at least one matching branch/node per structure" for optional-but-required structures:

1. **Tree-scoped check (`BaumGuid IS NOT NULL`)**
   - Sender side: `Ringe.OLIs=2 AND get>=2`
   - Recipient side: `Löcher.ILOS=2 AND fit>=2`
2. **Net/node-scoped check (`BaumGuid IS NULL`)**
   - Sender side: `Ringe.OLIs=2 AND get>=2`
   - Recipient side: `Löcher.ILOS=2 AND fit>=2`

Both checks are symmetric (sender requirements against recipient, and recipient requirements against sender).

## `fischen` orchestration logic (set processing)

`fischen` runs `beissen` for many pairs and synchronizes `Spiegel`:

1. Parameter behavior:
   - both GUIDs = zero GUID -> all `Code` x all `Angler`
   - only `CodeGuid` given -> one Code x all Angler
   - only `AnglerGuid` given -> all Code x one Angler
   - both given -> one specific pair
2. For each pair:
   - call `beissen`
   - if return `0`: ensure row exists in `Spiegel`
   - else: ensure row is removed from `Spiegel`
3. Runs inside one explicit transaction.

## Important implementation notes

1. Matching is **mutual**: sender and recipient constraints are both authoritative.
2. Threshold semantics are directional:
   - recipient must satisfy sender threshold: `ILOS >= get`
   - sender must satisfy recipient threshold: `OLIs >= fit`
3. Current procedure code explicitly evaluates strictness values `2` and `3`.
4. `Spiegel` acts as the materialized match table used by downstream delivery/read logic.

## Open analysis questions for next step

1. How strictness value `1` ("NOT/exclude") is enforced in current production flow (not explicit in these two procedures).
2. Whether additional matching filters exist outside `beissen`/`fischen` (views, triggers, caller-side predicates).
3. Exact scheduling strategy (when and how often `fischen` is invoked).
