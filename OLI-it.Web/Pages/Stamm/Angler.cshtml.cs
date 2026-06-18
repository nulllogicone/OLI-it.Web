using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;
using OLI_it.Web.Services;

namespace OLI_it.Web.Pages.Stamm
{
    public class AnglerModel : PageModel
    {
        private readonly OliItDbContext _context;
        private readonly AnglerCatchCountService _anglerCatchCountService;

        public AnglerModel(OliItDbContext context, AnglerCatchCountService anglerCatchCountService)
        {
            _context = context;
            _anglerCatchCountService = anglerCatchCountService;
        }

        public Models.Stamm? Stamm { get; set; }
        public Models.PostIt? PostIt { get; set; }
        public Models.Angler? Angler { get; set; }
        public Models.TopLab? TopLab { get; set; }
        public List<Models.Angler> Anglers { get; set; } = new();

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

            Anglers = await _context.Anglers
                .Where(a => a.StammGuid == id)
                .OrderByDescending(a => a.Datum)
                .ToListAsync();

            // Load catch counts for Anglers via Spiegel -> Code (distinct PostIts)
            var anglerGuids = Anglers.Select(a => a.AnglerGuid).ToList();
            var anglerCatchCounts = await _anglerCatchCountService.GetCatchCountsByAnglerGuidsAsync(anglerGuids);

            ViewData["AnglerCatchCounts"] = anglerCatchCounts;

            ViewData["Sidebar"] = "_SidebarUnified";
            
            return Page();
        }
    }
}
