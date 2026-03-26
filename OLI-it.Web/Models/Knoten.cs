using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class Knoten
{
    public Guid KnotenGuid { get; set; }

    public Guid NetzGuid { get; set; }

    public string Knoten1 { get; set; } = null!;

    public Guid? WeiterBaumGuid { get; set; }

    public Guid? WeiterNetzGuid { get; set; }

    public int VgbOlis { get; set; }

    public int VgbGet { get; set; }

    public int VgbIlos { get; set; }

    public int VgbFit { get; set; }

    public string? Beschreibung { get; set; }

    public DateTime Datum { get; set; }

    public string? Owner { get; set; }

    public string? State { get; set; }

    public string? EnName { get; set; }

    public string? EnDescription { get; set; }

    public virtual Netz Netz { get; set; } = null!;
}
