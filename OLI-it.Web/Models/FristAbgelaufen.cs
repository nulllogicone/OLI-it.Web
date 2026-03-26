using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class FristAbgelaufen
{
    public Guid StammGuid { get; set; }

    public string Stamm { get; set; } = null!;

    public string EMail { get; set; } = null!;

    public Guid PostItGuid { get; set; }

    public string? Titel { get; set; }

    public string PostIt { get; set; } = null!;

    public decimal Bezahlt { get; set; }

    public DateTime? Frist { get; set; }

    public bool Gemailt { get; set; }

    public bool Closed { get; set; }

    public int? AnzT { get; set; }
}
