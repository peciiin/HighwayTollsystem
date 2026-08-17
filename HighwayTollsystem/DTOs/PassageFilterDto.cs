namespace HighwayTollsystem.DTOs
{
    public class PassageFilterDto
    {
        public int? GateId { get; set; }
        public long? VehicleId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}