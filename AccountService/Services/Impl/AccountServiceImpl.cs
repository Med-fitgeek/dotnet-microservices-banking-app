using AccountService.Data;
using AccountService.Dtos;
using AccountService.Models;
using Microsoft.EntityFrameworkCore;
using DineroBank.Shared.DTOs.Transaction;

namespace AccountService.Services.Impl
{
    public class AccountServiceImpl : IAccountService
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


        public async Task<TransactionResult> ProcessTransactionAsync(TransactionDto transactionDto)
        {
            // 1️⃣ Vérifier idempotence
            var alreadyProcessed = await _context.ProcessedMessages
                                                .AnyAsync(m => m.MessageId == transactionDto.Id);
            if (alreadyProcessed)
                return new TransactionResult { Success = false, Message = "Message déjà traité" };

            // 2️⃣ Récupérer le compte source
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == transactionDto.AccountId && a.IsActive);
            if (account == null)
                return new TransactionResult { Success = false, Message = "Compte introuvable ou inactif" };

            // 3️⃣ Appliquer selon le type
            switch (transactionDto.Type.ToUpper())
            {
                case "CREDIT":
                    account.Balance += transactionDto.Amount;
                    break;

                case "DEBIT":
                    if (account.Balance < transactionDto.Amount)
                        return new TransactionResult { Success = false, Message = "Solde insuffisant" };
                    account.Balance -= transactionDto.Amount;
                    break;

                case "TRANSFER":
                    var targetAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == transactionDto.TargetAccountId && a.IsActive);
                    if (targetAccount == null)
                        return new TransactionResult { Success = false, Message = "Compte cible introuvable ou inactif" };
                    if (account.Balance < transactionDto.Amount)
                        return new TransactionResult { Success = false, Message = "Solde insuffisant" };

                    account.Balance -= transactionDto.Amount;
                    targetAccount.Balance += transactionDto.Amount;
                    targetAccount.UpdatedAt = DateTime.UtcNow;
                    break;

                default:
                    return new TransactionResult { Success = false, Message = "Type de transaction inconnu" };
            }

            account.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new TransactionResult { Success = true, Message = "Transaction appliquée" };
        }

    }
}
