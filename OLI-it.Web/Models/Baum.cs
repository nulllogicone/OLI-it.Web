using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class Baum
{
    public Guid BaumGuid { get; set; }

    public string Baum1 { get; set; } = null!;

    public string? Beschreibung { get; set; }

    public string? Datei { get; set; }

    public DateTime Datum { get; set; }

    public string? Owner { get; set; }

    public string? State { get; set; }

    public string? EnName { get; set; }

    public string? EnDescription { get; set; }

    public virtual ICollection<Zweig> Zweigs { get; set; } = new List<Zweig>();
}
