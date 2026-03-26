using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class Wurzeln
{
    public Guid StammGuid { get; set; }

    public Guid PostItGuid { get; set; }

    public int StammZust { get; set; }

    public decimal Bezahlt { get; set; }

    public DateTime? Frist { get; set; }

    public bool Gemailt { get; set; }

    public bool Closed { get; set; }

    public virtual PostIt PostIt { get; set; } = null!;

    public virtual Stamm Stamm { get; set; } = null!;
}
