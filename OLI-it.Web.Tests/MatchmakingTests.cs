using OLI_it.Web.Tests.Fixtures;
using OLI_it.Web.Tests.Helpers;
using OLI_it.Web.Tests.Models;
using Xunit.Abstractions;

namespace OLI_it.Web.Tests;

/// <summary>
/// Integration tests for the fischen → beissen matchmaking stored procedure pipeline.
///
/// Pre-requisite: place a SQL Server backup at the path configured in
/// <c>appsettings.Tests.json</c> (Matchmaking:BackupFilePath).
/// Tests are automatically skipped when the backup file is absent.
/// </summary>
[Collection(MatchmakingCollection.Name)]
public sealed class MatchmakingTests
{
    private readonly DatabaseFixture _db;
    private readonly ITestOutputHelper _output;
    private static SmokeRunSummary? _smokeSummary;

    private static readonly string CandidateFischenPath =
        Path.GetFullPath("TestData/StoredProcedures/candidate_fischen.sql");

    private static readonly string CandidateBeissenPath =
        Path.GetFullPath("TestData/StoredProcedures/candidate_beissen.sql");

    public MatchmakingTests(DatabaseFixture db, ITestOutputHelper output)
    {
        _db = db;
        _output = output;
    }

    /// <summary>
    /// Runs the full matchmaking pipeline twice — once with the baseline SPs from the backup,
    /// once with any candidate SPs provided in TestData/StoredProcedures/ — then diffs the
    /// Spiegel outcomes and reports timing.
    ///
    /// Only <c>fischen</c> is invoked directly; it calls <c>beissen</c> internally for every
    /// Code × Angler pair. Timing therefore covers the complete pipeline in one measurement.
    /// </summary>
    [Fact]
    public async Task FischenBeissen_CandidateProducesIdenticalOutcome()
    {
        if (!_db.IsAvailable)
        {
            _output.WriteLine($"SKIPPED: backup file not found at '{_db.BackupFilePath}'. " +
                "See OLI-it.Web.Tests/TestData/README.md for setup instructions.");
            return;
        }

        // ── Baseline run ──────────────────────────────────────────────────────
        _output.WriteLine("=== BASELINE RUN ===");
        var baseline = await RunPipelineAsync(applyCandidate: false);
        _output.WriteLine($"  fischen (+ beissen): {baseline.FischenElapsed.TotalMilliseconds:F0} ms");
        _output.WriteLine($"  Spiegel             : {baseline.Rows.Count} rows");

        // Re-provision DB for candidate run
        await DatabaseHelper.DropDatabaseAsync("OliItMatchmakingTest");
        await DatabaseHelper.RestoreBackupAsync(_db.BackupFilePath, "OliItMatchmakingTest");

        // ── Candidate run ─────────────────────────────────────────────────────
        _output.WriteLine("=== CANDIDATE RUN ===");
        var candidate = await RunPipelineAsync(applyCandidate: true);
        _output.WriteLine($"  fischen (+ beissen): {candidate.FischenElapsed.TotalMilliseconds:F0} ms");
        _output.WriteLine($"  Spiegel             : {candidate.Rows.Count} rows");

        // ── Timing delta ──────────────────────────────────────────────────────
        double deltaMs = candidate.FischenElapsed.TotalMilliseconds - baseline.FischenElapsed.TotalMilliseconds;
        _output.WriteLine("=== TIMING DELTA ===");
        _output.WriteLine($"  total delta: {deltaMs:+0.#;-0.#;0} ms  ({FormatSpeedUp(deltaMs, baseline.FischenElapsed.TotalMilliseconds)})");

        // ── Outcome diff ──────────────────────────────────────────────────────
        var diff = baseline.DiffWith(candidate);

        if (diff.HasDifferences)
        {
            _output.WriteLine("=== OUTCOME DIFF ===");

            foreach (var row in diff.Added)
                _output.WriteLine($"  [ADDED]   Code={row.CodeGuid:N} Angler={row.AnglerGuid:N} Status={row.Status}");

            foreach (var row in diff.Removed)
                _output.WriteLine($"  [REMOVED] Code={row.CodeGuid:N} Angler={row.AnglerGuid:N} Status={row.Status}");

            foreach (var c in diff.Changed)
                _output.WriteLine($"  [CHANGED] Code={c.CodeGuid:N} Angler={c.AnglerGuid:N} {c.BaselineStatus} → {c.CandidateStatus}");

            // Fail the test with a summary so the diff is visible in the runner
            Assert.Fail(
                $"Candidate outcome differs from baseline: " +
                $"{diff.Added.Count} added, {diff.Removed.Count} removed, {diff.Changed.Count} changed. " +
                "See test output for details.");
        }
        else
        {
            _output.WriteLine("=== OUTCOME: IDENTICAL ✓ ===");
        }

        string reportPath = MatchmakingHtmlReportWriter.WriteReport(_smokeSummary, baseline, candidate, diff);
        _output.WriteLine($"=== HTML REPORT ===");
        _output.WriteLine(reportPath);
    }

