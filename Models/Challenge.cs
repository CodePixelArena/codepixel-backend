namespace codepixel_backend.Models
{
    public class Challenge
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? CodeTemplate { get; set; }
        public string? TestCases { get; set; } // JSON or something
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedById { get; set; } = string.Empty;
        public ApplicationUser? CreatedBy { get; set; }
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    }
}