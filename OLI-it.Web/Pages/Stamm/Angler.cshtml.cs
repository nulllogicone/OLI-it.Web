using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;

namespace OLI_it.Web.Pages.Stamm
{
    public class AnglerModel : PageModel
    {
        private readonly OliItDbContext _context;

        public AnglerModel(OliItDbContext context)
        {
            _context = context;
        }

        public Models.Stamm? Stamm { get; set; }
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

            ViewData["Sidebar"] = "_SidebarStamm";
            
            return Page();
        }
    }
}
