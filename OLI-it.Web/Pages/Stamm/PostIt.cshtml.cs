using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;

namespace OLI_it.Web.Pages.Stamm
{
    public class PostItModel : PageModel
    {
        private readonly OliItDbContext _context;

        public PostItModel(OliItDbContext context)
        {
            _context = context;
        }

        public Models.Stamm? Stamm { get; set; }
        public List<Wurzeln> PostIts { get; set; } = new();

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

            PostIts = await _context.Wurzelns
                .Include(w => w.PostIt)
                .Where(w => w.StammGuid == id)
                .OrderByDescending(w => w.PostIt.Datum)
                .ToListAsync();

            ViewData["Sidebar"] = "_SidebarStamm";
            
            return Page();
        }
    }
}