    /// <summary>
    /// Smoke test: verifies that fischen and beissen can be called without error
    /// and that Spiegel contains at least one row afterwards.
    /// </summary>
    [Fact]
    public async Task FischenBeissen_BaselineProducesAtLeastOneSpiegelRow()
    {
        if (!_db.IsAvailable)
        {
            _output.WriteLine($"SKIPPED: backup file not found at '{_db.BackupFilePath}'.");
            return;
        }

        var result = await RunPipelineAsync(applyCandidate: false);

        _output.WriteLine($"fischen (+ beissen): {result.FischenElapsed.TotalMilliseconds:F0} ms");
        _output.WriteLine($"Spiegel rows: {result.Rows.Count}");

        _smokeSummary = new SmokeRunSummary(
            result.FischenElapsed,
            result.Rows.Count,
            result.CodeCount,
            result.AnglerCount);

        Assert.True(result.Rows.Count > 0,
            "Expected at least one row in oli.Spiegel after running fischen + beissen.");
    }

    private async Task<MatchmakingRunResult> RunPipelineAsync(bool applyCandidate)
    {
        var scale = await DatabaseHelper.GetMatchmakingScaleSnapshotAsync(_db.ConnectionString);

        bool fischenSwapped = false;
        bool beissenSwapped = false;

        if (applyCandidate)
        {
            fischenSwapped = await DatabaseHelper.ApplyCandidateSpAsync(_db.ConnectionString, CandidateFischenPath);
            beissenSwapped = await DatabaseHelper.ApplyCandidateSpAsync(_db.ConnectionString, CandidateBeissenPath);

            if (fischenSwapped)  _output.WriteLine("  → candidate_fischen.sql applied");
            if (beissenSwapped)  _output.WriteLine("  → candidate_beissen.sql applied");
            if (!fischenSwapped && !beissenSwapped)
                _output.WriteLine("  → no candidate SPs found; using baseline SPs");
        }

        // fischen calls beissen internally for every Code × Angler pair
        var fischenElapsed = await DatabaseHelper.ExecuteStoredProcedureAsync(
            _db.ConnectionString, _db.FischenProcedure);

        var rows = await DatabaseHelper.ReadSpiegelAsync(_db.ConnectionString);

        return new MatchmakingRunResult(
            rows,
            fischenElapsed,
            codeCount: scale.CodeCount,
            anglerCount: scale.AnglerCount,
            spiegelBefore: scale.SpiegelBefore,
            candidateFischenApplied: fischenSwapped,
            candidateBeissenApplied: beissenSwapped);
    }

    private static string FormatSpeedUp(double deltaMs, double baselineMs)
    {
        if (baselineMs <= 0) return "n/a";
        double pct = -deltaMs / baselineMs * 100;
        return pct >= 0 ? $"{pct:F1}% faster" : $"{-pct:F1}% slower";
    }
}
