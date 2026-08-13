using HighwayTollsystem.DTOs;

namespace HighwayTollsystem.Interfaces
{
    public interface IAnalyticsService
    {
        Task<AnalyticsDashboardDto> GetDashboardAsync();

        Task<List<AnalyticsGateStatsDto>> GetTopGatesAsync(int count = 5);

        Task<List<AnalyticsViolationsStatsDto>> GetViolationTypeBreakdownAsync();
    }
}
