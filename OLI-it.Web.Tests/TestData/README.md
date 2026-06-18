# TestData — Setup Instructions

## Database Backup

The test database backup is committed at:

```
data/null.bak
```

It is automatically copied to the test output directory at build time and resolved relative to the test binary. No manual setup is needed — just build and run.

> The `.gitignore` contains `!data/null.bak` to allow this specific file while still
> excluding other `.bak` files from accidental commit.

## Candidate Stored Procedures

To test a new (candidate) version of `fischen` or `beissen`:

1. Open the corresponding file in `StoredProcedures/`:
   - `candidate_fischen.sql` — candidate version of the `fischen` procedure
   - `candidate_beissen.sql` — candidate version of the `beissen` procedure

2. Replace the placeholder comment with a full `CREATE OR ALTER PROCEDURE` statement.

3. Run the tests. The suite will:
   - Run the baseline SPs (from the restored backup)
   - Then run the candidate SPs
   - Output a diff of the `oli.Spiegel` results and timing comparison

If a candidate file contains only the placeholder comment (no SQL), that SP is **not swapped**
and the backup's original SP version is used for the candidate run.

## Configuration

SP names and the backup path are configured in `appsettings.Tests.json`:

```json
{
  "Matchmaking": {
    "BackupFilePath": "TestData/oli-it-backup.bak",
    "FischenProcedure": "oli.fischen",
    "BeissenProcedure": "oli.beissen"
  }
}
```

Adjust these values if your SP names differ.
