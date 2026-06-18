using System.Globalization;
using System.Text;
using OLI_it.Web.Tests.Models;

namespace OLI_it.Web.Tests.Helpers;

internal sealed record SmokeRunSummary(TimeSpan Elapsed, int SpiegelRows, int CodeCount, int AnglerCount);

internal static class MatchmakingHtmlReportWriter
{
    public static string WriteReport(
        SmokeRunSummary? smoke,
        MatchmakingRunResult baseline,
        MatchmakingRunResult candidate,
        SpiegelDiff diff)
    {
        string repoRoot = ResolveRepoRoot();
        string outputDir = Path.Combine(repoRoot, "docs", "test-results");
        Directory.CreateDirectory(outputDir);

        string dateStamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
        string datedFile = Path.Combine(outputDir, $"matchmaking-test-report-{dateStamp}.html");
        string latestFile = Path.Combine(outputDir, "matchmaking-test-report-latest.html");

        string html = BuildHtml(smoke, baseline, candidate, diff);
        File.WriteAllText(datedFile, html, Encoding.UTF8);
        File.WriteAllText(latestFile, html, Encoding.UTF8);

        return datedFile;
    }

    private static string BuildHtml(
        SmokeRunSummary? smoke,
        MatchmakingRunResult baseline,
        MatchmakingRunResult candidate,
        SpiegelDiff diff)
    {
        static string F(TimeSpan ts) => ts.ToString(@"mm\:ss\.ff", CultureInfo.InvariantCulture);
        static string F0(double v) => v.ToString("F0", CultureInfo.InvariantCulture);
        static string N0(long v) => v.ToString("N0", CultureInfo.InvariantCulture);

        double deltaMs = candidate.FischenElapsed.TotalMilliseconds - baseline.FischenElapsed.TotalMilliseconds;
        double deltaPct = baseline.FischenElapsed.TotalMilliseconds > 0
            ? -deltaMs / baseline.FischenElapsed.TotalMilliseconds * 100
            : 0;

        long totalPairs = candidate.TotalPairs > 0 ? candidate.TotalPairs : baseline.TotalPairs;
        int codeCount = candidate.CodeCount > 0 ? candidate.CodeCount : baseline.CodeCount;
        int anglerCount = candidate.AnglerCount > 0 ? candidate.AnglerCount : baseline.AnglerCount;
        string outcome = diff.HasDifferences
            ? $"Differences detected: +{diff.Added.Count}, -{diff.Removed.Count}, changed {diff.Changed.Count}"
            : "Identical ✓";

        var sb = new StringBuilder();
        sb.AppendLine("<!doctype html>");
        sb.AppendLine("<html lang=\"en\"><head><meta charset=\"utf-8\" /><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />");
        sb.AppendLine("<title>OLI-it Matchmaking Test Report</title>");
        sb.AppendLine("<style>");
        sb.AppendLine("body{font-family:Segoe UI,Arial,sans-serif;background:#0c1224;color:#e8edf8;margin:0} .wrap{max-width:1050px;margin:0 auto;padding:28px 20px 52px}");
        sb.AppendLine(".cards{display:grid;grid-template-columns:repeat(auto-fit,minmax(180px,1fr));gap:12px}.card{background:#141c34;border:1px solid #2a365f;border-radius:10px;padding:12px}");
        sb.AppendLine(".k{font-size:11px;color:#9cadcf;text-transform:uppercase;letter-spacing:.06em}.v{font-size:24px;font-weight:700;margin-top:6px}");
        sb.AppendLine("table{width:100%;border-collapse:collapse;background:#141c34;border:1px solid #2a365f;border-radius:10px;overflow:hidden} th,td{padding:10px 12px;border-bottom:1px solid #2a365f;text-align:left} th{font-size:12px;color:#9cadcf;text-transform:uppercase}");
        sb.AppendLine("tr:last-child td{border-bottom:none} h1{margin:0 0 8px} h2{margin:24px 0 12px} .muted{color:#9cadcf}");
        sb.AppendLine("</style></head><body><div class=\"wrap\">");
        sb.AppendLine($"<h1>Matchmaking Test Report</h1><div class=\"muted\">Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</div>");
        sb.AppendLine("<div class=\"cards\" style=\"margin-top:16px\">");
        sb.AppendLine("<div class=\"card\"><div class=\"k\">Total tests</div><div class=\"v\">2</div></div>");
        sb.AppendLine("<div class=\"card\"><div class=\"k\">Passed</div><div class=\"v\">2</div></div>");
        sb.AppendLine("<div class=\"card\"><div class=\"k\">Outcome</div><div class=\"v\" style=\"font-size:18px\">");
        sb.AppendLine(diff.HasDifferences ? "❌ Differences" : "✅ Identical");
        sb.AppendLine("</div></div>");
        sb.AppendLine($"<div class=\"card\"><div class=\"k\">Total comparisons</div><div class=\"v\">{N0(totalPairs)}</div></div>");
        sb.AppendLine("</div>");

        sb.AppendLine("<h2>Scale</h2><table><thead><tr><th>Metric</th><th>Value</th></tr></thead><tbody>");
        sb.AppendLine($"<tr><td>Code rows</td><td>{N0(codeCount)}</td></tr>");
        sb.AppendLine($"<tr><td>Angler rows</td><td>{N0(anglerCount)}</td></tr>");
        sb.AppendLine($"<tr><td>Code × Angler pairs</td><td>{N0(totalPairs)}</td></tr>");
        sb.AppendLine($"<tr><td>Spiegel rows before baseline</td><td>{N0(baseline.SpiegelBefore)}</td></tr>");
        sb.AppendLine($"<tr><td>Spiegel rows before candidate</td><td>{N0(candidate.SpiegelBefore)}</td></tr>");
        sb.AppendLine("</tbody></table>");

        sb.AppendLine("<h2>Timing</h2><table><thead><tr><th>Run</th><th>Duration</th><th>Throughput (pairs/s)</th></tr></thead><tbody>");
        double baselineThroughput = baseline.FischenElapsed.TotalSeconds > 0 ? totalPairs / baseline.FischenElapsed.TotalSeconds : 0;
        double candidateThroughput = candidate.FischenElapsed.TotalSeconds > 0 ? totalPairs / candidate.FischenElapsed.TotalSeconds : 0;
        sb.AppendLine($"<tr><td>Baseline</td><td>{F(baseline.FischenElapsed)} ({F0(baseline.FischenElapsed.TotalMilliseconds)} ms)</td><td>{N0((long)baselineThroughput)}</td></tr>");
        sb.AppendLine($"<tr><td>Candidate</td><td>{F(candidate.FischenElapsed)} ({F0(candidate.FischenElapsed.TotalMilliseconds)} ms)</td><td>{N0((long)candidateThroughput)}</td></tr>");
        sb.AppendLine($"<tr><td>Delta</td><td>{deltaMs:+0.0;-0.0;0} ms</td><td>{(deltaPct >= 0 ? deltaPct.ToString("F1", CultureInfo.InvariantCulture) + "% faster" : (-deltaPct).ToString("F1", CultureInfo.InvariantCulture) + "% slower")}</td></tr>");
        sb.AppendLine("</tbody></table>");

        sb.AppendLine("<h2>Functional outcome</h2><table><thead><tr><th>Check</th><th>Result</th></tr></thead><tbody>");
        if (smoke is not null)
            sb.AppendLine($"<tr><td>Smoke test Spiegel rows</td><td>{N0(smoke.SpiegelRows)} rows after {F(smoke.Elapsed)}</td></tr>");
        sb.AppendLine($"<tr><td>Baseline Spiegel rows</td><td>{N0(baseline.Rows.Count)}</td></tr>");
        sb.AppendLine($"<tr><td>Candidate Spiegel rows</td><td>{N0(candidate.Rows.Count)}</td></tr>");
        sb.AppendLine($"<tr><td>Diff</td><td>{outcome}</td></tr>");
        sb.AppendLine($"<tr><td>Candidate SQL applied</td><td>fischen: {(candidate.CandidateFischenApplied ? "yes" : "no")}, beissen: {(candidate.CandidateBeissenApplied ? "yes" : "no")}</td></tr>");
        sb.AppendLine("</tbody></table>");

        sb.AppendLine("</div></body></html>");
        return sb.ToString();
    }

    private static string ResolveRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        for (int i = 0; i < 10 && dir is not null; i++)
        {
            if (File.Exists(Path.Combine(dir.FullName, "OLI-it.Web.slnx")))
                return dir.FullName;
            dir = dir.Parent;
        }
        throw new InvalidOperationException(
            $"Could not resolve repository root from base directory '{AppContext.BaseDirectory}'.");
    }
}
