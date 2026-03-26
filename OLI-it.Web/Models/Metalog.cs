using System;
using System.Collections.Generic;

namespace OLI_it.Web.Models;

public partial class Metalog
{
    public int Id { get; set; }

    public DateTime Zeit { get; set; }

    public string Ip { get; set; } = null!;

    public string? Host { get; set; }

    public string Site { get; set; } = null!;

    public string? Ref { get; set; }

    public string? Url { get; set; }

    public string? OliUser { get; set; }
}
