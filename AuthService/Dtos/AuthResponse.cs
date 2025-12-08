using System.ComponentModel.DataAnnotations;

namespace AuthService.Dtos
{
    public class AuthResponse
    {

        public Guid Id { get; set; }
        public required string FullName { get; set; }

        public required string Email { get; set; }
        public required string Role { get; set; }
        public bool isActive { get; set; } = false;
        public required string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
