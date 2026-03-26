using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class StammDurchToll
{
    public string Stamm { get; set; } = null!;

    public Guid StammGuid { get; set; }

    public int? DurchToll { get; set; }
}
