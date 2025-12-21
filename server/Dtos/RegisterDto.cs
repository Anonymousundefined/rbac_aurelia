using System.ComponentModel.DataAnnotations;

namespace Server.Dtos
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string Username { get; set; }

        public string Name { get; set; }

        public int? Age { get; set; }

        public string Role { get; set; } = "Client";
    }
}
