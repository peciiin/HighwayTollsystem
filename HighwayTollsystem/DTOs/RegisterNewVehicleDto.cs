using HighwayTollsystem.Enums;

namespace HighwayTollsystem.DTOs
{
    public class RegisterNewVehicleDto
    {
        public string Spz { get; set; } = null!;
        public VehicleType Type { get; set; }
        public FuelType FuelType { get; set; }
        public EmissionClass EmissionClass { get; set; }
        public string CountryCode { get; set; } = "CZ";
        public string? Vin { get; set; }
    }
}
