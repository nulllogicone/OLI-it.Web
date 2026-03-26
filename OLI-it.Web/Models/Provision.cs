using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class Provision
{
    public Guid ProvisionGuid { get; set; }

    public Guid StammGuid { get; set; }

    public Guid PostItGuid { get; set; }

    public DateTime Datum { get; set; }

    public decimal Betrag { get; set; }

    public virtual PostIt PostIt { get; set; } = null!;

    public virtual Stamm Stamm { get; set; } = null!;
}
