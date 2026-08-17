namespace HighwayTollsystem.DTOs
{
    public class RegisterTollPassDto
    {
        public int TollGateId { get; set; }
        public string? DetectedSpz { get; set; }
        public int VehicleSpeed { get; set; }
    }
}