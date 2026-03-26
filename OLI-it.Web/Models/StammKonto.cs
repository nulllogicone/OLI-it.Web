using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class StammKonto
{
    public int StammKontoId { get; set; }

    public Guid StammGuid { get; set; }

    public DateTime Datum { get; set; }

    public string? Kommentar { get; set; }

    public decimal Betrag { get; set; }

    public Guid? PostItGuid { get; set; }

    public Guid? TopLabGuid { get; set; }
}
