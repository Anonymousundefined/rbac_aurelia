using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Policy
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        // FK column name that exists in DB
        public int? CreatedByUserId { get; set; }

        // Navigation property
        [ForeignKey(nameof(CreatedByUserId))]
        public User CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
