using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class Inbox
{
    public Guid InboxGuid { get; set; }

    public Guid? StammGuid { get; set; }

    public Guid? TopLabGuid { get; set; }

    public DateTime? Gelesen { get; set; }

    public DateTime? Datum { get; set; }

    public DateTime? Gesehen { get; set; }

    public DateTime? Gemailt { get; set; }

    public virtual Stamm? Stamm { get; set; }

    public virtual TopLab? TopLab { get; set; }
}
