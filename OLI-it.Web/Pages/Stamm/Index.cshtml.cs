using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;

namespace OLI_it.Web.Pages.Stamm
{
    public class IndexModel : PageModel
    {
        private readonly OliItDbContext _context;

        public IndexModel(OliItDbContext context)
        {
            _context = context;
        }

        public Models.Stamm? Stamm { get; set; }
        public Models.PostIt? PostIt { get; set; }
        public Models.Angler? Angler { get; set; }
        public Models.TopLab? TopLab { get; set; }
        public List<Wurzeln>? StammPostIts { get; set; }
        public List<Models.Angler>? StammAnglers { get; set; }
        public List<Models.TopLab>? StammTopLabs { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Stamm = await _context.Stamms
                .FirstOrDefaultAsync(m => m.StammGuid == id);

            if (Stamm == null)
            {
                return NotFound();
            }

            // Load Stamm's PostIts
            StammPostIts = await _context.Wurzelns
                .Include(w => w.PostIt)
                    .ThenInclude(p => p.TopLabs)
                .Where(w => w.StammGuid == id.Value)
                .OrderByDescending(w => w.PostIt.Datum)
                .Take(20)
                .ToListAsync();

            // Load Stamm's Anglers
            StammAnglers = await _context.Anglers
                .Where(a => a.StammGuid == id.Value)
                .OrderByDescending(a => a.Datum)
                .Take(20)
                .ToListAsync();

            // Load counts for Anglers (News = matched PostIts)
            foreach (var anglerItem in StammAnglers)
            {
                anglerItem.News = await _context.News
                    .Where(n => n.AnglerGuid == anglerItem.AnglerGuid)
                    .ToListAsync();
            }

            // Load Stamm's TopLabs
            StammTopLabs = await _context.TopLabs
                .Include(t => t.PostIt)
                .Where(t => t.StammGuid == id.Value)
                .OrderByDescending(t => t.Datum)
                .Take(20)
                .ToListAsync();

            ViewData["Sidebar"] = "_SidebarUnified";

            return Page();
        }
    }
}
