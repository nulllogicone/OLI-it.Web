using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class Tolli
{
    public Guid StammGuid { get; set; }

    public Guid TopLabGuid { get; set; }

    public short? Toll { get; set; }

    public string? TollText { get; set; }

    public DateTime? Datum { get; set; }

    public virtual Stamm Stamm { get; set; } = null!;

    public virtual TopLab TopLab { get; set; } = null!;
}
