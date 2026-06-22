using Microsoft.EntityFrameworkCore;
using RMS_BACKEND.Data;
using RMS_BACKEND.DTOs;
using RMS_BACKEND.Models;
using RMS_BACKEND.Repositories;

namespace RMS_BACKEND.Services
{
    public interface ILeaveBalanceService
    {
        Task<LeaveBalanceDto> GetLeaveBalanceAsync(int employeeId, DateTime? asOfDate = null);
        Task<List<LeaveBalanceDto>> GetLeaveBalancesForDepartmentAsync(int departmentId, DateTime? asOfDate = null);
        Task<List<LeaveBalanceDto>> GetLeaveBalancesForManagerAsync(int managerId, DateTime? asOfDate = null);
        Task<List<LeaveBalanceDto>> GetAllLeaveBalancesAsync(DateTime? asOfDate = null);
    }

    public class LeaveBalanceService : ILeaveBalanceService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILeaveCalculationService _calculationService;

        public LeaveBalanceService(
            ApplicationDbContext context,
            ILeaveCalculationService calculationService)
        {
            _context = context;
            _calculationService = calculationService;
        }

        public async Task<LeaveBalanceDto> GetLeaveBalanceAsync(int employeeId, DateTime? asOfDate = null)
        {
            var calculationDate = asOfDate ?? DateTime.Now;

            var employee = await _context.Employees
                .Include(e => e.EmployeeLevel)
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
                throw new Exception($"Employee with ID {employeeId} not found");

            // ── Pro-Rata Calculation Pipeline ──────────────────────────────────────
            // All values are scoped to the CURRENT YEAR of the calculation date.
            // No hardcoded caps — balance is driven solely by entitlement and elapsed months.

            var monthsOfService = CalculateMonthsOfService(employee.DateOfEmployment, calculationDate);
            var isInProbation = _calculationService.IsInProbationPeriod(employee.DateOfEmployment, calculationDate);

            // Step 1: Determine the employee's annual entitlement and monthly rate.
            //   MonthlyRate = AnnualEntitlement / 12
            var annualLeaveEntitlement = employee.EmployeeLevel?.RegularLeaveperYear ?? 0;
            double monthlyAccrual = annualLeaveEntitlement / 12.0;

            // Step 2: Calculate days earned so far in the current year (pro-rata).
            //   EarnedDays = CurrentMonthNumber * MonthlyRate
            //   Uses calculationDate.Month so the "As of Date" picker works correctly.
            int currentMonth = calculationDate.Month;
            int currentYear  = calculationDate.Year;
            double earnedDays = currentMonth * monthlyAccrual;

            // Step 3: Fetch all APPROVED leave requests that START within the current year
            //   and on or before the calculation date, so future-dated leaves are excluded.
            //   StatusID = 3 → ApprovedByHR (fully approved, not pending/rejected/cancelled).
            var transactions = await _context.Transactions
                .Include(t => t.TransactionType)
                .Where(t => t.EmployeeId == employeeId &&
                            t.StartDate.Year == currentYear &&           // Current year only
                            t.StartDate <= calculationDate &&             // On or before as-of date
                            t.StatusID == TransactionStatus.ApprovedByHR) // Fully approved only
                .ToListAsync();

            // Sum total approved leave days taken this year.
            double totalApprovedLeaveDays = 0;
            foreach (var transaction in transactions)
            {
                var days = _calculationService.CalculateLeaveDays(transaction, transaction.TransactionType!);
                totalApprovedLeaveDays += Math.Abs(days);
            }

            // Step 4: AvailableBalance = EarnedDays - TotalApprovedLeaveDays
            double availableBalance = earnedDays - totalApprovedLeaveDays;

            // Step 5: Round all values to nearest half-day (0.5) — no awkward fractions.
            double RoundToHalfDay(double value) => Math.Round(value * 2, MidpointRounding.AwayFromZero) / 2.0;

            var roundedEarnedDays      = RoundToHalfDay(earnedDays);
            var roundedLeaveUsed       = RoundToHalfDay(totalApprovedLeaveDays);
            var roundedAvailableBalance = RoundToHalfDay(availableBalance);

            return new LeaveBalanceDto
            {
                EmployeeId              = employee.Id,
                EmployeeCode            = employee.Code,
                EmployeeName            = employee.Name,
                DateOfEmployment        = employee.DateOfEmployment,
                MonthsOfService         = monthsOfService,
                EmployeeLevel           = employee.EmployeeLevel?.LevelName ?? "",
                AnnualLeaveEntitlement  = annualLeaveEntitlement,
                MonthlyAccrual          = monthlyAccrual,          // AnnualEntitlement / 12
                TotalAccruedLeave       = roundedEarnedDays,       // Pro-rata earned days this year
                LeaveUsed               = roundedLeaveUsed,        // Approved leave days taken this year
                LeaveBalance            = roundedAvailableBalance, // EarnedDays - LeaveUsed
                CarryoverFromPreviousYear = 0,                     // Reserved for future carryover logic
                IsInProbation           = isInProbation,
                CalculationDate         = calculationDate,
                DepartmentName          = employee.Department?.StatusName ?? ""
            };
        }

        public async Task<List<LeaveBalanceDto>> GetLeaveBalancesForDepartmentAsync(int departmentId, DateTime? asOfDate = null)
        {
            var employees = await _context.Employees
                .Where(e => e.DepartmentID == departmentId)
                .Select(e => e.Id)
                .ToListAsync();

            var balances = new List<LeaveBalanceDto>();
            foreach (var employeeId in employees)
            {
                var balance = await GetLeaveBalanceAsync(employeeId, asOfDate);
                balances.Add(balance);
            }

            return balances;
        }

        public async Task<List<LeaveBalanceDto>> GetLeaveBalancesForManagerAsync(int managerId, DateTime? asOfDate = null)
        {
            var employees = await _context.Employees
                .Where(e => e.ManagerId == managerId)
                .Select(e => e.Id)
                .ToListAsync();

            var balances = new List<LeaveBalanceDto>();
            foreach (var employeeId in employees)
            {
                var balance = await GetLeaveBalanceAsync(employeeId, asOfDate);
                balances.Add(balance);
            }

            return balances;
        }

        public async Task<List<LeaveBalanceDto>> GetAllLeaveBalancesAsync(DateTime? asOfDate = null)
        {
            var employees = await _context.Employees
                .Select(e => e.Id)
                .ToListAsync();

            var balances = new List<LeaveBalanceDto>();
            foreach (var employeeId in employees)
            {
                var balance = await GetLeaveBalanceAsync(employeeId, asOfDate);
                balances.Add(balance);
            }

            return balances;
        }

        private int CalculateMonthsOfService(DateTime dateOfEmployment, DateTime asOfDate)
        {
            var years = asOfDate.Year - dateOfEmployment.Year;
            var months = asOfDate.Month - dateOfEmployment.Month;
            var totalMonths = (years * 12) + months;

            if (asOfDate.Day < dateOfEmployment.Day)
                totalMonths--;

            return totalMonths > 0 ? totalMonths : 0;
        }
    }
}
