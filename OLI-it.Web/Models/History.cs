using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class History
{
    public Guid HistoryGuid { get; set; }

    public DateTime? Datum { get; set; }

    public decimal? BoundKooK { get; set; }

    public decimal? FlowKooK { get; set; }

    public int? AnzStämme { get; set; }

    public int? AnzPostIt { get; set; }

    public int? AnzTopLab { get; set; }

    public int? AnzAngler { get; set; }

    public int? AnzX { get; set; }

    public decimal? Provision { get; set; }

    public decimal? SummeStammKonto { get; set; }

    public decimal? SummePostItKonto { get; set; }
}
