namespace codepixel_backend.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty; // e.g., "csharp", "python"
        public string? Result { get; set; } // execution result
        public bool IsSuccessful { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public int ChallengeId { get; set; }
        public Challenge? Challenge { get; set; }
    }
}