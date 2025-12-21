using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Dtos;
using Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<(bool Success, string Error)> RegisterAsync(RegisterDto dto, string adminSecretHeader = null)
        {
            dto.Role = string.IsNullOrWhiteSpace(dto.Role) ? "Client" : dto.Role;

            // Validate role
            if (dto.Role != "Client" && dto.Role != "Admin")
                return (false, "Role must be either 'Client' or 'Admin'.");

            // If admin creation requested, require admin secret
            // if (dto.Role == "Admin")
            // {
            //     var configuredSecret = _config["Admin:Secret"];
            //     if (string.IsNullOrWhiteSpace(configuredSecret) || adminSecretHeader != configuredSecret)
            //         return (false, "Admin creation is restricted. Provide a valid Admin secret header.");
            // }

            // Check email uniqueness
            var exists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists)
                return (false, "Email already registered.");

            var user = new User
            {
                Email = dto.Email,
                Username = dto.Username,
                Name = dto.Name,
                Age = dto.Age ?? 0,
                Role = dto.Role == "Admin" ? Role.Admin : Role.Client,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string Token, object User, string Error)> LoginAsync(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return (false, null, null, "Invalid email or password.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return (false, null, null, "Invalid email or password.");

            
            var token = GenerateJwtToken(user);
            var userObj = new
            {
                id = user.Id,
                email = user.Email,
                username = user.Username,
                name = user.Name,
                age = user.Age,
                role = user.Role.ToString()
            };

            return (true, token, userObj, null);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expireMinutes = int.TryParse(_config["Jwt:ExpireMinutes"], out var m) ? m : 1440;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
