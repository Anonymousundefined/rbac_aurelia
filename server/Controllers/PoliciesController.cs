using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Dtos;
using Server.Models;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PoliciesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<PoliciesController> _logger;

        public PoliciesController(AppDbContext db, ILogger<PoliciesController> logger)
        {
            _db = db;
            _logger = logger;
        }

        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var policies = await _db.Policies
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PolicyResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    CreatedByUserId = p.CreatedByUserId,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return Ok(policies);
        }

        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] PolicyCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int? userId = null;
            if (int.TryParse(userIdClaim, out var uid)) userId = uid;

            var policy = new Policy
            {
                Title = dto.Title.Trim(),
                Description = dto.Description,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _db.Policies.Add(policy);
            await _db.SaveChangesAsync();

            var response = new PolicyResponseDto
            {
                Id = policy.Id,
                Title = policy.Title,
                Description = policy.Description,
                CreatedByUserId = policy.CreatedByUserId,
                IsActive = policy.IsActive,
                CreatedAt = policy.CreatedAt
            };

            return CreatedAtAction(nameof(GetAll), new { id = policy.Id }, response);
        }
    }
}
