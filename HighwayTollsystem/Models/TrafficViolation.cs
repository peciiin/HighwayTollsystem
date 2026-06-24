using System;
using System.Collections.Generic;

namespace HighwayTollsystem.Models;

public partial class TrafficViolation
{
    public int ViolationId { get; set; }

    public long PassageId { get; set; }

    public int ViolationTypeId { get; set; }

    public string Details { get; set; } = null!;

    public decimal ActualPenaltyAmount { get; set; }

    public virtual Passage Passage { get; set; } = null!;

    public virtual ViolationType ViolationType { get; set; } = null!;
}
