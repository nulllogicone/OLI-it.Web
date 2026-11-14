using System.ComponentModel.DataAnnotations;
using System.Net;

namespace OLI_it.Web.Pages.TopLab;

public sealed class AnswerInput : IValidatableObject
{
    [StringLength(255)]
    public string? Title { get; set; }

    [Required, StringLength(3000)]
    public string Body { get; set; } = "";

    internal string? EncodedTitle => string.IsNullOrWhiteSpace(Title) ? null : WebUtility.HtmlEncode(Title.Trim());
    internal string EncodedBody => WebUtility.HtmlEncode(Body);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EncodedTitle?.Length > 255)
            yield return new("The title is too long after encoding for the shared database.", [nameof(Title)]);
        if (EncodedBody.Length > 3000)
            yield return new("The answer is too long after encoding for the shared database.", [nameof(Body)]);
    }
}
