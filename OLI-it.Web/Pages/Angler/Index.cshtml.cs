using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;

namespace OLI_it.Web.Pages.Angler
{
    public class IndexModel : PageModel
    {
        private readonly OliItDbContext _context;

        public IndexModel(OliItDbContext context)
        {
            _context = context;
        }

        public Models.Angler? Angler { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Angler = await _context.Anglers
                .Include(a => a.Stamm)
                .FirstOrDefaultAsync(m => m.AnglerGuid == id);

            if (Angler == null)
            {
                return NotFound();
            }
            
            return Page();
        }
    }
}
