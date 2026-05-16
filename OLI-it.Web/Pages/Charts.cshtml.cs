using Microsoft.AspNetCore.Mvc.RazorPages;
using OLI_it.Web.Services;

namespace OLI_it.Web.Pages;

public class ChartsModel(ChartService chartService) : PageModel
{
    public ChartData Data { get; private set; } = null!;

    public async Task OnGetAsync()
    {
        Data = await chartService.GetChartDataAsync();
    }
}
