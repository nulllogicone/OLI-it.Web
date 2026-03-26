using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class PostItAngler
{
    public Guid StammGuid { get; set; }

    public string Stamm { get; set; } = null!;

    public Guid AnglerGuid { get; set; }

    public string Angler { get; set; } = null!;

    public string? Sdatei { get; set; }

    public string PostIt { get; set; } = null!;

    public string? Pdatei { get; set; }

    public Guid PostItGuid { get; set; }

    public string? Titel { get; set; }

    public string? Beschreibung { get; set; }
}
