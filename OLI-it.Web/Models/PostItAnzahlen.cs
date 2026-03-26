using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class PostItAnzahlen
{
    public int? AnzS { get; set; }

    public int? AnzA { get; set; }

    public int? AnzT { get; set; }

    public Guid PostItGuid { get; set; }

    public string PostIt { get; set; } = null!;

    public DateTime Datum { get; set; }

    public decimal KooK { get; set; }

    public int? PostItZust { get; set; }

    public string? Datei { get; set; }

    public bool? Closed { get; set; }

    public string? Titel { get; set; }

    public string? Url { get; set; }

    public string Typ { get; set; } = null!;
}
