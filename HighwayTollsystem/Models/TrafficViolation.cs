
using HighwayTollsystem.Enums;

namespace HighwayTollsystem.Models;

public partial class TrafficViolation
{
    public long ViolationId { get; set; }

    public long PassageId { get; set; }

    public ViolationTypeCode ViolationType { get; set; }

    public string Details { get; set; } = null!;

    public decimal ActualPenaltyAmount { get; set; }

    public virtual Passage Passage { get; set; } = null!;
}