using System;
using System.Collections.Generic;

namespace HighwayTollsystem.Models;

public partial class ViolationType
{
    public int ViolationTypeId { get; set; }

    public string Code { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal DefaultPenaltyAmount { get; set; }

    public virtual ICollection<TrafficViolation> TrafficViolations { get; set; } = new List<TrafficViolation>();
}
