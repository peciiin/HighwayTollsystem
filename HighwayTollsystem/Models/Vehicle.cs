using System;
using System.Collections.Generic;

namespace HighwayTollsystem.Models;

public partial class Vehicle
{
    public string Spz { get; set; } = null!;

    public int TypeId { get; set; }

    public string? EmissionClass { get; set; }

    public DateTime RegisteredAt { get; set; }

    public virtual ICollection<Passage> Passages { get; set; } = new List<Passage>();

    public virtual ICollection<Stk> Stks { get; set; } = new List<Stk>();

    public virtual VehicleType Type { get; set; } = null!;

    public virtual ICollection<Vignette> Vignettes { get; set; } = new List<Vignette>();
}
