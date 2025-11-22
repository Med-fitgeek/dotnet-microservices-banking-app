using AccountService.Data;
using AccountService.Dtos;
using AccountService.Models;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Services.Impl
{
    public class AccountServiceImpl : IAccoutService
    {
        private readonly AccountDbContext _context;

        public AccountServiceImpl(AccountDbContext context)
        {
            _context = context;
        }

        public async Task<AccountResponse> CreateAccountAsync(Guid userId, AccountCreateRequest request)
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                AccountType = request.AccountType,
                Balance = request.InitialBalance
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return new AccountResponse
            {
                Id = account.Id,
                AccountType = account.AccountType,
                Balance = account.Balance,
                IsActive = account.IsActive,
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt
            };
        }

        public async Task<List<AccountResponse>> GetAccountsAsync(Guid userId)
        {
            return await _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => new AccountResponse
                {
                    Id = a.Id,
                    AccountType = a.AccountType,
                    Balance = a.Balance,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<AccountResponse> GetAccountByIdAsync(Guid userId, Guid accountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId && a.Id == accountId);
            if (account == null) return null;

            return new AccountResponse
            {
                Id = account.Id,
                AccountType = account.AccountType,
                Balance = account.Balance,
                IsActive = account.IsActive,
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt
            };
        }

        public async Task<bool> CloseAccountAsync(Guid userId, Guid accountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId && a.Id == accountId);
            if (account == null) return false;

            account.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
