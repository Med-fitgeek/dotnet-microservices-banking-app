using System.ComponentModel.DataAnnotations;

namespace AccountService.Models
{
    public class Account
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public required Guid UserId { get; set; } // récupéré depuis le JWT
        [Required]
        [MaxLength(50)]
        public required string AccountType { get; set; } // ex: Checking, Saving
        [Required]
        public required decimal Balance { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
