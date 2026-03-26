using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;

namespace OLI_it.Web.Pages.PostIt
{
    public class IndexModel : PageModel
    {
        private readonly OliItDbContext _context;

        public IndexModel(OliItDbContext context)
        {
            _context = context;
        }

        public Models.PostIt? PostIt { get; set; }

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

            ViewData["Sidebar"] = "_SidebarPostIt";
            
            return Page();
        }
    }
}
