using Microsoft.AspNetCore.Mvc;

namespace OLI_it.Web.ViewComponents;

public class ImageThumbnailViewComponent : ViewComponent
{
    private readonly IConfiguration _configuration;

    public ImageThumbnailViewComponent(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IViewComponentResult Invoke(string? dateiPath, string altText = "Image", int width = 150, int height = 150)
    {
        if (string.IsNullOrEmpty(dateiPath))
        {
            return Content(string.Empty);
        }

        string fullImageUrl;

        // Check if it's already a full URL (starts with http:// or https://)
        if (dateiPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
            dateiPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            // Use the URL as-is
            fullImageUrl = dateiPath;
        }
        else
        {
            // It's a relative path, prefix with ImagesRootUrl
            var imagesRootUrl = _configuration["ImagesRootUrl"] ?? string.Empty;
            var normalizedPath = dateiPath.StartsWith("/") ? dateiPath : "/" + dateiPath;
            fullImageUrl = imagesRootUrl.TrimEnd('/') + normalizedPath;
        }

        var model = new ImageThumbnailViewModel
        {
            ImageUrl = fullImageUrl,
            AltText = altText,
            Width = width,
            Height = height,
            OriginalPath = dateiPath
        };

        return View(model);
    }
}

public class ImageThumbnailViewModel
{
    public string ImageUrl { get; set; } = string.Empty;
    public string AltText { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public string OriginalPath { get; set; } = string.Empty;
}

