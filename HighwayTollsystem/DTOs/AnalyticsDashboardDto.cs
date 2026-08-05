namespace HighwayTollsystem.DTOs
{
    public class AnalyticsDashboardDto
    {
        public int TotalPassages { get; set; }
        public int TotalViolations { get; set; }
        public decimal TotalPenaltyAmount { get; set; }
        public int TotalPersonalCars { get; set; }
        public int TotalPersonalCarsUnique { get; set; }
    }
}
