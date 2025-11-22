using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TransactionService.Dtos;
using TransactionService.Services;

namespace TransactionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue("sub"));
        }

        [HttpPost]
        public async Task<IActionResult> Create(TransactionCreateRequest request)
        {
            var userId = GetUserId();
            var result = await _transactionService.CreateAsync(request, userId);
            return Ok(result);
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetByAccount(Guid accountId)
        {
            var userId = GetUserId();
            var result = await _transactionService.GetByAccountAsync(accountId, userId);
            return Ok(result);
        }
    }
}
