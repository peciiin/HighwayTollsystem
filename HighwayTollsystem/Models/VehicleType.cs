using System;
using System.Collections.Generic;

namespace HighwayTollsystem.Models;

public partial class VehicleType
{
    public int Id { get; set; }

    public string? TypeName { get; set; }

    public decimal? BaseTarif { get; set; }

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
