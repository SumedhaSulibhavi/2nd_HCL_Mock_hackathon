using Microsoft.AspNetCore.Mvc;
using RemoteTechTeamApi.DTOs;
using RemoteTechTeamApi.Helpers;

namespace RemoteTechTeamApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtHelper _jwtHelper;

        // In-memory user store for Person 2 testing
        private static readonly List<TempUserEntity> InMemoryUsers = new();
        private static int _nextUserId = 1;

        public AuthController(JwtHelper jwtHelper)
        {
            _jwtHelper = jwtHelper;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var emailExists = InMemoryUsers.Any(u => u.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase));
            if (emailExists)
            {
                return BadRequest(new { message = "Email is already registered inside the team database system." });
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var newUser = new TempUserEntity
            {
                Id = _nextUserId++,
                Email = dto.Email,
                Username = dto.Username,
                PasswordHash = hashedPassword,
                Role = dto.Role
            };

            InMemoryUsers.Add(newUser);

            return Ok(new { message = "User registration completed successfully." });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = InMemoryUsers.FirstOrDefault(u => u.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email profile credentials provided." });
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "Invalid password security credential parameters." });
            }

            var token = _jwtHelper.GenerateToken(user.Id, user.Email, user.Username, user.Role);

            var response = new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Role = user.Role
            };

            return Ok(response);
        }
    }

    public class TempUserEntity
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Employee";
    }
}