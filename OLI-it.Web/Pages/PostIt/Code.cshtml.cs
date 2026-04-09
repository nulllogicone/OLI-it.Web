using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;

namespace OLI_it.Web.Pages.PostIt
{
    public class CodeModel : PageModel
    {
        private readonly OliItDbContext _context;

        public CodeModel(OliItDbContext context)
        {
            _context = context;
        }

        public Models.Stamm? Stamm { get; set; }
        public Models.PostIt? PostIt { get; set; }
        public Models.Angler? Angler { get; set; }
        public Models.TopLab? TopLab { get; set; }
        public List<Models.Code> Codes { get; set; } = new();

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

            // Load author Stamm (StammZust = 1)
            var authorWurzel = await _context.Wurzelns
                .Include(w => w.Stamm)
                .FirstOrDefaultAsync(w => w.PostItGuid == id.Value && w.StammZust == 1);

            Stamm = authorWurzel?.Stamm;

            Codes = await _context.Codes
                .Where(c => c.PostItGuid == id)
                .OrderByDescending(c => c.Gescannt)
                .ToListAsync();

            ViewData["Sidebar"] = "_SidebarUnified";

            return Page();
        }
    }
}
