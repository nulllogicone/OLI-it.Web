using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class Angler
{
    public Guid AnglerGuid { get; set; }

    public Guid StammGuid { get; set; }

    public string Angler1 { get; set; } = null!;

    public string? Versionsnummer { get; set; }

    public bool Gescannt { get; set; }

    public DateTime Datum { get; set; }

    public string? Beschreibung { get; set; }

    public virtual ICollection<Löcher> Löchers { get; set; } = new List<Löcher>();

    public virtual ICollection<News> News { get; set; } = new List<News>();

    public virtual ICollection<Spiegel> Spiegels { get; set; } = new List<Spiegel>();

    public virtual Stamm Stamm { get; set; } = null!;
}
