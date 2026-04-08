using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;

namespace OLI_it.Web.Pages.Code
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
        public Models.Code? Code { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Code = await _context.Codes
                .Include(c => c.PostIt)
                .FirstOrDefaultAsync(m => m.CodeGuid == id);

            if (Code == null)
            {
                return NotFound();
            }

            // Load parent PostIt
            PostIt = Code.PostIt;

            // Load parent Stamm (author of the PostIt)
            if (PostIt != null)
            {
                var authorWurzel = await _context.Wurzelns
                    .Include(w => w.Stamm)
                    .FirstOrDefaultAsync(w => w.PostItGuid == PostIt.PostItGuid && w.StammZust == 1);

                Stamm = authorWurzel?.Stamm;
            }

            ViewData["Sidebar"] = "_SidebarUnified";

            return Page();
        }
    }
}
