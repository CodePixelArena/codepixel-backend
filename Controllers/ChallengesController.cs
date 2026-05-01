using codepixel_backend.Data;
using codepixel_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace codepixel_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChallengesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChallengesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetChallenges()
        {
            var challenges = await _context.Challenges.Include(c => c.CreatedBy).ToListAsync();
            return Ok(challenges);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetChallenge(int id)
        {
            var challenge = await _context.Challenges.Include(c => c.CreatedBy).FirstOrDefaultAsync(c => c.Id == id);
            if (challenge == null) return NotFound();
            return Ok(challenge);
        }

        [HttpPost]
        public async Task<IActionResult> CreateChallenge([FromBody] Challenge challenge)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();
            challenge.CreatedById = userId;
            _context.Challenges.Add(challenge);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetChallenge), new { id = challenge.Id }, challenge);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChallenge(int id, [FromBody] Challenge challenge)
        {
            if (id != challenge.Id) return BadRequest();
            _context.Entry(challenge).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChallenge(int id)
        {
            var challenge = await _context.Challenges.FindAsync(id);
            if (challenge == null) return NotFound();
            _context.Challenges.Remove(challenge);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}