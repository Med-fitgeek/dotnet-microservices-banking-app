using AccountService.Dtos;

namespace AccountService.Services
{
    public interface IAccoutService
    {
        Task<AccountResponse> CreateAccountAsync(Guid userId, AccountCreateRequest request);
        Task<List<AccountResponse>> GetAccountsAsync(Guid userId);

        Task<AccountResponse> GetAccountByIdAsync(Guid userId, Guid accountId);

        Task<bool> CloseAccountAsync(Guid userId, Guid accountId);
    }
}
