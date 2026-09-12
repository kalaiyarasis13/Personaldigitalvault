namespace PersonalDigitalVaultBackend.DTOs.ResponseDtos.Documents
{
    public class DocumentResponesDto
    {
        public int Id { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public string FileHash { get; set; } = string.Empty;
        public int? FolderId { get; set; }
        public DateTime UploadedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
