using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class Zweig
{
    public Guid ZweigGuid { get; set; }

    public Guid BaumGuid { get; set; }

    public string Zweig1 { get; set; } = null!;

    public Guid? WeiterBaumGuid { get; set; }

    public Guid? WeiterNetzGuid { get; set; }

    public DateTime Datum { get; set; }

    public string? Owner { get; set; }

    public string? State { get; set; }

    public string? EnName { get; set; }

    public virtual Baum Baum { get; set; } = null!;
}
