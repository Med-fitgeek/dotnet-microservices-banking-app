using TransactionService.Dtos;

namespace TransactionService.Services
{
    public interface ITransactionService
    {
        Task<TransactionResponse> CreateAsync(TransactionCreateRequest request, Guid userId);
        Task<IEnumerable<TransactionResponse>> GetByAccountAsync(Guid accountId, Guid userId);
    }
}
