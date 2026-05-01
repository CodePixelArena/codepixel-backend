namespace codepixel_backend.Models
{
    public class Pixel
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Color { get; set; } = "#000000"; // hex color
        public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
    }
}