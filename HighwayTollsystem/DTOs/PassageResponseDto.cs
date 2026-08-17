namespace HighwayTollsystem.DTOs
{
    public class PassageResponseDto
    {
        public long PassageId { get; set; }
        public int GateId { get; set; }
        public long? VehicleId { get; set; }
        public string? DetectedSpz { get; set; }
        public DateTime Timestamp { get; set; }
        public int VehicleSpeed { get; set; }
        public decimal CalculatedFee { get; set; }
        public bool HasViolations { get; set; }
    }
}