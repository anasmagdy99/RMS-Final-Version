using Microsoft.AspNetCore.Mvc;
using RMS_BACKEND.DTOs;
using RMS_BACKEND.Services;

namespace RMS_BACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Create a new leave request
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TransactionResponseDto>> CreateTransaction(
            [FromHeader(Name = "X-Employee-Id")] int employeeId,
            [FromBody] CreateTransactionRequestDto request)
        {
            try
            {
                var result = await _transactionService.CreateTransactionAsync(employeeId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update a pending leave request (only while Pending)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TransactionResponseDto>> UpdateTransaction(
            [FromHeader(Name = "X-Employee-Id")] int employeeId,
            int id,
            [FromBody] UpdateTransactionRequestDto request)
        {
            try
            {
                request.Id = id;
                var result = await _transactionService.UpdateTransactionAsync(employeeId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cancel a leave request (before final decision)
        /// </summary>
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<TransactionResponseDto>> CancelTransaction(
            [FromHeader(Name = "X-Employee-Id")] int employeeId,
            int id)
        {
            try
            {
                var result = await _transactionService.CancelTransactionAsync(employeeId, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Approve a leave request (Manager/HR/Board)
        /// </summary>
        [HttpPost("{id}/approve")]
        public async Task<ActionResult<TransactionResponseDto>> ApproveTransaction(
            [FromHeader(Name = "X-Employee-Id")] int employeeId,
            int id,
            [FromBody] ApproveRejectRequestDto request)
        {
            try
            {
                var result = await _transactionService.ApproveTransactionAsync(employeeId, id, request.ResponseMessage);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Reject a leave request (Manager/HR/Board)
        /// </summary>
        [HttpPost("{id}/reject")]
        public async Task<ActionResult<TransactionResponseDto>> RejectTransaction(
            [FromHeader(Name = "X-Employee-Id")] int employeeId,
            int id,
            [FromBody] ApproveRejectRequestDto request)
        {
            try
            {
                var result = await _transactionService.RejectTransactionAsync(employeeId, id, request.ResponseMessage);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get a specific transaction by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionResponseDto>> GetTransaction(
            [FromHeader(Name = "X-Employee-Id")] int employeeId,
            int id)
        {
            try
            {
                var result = await _transactionService.GetTransactionByIdAsync(id, employeeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get my own requests (Employee)
        /// </summary>
        [HttpGet("my-requests")]
        public async Task<ActionResult<List<TransactionResponseDto>>> GetMyRequests(
            [FromHeader(Name = "X-Employee-Id")] int employeeId)
        {
            try
            {
                var result = await _transactionService.GetMyRequestsAsync(employeeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get my team's requests (Manager)
        /// </summary>
        [HttpGet("my-team-requests")]
        public async Task<ActionResult<List<TransactionResponseDto>>> GetMyTeamRequests(
            [FromHeader(Name = "X-Employee-Id")] int employeeId)
        {
            try
            {
                var result = await _transactionService.GetMyTeamRequestsAsync(employeeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all requests (HR/Board)
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult<List<TransactionResponseDto>>> GetAllRequests(
            [FromHeader(Name = "X-Employee-Id")] int employeeId,
            [FromHeader(Name = "X-Employee-Role")] string role)
        {
            try
            {
                var result = await _transactionService.GetAllRequestsAsync(employeeId, role);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get filtered requests with filters
        /// </summary>
        [HttpPost("filter")]
        public async Task<ActionResult<List<TransactionResponseDto>>> GetFilteredRequests(
            [FromHeader(Name = "X-Employee-Id")] int employeeId,
            [FromHeader(Name = "X-Employee-Role")] string role,
            [FromBody] DashboardFilterDto filter)
        {
            try
            {
                var result = await _transactionService.GetFilteredRequestsAsync(filter, employeeId, role);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
