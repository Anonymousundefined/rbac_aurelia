// using System;
// using System.ComponentModel.DataAnnotations.Schema;

// namespace Server.Models
// {


//     public class Submission
//     {
//         public int Id { get; set; }

//         // Policy relationship (unchanged)
//         public int PolicyId { get; set; }
//         public Policy Policy { get; set; }

//         // SubmittedBy relationship
//         public int SubmittedByUserId { get; set; }

//         // Explicitly mark this navigation's foreign key and inverse property on User
//         [ForeignKey(nameof(SubmittedByUserId))]
//         [InverseProperty(nameof(User.SubmittedSubmissions))]
//         public User SubmittedBy { get; set; }

//         public string Content { get; set; }
//         [Column("Status")]
//         public string SubmissionStatus { get; set; } = "Pending";

//         public string SubmissionStatus { get; set; } = "Pending";
//         public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

//         // ReviewedBy relationship (optional)
//         public DateTime? ReviewedAt { get; set; }
//         public int? ReviewedByUserId { get; set; }

//         // Mark FK and inverse property for ReviewedBy
//         [ForeignKey(nameof(ReviewedByUserId))]
//         [InverseProperty(nameof(User.ReviewedSubmissions))]
//         public User ReviewedBy { get; set; }
//     }
// }
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Submission
    {
        public int Id { get; set; }

        // Policy relationship
        public int PolicyId { get; set; }
        public Policy Policy { get; set; }

        // SubmittedBy relationship
        public int SubmittedByUserId { get; set; }

        [ForeignKey(nameof(SubmittedByUserId))]
        [InverseProperty(nameof(User.SubmittedSubmissions))]
        public User SubmittedBy { get; set; }

        public string Content { get; set; }

        // IMPORTANT: map C# property to DB column "Status"
        [Column("Status")]
        public string SubmissionStatus { get; set; } = "Pending";

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        // ReviewedBy relationship
        public DateTime? ReviewedAt { get; set; }
        public int? ReviewedByUserId { get; set; }

        [ForeignKey(nameof(ReviewedByUserId))]
        [InverseProperty(nameof(User.ReviewedSubmissions))]
        public User ReviewedBy { get; set; }
    }
}
