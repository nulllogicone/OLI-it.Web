using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class Code
{
    public Guid CodeGuid { get; set; }

    public Guid PostItGuid { get; set; }

    public Guid StammGuid { get; set; }

    public int? CodeZust { get; set; }

    public string Kommentar { get; set; } = null!;

    public bool Gescannt { get; set; }

    public string? Versionsnummer { get; set; }

    public virtual ICollection<News> News { get; set; } = new List<News>();

    public virtual PostIt PostIt { get; set; } = null!;

    public virtual ICollection<Ringe> Ringes { get; set; } = new List<Ringe>();

    public virtual ICollection<Spiegel> Spiegels { get; set; } = new List<Spiegel>();
}
