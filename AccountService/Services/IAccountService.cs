using AccountService.Dtos;
using DineroBank.Shared.DTOs.Transaction;


namespace AccountService.Services
{
    public interface IAccountService
    {
        Task<AccountResponse> CreateAccountAsync(Guid userId, AccountCreateRequest request);
        Task<List<AccountResponse>> GetAccountsAsync(Guid userId);

        Task<AccountResponse> GetAccountByIdAsync(Guid userId, Guid accountId);

        Task<bool> CloseAccountAsync(Guid userId, Guid accountId);
        Task<TransactionResult> ProcessTransactionAsync(TransactionDto dto);
    }
}
