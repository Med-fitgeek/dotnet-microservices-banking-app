using System.ComponentModel.DataAnnotations;

namespace AccountService.Models
{
    public class Account
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; } // récupéré depuis le JWT
        [Required]
        [MaxLength(50)]
        public string AccountType { get; set; } // ex: Checking, Saving
        [Required]
        public decimal Balance { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
