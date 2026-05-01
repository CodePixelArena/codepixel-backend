using codepixel_backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace codepixel_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var activeUsers = await _context.Pixels
                    .Select(p => p.UserId)
                    .Distinct()
                    .CountAsync();

                var uniqueOwnedPixels = await _context.Pixels
                    .Select(p => new { p.X, p.Y })
                    .Distinct()
                    .CountAsync();

                var acceptedCount = await _context.Submissions
                    .CountAsync(s => s.IsSuccessful);

                var submissionCount = await _context.Submissions.CountAsync();

                var playerStatsRaw = await _context.Users
                    .Select(u => new
                    {
                        Username = u.DisplayName ?? u.UserName ?? "Unknown",
                        OwnedPixels = _context.Pixels.Count(p => p.UserId == u.Id),
                        SolvedChallenges = _context.Submissions.Count(s => s.UserId == u.Id && s.IsSuccessful),
                        Submissions = _context.Submissions.Count(s => s.UserId == u.Id),
                    })
                    .Where(x => x.OwnedPixels > 0 || x.Submissions > 0)
                    .ToListAsync();

                var playerStats = playerStatsRaw
                    .Select(x => new
                    {
                        x.Username,
                        x.OwnedPixels,
                        x.SolvedChallenges,
                        x.Submissions,
                        Accuracy = x.Submissions == 0
                            ? 0
                            : (int)Math.Round((double)x.SolvedChallenges * 100 / x.Submissions),
                    })
                    .ToList();

                var topPlayers = playerStats
                    .OrderByDescending(x => x.SolvedChallenges)
                    .ThenByDescending(x => x.Accuracy)
                    .ThenByDescending(x => x.OwnedPixels)
                    .Take(5)
                    .Select((x, index) => new
                    {
                        Rank = index + 1,
                        x.Username,
                        x.OwnedPixels,
                        x.SolvedChallenges,
                        x.Submissions,
                        x.Accuracy,
                    })
                    .ToList();

                var pixelOwners = playerStats
                    .OrderByDescending(x => x.OwnedPixels)
                    .ThenByDescending(x => x.SolvedChallenges)
                    .Take(5)
                    .Select((x, index) => new
                    {
                        Rank = index + 1,
                        x.Username,
                        x.OwnedPixels,
                        x.SolvedChallenges,
                        x.Submissions,
                        x.Accuracy,
                    })
                    .ToList();

                var recentPixels = await _context.Pixels
                    .Include(p => p.User)
                    .OrderByDescending(p => p.PlacedAt)
                    .Take(8)
                    .Select(p => new
                    {
                        Label = $"Pixel ({p.X}, {p.Y}) by @{(p.User != null ? (p.User.DisplayName ?? p.User.UserName ?? "unknown") : "unknown")}",
                        Time = p.PlacedAt
                    })
                    .ToListAsync();

                return Ok(new
                {
                    Overview = new object[]
                    {
                        new { Label = "Active players", Value = activeUsers.ToString("D2"), Note = "Users with pixels on the board" },
                        new { Label = "Pixels owned", Value = uniqueOwnedPixels.ToString(), Note = "Unique cells currently claimed" },
                        new { Label = "Accepted submissions", Value = acceptedCount.ToString(), Note = "Challenge solutions verified" },
                        new { Label = "Total submissions", Value = submissionCount.ToString(), Note = "All challenge attempts recorded" }
                    },
                    TopPlayers = topPlayers,
                    PixelOwners = pixelOwners,
                    RecentHistory = recentPixels.Select(p => new
                    {
                        p.Label,
                        Time = ToRelativeTime(p.Time),
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private static string ToRelativeTime(DateTime dateTimeUtc)
        {
            var delta = DateTime.UtcNow - dateTimeUtc;
            if (delta.TotalMinutes < 1) return "just now";
            if (delta.TotalHours < 1) return $"{Math.Max(1, (int)delta.TotalMinutes)}m ago";
            if (delta.TotalDays < 1) return $"{(int)delta.TotalHours}h ago";
            return $"{(int)delta.TotalDays}d ago";
        }
    }
}
