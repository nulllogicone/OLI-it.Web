using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class PostItStamm
{
    public Guid PostItGuid { get; set; }

    public decimal Bezahlt { get; set; }

    public DateTime? Frist { get; set; }

    public Guid StammGuid { get; set; }

    public string Stamm { get; set; } = null!;

    public decimal? KooK { get; set; }

    public DateTime Datum { get; set; }

    public string? Datei { get; set; }

    public int StammZust { get; set; }

    public bool Closed { get; set; }
}
