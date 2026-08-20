using HighwayTollsystem.DTOs;

namespace HighwayTollsystem.Interfaces
{
    public interface IAnalyticsService
    {
        Task<AnalyticsDashboardDto> GetDashboardAsync();
        Task<List<AnalyticsGateStatsDto>> GetTopGatesAsync(int count);
        Task<List<AnalyticsViolationsStatsDto>> GetViolationTypeBreakdownAsync();
    }
}
