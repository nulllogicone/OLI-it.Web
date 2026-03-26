using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OLI_it.Web.Data;
using OLI_it.Web.Models;

namespace OLI_it.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly OliItDbContext _context;

        public IndexModel(OliItDbContext context)
        {
            _context = context;
        }

        public List<Models.Stamm> StammList { get; set; } = new();

        public async Task OnGetAsync()
        {
            StammList = await _context.Stamms
                .OrderBy(s=>s.Datum)
                .Take(10)
                .ToListAsync();
        }
    }
}
