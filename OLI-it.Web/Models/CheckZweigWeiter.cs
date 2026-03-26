using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class CheckZweigWeiter
{
    public Guid? WeiterBaumGuid { get; set; }

    public string? Baum { get; set; }

    public Guid? WeiterNetzGuid { get; set; }

    public string? Netz { get; set; }
}
