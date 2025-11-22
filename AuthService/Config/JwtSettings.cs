using AuthService.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AuthService.Config
{
    public class JwtSettings
    {
        private readonly IConfiguration _config;
        private readonly RsaSecurityKey _privateKey;


        public JwtSettings(IConfiguration config)
        {
            _config = config;

            // Charger la clé privée pour signer le JWT
            var privateKeyPath = _config["Jwt:PrivateKeyPath"];
            using var rsa = RSA.Create();
            rsa.ImportFromPem(File.ReadAllText(privateKeyPath));
            _privateKey = new RsaSecurityKey(rsa);

        }


        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("FullName", user.FullName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var creds = new SigningCredentials(_privateKey, SecurityAlgorithms.RsaSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:TokenExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
