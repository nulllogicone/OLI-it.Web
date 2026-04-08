using Microsoft.AspNetCore.Mvc;
using OLI_it.Web.Services;

namespace OLI_it.Web.ViewComponents;

public class ImageGalleryViewComponent : ViewComponent
{
    private readonly AzureBlobStorageService _blobService;
    private readonly IConfiguration _configuration;

    public ImageGalleryViewComponent(AzureBlobStorageService blobService, IConfiguration configuration)
    {
        _blobService = blobService;
        _configuration = configuration;
    }

    public async Task<IViewComponentResult> InvokeAsync(Guid stammGuid, string? currentImage)
    {
        var images = await _blobService.ListStammImagesAsync(stammGuid);
        var imagesRootUrl = _configuration["ImagesRootUrl"] ?? "https://oliit.blob.core.windows.net/oliupload";

        // Create view model with both relative paths and full URLs
        var imageItems = images.Select(img => new ImageGalleryItem
        {
            RelativePath = $"/{img}",  // Store relative path: /{guid}/name.ext
            FullUrl = $"{imagesRootUrl}/{img}"  // Display full URL
        }).ToList();

        var model = new ImageGalleryViewModel
        {
            StammGuid = stammGuid,
            Images = imageItems,
            CurrentImage = currentImage
        };

        return View(model);
    }
}

public class ImageGalleryViewModel
{
    public Guid StammGuid { get; set; }
    public List<ImageGalleryItem> Images { get; set; } = new();
    public string? CurrentImage { get; set; }
}

public class ImageGalleryItem
{
    public string RelativePath { get; set; } = string.Empty;
    public string FullUrl { get; set; } = string.Empty;
}
