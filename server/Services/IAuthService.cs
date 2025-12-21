using Server.Dtos;
using Server.Models;

namespace Server.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Error)> RegisterAsync(RegisterDto dto, string adminSecretHeader = null);
        Task<(bool Success, string Token, object User, string Error)> LoginAsync(LoginDto dto);
    }
}
