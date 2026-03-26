using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class StammPostItTopLabTolli
{
    public Guid? StammGuid { get; set; }

    public string? Stamm { get; set; }

    public short? Toll { get; set; }

    public Guid? TopLabGuid { get; set; }

    public string? TopLab { get; set; }

    public Guid? TstammGuid { get; set; }

    public Guid PostItGuid { get; set; }

    public string PostIt { get; set; } = null!;

    public string? Tstamm { get; set; }
}
