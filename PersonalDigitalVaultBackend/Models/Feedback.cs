namespace PersonalDigitalVaultBackend.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        public int UserID { get; set; }
        public int UserId { get; internal set; }
        public ApplicationUser? User { get; set; }
        public int Rating { get; set; } // 1-5
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
