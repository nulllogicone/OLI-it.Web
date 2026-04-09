using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;

namespace OLI_it.Web.Pages.Stamm
{
    public class StatisticsModel : PageModel
    {
        private readonly OliItDbContext _context;

        public StatisticsModel(OliItDbContext context)
        {
            _context = context;
        }

        public Models.Stamm? Stamm { get; set; }
        public Models.PostIt? PostIt { get; set; }
        public Models.Angler? Angler { get; set; }
        public Models.TopLab? TopLab { get; set; }

        // Chart data — last 12 calendar months
        public List<string> MonthLabels { get; set; } = new();
        public List<int> PostItCounts { get; set; } = new();
        public List<int> TopLabCounts { get; set; } = new();
        public List<decimal> CreditDeltas { get; set; } = new();

        // Summary tiles
        public int TotalPostIts { get; set; }
        public int TotalTopLabs { get; set; }
        public int TotalAnglers { get; set; }
        public decimal TotalCreditIn { get; set; }
        public decimal TotalCreditOut { get; set; }
        public decimal CurrentBalance { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
                return NotFound();

            Stamm = await _context.Stamms
                .FirstOrDefaultAsync(m => m.StammGuid == id);

            if (Stamm == null)
                return NotFound();

            // Build a window starting from when the Stamm was created
            var now = DateTime.UtcNow;
            var windowStart = new DateTime(Stamm.Datum.Year, Stamm.Datum.Month, 1);
            var currentMonthStart = new DateTime(now.Year, now.Month, 1);

            // Calculate the number of months from creation to now
            int monthCount = ((currentMonthStart.Year - windowStart.Year) * 12) + currentMonthStart.Month - windowStart.Month + 1;

            for (int i = 0; i < monthCount; i++)
                MonthLabels.Add(windowStart.AddMonths(i).ToString("MMM yyyy"));

            // Fetch relevant dates in bulk, then bucket client-side to avoid 12 round-trips
            var postItDates = await _context.Wurzelns
                .Include(w => w.PostIt)
                .Where(w => w.StammGuid == id && w.PostIt.Datum >= windowStart)
                .Select(w => w.PostIt.Datum)
                .ToListAsync();

            var topLabDates = await _context.TopLabs
                .Where(t => t.StammGuid == id && t.Datum >= windowStart)
                .Select(t => t.Datum)
                .ToListAsync();

            var kontoRows = await _context.StammKontos
                .Where(k => k.StammGuid == id && k.Datum >= windowStart)
                .Select(k => new { k.Datum, k.Betrag })
                .ToListAsync();

            for (int i = 0; i < monthCount; i++)
            {
                var monthStart = windowStart.AddMonths(i);
                var monthEnd = monthStart.AddMonths(1);

                PostItCounts.Add(postItDates.Count(d => d >= monthStart && d < monthEnd));
                TopLabCounts.Add(topLabDates.Count(d => d >= monthStart && d < monthEnd));
                CreditDeltas.Add(kontoRows
                    .Where(k => k.Datum >= monthStart && k.Datum < monthEnd)
                    .Sum(k => k.Betrag));
            }

            // Summary totals (all-time)
            TotalPostIts = await _context.Wurzelns.CountAsync(w => w.StammGuid == id);
            TotalTopLabs = await _context.TopLabs.CountAsync(t => t.StammGuid == id);
            TotalAnglers = await _context.Anglers.CountAsync(a => a.StammGuid == id);

            var allBalances = await _context.StammKontos
                .Where(k => k.StammGuid == id)
                .Select(k => k.Betrag)
                .ToListAsync();

            TotalCreditIn = allBalances.Where(b => b > 0).Sum();
            TotalCreditOut = allBalances.Where(b => b < 0).Sum();
            CurrentBalance = allBalances.Sum();

            ViewData["Sidebar"] = "_SidebarUnified";

            return Page();
        }
    }
}
