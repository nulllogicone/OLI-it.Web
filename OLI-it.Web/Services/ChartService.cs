using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;

namespace OLI_it.Web.Services;

public class ChartService(OliItDbContext context)
{
    public const int DefaultCount = 5;

    public async Task<ChartData> GetChartDataAsync(int count = DefaultCount)
    {
        count = Math.Clamp(count, 1, 20);

        var richStamm = await context.Stamms
            .Where(s => s.KooK.HasValue)
            .OrderByDescending(s => s.KooK)
            .Take(count)
            .ToListAsync();

        var poorStamm = await context.Stamms
            .Where(s => s.KooK.HasValue)
            .OrderBy(s => s.KooK)
            .Take(count)
            .ToListAsync();

        var expensivePostIt = await context.PostIts
            .OrderByDescending(p => p.KooK)
            .Take(count)
            .ToListAsync();

        var cheapPostIt = await context.PostIts
            .OrderBy(p => p.KooK)
            .Take(count)
            .ToListAsync();

        var topTopLab = await context.TopLabs
            .OrderByDescending(t => t.Lohn)
            .Take(count)
            .ToListAsync();

        var flopTopLab = await context.TopLabs
            .OrderBy(t => t.Lohn)
            .Take(count)
            .ToListAsync();

        var topClicksPostIt = await context.PostIts
            .OrderByDescending(p => p.Hits)
            .Take(count)
            .ToListAsync();

        var flopClicksPostIt = await context.PostIts
            .Where(p => p.Hits > 0)
            .OrderBy(p => p.Hits)
            .Take(count)
            .ToListAsync();

        return new ChartData(
            richStamm, poorStamm,
            expensivePostIt, cheapPostIt,
            topTopLab, flopTopLab,
            topClicksPostIt, flopClicksPostIt);
    }
}

public record ChartData(
    List<Stamm> RichStamm,
    List<Stamm> PoorStamm,
    List<PostIt> ExpensivePostIt,
    List<PostIt> CheapPostIt,
    List<TopLab> TopTopLab,
    List<TopLab> FlopTopLab,
    List<PostIt> TopClicksPostIt,
    List<PostIt> FlopClicksPostIt);
