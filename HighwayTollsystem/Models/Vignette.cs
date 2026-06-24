using System;
using System.Collections.Generic;

namespace HighwayTollsystem.Models;

public partial class Vignette
{
    public int VignetteId { get; set; }

    public string Spz { get; set; } = null!;

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    public DateTime PurchaseDate { get; set; }

    public virtual Vehicle SpzNavigation { get; set; } = null!;
}
