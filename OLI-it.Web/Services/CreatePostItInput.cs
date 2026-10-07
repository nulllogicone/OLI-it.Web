using System.ComponentModel.DataAnnotations;
using System.Net;

namespace OLI_it.Web.Services;

public sealed class CreatePostItInput : IValidatableObject
{
    [Required, StringLength(255)]
    public string Title { get; set; } = "";

    [Required, StringLength(3000)]
    public string Body { get; set; } = "";

    [StringLength(255)]
    public string? Url { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (WebUtility.HtmlEncode(Title.Trim()).Length > 255)
            yield return new("The title is too long after encoding for the shared database.", [nameof(Title)]);
        if (WebUtility.HtmlEncode(Body).Length > 3000)
            yield return new("The message is too long after encoding for the shared database.", [nameof(Body)]);
        if (!string.IsNullOrWhiteSpace(Url) &&
            (!Uri.TryCreate(Url.Trim(), UriKind.Absolute, out var uri) ||
             (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)))
            yield return new("Enter an absolute HTTP or HTTPS URL.", [nameof(Url)]);
    }
}

public static class PostItText
{
    // The legacy UI stores HTML-encoded plain text in a varchar column.
    // Return a string (never HtmlString); Razor still escapes the decoded content.
    public static string Title(string? value) => WebUtility.HtmlDecode(value ?? "");
    public static string Body(string value, string type) =>
        type.Trim() == "txt" ? WebUtility.HtmlDecode(value) : value;
}
