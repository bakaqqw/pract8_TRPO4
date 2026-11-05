using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using pract8_TRPO4.Models;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace pract8_TRPO4.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : Controller
    {
        private readonly UdilovTrpo48zadContext _context;
        private readonly IPasswordHasher<User> _hasher;
        private readonly IConfiguration _configuration;

        public AuthController(UdilovTrpo48zadContext context, IPasswordHasher<User> hasher, IConfiguration configuration)
        {
            _context = context;
            _hasher = hasher;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register(AuthRequets request)
        {
            if (_context.Users.Any(u => u.Login == request.Login))
                return Conflict("Логин уже существует");
            var user = new User 
            {
                Login = request.Login,
                Role = "User"
            };
            user.PasswordHash = _hasher.HashPassword(user, request.Password);
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok("Пользователь зарегистрирован");
        }

        [HttpPost("login")]
        public IActionResult Login(AuthRequets request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Login == request.Login);
            if (user == null)
                return Unauthorized("Неверная пара логин/пароль");
            var result = _hasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password
            );

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Неверная пара логин/пароль");

            var token = GenerateJWTToken(user);

            return Ok(new
            {
                access_token = token
            });
        }

        private string GenerateJWTToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var key = new
            SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "TasksApi",
                audience: "TasksApiClient",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
