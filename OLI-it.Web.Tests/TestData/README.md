# TestData — Setup Instructions

## Database Backup

Place the OLI-it SQL Server backup file at:

```
OLI-it.Web.Tests/TestData/oli-it-backup.bak
```

The `.bak` file is **not committed to the repository** (it is excluded by `.gitignore`).

You can use any recent production or staging backup. The test will restore it to a fresh
LocalDB database (`OliItMatchmakingTest`) and drop it when finished.

If the backup file is not present, the matchmaking tests are **automatically skipped**
with a clear message — the build will not fail.

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
