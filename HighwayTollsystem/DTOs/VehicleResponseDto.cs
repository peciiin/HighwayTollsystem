using HighwayTollsystem.Enums;

namespace HighwayTollsystem.DTOs
{
    public class VehicleResponseDto
    {
        public long VehicleId { get; set; }
        public string Spz { get; set; } = string.Empty;
        public VehicleType Type { get; set; }
        public FuelType FuelType { get; set; }
        public EmissionClass EmissionClass { get; set; }
        public string CountryCode { get; set; } = string.Empty;
        public string? Vin { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}