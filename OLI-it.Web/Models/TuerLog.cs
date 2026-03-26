using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class TuerLog
{
    public DateTime? Datum { get; set; }

    public string? Ip { get; set; }

    public string? Host { get; set; }

    public string? EglStamm { get; set; }

    public string? Stamm { get; set; }

    public string? PostIt { get; set; }

    public string? Kommentar { get; set; }

    public string? Angler { get; set; }

    public string? TopLab { get; set; }
}
