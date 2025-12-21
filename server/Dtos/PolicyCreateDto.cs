using System.ComponentModel.DataAnnotations;

namespace Server.Dtos
{
    public class PolicyCreateDto
    {
        [Required, MaxLength(250)]
        public string Title { get; set; }

        public string? Description { get; set; }
    }
}
