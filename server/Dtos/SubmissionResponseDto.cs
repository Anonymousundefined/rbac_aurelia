using System;

namespace Server.Dtos
{
    public class SubmissionResponseDto
    {
        public int Id { get; set; }
        public int PolicyId { get; set; }
        public int SubmittedByUserId { get; set; }
        public string SubmittedByName { get; set; }
        public string? Content { get; set; }
        public string Status { get; set; } = null!;
        public DateTime SubmittedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public int? ReviewedByUserId { get; set; }
    }
}
