using System;
using System.Collections.Generic;

namespace HighwayTollsystem.Models;

public partial class TollGate
{
    public int GateId { get; set; }

    public string HighwayName { get; set; } = null!;

    public decimal KilometerPost { get; set; }

    public string Direction { get; set; } = null!;

    public decimal GpsLatitude { get; set; }

    public decimal GpsLongitude { get; set; }

    public virtual ICollection<Passage> Passages { get; set; } = new List<Passage>();
}
