using System.ComponentModel.DataAnnotations;

namespace AccountService.Dtos
{
    public class AccountCreateRequest
    {
        [Required]
        [MaxLength(50)]
        public required string AccountType { get; set; }
        public decimal InitialBalance { get; set; } = 0;
    }
}
