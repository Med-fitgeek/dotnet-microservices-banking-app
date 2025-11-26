using DineroBank.Shared.DTOs.Transaction;
using Microsoft.EntityFrameworkCore;
using TransactionService.Data;
using TransactionService.Dtos;
using TransactionService.Messaging;
using TransactionService.Models;

namespace TransactionService.Services
{
    public class TransactionServiceImpl : ITransactionService
    {
        private readonly TransactionDbContext _db;
        private readonly IMessagePublisher _publisher;


        public TransactionServiceImpl(TransactionDbContext db, IMessagePublisher publisher)
        {
            _db = db;
            _publisher = publisher;
        }

        public async Task<TransactionResponse> CreateAsync(TransactionCreateRequest req, Guid userId)
        {
            if (req.Amount <= 0)
                throw new Exception("Amount must be greater than 0.");

            if (req.Type == "TRANSFER" && req.TargetAccountId == null)
                throw new Exception("TargetAccountId is required for transfers.");

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = req.AccountId,
                TargetAccountId = req.TargetAccountId,
                Amount = req.Amount,
                Type = req.Type,
                Description = req.Description,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync();

            await _publisher.PublishAsync("transaction.created", new TransactionDto
            {
                Id = transaction.Id,
                AccountId = transaction.AccountId,
                Amount = transaction.Amount,
                TargetAccountId = transaction.TargetAccountId,
                Type = transaction.Type,
                Description = transaction.Description,
                CreatedAt = transaction.CreatedAt
            });

            return new TransactionResponse
            {
                Id = transaction.Id,
                AccountId = transaction.AccountId,
                TargetAccountId = transaction.TargetAccountId,
                Amount = transaction.Amount,
                Type = transaction.Type,
                Description = transaction.Description,
                CreatedAt = transaction.CreatedAt
            };
        }

        public async Task<IEnumerable<TransactionResponse>> GetByAccountAsync(Guid accountId, Guid userId)
        {
            var list = await _db.Transactions
                .Where(x => x.AccountId == accountId && x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return list.Select(x => new TransactionResponse
            {
                Id = x.Id,
                AccountId = x.AccountId,
                TargetAccountId = x.TargetAccountId,
                Amount = x.Amount,
                Type = x.Type,
                Description = x.Description,
                CreatedAt = x.CreatedAt
            });
        }
    }
}
