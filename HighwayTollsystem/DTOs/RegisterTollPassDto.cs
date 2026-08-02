using System.ComponentModel.DataAnnotations;

namespace HighwayTollsystem.DTOs
{
    public class RegisterTollPassDto
    {
        
        [Required(ErrorMessage = "Detected SPZ is required.")]
        public string DetectedSpz { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "TollGateId must be greater than 0.")]
        public int TollGateId { get; set; }

        [Range(0, 300, ErrorMessage = "0 ... 300 km/h.")]
        public int VehicleSpeed { get; set; }
    }


}

