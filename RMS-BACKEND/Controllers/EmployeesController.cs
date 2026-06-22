using Microsoft.AspNetCore.Mvc;
using RMS_BACKEND.DTOs;
using RMS_BACKEND.Models;
using RMS_BACKEND.Repositories;

namespace RMS_BACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepo;

        public EmployeesController(IEmployeeRepository employeeRepo)
        {
            _employeeRepo = employeeRepo;
        }

        /// <summary>
        /// Get all employees (HR only)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<EmployeeListDto>>> GetAllEmployees()
        {
            try
            {
                var employees = await _employeeRepo.GetAllAsync();
                var result = employees.Select(e => new EmployeeListDto
                {
                    Id = e.Id,
                    Code = e.Code,
                    Name = e.Name,
                    DateOfEmployment = e.DateOfEmployment,
                    EmployeeRole = e.EmployeeRole.ToString(),
                    EmployeeLevel = e.EmployeeLevel?.LevelName ?? "",
                    ManagerId = e.ManagerId,
                    ManagerName = e.Manager?.Name,
                    DepartmentID = e.DepartmentID,
                    DepartmentName = e.Department?.StatusName ?? "",
                    IsActive = true,
                    IsManager = e.EmployeeRole == EmployeeRole.Manager
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get employee by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeListDto>> GetEmployee(int id)
        {
            try
            {
                var employee = await _employeeRepo.GetByIdAsync(id);
                if (employee == null)
                    return NotFound(new { message = "Employee not found" });

                var result = new EmployeeListDto
                {
                    Id = employee.Id,
                    Code = employee.Code,
                    Name = employee.Name,
                    DateOfEmployment = employee.DateOfEmployment,
                    EmployeeRole = employee.EmployeeRole.ToString(),
                    EmployeeLevel = employee.EmployeeLevel?.LevelName ?? "",
                    ManagerId = employee.ManagerId,
                    ManagerName = employee.Manager?.Name,
                    DepartmentID = employee.DepartmentID,
                    DepartmentName = employee.Department?.StatusName ?? "",
                    IsActive = true,
                    IsManager = employee.EmployeeRole == EmployeeRole.Manager
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Create new employee (HR only)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<EmployeeListDto>> CreateEmployee([FromBody] CreateEmployeeDto request)
        {
            try
            {
                // Check if code already exists
                if (await _employeeRepo.CodeExistsAsync(request.Code))
                    return BadRequest(new { message = "Employee code already exists" });

                // Get next ID
                var allEmployees = await _employeeRepo.GetAllAsync();
                var nextId = allEmployees.Any() ? allEmployees.Max(e => e.Id) + 1 : 1;

                var employee = new Employee
                {
                    Id = nextId,
                    Code = request.Code,
                    Name = request.Name,
                    Password = request.Password,
                    DateOfEmployment = request.DateOfEmployment,
                    EmployeeRole = (EmployeeRole)request.EmployeeRole,
                    EmployeeLevelId = request.EmployeeLevelId,
                    ManagerId = request.ManagerId,
                    DepartmentID = request.DepartmentID
                };

                var created = await _employeeRepo.CreateAsync(employee);
                Console.WriteLine($"[CreateEmployee] Created employee {created.Id}, Manager ID: {request.ManagerId}");
                
                // Update manager status for the selected manager
                if (request.ManagerId != null)
                {
                    Console.WriteLine($"[CreateEmployee] Calling UpdateManagerStatus for manager {request.ManagerId}");
                    await UpdateManagerStatus(request.ManagerId);
                }
                
                var result = await _employeeRepo.GetByIdAsync(created.Id);

                return Ok(new EmployeeListDto
                {
                    Id = result!.Id,
                    Code = result.Code,
                    Name = result.Name,
                    DateOfEmployment = result.DateOfEmployment,
                    EmployeeRole = result.EmployeeRole.ToString(),
                    EmployeeLevel = result.EmployeeLevel?.LevelName ?? "",
                    ManagerId = result.ManagerId,
                    ManagerName = result.Manager?.Name,
                    DepartmentID = result.DepartmentID,
                    DepartmentName = result.Department?.StatusName ?? "",
                    IsActive = true,
                    IsManager = result.EmployeeRole == EmployeeRole.Manager
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update employee (HR only)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<EmployeeListDto>> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto request)
        {
            try
            {
                var employee = await _employeeRepo.GetByIdAsync(id);
                if (employee == null)
                    return NotFound(new { message = "Employee not found" });

                employee.Code = request.Code;
                employee.Name = request.Name;
                employee.DateOfEmployment = request.DateOfEmployment;
                employee.EmployeeRole = (EmployeeRole)request.EmployeeRole;
                employee.EmployeeLevelId = request.EmployeeLevelId;
                employee.ManagerId = request.ManagerId;
                employee.DepartmentID = request.DepartmentID;

                var updated = await _employeeRepo.UpdateAsync(employee);
                
                // Update manager status for the selected manager
                await UpdateManagerStatus(request.ManagerId);
                
                var result = await _employeeRepo.GetByIdAsync(updated.Id);

                return Ok(new EmployeeListDto
                {
                    Id = result!.Id,
                    Code = result.Code,
                    Name = result.Name,
                    DateOfEmployment = result.DateOfEmployment,
                    EmployeeRole = result.EmployeeRole.ToString(),
                    EmployeeLevel = result.EmployeeLevel?.LevelName ?? "",
                    ManagerId = result.ManagerId,
                    ManagerName = result.Manager?.Name,
                    DepartmentID = result.DepartmentID,
                    DepartmentName = result.Department?.StatusName ?? "",
                    IsActive = true,
                    IsManager = result.EmployeeRole == EmployeeRole.Manager
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Soft delete employee (HR only)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            try
            {
                var result = await _employeeRepo.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = "Employee not found" });

                return Ok(new { message = "Employee deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Recalculate manager status for all employees (HR only)
        /// </summary>
        [HttpPost("recalculate-managers")]
        public async Task<ActionResult> RecalculateAllManagers()
        {
            try
            {
                var allEmployees = await _employeeRepo.GetAllAsync();
                
                // Get all unique manager IDs
                var managerIds = allEmployees
                    .Where(e => e.ManagerId != null)
                    .Select(e => e.ManagerId!.Value)
                    .Distinct()
                    .ToList();
                
                int updatedCount = 0;
                
                // Update each manager
                foreach (var managerId in managerIds)
                {
                    var manager = allEmployees.FirstOrDefault(e => e.Id == managerId);
                    if (manager != null && manager.EmployeeRole != EmployeeRole.Manager)
                    {
                        manager.EmployeeRole = EmployeeRole.Manager;
                        await _employeeRepo.UpdateAsync(manager);
                        updatedCount++;
                    }
                }
                
                return Ok(new { 
                    message = $"Successfully updated {updatedCount} employees to Manager role",
                    updatedCount = updatedCount,
                    totalManagers = managerIds.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Helper method to update manager status for an employee
        /// </summary>
        private async Task UpdateManagerStatus(int? managerId)
        {
            if (managerId == null) return;

            try
            {
                var manager = await _employeeRepo.GetByIdAsync(managerId.Value);
                if (manager != null)
                {
                    // Check if this employee has any subordinates
                    var allEmployees = await _employeeRepo.GetAllAsync();
                    var hasSubordinates = allEmployees.Any(e => e.ManagerId == managerId.Value);
                    
                    Console.WriteLine($"[UpdateManagerStatus] Manager ID: {managerId}, Has Subordinates: {hasSubordinates}, Current Role: {manager.EmployeeRole}");
                    
                    // Update EmployeeRole to Manager if has subordinates
                    if (hasSubordinates && manager.EmployeeRole != EmployeeRole.Manager)
                    {
                        Console.WriteLine($"[UpdateManagerStatus] Updating employee {manager.Id} ({manager.Name}) to Manager role");
                        manager.EmployeeRole = EmployeeRole.Manager;
                        await _employeeRepo.UpdateAsync(manager);
                        Console.WriteLine($"[UpdateManagerStatus] Successfully updated employee {manager.Id} to Manager");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the main operation
                Console.WriteLine($"[UpdateManagerStatus] Error updating manager status: {ex.Message}");
            }
        }
    }
}
