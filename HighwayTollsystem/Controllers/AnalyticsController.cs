using HighwayTollsystem.Models;
using Microsoft.AspNetCore.Mvc;
using HighwayTollsystem.Services;

namespace HighwayTollsystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {

        private readonly AnalyticsService _analyticsService;
        public AnalyticsController(AnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var dashboardData = await _analyticsService.GetDashboardAsync();
            return Ok(dashboardData);
        }
        [HttpGet("top-gates")]
        public async Task<IActionResult> GetTopGates([FromQuery] int count = 5)
        {
            var topGates = await _analyticsService.GetTopGatesAsync(count);
            return Ok(topGates);
        }
        [HttpGet("breakdown-data")]
        public async Task<IActionResult> GetBreakdownData()
        {
            var breakdownData = await _analyticsService.GetViolationTypeBreakdownAsync();
            return Ok(breakdownData);
        }
    }
}
