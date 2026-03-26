using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class PostIt
{
    public Guid PostItGuid { get; set; }

    public string? Titel { get; set; }

    public string PostIt1 { get; set; } = null!;

    public DateTime Datum { get; set; }

    public decimal KooK { get; set; }

    public int? PostItZust { get; set; }

    public string? Url { get; set; }

    public string? Datei { get; set; }

    public int Hits { get; set; }

    public string Typ { get; set; } = null!;

    public virtual ICollection<Code> Codes { get; set; } = new List<Code>();

    public virtual ICollection<Provision> Provisions { get; set; } = new List<Provision>();

    public virtual ICollection<TopLab> TopLabs { get; set; } = new List<TopLab>();

    public virtual ICollection<Wurzeln> Wurzelns { get; set; } = new List<Wurzeln>();
}
