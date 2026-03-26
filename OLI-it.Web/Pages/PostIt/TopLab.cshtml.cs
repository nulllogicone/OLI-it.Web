using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;

namespace OLI_it.Web.Pages.PostIt
{
    public class TopLabModel : PageModel
    {
        private readonly OliItDbContext _context;

        public TopLabModel(OliItDbContext context)
        {
            _context = context;
        }

        public Models.PostIt? PostIt { get; set; }
        public List<Models.TopLab> TopLabs { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PostIt = await _context.PostIts
                .FirstOrDefaultAsync(m => m.PostItGuid == id);

            if (PostIt == null)
            {
                return NotFound();
            }

            TopLabs = await _context.TopLabs
                .Include(t => t.Stamm)
                .Where(t => t.PostItGuid == id)
                .OrderByDescending(t => t.Datum)
                .ToListAsync();

            ViewData["Sidebar"] = "_SidebarPostIt";
            
            return Page();
        }
    }
}
