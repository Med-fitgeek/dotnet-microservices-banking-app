using System.ComponentModel.DataAnnotations;

namespace AuthService.Dtos
{
    public class RegisterRequest
    {
        [Required]
        public required string FullName { get; set; }
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }

    }
}
