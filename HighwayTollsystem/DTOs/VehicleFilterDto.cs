using HighwayTollsystem.Enums;

namespace HighwayTollsystem.DTOs
{
    public class VehicleFilterDto
    {
        public FuelType? FuelType { get; set; }
        public VehicleType? VehicleType { get; set; }
        
        public EmissionClass? EmissionClass { get; set; }

        public string? CountryCode { get; set; }

        public string? Vin { get; set; }    
        public string? Spz { get; set; }
        public DateTime? RegisteredFrom { get; set; }
        public DateTime? RegisteredTo { get; set; }




        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
