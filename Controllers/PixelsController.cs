using codepixel_backend.Data;
using codepixel_backend.Hubs;
using codepixel_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace codepixel_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PixelsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<PixelHub> _hubContext;

        public PixelsController(ApplicationDbContext context, IHubContext<PixelHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetPixels()
        {
            var pixels = await _context.Pixels
                .Include(p => p.User)
                .OrderBy(p => p.PlacedAt)
                .ToListAsync();
            return Ok(pixels);
        }

        [HttpPost]
        public async Task<IActionResult> PlacePixel([FromBody] Pixel pixel)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var savedPixel = await _context.Pixels
                .FirstOrDefaultAsync(p => p.X == pixel.X && p.Y == pixel.Y);

            if (savedPixel == null)
            {
                savedPixel = pixel;
                savedPixel.UserId = userId;
                savedPixel.PlacedAt = DateTime.UtcNow;
                _context.Pixels.Add(savedPixel);
            }
            else
            {
                savedPixel.Color = pixel.Color;
                savedPixel.UserId = userId;
                savedPixel.PlacedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            // Notify all clients
            await _hubContext.Clients.All.SendAsync("ReceivePixelUpdate", savedPixel.X, savedPixel.Y, savedPixel.Color, savedPixel.UserId);

            return Ok(savedPixel);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePixel(int id)
        {
            var pixel = await _context.Pixels.FindAsync(id);
            if (pixel == null) return NotFound();
            _context.Pixels.Remove(pixel);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
