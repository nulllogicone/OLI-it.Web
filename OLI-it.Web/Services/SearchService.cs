using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;

namespace OLI_it.Web.Services;

public record SearchResults(
    List<Stamm> Stamms,
    List<PostIt> PostIts,
    List<TopLab> TopLabs);

public class SearchService(OliItDbContext context)
{
    private const int MaxPerEntity = 25;

    public async Task<SearchResults> SearchAsync(string term)
    {
        var pattern = ToLikePattern(term);

        var stamms = await context.Stamms
            .Where(s => EF.Functions.Like(s.Stamm1, pattern)
                     || (s.Beschreibung != null && EF.Functions.Like(s.Beschreibung, pattern)))
            .OrderByDescending(s => s.Datum)
            .Take(MaxPerEntity)
            .ToListAsync();

        var postIts = await context.PostIts
            .Where(p => (p.Titel != null && EF.Functions.Like(p.Titel, pattern))
                     || EF.Functions.Like(p.PostIt1, pattern))
            .OrderByDescending(p => p.Datum)
            .Take(MaxPerEntity)
            .ToListAsync();

        var topLabs = await context.TopLabs
            .Where(t => (t.Titel != null && EF.Functions.Like(t.Titel, pattern))
                     || EF.Functions.Like(t.TopLab1, pattern))
            .OrderByDescending(t => t.Datum)
            .Take(MaxPerEntity)
            .ToListAsync();

        return new SearchResults(stamms, postIts, topLabs);
    }

    /// <summary>
    /// Converts user-supplied wildcard term (using *) to a SQL LIKE pattern (using %).
    /// A bare term with no wildcards is wrapped in %…%.
    /// </summary>
    private static string ToLikePattern(string term)
    {
        // Escape SQL LIKE special characters first (except the user's *)
        var escaped = term
            .Replace("%", "[%]")
            .Replace("[", "[[]")
            .Replace("_", "[_]");

        // Replace user wildcards with SQL wildcards
        var pattern = escaped.Replace("*", "%");

        // If the user didn't specify any wildcards, do a contains search
        if (!term.Contains('*'))
            pattern = $"%{pattern}%";

        return pattern;
    }
}
