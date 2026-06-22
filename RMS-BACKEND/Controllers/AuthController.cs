using Microsoft.AspNetCore.Mvc;
using RMS_BACKEND.DTOs;
using RMS_BACKEND.Repositories;
using RMS_BACKEND.Services;

namespace RMS_BACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IRequestStateMachineService _stateMachine;

        public AuthController(
            IEmployeeRepository employeeRepo,
            IRequestStateMachineService stateMachine)
        {
            _employeeRepo = employeeRepo;
            _stateMachine = stateMachine;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var employee = await _employeeRepo.AuthenticateAsync(request.Code, request.Password);

                if (employee == null)
                    return Unauthorized(new { message = "Invalid credentials" });

                var isManager = employee.EmployeeRole == Models.EmployeeRole.Manager;
                var role = _stateMachine.DetermineUserRole(employee.DepartmentID, isManager);

                var response = new LoginResponseDto
                {
                    Id = employee.Id,
                    Code = employee.Code,
                    Name = employee.Name,
                    DepartmentID = employee.DepartmentID,
                    DepartmentName = employee.Department?.StatusName ?? "",
                    Role = role,
                    IsManager = isManager,
                    ManagerId = employee.ManagerId,
                    Token = GenerateToken(employee.Id) // Simple token for now
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        private string GenerateToken(int employeeId)
        {
            // Simple token generation - in production, use JWT
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{employeeId}:{DateTime.UtcNow.Ticks}"));
        }
    }
}
