using AuthService.Config;
using AuthService.Data;
using AuthService.Dtos;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services.Impl
{
    public class AuthServiceImpl : IAuthService
    {
        private readonly AppDbContext _context;
        private  readonly JwtSettings _jwtSettings;

        public AuthServiceImpl(AppDbContext context, JwtSettings jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings;
        }


        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                throw new Exception("Email already used");

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "Client",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                isActive = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();


            return new AuthResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                isActive = user.isActive,
                Token = _jwtSettings.GenerateToken(user),
                ExpiresAt = DateTime.UtcNow,
            };
        }
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new Exception("Email or password invalid");

            return new AuthResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                isActive = user.isActive,
                Token = _jwtSettings.GenerateToken(user),
                ExpiresAt = DateTime.UtcNow,
            };

        }

    }

}
