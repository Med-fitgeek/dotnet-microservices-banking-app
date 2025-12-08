using System.ComponentModel.DataAnnotations;

namespace AccountService.Dtos
{
    public record AccountCreateRequest
    {
        [Required]
        [MaxLength(50)]
        public required string AccountType { get; set; }
        public decimal InitialBalance { get; set; } = 0;
    }
}
