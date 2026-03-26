using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;

namespace OLI_it.Web.Pages
{
    public class BrowseModel : PageModel
    {
        private readonly OliItDbContext _context;

        public BrowseModel(OliItDbContext context)
        {
            _context = context;
        }

        // Stack of entities
        public Models.Stamm? Stamm { get; set; }
        public Models.PostIt? PostIt { get; set; }
        public Models.Angler? Angler { get; set; }
        public Models.TopLab? TopLab { get; set; }
        public Models.Code? Code { get; set; }

        // Child lists for the current context
        public List<Wurzeln>? StammPostIts { get; set; }
        public List<Models.Angler>? StammAnglers { get; set; }
        public List<Models.TopLab>? StammTopLabs { get; set; }
        public List<Models.Code>? PostItCodes { get; set; }
        public List<Models.TopLab>? PostItTopLabs { get; set; }

        public async Task<IActionResult> OnGetAsync(
            Guid? stamm, 
            Guid? postit, 
            Guid? angler, 
            Guid? toplab, 
            Guid? code)
        {
            // Load Stamm if provided
            if (stamm.HasValue)
            {
                Stamm = await _context.Stamms
                    .FirstOrDefaultAsync(s => s.StammGuid == stamm.Value);

                if (Stamm == null)
                    return NotFound();

                // Load Stamm's children
                StammPostIts = await _context.Wurzelns
                    .Include(w => w.PostIt)
                        .ThenInclude(p => p.TopLabs)
                    .Where(w => w.StammGuid == stamm.Value)
                    .OrderByDescending(w => w.PostIt.Datum)
                    .Take(20)
                    .ToListAsync();

                StammAnglers = await _context.Anglers
                    .Where(a => a.StammGuid == stamm.Value)
                    .OrderByDescending(a => a.Datum)
                    .Take(20)
                    .ToListAsync();

                // Load counts for Anglers (News = matched PostIts)
                foreach (var anglerItem in StammAnglers)
                {
                    anglerItem.News = await _context.News
                        .Where(n => n.AnglerGuid == anglerItem.AnglerGuid)
                        .ToListAsync();
                }

                StammTopLabs = await _context.TopLabs
                    .Include(t => t.PostIt)
                    .Where(t => t.StammGuid == stamm.Value)
                    .OrderByDescending(t => t.Datum)
                    .Take(20)
                    .ToListAsync();
            }

            // Load PostIt if provided
            if (postit.HasValue)
            {
                PostIt = await _context.PostIts
                    .FirstOrDefaultAsync(p => p.PostItGuid == postit.Value);

                if (PostIt == null)
                    return NotFound();

                // Load PostIt's children
                PostItCodes = await _context.Codes
                    .Where(c => c.PostItGuid == postit.Value)
                    .OrderByDescending(c => c.Gescannt)
                    .Take(20)
                    .ToListAsync();

                // Load counts for Codes (Spiegel = receivers)
                foreach (var codeItem in PostItCodes)
                {
                    codeItem.Spiegels = await _context.Spiegels
                        .Where(s => s.CodeGuid == codeItem.CodeGuid)
                        .ToListAsync();
                }

                PostItTopLabs = await _context.TopLabs
                    .Include(t => t.Stamm)
                    .Where(t => t.PostItGuid == postit.Value)
                    .OrderByDescending(t => t.Datum)
                    .Take(20)
                    .ToListAsync();
            }

            // Load Angler if provided
            if (angler.HasValue)
            {
                Angler = await _context.Anglers
                    .Include(a => a.Stamm)
                    .FirstOrDefaultAsync(a => a.AnglerGuid == angler.Value);

                if (Angler == null)
                    return NotFound();
            }

            // Load TopLab if provided
            if (toplab.HasValue)
            {
                TopLab = await _context.TopLabs
                    .Include(t => t.PostIt)
                    .Include(t => t.Stamm)
                    .FirstOrDefaultAsync(t => t.TopLabGuid == toplab.Value);

                if (TopLab == null)
                    return NotFound();
            }

            // Load Code if provided
            if (code.HasValue)
            {
                Code = await _context.Codes
                    .Include(c => c.PostIt)
                    .FirstOrDefaultAsync(c => c.CodeGuid == code.Value);

                if (Code == null)
                    return NotFound();
            }

            ViewData["Sidebar"] = "_SidebarBrowse";
            
            return Page();
        }
    }
}
