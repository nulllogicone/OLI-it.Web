using OLI_it.Web.Tests.Helpers;

namespace OLI_it.Web.Tests.Models;

/// <summary>
/// Captures the full output of one matchmaking pipeline run:
/// the Spiegel rows after <c>fischen</c> + <c>beissen</c> have executed,
/// plus the wall-clock time each stored procedure took.
/// </summary>
public sealed class MatchmakingRunResult
{
    public IReadOnlyList<SpiegelRow> Rows { get; }
    public TimeSpan FischenElapsed { get; }
    public TimeSpan BeissenElapsed { get; }
    public TimeSpan TotalElapsed => FischenElapsed + BeissenElapsed;

    public MatchmakingRunResult(
        IReadOnlyList<SpiegelRow> rows,
        TimeSpan fischenElapsed,
        TimeSpan beissenElapsed)
    {
        Rows = rows;
        FischenElapsed = fischenElapsed;
        BeissenElapsed = beissenElapsed;
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

        // Rows with same key but different Status
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

/// <summary>
/// Summary of differences between a baseline and candidate Spiegel snapshot.
/// </summary>
public sealed record SpiegelDiff(
    IReadOnlyList<SpiegelRow> Added,
    IReadOnlyList<SpiegelRow> Removed,
    IReadOnlyList<SpiegelChange> Changed)
{
    public bool HasDifferences => Added.Count > 0 || Removed.Count > 0 || Changed.Count > 0;
}

/// <summary>
/// A Spiegel row whose Status changed between baseline and candidate.
/// </summary>
public sealed record SpiegelChange(
    Guid CodeGuid,
    Guid AnglerGuid,
    string? BaselineStatus,
    string? CandidateStatus);
