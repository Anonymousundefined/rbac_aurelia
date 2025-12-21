using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Dtos;
using Server.Models;
using System.Security.Claims;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubmissionsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<SubmissionsController> _logger;

        public SubmissionsController(AppDbContext db, ILogger<SubmissionsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // POST /api/submissions  (Client)
        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Create([FromBody] SubmissionCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var policy = await _db.Policies.FirstOrDefaultAsync(p => p.Id == dto.PolicyId && p.IsActive);
            if (policy == null) return BadRequest(new { error = "Policy not found or inactive." });

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return Forbid();

            var submission = new Submission
            {
                PolicyId = dto.PolicyId,
                SubmittedByUserId = userId,
                Content = dto.Content,
                SubmissionStatus = "Pending",
                SubmittedAt = DateTime.UtcNow
            };

            _db.Submissions.Add(submission);
            await _db.SaveChangesAsync();

            var resp = new SubmissionResponseDto
            {
                Id = submission.Id,
                PolicyId = submission.PolicyId,
                SubmittedByUserId = submission.SubmittedByUserId,
                Content = submission.Content,
                Status = submission.SubmissionStatus,
                SubmittedAt = submission.SubmittedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = resp.Id }, resp);
        }

        // GET /api/submissions  (Admin: all, Client: own)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var isAdmin = User.IsInRole("Admin");
            if (isAdmin)
            {
                var list = await _db.Submissions
                    .OrderByDescending(s => s.SubmittedAt)
                    .Select(s => new SubmissionResponseDto
                    {
                        Id = s.Id,
                        PolicyId = s.PolicyId,
                        SubmittedByUserId = s.SubmittedByUserId,
                        SubmittedByName = s.SubmittedBy.Name,
                        Content = s.Content,
                        Status = s.SubmissionStatus,
                        SubmittedAt = s.SubmittedAt,
                        ReviewedAt = s.ReviewedAt,
                        ReviewedByUserId = s.ReviewedByUserId
                    }).ToListAsync();

                return Ok(list);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return Forbid();

            var mine = await _db.Submissions
                .Where(s => s.SubmittedByUserId == userId)
                .OrderByDescending(s => s.SubmittedAt)
                .Select(s => new SubmissionResponseDto
                {
                    Id = s.Id,
                    PolicyId = s.PolicyId,
                    SubmittedByUserId = s.SubmittedByUserId,
                    Content = s.Content,
                    Status = s.SubmissionStatus,
                    SubmittedAt = s.SubmittedAt,
                    ReviewedAt = s.ReviewedAt,
                    ReviewedByUserId = s.ReviewedByUserId
                }).ToListAsync();

            return Ok(mine);
        }

        // GET /api/submissions/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var submission = await _db.Submissions.FindAsync(id);
            if (submission == null) return NotFound();

            var isAdmin = User.IsInRole("Admin");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdClaim, out var userId);

            if (!isAdmin && submission.SubmittedByUserId != userId) return Forbid();

            var resp = new SubmissionResponseDto
            {
                Id = submission.Id,
                PolicyId = submission.PolicyId,
                SubmittedByUserId = submission.SubmittedByUserId,
                Content = submission.Content,
                Status = submission.SubmissionStatus,
                SubmittedAt = submission.SubmittedAt,
                ReviewedAt = submission.ReviewedAt,
                ReviewedByUserId = submission.ReviewedByUserId
            };

            return Ok(resp);
        }

        // POST /api/submissions/{id}/review   (Admin)
        [HttpPost("{id:int}/review")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Review(int id, [FromBody] SubmissionReviewDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var submission = await _db.Submissions.FirstOrDefaultAsync(s => s.Id == id);
            if (submission == null) return NotFound();

            var statusStr = dto.Status?.Trim();
            if (string.IsNullOrEmpty(statusStr)
                || !(string.Equals(statusStr, "Approved", StringComparison.OrdinalIgnoreCase)
                   || string.Equals(statusStr, "Rejected", StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest(new { error = "Status must be 'Approved' or 'Rejected'." });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var reviewerId)) return Forbid();

            // normalize e.g. "approved" -> "Approved"
            submission.SubmissionStatus = char.ToUpperInvariant(statusStr[0]) + statusStr.Substring(1).ToLowerInvariant();
            submission.ReviewedAt = DateTime.UtcNow;
            submission.ReviewedByUserId = reviewerId;

            await _db.SaveChangesAsync();

            // Optionally notify the client via SignalR here

            var resp = new SubmissionResponseDto
            {
                Id = submission.Id,
                PolicyId = submission.PolicyId,
                SubmittedByUserId = submission.SubmittedByUserId,
                Content = submission.Content,
                Status = submission.SubmissionStatus,
                SubmittedAt = submission.SubmittedAt,
                ReviewedAt = submission.ReviewedAt,
                ReviewedByUserId = submission.ReviewedByUserId
            };

            return Ok(resp);
        }
    }
}
