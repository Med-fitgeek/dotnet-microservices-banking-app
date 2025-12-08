using System.ComponentModel.DataAnnotations;

namespace AccountService.Dtos
{
    public record AccountResponse
    {
        public Guid Id { get; set; }
        [Required]
        public required string AccountType { get; set; }
        [Required]
        public required decimal Balance { get; set; }
        [Required]
        public required bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }
    }
}
