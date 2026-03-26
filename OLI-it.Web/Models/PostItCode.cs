using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class PostItCode
{
    public Guid CodeGuid { get; set; }

    public Guid StammGuid { get; set; }

    public Guid PostItGuid { get; set; }

    public int? CodeZust { get; set; }

    public string Kommentar { get; set; } = null!;

    public bool Gescannt { get; set; }

    public string? Versionsnummer { get; set; }

    public int? AnzA { get; set; }
}
