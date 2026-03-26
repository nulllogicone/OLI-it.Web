using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class Netz
{
    public Guid NetzGuid { get; set; }

    public string Netz1 { get; set; } = null!;

    public string? Beschreibung { get; set; }

    public string? Datei { get; set; }

    public DateTime Datum { get; set; }

    public string? Owner { get; set; }

    public string? State { get; set; }

    public string? EnName { get; set; }

    public string? EnDescription { get; set; }

    public bool? Rdf { get; set; }

    public virtual ICollection<Knoten> Knotens { get; set; } = new List<Knoten>();
}
