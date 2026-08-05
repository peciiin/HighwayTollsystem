namespace HighwayTollsystem.DTOs
{
    public class AnalyticsGateStatsDto
    {
        public int GateId { get; set; }
        public int PassageCount { get; set; }
        public decimal TotalFeesCollected { get; set; }
    }
}
