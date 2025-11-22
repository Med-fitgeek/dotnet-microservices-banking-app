using System.Security.Claims;
using AccountService.Dtos;
using AccountService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // JWT required
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpPost]
        public async Task<ActionResult<AccountResponse>> CreateAccount(AccountCreateRequest request)
        {
            var userId = GetUserId();
            var result = await _accountService.CreateAccountAsync(userId, request);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<AccountResponse>>> GetAccounts()
        {
            var userId = GetUserId();
            var result = await _accountService.GetAccountsAsync(userId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountResponse>> GetAccount(Guid id)
        {
            var userId = GetUserId();
            var result = await _accountService.GetAccountByIdAsync(userId, id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("{id}/close")]
        public async Task<ActionResult> CloseAccount(Guid id)
        {
            var userId = GetUserId();
            var success = await _accountService.CloseAccountAsync(userId, id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
