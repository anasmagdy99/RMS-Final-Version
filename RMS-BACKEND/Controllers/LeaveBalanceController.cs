using Microsoft.AspNetCore.Mvc;
using RMS_BACKEND.DTOs;
using RMS_BACKEND.Services;

namespace RMS_BACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveBalanceController : ControllerBase
    {
        private readonly ILeaveBalanceService _leaveBalanceService;

        public LeaveBalanceController(ILeaveBalanceService leaveBalanceService)
        {
            _leaveBalanceService = leaveBalanceService;
        }

        /// <summary>
        /// Get leave balance for a specific employee
        /// </summary>
        [HttpGet("{employeeId}")]
        public async Task<ActionResult<LeaveBalanceDto>> GetLeaveBalance(
            int employeeId,
            [FromQuery] DateTime? asOfDate = null)
        {
            try
            {
                var result = await _leaveBalanceService.GetLeaveBalanceAsync(employeeId, asOfDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get my leave balance (Employee)
        /// </summary>
        [HttpGet("my-balance")]
        public async Task<ActionResult<LeaveBalanceDto>> GetMyLeaveBalance(
            [FromHeader(Name = "X-Employee-Id")] int employeeId,
            [FromQuery] DateTime? asOfDate = null)
        {
            try
            {
                var result = await _leaveBalanceService.GetLeaveBalanceAsync(employeeId, asOfDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get leave balances for my team (Manager)
        /// </summary>
        [HttpGet("team-balances")]
        public async Task<ActionResult<List<LeaveBalanceDto>>> GetTeamLeaveBalances(
            [FromHeader(Name = "X-Employee-Id")] int managerId,
            [FromQuery] DateTime? asOfDate = null)
        {
            try
            {
                var result = await _leaveBalanceService.GetLeaveBalancesForManagerAsync(managerId, asOfDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get leave balances for a department (HR/Board)
        /// </summary>
        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<List<LeaveBalanceDto>>> GetDepartmentLeaveBalances(
            int departmentId,
            [FromQuery] DateTime? asOfDate = null)
        {
            try
            {
                var result = await _leaveBalanceService.GetLeaveBalancesForDepartmentAsync(departmentId, asOfDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all leave balances (HR/Board)
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult<List<LeaveBalanceDto>>> GetAllLeaveBalances(
            [FromQuery] DateTime? asOfDate = null)
        {
            try
            {
                var result = await _leaveBalanceService.GetAllLeaveBalancesAsync(asOfDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
