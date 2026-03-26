using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class StammAngler
{
    public Guid StammGuid { get; set; }

    public Guid AnglerGuid { get; set; }

    public string Angler { get; set; } = null!;

    public int? AnzP { get; set; }

    public decimal? SumT { get; set; }

    public string? Beschreibung { get; set; }

    public DateTime Datum { get; set; }
}
