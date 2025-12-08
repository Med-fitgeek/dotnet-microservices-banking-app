using System.ComponentModel.DataAnnotations;

namespace AuthService.Dtos
{
    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password{ get; set; }
    }
}
