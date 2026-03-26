using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class ServiceLog
{
    public int Id { get; set; }

    public DateTime Datum { get; set; }

    public string? Ip { get; set; }

    public string? Target { get; set; }

    public string? Service { get; set; }

    public string? Stamm { get; set; }

    public string? Emailid { get; set; }

    public string? Kommentar { get; set; }
}
