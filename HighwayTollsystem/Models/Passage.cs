using System;
using System.Collections.Generic;

namespace HighwayTollsystem.Models;

public partial class Passage
{
    public long PassageId { get; set; }

    public string Spz { get; set; } = null!;

    public int GateId { get; set; }

    public DateTime Timestamp { get; set; }

    public int VehicleSpeed { get; set; }

    public decimal CalculatedFee { get; set; }

    public bool IsVignetteValid { get; set; }

    public virtual TollGate Gate { get; set; } = null!;

    public virtual Vehicle SpzNavigation { get; set; } = null!;

    public virtual ICollection<TrafficViolation> TrafficViolations { get; set; } = new List<TrafficViolation>();
}
