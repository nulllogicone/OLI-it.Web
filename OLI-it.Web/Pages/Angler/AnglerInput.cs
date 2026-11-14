using System.ComponentModel.DataAnnotations;
using System.Net;

namespace OLI_it.Web.Pages.Angler;

public sealed class AnglerInput : IValidatableObject
{
    [Required, StringLength(50)]
    public string Title { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Description { get; set; }

    internal string EncodedTitle => WebUtility.HtmlEncode(Title.Trim());
    internal string? EncodedDescription => WebUtility.HtmlEncode(Description);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EncodedTitle.Length > 50)
            yield return new ValidationResult("Shorten the title so it fits after encoding.", [nameof(Title)]);
        if (EncodedDescription?.Length > 255)
            yield return new ValidationResult("Shorten the description so it fits after encoding.", [nameof(Description)]);
    }
}
