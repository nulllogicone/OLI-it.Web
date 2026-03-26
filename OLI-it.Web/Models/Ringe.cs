using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class Ringe
{
    public Guid RingGuid { get; set; }

    public Guid CodeGuid { get; set; }

    public Guid NetzGuid { get; set; }

    public Guid KnotenGuid { get; set; }

    public Guid? BaumGuid { get; set; }

    public Guid? ZweigGuid { get; set; }

    public int Olis { get; set; }

    public int Get { get; set; }

    public virtual Code Code { get; set; } = null!;
}
