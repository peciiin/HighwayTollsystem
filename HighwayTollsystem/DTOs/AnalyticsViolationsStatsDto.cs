using HighwayTollsystem.Enums;
namespace HighwayTollsystem.DTOs
{
    public class AnalyticsViolationsStatsDto
    {
        public ViolationTypeCode ViolationCode { get; set; }
        public int Count { get; set; }
        public decimal TotalFines { get; set; }
    }
}
