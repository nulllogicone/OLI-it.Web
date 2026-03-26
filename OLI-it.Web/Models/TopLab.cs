using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class TopLab
{
    public Guid TopLabGuid { get; set; }

    public Guid StammGuid { get; set; }

    public Guid PostItGuid { get; set; }

    public string? Titel { get; set; }

    public string TopLab1 { get; set; } = null!;

    public string? Url { get; set; }

    public decimal Lohn { get; set; }

    public DateTime Datum { get; set; }

    public string? Datei { get; set; }

    public Guid? TopTopLabGuid { get; set; }

    public string Typ { get; set; } = null!;

    public virtual ICollection<Inbox> Inboxes { get; set; } = new List<Inbox>();

    public virtual ICollection<TopLab> InverseTopTopLab { get; set; } = new List<TopLab>();

    public virtual PostIt PostIt { get; set; } = null!;

    public virtual Stamm Stamm { get; set; } = null!;

    public virtual ICollection<Tolli> Tollis { get; set; } = new List<Tolli>();

    public virtual TopLab? TopTopLab { get; set; }
}
