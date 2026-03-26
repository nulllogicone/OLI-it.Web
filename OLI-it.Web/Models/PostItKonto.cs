using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class PostItKonto
{
    public int PostItKontoId { get; set; }

    public Guid PostItGuid { get; set; }

    public DateTime Datum { get; set; }

    public string? Kommentar { get; set; }

    public decimal Betrag { get; set; }

    public Guid? StammGuid { get; set; }

    public Guid? TopLabGuid { get; set; }
}
