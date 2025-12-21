using System.ComponentModel.DataAnnotations;

namespace Server.Dtos
{
    public class SubmissionReviewDto
    {
        [Required]
        [RegularExpression("Approved|Rejected", ErrorMessage = "Status must be 'Approved' or 'Rejected'.")]
        public string Status { get; set; } = null!;

        public string? Comment { get; set; }
    }
}
