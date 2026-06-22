using Microsoft.AspNetCore.Mvc;
using RMS_BACKEND.DTOs;
using RMS_BACKEND.Services;

namespace RMS_BACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Get dashboard statistics with filters
        /// </summary>
        [HttpPost("stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats(
            [FromHeader(Name = "X-Employee-Id")] int employeeId,
            [FromHeader(Name = "X-Employee-Role")] string role,
            [FromBody] DashboardFilterDto filter)
        {
            try
            {
                var result = await _dashboardService.GetDashboardStatsAsync(filter, employeeId, role);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get chart data (bar and pie charts) with filters
        /// </summary>
        [HttpPost("charts")]
        public async Task<ActionResult<ChartDataDto>> GetChartData(
            [FromHeader(Name = "X-Employee-Id")] int employeeId,
            [FromHeader(Name = "X-Employee-Role")] string role,
            [FromBody] DashboardFilterDto filter)
        {
            try
            {
                var result = await _dashboardService.GetChartDataAsync(filter, employeeId, role);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
