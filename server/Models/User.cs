using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public enum Role { Admin, Client }

    public class User
    {
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string Name { get; set; }
        public int Age { get; set; }
        public Role Role { get; set; } = Role.Client;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // REMOVE this — it causes EF confusion:
        // public ICollection<Submission> Submissions { get; set; }

        // ADD these two correct inverse navigation properties:
        public ICollection<Submission> SubmittedSubmissions { get; set; } = new List<Submission>();
        public ICollection<Submission> ReviewedSubmissions { get; set; } = new List<Submission>();
    }
}
