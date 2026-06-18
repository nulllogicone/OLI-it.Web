using OLI_it.Web.Tests.Helpers;

namespace OLI_it.Web.Tests.Models;

/// <summary>
/// Captures the output of one full matchmaking pipeline run.
/// <c>fischen</c> is the only SP called directly — it calls <c>beissen</c> internally
/// for each Code × Angler pair, so <c>FischenElapsed</c> covers the complete pipeline.
/// </summary>
public sealed class MatchmakingRunResult
{
    public IReadOnlyList<SpiegelRow> Rows { get; }

    /// <summary>Wall-clock time for the complete <c>fischen</c> call (includes all <c>beissen</c> sub-calls).</summary>
    public TimeSpan FischenElapsed { get; }

    public int CodeCount { get; }
    public int AnglerCount { get; }
    public int SpiegelBefore { get; }
    public bool CandidateFischenApplied { get; }
    public bool CandidateBeissenApplied { get; }
    public long TotalPairs => (long)CodeCount * AnglerCount;

    public MatchmakingRunResult(
        IReadOnlyList<SpiegelRow> rows,
        TimeSpan fischenElapsed,
        int codeCount = 0,
        int anglerCount = 0,
        int spiegelBefore = 0,
        bool candidateFischenApplied = false,
        bool candidateBeissenApplied = false)
    {
        Rows = rows;
        FischenElapsed = fischenElapsed;
        CodeCount = codeCount;
        AnglerCount = anglerCount;
        SpiegelBefore = spiegelBefore;
        CandidateFischenApplied = candidateFischenApplied;
        CandidateBeissenApplied = candidateBeissenApplied;
    }

    /// <summary>
    /// Computes the difference between this result (baseline) and <paramref name="candidate"/>.
    /// </summary>
    public SpiegelDiff DiffWith(MatchmakingRunResult candidate)
    {
        var baselineSet = Rows.ToHashSet();
        var candidateSet = candidate.Rows.ToHashSet();

        var added = candidateSet.Except(baselineSet).ToList();
        var removed = baselineSet.Except(candidateSet).ToList();

        var baselineByKey = Rows.ToDictionary(r => (r.CodeGuid, r.AnglerGuid));
        var changed = new List<SpiegelChange>();
        foreach (var row in candidate.Rows)
        {
            if (baselineByKey.TryGetValue((row.CodeGuid, row.AnglerGuid), out var baselineRow)
                && baselineRow.Status != row.Status)
            {
                changed.Add(new SpiegelChange(row.CodeGuid, row.AnglerGuid, baselineRow.Status, row.Status));
            }
        }

        return new SpiegelDiff(added, removed, changed);
    }
}

/// <summary>Summary of differences between a baseline and candidate Spiegel snapshot.</summary>
public sealed record SpiegelDiff(
    IReadOnlyList<SpiegelRow> Added,
    IReadOnlyList<SpiegelRow> Removed,
    IReadOnlyList<SpiegelChange> Changed)
{
    public bool HasDifferences => Added.Count > 0 || Removed.Count > 0 || Changed.Count > 0;
}

/// <summary>A Spiegel row whose Status changed between baseline and candidate.</summary>
public sealed record SpiegelChange(
    Guid CodeGuid,
    Guid AnglerGuid,
    string? BaselineStatus,
    string? CandidateStatus);
