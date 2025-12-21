using Microsoft.AspNetCore.Mvc;
using Server.Dtos;
using Server.Services;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
           
            var adminSecretHeader = Request.Headers["X-Admin-Secret"].FirstOrDefault();
            var (Success, Error) = await _auth.RegisterAsync(dto, adminSecretHeader);
            if (!Success) return BadRequest(new { error = Error });
            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var (Success, Token, User, Error) = await _auth.LoginAsync(dto);
            if (!Success) return Unauthorized(new { error = Error });

            return Ok(new { token = Token, user = User });
        }
    }
}
