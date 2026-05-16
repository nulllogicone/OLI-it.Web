using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OLI_it.Web.Services;

namespace OLI_it.Web.Pages;

public class SearchModel(SearchService searchService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Term { get; set; }

    public SearchResults? Results { get; private set; }

    public async Task OnGetAsync()
    {
        if (!string.IsNullOrWhiteSpace(Term))
        {
            Results = await searchService.SearchAsync(Term.Trim());
        }
    }
}
