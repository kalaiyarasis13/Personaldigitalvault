namespace PersonalDigitalVaultBackend.DTOs.ResponseDtos.Sharing
{
    public class ShareLinkResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string ShareUrl { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    public class ShareLinkListItemResponseDto
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public int DocumentId { get; set; }
        public string DocumentFileName { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public int DownloadCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
