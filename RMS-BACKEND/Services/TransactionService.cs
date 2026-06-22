using RMS_BACKEND.DTOs;
using RMS_BACKEND.Models;
using RMS_BACKEND.Repositories;

namespace RMS_BACKEND.Services
{
    public interface ITransactionService
    {
        Task<TransactionResponseDto> CreateTransactionAsync(int employeeId, CreateTransactionRequestDto request);
        Task<TransactionResponseDto> UpdateTransactionAsync(int employeeId, UpdateTransactionRequestDto request);
        Task<TransactionResponseDto> CancelTransactionAsync(int employeeId, int transactionId);
        Task<TransactionResponseDto> ApproveTransactionAsync(int approverId, int transactionId, string responseMessage);
        Task<TransactionResponseDto> RejectTransactionAsync(int rejecterId, int transactionId, string responseMessage);
        Task<TransactionResponseDto> GetTransactionByIdAsync(int transactionId, int requestingEmployeeId);
        Task<List<TransactionResponseDto>> GetMyRequestsAsync(int employeeId);
        Task<List<TransactionResponseDto>> GetMyTeamRequestsAsync(int managerId);
        Task<List<TransactionResponseDto>> GetAllRequestsAsync(int requestingEmployeeId, string role);
        Task<List<TransactionResponseDto>> GetFilteredRequestsAsync(DashboardFilterDto filter, int requestingEmployeeId, string role);
    }

    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IRequestStateMachineService _stateMachine;
        private readonly ILeaveCalculationService _calculationService;

        public TransactionService(
            ITransactionRepository transactionRepo,
            IEmployeeRepository employeeRepo,
            IRequestStateMachineService stateMachine,
            ILeaveCalculationService calculationService)
        {
            _transactionRepo = transactionRepo;
            _employeeRepo = employeeRepo;
            _stateMachine = stateMachine;
            _calculationService = calculationService;
        }

        public async Task<TransactionResponseDto> CreateTransactionAsync(int employeeId, CreateTransactionRequestDto request)
        {
            var employee = await _employeeRepo.GetByIdAsync(employeeId);
            if (employee == null)
                throw new Exception("Employee not found");

            var nextId = await _transactionRepo.GetNextIdAsync();

            var transaction = new Transaction
            {
                Id = nextId,
                EmployeeId = employeeId,
                TransactionTypesID = request.TransactionTypesID,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                SubstituteEmployeeId = request.SubstituteEmployeeId,
                LeaveRationale = request.LeaveRationale,
                StatusID = TransactionStatus.Pending,
                CreationDate = DateTime.Now,
                ResponseMessage = string.Empty
            };

            var created = await _transactionRepo.CreateAsync(transaction);
            var result = await _transactionRepo.GetByIdAsync(created.Id);

            return MapToDto(result!, employeeId);
        }

        public async Task<TransactionResponseDto> UpdateTransactionAsync(int employeeId, UpdateTransactionRequestDto request)
        {
            var transaction = await _transactionRepo.GetByIdAsync(request.Id);
            if (transaction == null)
                throw new Exception("Transaction not found");

            // Check if employee can edit
            if (!_stateMachine.CanEdit(transaction.StatusID, employeeId, transaction.EmployeeId))
                throw new Exception("Cannot edit this request. Only pending requests can be edited.");

            // Update fields
            transaction.TransactionTypesID = request.TransactionTypesID;
            transaction.StartDate = request.StartDate;
            transaction.EndDate = request.EndDate;
            transaction.SubstituteEmployeeId = request.SubstituteEmployeeId;
            transaction.LeaveRationale = request.LeaveRationale;

            var updated = await _transactionRepo.UpdateAsync(transaction);
            var result = await _transactionRepo.GetByIdAsync(updated.Id);

            return MapToDto(result!, employeeId);
        }

        public async Task<TransactionResponseDto> CancelTransactionAsync(int employeeId, int transactionId)
        {
            var transaction = await _transactionRepo.GetByIdAsync(transactionId);
            if (transaction == null)
                throw new Exception("Transaction not found");

            // Check if employee can cancel
            if (!_stateMachine.CanCancel(transaction.StatusID, employeeId, transaction.EmployeeId))
                throw new Exception("Cannot cancel this request. Request is already finalized.");

            transaction.StatusID = TransactionStatus.CancelledByEmployee;
            transaction.ResponseDate = DateTime.Now;
            transaction.ResponseMessage = "Cancelled by employee";

            var updated = await _transactionRepo.UpdateAsync(transaction);
            var result = await _transactionRepo.GetByIdAsync(updated.Id);

            return MapToDto(result!, employeeId);
        }

