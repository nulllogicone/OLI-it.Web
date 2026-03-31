using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;

namespace OLI_it.Web.Pages.TopLab
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
        public Models.TopLab? TopLab { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TopLab = await _context.TopLabs
                .Include(t => t.PostIt)
                .Include(t => t.Stamm)
                .Include(t => t.TopTopLab)
                .FirstOrDefaultAsync(m => m.TopLabGuid == id);

            if (TopLab == null)
            {
                return NotFound();
            }

            // Load parent PostIt (question)
            PostIt = TopLab.PostIt;

            // Load parent Stamm (author)
            Stamm = TopLab.Stamm;

            return Page();
        }
    }
}
