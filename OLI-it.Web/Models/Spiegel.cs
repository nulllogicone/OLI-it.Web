using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class Spiegel
{
    public Guid CodeGuid { get; set; }

    public Guid AnglerGuid { get; set; }

    public string? Status { get; set; }

    public DateTime? Zeit { get; set; }

    public DateTime? Gelesen { get; set; }

    public virtual Angler Angler { get; set; } = null!;

    public virtual Code Code { get; set; } = null!;
}
