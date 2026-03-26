using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class PostItTopLab
{
    public Guid? StammGuid { get; set; }

    public string? Stamm { get; set; }

    public string? Sdatei { get; set; }

    public Guid? PostItGuid { get; set; }

    public string? PostIt { get; set; }

    public string? Pdatei { get; set; }

    public int? MittelToll { get; set; }

    public int? DurchToll { get; set; }

    public Guid TopLabGuid { get; set; }

    public string? Tdatei { get; set; }

    public decimal Lohn { get; set; }

    public DateTime Datum { get; set; }

    public string TopLab { get; set; } = null!;

    public string? Turl { get; set; }

    public string? Purl { get; set; }

    public Guid? TopTopLabGuid { get; set; }

    public string? Titel { get; set; }
}
