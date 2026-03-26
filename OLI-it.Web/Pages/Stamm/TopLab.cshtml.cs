using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;

namespace OLI_it.Web.Pages.Stamm
{
    public class TopLabModel : PageModel
    {
        private readonly OliItDbContext _context;

        public TopLabModel(OliItDbContext context)
        {
            _context = context;
        }

        public Models.Stamm? Stamm { get; set; }
        public List<Models.TopLab> TopLabs { get; set; } = new();

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

            TopLabs = await _context.TopLabs
                .Include(t => t.PostIt)
                .Where(t => t.StammGuid == id)
                .OrderByDescending(t => t.Datum)
                .ToListAsync();

            ViewData["Sidebar"] = "_SidebarStamm";
            
            return Page();
        }
    }
}
