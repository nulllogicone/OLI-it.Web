using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class Stamm
{
    public Guid StammGuid { get; set; }

    public string Stamm1 { get; set; } = null!;

    public DateTime? GebDate { get; set; }

    public string? Tel { get; set; }

    public string EMail { get; set; } = null!;

    public decimal? KooK { get; set; }

    public DateTime Datum { get; set; }

    public string? Unterschrift { get; set; }

    public string? Versionsnummer { get; set; }

    public string? Bank { get; set; }

    public string? Blz { get; set; }

    public string? Kto { get; set; }

    public string? Datei { get; set; }

    public string? Beschreibung { get; set; }

    public string? Link { get; set; }

    public int ZuQid { get; set; }

    public virtual ICollection<Angler> Anglers { get; set; } = new List<Angler>();

    public virtual ICollection<Extra> Extras { get; set; } = new List<Extra>();

    public virtual ICollection<Inbox> Inboxes { get; set; } = new List<Inbox>();

    public virtual ICollection<Provision> Provisions { get; set; } = new List<Provision>();

    public virtual ICollection<ShortCut> ShortCuts { get; set; } = new List<ShortCut>();

    public virtual ICollection<Tolli> Tollis { get; set; } = new List<Tolli>();

    public virtual ICollection<TopLab> TopLabs { get; set; } = new List<TopLab>();

    public virtual ICollection<Wurzeln> Wurzelns { get; set; } = new List<Wurzeln>();
}
