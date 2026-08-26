using HighwayTollsystem.DTOs;
using HighwayTollsystem.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HighwayTollsystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {

        private readonly IAnalyticsService _analyticsService;
        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<AnalyticsDashboardDto>> GetDashboard()
        {
            var dashboardData = await _analyticsService.GetDashboardAsync();
            return Ok(dashboardData);
        }

        [HttpGet("top-gates")]
        public async Task<ActionResult<List<AnalyticsGateStatsDto>>> GetTopGates([FromQuery, Range(1, 100)] int count = 10)
        {
            var topGates = await _analyticsService.GetTopGatesAsync(count);
            return Ok(topGates);
        }

        [HttpGet("breakdown-violations")]
        public async Task<ActionResult<List<AnalyticsViolationsStatsDto>>> GetViolationBreakdown()
        {
            var breakdownData = await _analyticsService.GetViolationTypeBreakdownAsync();
            return Ok(breakdownData);
        }
    }
}