        public async Task<TransactionResponseDto> ApproveTransactionAsync(int approverId, int transactionId, string responseMessage)
        {
            var transaction = await _transactionRepo.GetByIdAsync(transactionId);
            if (transaction == null)
                throw new Exception("Transaction not found");

            var approver = await _employeeRepo.GetByIdAsync(approverId);
            if (approver == null)
                throw new Exception("Approver not found");

            var role = _stateMachine.DetermineUserRole(approver.DepartmentID, approver.EmployeeRole == EmployeeRole.Manager);

            // Check if approver can approve
            if (!_stateMachine.CanApprove(transaction.StatusID, role, approver.DepartmentID, approver.ManagerId, transaction.EmployeeId))
                throw new Exception("You do not have permission to approve this request.");

            // Update status
            var nextStatus = _stateMachine.GetNextStatusOnApproval(transaction.StatusID, role);
            transaction.StatusID = nextStatus;
            transaction.ResponseDate = DateTime.Now;
            transaction.ResponseMessage = responseMessage;

            var updated = await _transactionRepo.UpdateAsync(transaction);
            var result = await _transactionRepo.GetByIdAsync(updated.Id);

            return MapToDto(result!, approverId);
        }

        public async Task<TransactionResponseDto> RejectTransactionAsync(int rejecterId, int transactionId, string responseMessage)
        {
            var transaction = await _transactionRepo.GetByIdAsync(transactionId);
            if (transaction == null)
                throw new Exception("Transaction not found");

            var rejecter = await _employeeRepo.GetByIdAsync(rejecterId);
            if (rejecter == null)
                throw new Exception("Rejecter not found");

            var role = _stateMachine.DetermineUserRole(rejecter.DepartmentID, rejecter.EmployeeRole == EmployeeRole.Manager);

            // Check if rejecter can reject
            if (!_stateMachine.CanReject(transaction.StatusID, role, rejecter.DepartmentID, rejecter.ManagerId, transaction.EmployeeId))
                throw new Exception("You do not have permission to reject this request.");

            // Update status
            var nextStatus = _stateMachine.GetNextStatusOnRejection(transaction.StatusID, role);
            transaction.StatusID = nextStatus;
            transaction.ResponseDate = DateTime.Now;
            transaction.ResponseMessage = responseMessage;

            var updated = await _transactionRepo.UpdateAsync(transaction);
            var result = await _transactionRepo.GetByIdAsync(updated.Id);

            return MapToDto(result!, rejecterId);
        }

        public async Task<TransactionResponseDto> GetTransactionByIdAsync(int transactionId, int requestingEmployeeId)
        {
            var transaction = await _transactionRepo.GetByIdAsync(transactionId);
            if (transaction == null)
                throw new Exception("Transaction not found");

            return MapToDto(transaction, requestingEmployeeId);
        }

        public async Task<List<TransactionResponseDto>> GetMyRequestsAsync(int employeeId)
        {
            var transactions = await _transactionRepo.GetByEmployeeIdAsync(employeeId);
            return transactions.Select(t => MapToDto(t, employeeId)).ToList();
        }

        public async Task<List<TransactionResponseDto>> GetMyTeamRequestsAsync(int managerId)
        {
            var transactions = await _transactionRepo.GetByManagerAsync(managerId);
            return transactions.Select(t => MapToDto(t, managerId)).ToList();
        }

        public async Task<List<TransactionResponseDto>> GetAllRequestsAsync(int requestingEmployeeId, string role)
        {
            var transactions = await _transactionRepo.GetAllAsync();
            return transactions.Select(t => MapToDto(t, requestingEmployeeId)).ToList();
        }

        public async Task<List<TransactionResponseDto>> GetFilteredRequestsAsync(DashboardFilterDto filter, int requestingEmployeeId, string role)
        {
            var transactions = await _transactionRepo.GetFilteredAsync(
                filter.StatusID,
                filter.DepartmentID,
                filter.EmployeeId,
                filter.StartDate,
                filter.EndDate);

            return transactions.Select(t => MapToDto(t, requestingEmployeeId)).ToList();
        }

        private TransactionResponseDto MapToDto(Transaction transaction, int requestingEmployeeId)
        {
            var calculatedDays = transaction.TransactionType != null 
                ? Math.Abs(_calculationService.CalculateLeaveDays(transaction, transaction.TransactionType))
                : 0;

            var canEdit = _stateMachine.CanEdit(transaction.StatusID, requestingEmployeeId, transaction.EmployeeId);
            var canCancel = _stateMachine.CanCancel(transaction.StatusID, requestingEmployeeId, transaction.EmployeeId);

            return new TransactionResponseDto
            {
                Id = transaction.Id,
                TransactionTypesID = transaction.TransactionTypesID,
                TransactionTypeName = transaction.TransactionType?.Name ?? "",
                StartDate = transaction.StartDate,
                EndDate = transaction.EndDate,
                SubstituteEmployeeId = transaction.SubstituteEmployeeId,
                SubstituteEmployeeName = transaction.SubstituteEmployee?.Name ?? "",
                LeaveRationale = transaction.LeaveRationale,
                ResponseDate = transaction.ResponseDate,
                ResponseMessage = transaction.ResponseMessage,
                StatusID = transaction.StatusID,
                StatusName = transaction.Status?.StatusName ?? "",
                CreationDate = transaction.CreationDate,
                EmployeeId = transaction.EmployeeId,
                EmployeeName = transaction.Employee?.Name ?? "",
                EmployeeCode = transaction.Employee?.Code ?? "",
                DepartmentName = transaction.Employee?.Department?.StatusName ?? "",
                CalculatedDays = calculatedDays,
                CanEdit = canEdit,
                CanCancel = canCancel
            };
        }
    }
}
