using System.ComponentModel.DataAnnotations;

namespace TransactionService.Models
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public required Guid AccountId { get; set; }

        public required Guid? TargetAccountId { get; set; } // transfert uniquement

        [Required]
        public required decimal Amount { get; set; }

        [Required]
        public required string Type { get; set; } // CREDIT | DEBIT | TRANSFER

        public string? Description { get; set; }

        public Guid UserId { get; set; } // Extracted from JWT

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
