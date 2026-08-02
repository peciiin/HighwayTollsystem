namespace HighwayTollsystem.Models;

public partial class Passage
{
    public long PassageId { get; set; }

    public long? VehicleId { get; set; }

    public int GateId { get; set; }

    public DateTime Timestamp { get; set; }

    public int VehicleSpeed { get; set; }

    public decimal CalculatedFee { get; set; }

    public virtual TollGate Gate { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;

    public virtual ICollection<TrafficViolation> TrafficViolations { get; set; } = new List<TrafficViolation>();
}