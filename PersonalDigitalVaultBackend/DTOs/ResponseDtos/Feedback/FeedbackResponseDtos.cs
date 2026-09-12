namespace PersonalDigitalVaultBackend.DTOs.ResponseDtos.Feedback
{
    public class FeedbackResponseDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class AdminFeedbackListItemResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
