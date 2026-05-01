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
    public class SubmissionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubmissionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetSubmissions()
        {
            var submissions = await _context.Submissions.Include(s => s.User).Include(s => s.Challenge).ToListAsync();
            return Ok(submissions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubmission(int id)
        {
            var submission = await _context.Submissions.Include(s => s.User).Include(s => s.Challenge).FirstOrDefaultAsync(s => s.Id == id);
            if (submission == null) return NotFound();
            return Ok(submission);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubmission([FromBody] Submission submission)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();
            submission.UserId = userId;
            // Here you would execute the code and set Result and IsSuccessful
            // For now, just save
            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSubmission), new { id = submission.Id }, submission);
        }

        [HttpGet("challenge/{challengeId}")]
        public async Task<IActionResult> GetSubmissionsByChallenge(int challengeId)
        {
            var submissions = await _context.Submissions.Where(s => s.ChallengeId == challengeId).Include(s => s.User).ToListAsync();
            return Ok(submissions);
        }
    }
}