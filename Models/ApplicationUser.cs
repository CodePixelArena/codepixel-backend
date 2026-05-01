using Microsoft.AspNetCore.Identity;

namespace codepixel_backend.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Additional properties can be added here
        public string? DisplayName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}