using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class CheckLöcher
{
    public Guid LochGuid { get; set; }

    public string? Netz { get; set; }

    public string? Knoten { get; set; }

    public string? Baum { get; set; }

    public string? Zweig { get; set; }

    public Guid? ZweigGuid { get; set; }
}
