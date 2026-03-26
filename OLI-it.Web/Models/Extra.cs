using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class Extra
{
    public Guid ExtrasGuid { get; set; }

    public Guid StammGuid { get; set; }

    public bool Hilfe { get; set; }

    public bool Werbefrei { get; set; }

    public bool Freakmode { get; set; }

    public bool Newsletter { get; set; }

    public bool NewT { get; set; }

    public bool NewP { get; set; }

    public bool Fristablauf { get; set; }

    public bool Gutschrift { get; set; }

    public bool Tipps { get; set; }

    public bool HtmlMail { get; set; }

    public bool Showclosed { get; set; }

    public string Sort { get; set; } = null!;

    public bool Descend { get; set; }

    public int ZeilenZahl { get; set; }

    public bool TxtOnly { get; set; }

    public virtual Stamm Stamm { get; set; } = null!;
}
