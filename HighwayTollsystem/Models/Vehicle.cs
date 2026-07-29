using HighwayTollsystem.Enums;

namespace HighwayTollsystem.Models;

public partial class Vehicle
{
    public long VehicleId { get; set; }

    public string Spz { get; set; } = null!;

    public string CountryCode { get; set; } = "CZ";

    public string? Vin { get; set; }

    public VehicleType Type { get; set; }

    public FuelType FuelType { get; set; }

    public EmissionClass EmissionClass { get; set; }

    public DateTime RegisteredAt { get; set; }

    public virtual ICollection<Passage> Passages { get; set; } = new List<Passage>();

    public virtual ICollection<VehicleInspection> VehicleInspections { get; set; } = new List<VehicleInspection>();

    public virtual ICollection<Vignette> Vignettes { get; set; } = new List<Vignette>();
}