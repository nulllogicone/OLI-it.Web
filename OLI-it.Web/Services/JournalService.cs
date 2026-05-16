using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;

namespace OLI_it.Web.Services;

public class JournalService(OliItDbContext context)
{
    public const int DefaultCount = 20;
    public const int MaxCount = 200;

    /// <summary>
    /// Returns the latest journal entries, optionally filtered by entry type (Zeichen).
    /// </summary>
    public async Task<List<UnionJournale>> GetLatestAsync(string? filter, int count)
    {
        count = Math.Clamp(count, 1, MaxCount);

        var query = context.UnionJournales.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter))
            query = query.Where(j => j.Zeichen == filter);

        return await query
            .OrderByDescending(j => j.Datum)
            .Take(count)
            .ToListAsync();
    }
}
