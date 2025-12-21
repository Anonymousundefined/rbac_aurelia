using System.ComponentModel.DataAnnotations;

namespace Server.Dtos
{
    public class SubmissionCreateDto
    {
        [Required]
        public int PolicyId { get; set; }

        public string? Content { get; set; }
    }
}
