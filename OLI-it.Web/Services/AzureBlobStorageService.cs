using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace OLI_it.Web.Services;

public class AzureBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    private readonly ILogger<AzureBlobStorageService> _logger;

    public AzureBlobStorageService(
        IConfiguration configuration,
        ILogger<AzureBlobStorageService> logger)
    {
        var connectionString = configuration.GetConnectionString("OliItStorageConnectionString");
        _containerName = configuration["AzureStorage:ContainerName"] ?? "oliupload";
        _blobServiceClient = new BlobServiceClient(connectionString);
        _logger = logger;
    }

    /// <summary>
    /// Lists all images for a specific Stamm (from their folder)
    /// </summary>
    public async Task<List<string>> ListStammImagesAsync(Guid stammGuid)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var stammFolder = $"{stammGuid}/";
            var images = new List<string>();

            await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: stammFolder))
            {
                // Only include image files
                if (IsImageFile(blobItem.Name))
                {
                    images.Add(blobItem.Name);
                }
            }

            _logger.LogInformation("Listed {Count} images for Stamm {StammGuid}", images.Count, stammGuid);
            return images;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing images for Stamm {StammGuid}", stammGuid);
            return new List<string>();
        }
    }

    /// <summary>
    /// Uploads an image to the Stamm's folder
    /// </summary>
    /// <returns>The blob name if successful, null if file already exists or error occurs</returns>
    public async Task<(bool Success, string? BlobName, string? ErrorMessage)> UploadImageAsync(Guid stammGuid, IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return (false, null, "No file was selected.");
            }

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Sanitize and keep original filename
            var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
            var fileExtension = Path.GetExtension(file.FileName);
            var sanitizedFileName = SanitizeBlobName(originalFileName);

            var uniqueFileName = $"{stammGuid}/{sanitizedFileName}{fileExtension}";
            var blobClient = containerClient.GetBlobClient(uniqueFileName);

            // Check if blob already exists
            if (await blobClient.ExistsAsync())
            {
                var message = $"A file named '{sanitizedFileName}{fileExtension}' already exists. Please rename your file or delete the existing one first.";
                _logger.LogWarning("Upload failed: File {FileName} already exists for Stamm {StammGuid}", uniqueFileName, stammGuid);
                return (false, null, message);
            }

            // Set content type based on file extension
            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = GetContentType(fileExtension)
            };

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });
            }

            _logger.LogInformation("Uploaded image {FileName} for Stamm {StammGuid}", uniqueFileName, stammGuid);
            return (true, uniqueFileName, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image for Stamm {StammGuid}", stammGuid);
            return (false, null, "An error occurred while uploading the file. Please try again.");
        }
    }

    /// <summary>
    /// Deletes an image from the Stamm's folder
    /// </summary>
    public async Task<bool> DeleteImageAsync(Guid stammGuid, string blobName)
    {
        try
        {
            // Ensure the blob name belongs to this Stamm
            if (!blobName.StartsWith($"{stammGuid}/"))
            {
                _logger.LogWarning("Unauthorized delete attempt: Stamm {StammGuid} tried to delete {BlobName}", stammGuid, blobName);
                return false;
            }

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            var result = await blobClient.DeleteIfExistsAsync();

            _logger.LogInformation("Deleted image {BlobName} for Stamm {StammGuid}", blobName, stammGuid);
            return result.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image {BlobName} for Stamm {StammGuid}", blobName, stammGuid);
            return false;
        }
    }

    /// <summary>
    /// Gets the public URL for a blob
    /// </summary>
    public string GetBlobUrl(string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        return blobClient.Uri.ToString();
    }

    private bool IsImageFile(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension is ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" or ".bmp" or ".svg";
    }

    private string GetContentType(string fileExtension)
    {
        return fileExtension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream"
        };
    }

    /// <summary>
    /// Sanitizes a filename to be blob storage compatible
    /// </summary>
    private string SanitizeBlobName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return "image";
        }

        // Replace spaces with hyphens
        var sanitized = fileName.Replace(" ", "-");

        // Remove invalid characters (keep only letters, numbers, dots, hyphens, underscores)
        sanitized = new string(sanitized
            .Where(c => char.IsLetterOrDigit(c) || c == '.' || c == '-' || c == '_')
            .ToArray());

        // Remove leading/trailing dots and hyphens
        sanitized = sanitized.Trim('.', '-', '_');

        // Ensure it's not empty after sanitization
        if (string.IsNullOrWhiteSpace(sanitized))
        {
            sanitized = "image";
        }

        // Limit length to avoid issues
        if (sanitized.Length > 100)
        {
            sanitized = sanitized.Substring(0, 100);
        }

        return sanitized;
    }
}
