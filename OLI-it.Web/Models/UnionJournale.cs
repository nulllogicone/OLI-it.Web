using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class UnionJournale
{
    public string Zeichen { get; set; } = null!;

    public DateTime? Datum { get; set; }

    public Guid Guid { get; set; }

    public string? Wert { get; set; }

    public string? Datei { get; set; }

    public string? Titel { get; set; }
}
