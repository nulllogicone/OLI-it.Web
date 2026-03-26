using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class StammNews
{
    public Guid AnglerGuid { get; set; }

    public string Angler { get; set; } = null!;

    public Guid StammGuid { get; set; }

    public Guid NewsGuid { get; set; }

    public Guid? CodeGuid { get; set; }

    public DateTime? Datum { get; set; }

    public DateTime? Gelesen { get; set; }

    public Guid PostItGuid { get; set; }

    public string PostIt { get; set; } = null!;

    public DateTime Pdatum { get; set; }

    public string? Datei { get; set; }

    public int? AnzT { get; set; }

    public DateTime? Gesehen { get; set; }

    public bool? Closed { get; set; }

    public string? Titel { get; set; }

    public decimal KooK { get; set; }
}
