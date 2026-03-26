using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class StammTopLab
{
    public Guid PostItGuid { get; set; }

    public string PostIt { get; set; } = null!;

    public DateTime Pdatum { get; set; }

    public decimal KooK { get; set; }

    public string? Pdatei { get; set; }

    public Guid? TopLabGuid { get; set; }

    public string? TopLab { get; set; }

    public Guid? StammGuid { get; set; }

    public decimal? Lohn { get; set; }

    public DateTime? Tdatum { get; set; }

    public string? Tdatei { get; set; }

    public int? DurchToll { get; set; }

    public string? Purl { get; set; }

    public string? Turl { get; set; }

    public string? Ptitel { get; set; }

    public string? Ttitel { get; set; }
}
