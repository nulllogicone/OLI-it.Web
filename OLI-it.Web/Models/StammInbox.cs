using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class StammInbox
{
    public Guid InboxGuid { get; set; }

    public Guid? StammGuid { get; set; }

    public DateTime? Gelesen { get; set; }

    public DateTime? Datum { get; set; }

    public Guid PostItGuid { get; set; }

    public string PostIt { get; set; } = null!;

    public Guid TopLabGuid { get; set; }

    public string TopLab { get; set; } = null!;

    public int? DurchToll { get; set; }

    public string? Tdatei { get; set; }

    public DateTime Tdatum { get; set; }

    public DateTime? Gesehen { get; set; }

    public bool? Closed { get; set; }

    public string? Ttitel { get; set; }

    public string? Ptitel { get; set; }

    public string? Pdatei { get; set; }
}
