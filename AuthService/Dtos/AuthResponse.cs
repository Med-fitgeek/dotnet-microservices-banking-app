using System.ComponentModel.DataAnnotations;

namespace AuthService.Dtos
{
    public class AuthResponse
    {

        public Guid Id { get; set; }
        [Required]
        public required string FullName { get; set; }
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Role { get; set; }
        public bool isActive { get; set; } = false;
        [Required]
        public required string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
