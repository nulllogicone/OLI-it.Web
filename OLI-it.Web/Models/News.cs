using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class News
{
    public Guid NewsGuid { get; set; }

    public Guid? AnglerGuid { get; set; }

    public Guid? CodeGuid { get; set; }

    public DateTime? Datum { get; set; }

    public DateTime? Gelesen { get; set; }

    public DateTime? Gesehen { get; set; }

    public DateTime? Gemailt { get; set; }

    public virtual Angler? Angler { get; set; }

    public virtual Code? Code { get; set; }
}
