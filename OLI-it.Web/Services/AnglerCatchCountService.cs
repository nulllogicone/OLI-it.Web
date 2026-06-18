using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;

namespace OLI_it.Web.Services;

public class AnglerCatchCountService(OliItDbContext context)
{
    public async Task<Dictionary<Guid, int>> GetCatchCountsByAnglerGuidsAsync(IEnumerable<Guid> anglerGuids)
    {
        var guidList = anglerGuids.Distinct().ToList();
        if (guidList.Count == 0)
        {
            return new Dictionary<Guid, int>();
        }

        return await context.Spiegels
            .Where(s => guidList.Contains(s.AnglerGuid))
            .Join(
                context.Codes,
                spiegel => spiegel.CodeGuid,
                code => code.CodeGuid,
                (spiegel, code) => new { spiegel.AnglerGuid, code.PostItGuid })
            .Distinct()
            .GroupBy(x => x.AnglerGuid)
            .Select(g => new { AnglerGuid = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.AnglerGuid, x => x.Count);
    }
}
