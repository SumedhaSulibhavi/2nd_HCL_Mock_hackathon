using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace RemoteTechTeamApi.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(int userId, string email, string username, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Fetch the secret key from appsettings.json (Fallback to a hardcoded 32-byte key if empty for safety)
            var keyString = _configuration["Jwt:Key"] ?? "SuperSecretHackathonBackupKey2026!!";
            var key = Encoding.ASCII.GetBytes(keyString);

            // Define the 4 custom security claims as required by the playbook
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(8), // Secure 8-hour session window
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}