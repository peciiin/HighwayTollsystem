using System;
using System.Collections.Generic;

namespace HighwayTollsystem.Models;

public partial class Stk
{
    public int StkId { get; set; }

    public string Spz { get; set; } = null!;

    public DateTime ValidTo { get; set; }

    public DateTime EmissionsValidTo { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Vehicle SpzNavigation { get; set; } = null!;
}
