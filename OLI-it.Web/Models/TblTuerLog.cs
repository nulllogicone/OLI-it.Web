using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class TblTuerLog
{
    public int TuerLogId { get; set; }

    public DateTime? Datum { get; set; }

    public string? Ip { get; set; }

    public string? Host { get; set; }

    public Guid? Eglsguid { get; set; }

    public Guid? Sguid { get; set; }

    public Guid? Aguid { get; set; }

    public Guid? Pguid { get; set; }

    public Guid? Cguid { get; set; }

    public Guid? Tguid { get; set; }

    public string? Kommentar { get; set; }
}
