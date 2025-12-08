using System.ComponentModel.DataAnnotations;

namespace TransactionService.Dtos
{
    public class TransactionCreateRequest
    {
        public Guid AccountId { get; set; }
        public Guid? TargetAccountId { get; set; }
        [Required]
        public required decimal Amount { get; set; }
        [Required]
        public required string Type { get; set; } // CREDIT | DEBIT | TRANSFER
        public string? Description { get; set; }
    }
}
