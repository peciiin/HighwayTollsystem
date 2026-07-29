namespace HighwayTollsystem.Models;

public partial class VehicleInspection
{
    public long VehicleInspectionId { get; set; }

    public long VehicleId { get; set; }

    public DateTime ValidTo { get; set; }

    public DateTime EmissionsValidTo { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Vehicle Vehicle { get; set; } = null!;
}