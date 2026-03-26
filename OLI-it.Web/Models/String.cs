using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class String
{
    public Guid StringsGuid { get; set; }

    public Guid? ShortCutsGuid { get; set; }

    public Guid NetzGuid { get; set; }

    public Guid KnotenGuid { get; set; }

    public Guid? BaumGuid { get; set; }

    public Guid? ZweigGuid { get; set; }

    public int Verb { get; set; }

    public int Attrib { get; set; }

    public virtual ShortCut? ShortCuts { get; set; }
}
