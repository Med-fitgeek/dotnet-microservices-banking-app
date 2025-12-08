using System.ComponentModel.DataAnnotations;

namespace TransactionService.Dtos
{
    public class TransactionResponse
    {
        public Guid Id { get; set; }
        [Required]
        public required Guid AccountId { get; set; }
        [Required]
        public required Guid? TargetAccountId { get; set; }
        [Required]
        public required decimal Amount { get; set; }
        [Required]
        public required string Type { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
